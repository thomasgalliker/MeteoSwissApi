using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MeteoSwissApi
{
    public sealed class SlfDataService : ISlfDataService, IDisposable
    {
        private static readonly Uri SlfApiEndpoint = new Uri("https://public-meas-data-v2.slf.ch", UriKind.Absolute);
        private static readonly Uri WhiteRiskApiEndpoint = new Uri("https://whiterisk.ch", UriKind.Absolute);

        private const string ParentPathSegment = "..";
        private const string CurrentPathSegment = ".";

        /// <summary>
        /// Characters that would change the URL instead of naming a network or station.
        /// </summary>
        private static readonly char[] InvalidIdentifierChars = { '/', '\\', '?', '#', '%' };

        /// <summary>
        /// The SLF parameters that make up the latest measurements of all stations, each with the query that selects
        /// the stations which report it, the check whether a station reported the parameter, and the assignment of the
        /// reported value to the measurement.
        /// </summary>
        private static readonly (string Parameter, string? Query, Func<SlfProperties, bool> HasValue, Action<SlfProperties, SlfStationMeasurement, DateTime> AssignValue)[] ValueMappings =
        {
            ("HEIGHT_NEW_SNOW_1D", null, HasSingleValue, (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.NewSnowHeight1d = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("HEIGHT_NEW_SNOW_3D", null, HasSingleValue, (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.NewSnowHeight3d = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("HEIGHT_NEW_SNOW_7D", null, HasSingleValue, (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.NewSnowHeight7d = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("SNOW_HEIGHT", null, HasSingleValue, (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.SnowHeight = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("TEMPERATURE_AIR", null, HasSingleValue, (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.AirTemperature = new SlfStationDateTemperature { Date = timestamp, Value = Temperature.FromDegreesCelsius(RequireValue(p.Value, nameof(p.Value)))}),

            ("TEMPERATURE_SNOW_SURFACE", null, HasSingleValue, (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.SurfaceTemperature = new SlfStationDateTemperature { Date = timestamp, Value = Temperature.FromDegreesCelsius(RequireValue(p.Value, nameof(p.Value)))}),

            // Only wind stations report wind values.
            ("WIND_MEAN", "stationTypeFilter=WIND", HasWindValue, (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
            {
                // SLF reports wind speed and direction independently: a station may deliver
                // a velocity while its direction is null (or vice versa).
                if (p.Velocity is decimal velocity)
                {
                    m.WindSpeedMean = new SlfStationDateSpeed { Date = timestamp, Value = Speed.FromKilometersPerHour(velocity) };
                }

                if (p.Direction is decimal direction)
                {
                    m.WindDirection = new SlfStationDateAngle { Date = timestamp, Value = Angle.FromDegrees(direction) };
                }
            }),
        };

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly bool disposeHttpClient;
        private readonly bool verboseLogging;

        /// <summary>
        /// Creates the service with the default options, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        public SlfDataService()
            : this(NullLogger<SlfDataService>.Instance)
        {
        }

        /// <summary>
        /// Creates the service with the default options and its own <see cref="HttpClient"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public SlfDataService(
            ILogger<SlfDataService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        /// <summary>
        /// Creates the service with options from dependency injection, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid.</exception>
        public SlfDataService(
            IOptions<MeteoSwissApiOptions> options)
          : this(NullLogger<SlfDataService>.Instance, options)
        {
        }

        /// <summary>
        /// Creates the service with the given options, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid.</exception>
        public SlfDataService(
            MeteoSwissApiOptions options)
          : this(NullLogger<SlfDataService>.Instance, options)
        {
        }

        /// <summary>
        /// Creates the service with options from dependency injection and its own <see cref="HttpClient"/>.
        /// This is the constructor that <c>AddMeteoSwissApi</c> uses.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid.</exception>
        public SlfDataService(
            ILogger<SlfDataService> logger,
            IOptions<MeteoSwissApiOptions> options)
          : this(logger, (options ?? throw new ArgumentNullException(nameof(options))).Value)
        {
        }

        /// <summary>
        /// Creates the service with the given options and its own <see cref="HttpClient"/>.
        /// </summary>
        /// <remarks>
        /// The client keeps its connections for the lifetime of the service and uses the default timeout of <see cref="HttpClient"/>,
        /// 100 seconds. Create the service once and reuse it, and dispose it to release the client.
        /// Pass your own client for a different timeout or a retry policy.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid.</exception>
        public SlfDataService(
            ILogger<SlfDataService> logger,
            MeteoSwissApiOptions options)
          : this(logger, options, null)
        {
        }

        /// <summary>
        /// Creates the service with the given <see cref="HttpClient"/>, for example one with a shorter timeout or with your own message handlers.
        /// The service never changes the client and does not dispose it.
        /// </summary>
        /// <inheritdoc cref="MeteoSwissWeatherService(ILogger{MeteoSwissWeatherService}, HttpClient, MeteoSwissApiOptions)" path="/remarks"/>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/>, <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid.</exception>
        public SlfDataService(
            ILogger<SlfDataService> logger,
            HttpClient httpClient,
            MeteoSwissApiOptions options)
          : this(logger, options, httpClient ?? throw new ArgumentNullException(nameof(httpClient)))
        {
        }

        /// <summary>
        /// Creates the service with <paramref name="httpClient"/>, or with its own client, which it disposes, if <paramref name="httpClient"/> is null.
        /// </summary>
        private SlfDataService(
            ILogger<SlfDataService> logger,
            MeteoSwissApiOptions options,
            HttpClient? httpClient)
        {
            MeteoSwissApiOptionsValidator.ThrowIfInvalid(options, nameof(options));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.disposeHttpClient = httpClient is null;
            this.httpClient = httpClient ?? new HttpClient();
            this.verboseLogging = options.VerboseLogging;
        }

        public Task<SlfStationInfo> GetStationInfoAsync(string network, string stationCode, CancellationToken cancellationToken = default)
        {
            ValidateIdentifier(network, nameof(network));
            ValidateIdentifier(stationCode, nameof(stationCode));

            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/info/{network}/{stationCode}",
            };

            return this.GetJsonAsync<SlfStationInfo>(nameof(this.GetStationInfoAsync), builder.Uri, cancellationToken);
        }

        public async Task<SlfStationMeasurement> GetLatestMeasurementByStationCodeAsync(string network, string stationCode, CancellationToken cancellationToken = default)
        {
            ValidateIdentifier(network, nameof(network));
            ValidateIdentifier(stationCode, nameof(stationCode));

            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/timeseries/current/{network}/{stationCode}",
            };

            // The measurement and the station info do not depend on each other, so both requests run at once.
            var latestMeasurementTask = this.GetJsonAsync<SlfStationLatestMeasurementResponse>(nameof(this.GetLatestMeasurementByStationCodeAsync), builder.Uri, cancellationToken);
            var stationInfoTask = this.GetStationInfoAsync(network, stationCode, cancellationToken);
            await Task.WhenAll(latestMeasurementTask, stationInfoTask).ConfigureAwait(false);

            var latestMeasurement = await latestMeasurementTask.ConfigureAwait(false);
            var stationInfo = await stationInfoTask.ConfigureAwait(false);

            var measurement = new SlfStationMeasurement
            {
                Station = stationInfo,
                NewSnowHeight1d = ToDateValue<SlfStationDateLength, Length>(latestMeasurement.HeightNewSnow),
                NewSnowHeight3d = ToDateValue<SlfStationDateLength, Length>(latestMeasurement.HeightNewSnow3d),
                NewSnowHeight7d = ToDateValue<SlfStationDateLength, Length>(latestMeasurement.HeightNewSnow7d),
                SnowHeight = ToDateValue<SlfStationDateLength, Length>(latestMeasurement.SnowHeight),
                AirTemperature = ToDateValue<SlfStationDateTemperature, Temperature>(latestMeasurement.TemperatureAir),
                SurfaceTemperature = ToDateValue<SlfStationDateTemperature, Temperature>(latestMeasurement.TemperatureSnowSurface),
                WindSpeedMean = ToDateValue<SlfStationDateSpeed, Speed>(latestMeasurement.WindVelocityMean),
                WindDirection = ToDateValue<SlfStationDateAngle, Angle>(latestMeasurement.WindDirectionMean),
                WindSpeedMax = ToDateValue<SlfStationDateSpeed, Speed>(latestMeasurement.WindVelocityMax),
            };

            return measurement;
        }

        /// <summary>
        /// Converts a value read from SLF into its public model, or returns <c>null</c> if the station reported no value.
        /// </summary>
        private static TDateValue? ToDateValue<TDateValue, TValue>(SlfStationDateValue<TValue?>? dateValue)
            where TDateValue : SlfStationDateValue<TValue>, new()
            where TValue : struct
        {
            if (dateValue?.Value is TValue value)
            {
                return new TDateValue { Date = dateValue.Date, Value = value };
            }

            return null;
        }

        public async Task<IReadOnlyList<SlfStationMeasurementItem>> GetMeasurementsByStationCodeAsync(string network, string stationCode, CancellationToken cancellationToken = default)
        {
            ValidateIdentifier(network, nameof(network));
            ValidateIdentifier(stationCode, nameof(stationCode));

            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/timeseries/week/current/{network}/{stationCode}",
            };

            var timeseries = await this.GetJsonAsync<SlfStationMeasurementsResponse>(nameof(this.GetMeasurementsByStationCodeAsync), builder.Uri, cancellationToken).ConfigureAwait(false);

            var temperatureAirByDate = ToDateLookup(timeseries.TemperatureAir, t => t.Date);
            var windVelocityMeanByDate = ToDateLookup(timeseries.WindVelocityMean, w => w.Date);
            var windVelocityMaxByDate = ToDateLookup(timeseries.WindVelocityMax, w => w.Date);
            var windDirectionMeanByDate = ToDateLookup(timeseries.WindDirectionMean, w => w.Date);
            var snowHeightByDate = ToDateLookup(timeseries.SnowHeight, s => s.Date);
            var newSnowHeightByDate = ToDateLookup(timeseries.HeightNewSnow, s => s.Date);
            var surfaceTemperatureByDate = ToDateLookup(timeseries.TemperatureSnowSurface, s => s.Date);

            // Stations report different series: observer stations, for example, have snow values but no air temperature.
            // Every timestamp of any series becomes a row, and a value its series does not have for that timestamp stays null.
            // New snow height is reported on a coarser (daily) grid, so most rows carry no value for it.
            var dates = temperatureAirByDate.Keys
                .Concat(windVelocityMeanByDate.Keys)
                .Concat(windVelocityMaxByDate.Keys)
                .Concat(windDirectionMeanByDate.Keys)
                .Concat(snowHeightByDate.Keys)
                .Concat(newSnowHeightByDate.Keys)
                .Concat(surfaceTemperatureByDate.Keys)
                .Distinct()
                .OrderBy(d => d);

            var slfStationMeasurementItems = dates
                .Select(date =>
                {
                    var slfWindInfo = new SlfWindInfo
                    {
                        VelocityMax = windVelocityMaxByDate.TryGetValue(date, out var velocityMax)
                            ? velocityMax.Value
                            : null,
                        VelocityMean = windVelocityMeanByDate.TryGetValue(date, out var velocityMean)
                            ? velocityMean.Value
                            : null,
                        Direction = windDirectionMeanByDate.TryGetValue(date, out var direction)
                            ? direction.Value
                            : null,
                    };

                    return new SlfStationMeasurementItem
                    {
                        Date = date,
                        TemperatureAir = temperatureAirByDate.TryGetValue(date, out var temperatureAir)
                            ? temperatureAir.Value
                            : null,
                        Wind = slfWindInfo,
                        SnowHeight = snowHeightByDate.TryGetValue(date, out var snowHeight)
                            ? snowHeight.Value
                            : null,
                        NewSnowHeight = newSnowHeightByDate.TryGetValue(date, out var newSnowHeight)
                            ? newSnowHeight.Value
                            : null,
                        SurfaceTemperature = surfaceTemperatureByDate.TryGetValue(date, out var surfaceTemperature)
                            ? surfaceTemperature.Value
                            : null,
                    };
                })
                .ToArray();

            return Array.AsReadOnly(slfStationMeasurementItems);
        }

        private static Dictionary<DateTime, TItem> ToDateLookup<TItem>(IEnumerable<TItem> items, Func<TItem, DateTime> dateSelector)
        {
            var lookup = new Dictionary<DateTime, TItem>();

            foreach (var item in items)
            {
                lookup[dateSelector(item)] = item;
            }

            return lookup;
        }

        private static decimal RequireValue(decimal? value, string propertyName)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue;
            }

            throw new InvalidDataException($"Required SLF property '{propertyName}' is missing.");
        }

        public async Task<IReadOnlyList<SlfStationMeasurement>> GetLatestMeasurementsAsync(CancellationToken cancellationToken = default)
        {
            var responseTasks = ValueMappings
                .Select(m => this.GetLatestMeasurementsAsync(m.Parameter, m.Query, cancellationToken))
                .ToArray();

            var responses = await Task.WhenAll(responseTasks).ConfigureAwait(false);

            // Station codes are unique across the SLF networks today; the network in the key keeps it that way.
            var measurements = new Dictionary<(string Network, string Code), SlfStationMeasurement>();

            for (var i = 0; i < ValueMappings.Length; i++)
            {
                var (parameter, _, hasValue, assignValue) = ValueMappings[i];
                var stationsWithoutValue = 0;

                foreach (var feature in responses[i].Features)
                {
                    var properties = feature.Properties;
                    var key = (properties.Network, properties.Code);
                    if (!measurements.TryGetValue(key, out var measurement))
                    {
                        measurement = new SlfStationMeasurement { Station = ToStation(feature) };
                        measurements.Add(key, measurement);
                    }

                    if (properties.Timestamp is DateTime timestamp && hasValue(properties))
                    {
                        assignValue(properties, measurement, timestamp);
                    }
                    else
                    {
                        stationsWithoutValue++;
                    }
                }

                if (stationsWithoutValue > 0)
                {
                    this.logger.LogDebug("GetLatestMeasurementsAsync: {Count} stations reported no value for {Parameter}", stationsWithoutValue, parameter);
                }
            }

            return Array.AsReadOnly(measurements.Values.ToArray());
        }

        private static bool HasSingleValue(SlfProperties properties)
        {
            return properties.Value != null;
        }

        private static bool HasWindValue(SlfProperties properties)
        {
            return properties.Velocity != null || properties.Direction != null;
        }

        /// <summary>
        /// Copies the station of <paramref name="feature"/>, whose properties also carry the measured value.
        /// </summary>
        private static SlfStation ToStation(SlfFeature feature)
        {
            var properties = feature.Properties;

            return new SlfStation
            {
                Network = properties.Network,
                Code = properties.Code,
                Type = properties.Type,
                Elevation = properties.Elevation,
                Name = properties.Name,
                Manual = properties.Manual,
                Location = new SlfLocation
                {
                    Type = feature.Geometry.Type,
                    Coordinates = feature.Geometry.Coordinates,
                },
            };
        }

        private Task<SlfStationMeasurementResponse> GetLatestMeasurementsAsync(string parameter, string? query, CancellationToken cancellationToken)
        {
            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/timepoint/{parameter}/current/geojson",
            };

            if (query != null)
            {
                builder.Query = query;
            }

            return this.GetJsonAsync<SlfStationMeasurementResponse>(nameof(this.GetLatestMeasurementsAsync), builder.Uri, cancellationToken);
        }

        public async Task<Stream> GetMapTeaserImageAsync(string network, string stationCode, CancellationToken cancellationToken = default)
        {
            ValidateIdentifier(network, nameof(network));
            ValidateIdentifier(stationCode, nameof(stationCode));

            var builder = new UriBuilder(WhiteRiskApiEndpoint)
            {
                Path = $"measurement-station-teaser/{network}-{stationCode}-no-marker-m.webp",
            };

            var uri = builder.Uri;
            this.logger.LogDebug("{MethodName}: GET {Uri}", nameof(this.GetMapTeaserImageAsync), uri);

            var imageBytes = await this.httpClient.GetBytesOrThrowAsync(uri, null, cancellationToken).ConfigureAwait(false);
            return new MemoryStream(imageBytes, writable: false);
        }

        /// <summary>
        /// Disposes the <see cref="HttpClient"/> that the service created itself. A client passed to the constructor is not disposed:
        /// it belongs to the caller. Dispose the service after its last call has completed.
        /// </summary>
        public void Dispose()
        {
            if (this.disposeHttpClient)
            {
                this.httpClient.Dispose();
            }
        }

        /// <summary>
        /// Reads the JSON response of <paramref name="uri"/> as <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        private async Task<T> GetJsonAsync<T>(string methodName, Uri uri, CancellationToken cancellationToken) where T : class
        {
            this.logger.LogDebug("{MethodName}: GET {Uri}", methodName, uri);

            if (this.verboseLogging)
            {
                // The content is logged, so it is read as a string before it is parsed.
                var responseJson = await this.httpClient.GetStringOrThrowAsync(uri, null, cancellationToken).ConfigureAwait(false);
                this.logger.LogDebug("{MethodName} returned content: {Content}", methodName, responseJson);
                return JsonSerialization.Deserialize<T>(responseJson, uri, JsonSerialization.Default);
            }

            return await this.httpClient.GetJsonOrThrowAsync<T>(uri, null, JsonSerialization.Default, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks that <paramref name="identifier"/>, an SLF network or station code, only names a path segment:
        /// no path characters, no whitespace, and neither "." nor "..", which the URL would resolve to another path.
        /// Codes are rejected rather than escaped: servers may decode an escaped "/" or "." again,
        /// and real codes such as IMIS, DAV2 or *ABG0 never contain these characters.
        /// </summary>
        private static void ValidateIdentifier(string identifier, string parameterName)
        {
            if (identifier is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrWhiteSpace(identifier) ||
                identifier.Any(char.IsWhiteSpace) ||
                identifier.IndexOfAny(InvalidIdentifierChars) >= 0 ||
                identifier == CurrentPathSegment ||
                identifier.Contains(ParentPathSegment))
            {
                throw new ArgumentException($"'{identifier}' is not a valid SLF network or station code.", parameterName);
            }
        }
    }
}
