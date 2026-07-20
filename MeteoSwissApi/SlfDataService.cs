using System.Text.Json;
using MeteoSwissApi.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MeteoSwissApi
{
    public class SlfDataService : ISlfDataService
    {
        private static readonly Uri SlfApiEndpoint = new Uri("https://public-meas-data-v2.slf.ch", UriKind.Absolute);
        private static readonly Uri WhiteRiskApiEndpoint = new Uri("https://whiterisk.ch", UriKind.Absolute);

        public const int PlzMinLength = 4;
        public const int PlzPaddingLength = 6;

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly bool verboseLogging;

        public SlfDataService()
            : this(new NullLogger<SlfDataService>())
        {
        }

        public SlfDataService(
            ILogger<SlfDataService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        public SlfDataService(
            IOptions<MeteoSwissApiOptions> options)
          : this(options.Value)
        {
        }

        public SlfDataService(
            MeteoSwissApiOptions options)
          : this(new NullLogger<SlfDataService>(), options)
        {
        }

        public SlfDataService(
            ILogger<SlfDataService> logger,
            IOptions<MeteoSwissApiOptions> options)
          : this(logger, options.Value)
        {
        }

        public SlfDataService(
            ILogger<SlfDataService> logger,
            MeteoSwissApiOptions options)
          : this(logger, new HttpClient(), options)
        {
        }

        public SlfDataService(
            ILogger<SlfDataService> logger,
            HttpClient httpClient,
            MeteoSwissApiOptions options)
        {
            this.logger = logger;
            this.verboseLogging = options.VerboseLogging;
            this.httpClient = httpClient;
        }

        public async Task<SlfStationInfo> GetStationInfoAsync(string network, string code)
        {
            this.logger.LogDebug($"GetStationInfoAsync");

            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/info/{network}/{code}",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetStationInfoAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetStationInfoAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var stationInfo = JsonSerializer.Deserialize<SlfStationInfo>(responseJson, JsonSerialization.CreateOptions())!;
            return stationInfo;
        }

        public async Task<SlfStationMeasurement> GetLatestMeasurementByStationCodeAsync(string network, string code)
        {
            this.logger.LogDebug($"GetLatestMeasurementByStationCodeAsync");

            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/timeseries/current/{network}/{code}",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetLatestMeasurementByStationCodeAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetLatestMeasurementByStationCodeAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var measurement = JsonSerializer.Deserialize<SlfStationMeasurement>(responseJson, JsonSerialization.CreateOptions())!;

            var stationinfo = await this.GetStationInfoAsync(network, code);
            measurement.Station = stationinfo;
            return measurement;
        }

        public async Task<IEnumerable<SlfStationMeasurementItem>> GetMeasurementsByStationCodeAsync(string network, string code)
        {
            this.logger.LogDebug($"GetMeasurementsByStationCodeAsync");

            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/timeseries/week/current/{network}/{code}",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetMeasurementsByStationCodeAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetMeasurementsByStationCodeAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var timeseries = JsonSerializer.Deserialize<SlfStationMeasurementsResponse>(responseJson, JsonSerialization.CreateOptions())!;

            var windVelocityMeanByDate = ToDateLookup(timeseries.WindVelocityMean, w => w.Date);
            var windVelocityMaxByDate = ToDateLookup(timeseries.WindVelocityMax, w => w.Date);
            var windDirectionMeanByDate = ToDateLookup(timeseries.WindDirectionMean, w => w.Date);

            // The snow series are optional: SLF reports new snow height on a coarser (daily) grid
            // than the other measurements, so most timestamps carry no value and stay null.
            var snowHeightByDate = ToDateLookup(timeseries.SnowHeight, s => s.Date);
            var newSnowHeightByDate = ToDateLookup(timeseries.HeightNewSnow, s => s.Date);
            var surfaceTemperatureByDate = ToDateLookup(timeseries.TemperatureSnowSurface, s => s.Date);

            var slfStationMeasurementItems = timeseries.TemperatureAir
                .Select(t =>
                {
                    var slfWindInfo = new SlfWindInfo
                    {
                        VelocityMax = windVelocityMaxByDate[t.Date].Value,
                        VelocityMean = windVelocityMeanByDate[t.Date].Value,
                        Direction = windDirectionMeanByDate[t.Date].Value,
                    };

                    return new SlfStationMeasurementItem
                    {
                        Date = t.Date,
                        TemperatureAir = t.Value,
                        Wind = slfWindInfo,
                        SnowHeight = snowHeightByDate.TryGetValue(t.Date, out var snowHeight)
                            ? snowHeight.Value
                            : null,
                        NewSnowHeight = newSnowHeightByDate.TryGetValue(t.Date, out var newSnowHeight)
                            ? newSnowHeight.Value
                            : null,
                        SurfaceTemperature = surfaceTemperatureByDate.TryGetValue(t.Date, out var surfaceTemperature)
                            ? surfaceTemperature.Value
                            : null,
                    };
                })
                .OrderBy(t => t.Date)
                .ToArray();

            return slfStationMeasurementItems;
        }

        private static readonly (string QueryParameter, Action<SlfProperties, SlfStationMeasurement, DateTime> AssignmentAction)[] ValueMappings =
        {
            ("HEIGHT_NEW_SNOW_1D", (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.NewSnowHeight1d = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("HEIGHT_NEW_SNOW_3D", (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.NewSnowHeight3d = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("HEIGHT_NEW_SNOW_7D", (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.NewSnowHeight7d = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("SNOW_HEIGHT", (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.SnowHeight = new SlfStationDateLength { Date = timestamp, Value = Length.FromCentimeters(RequireValue(p.Value, nameof(p.Value)))}),

            ("TEMPERATURE_AIR", (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.AirTemperature = new SlfStationDateTemperature { Date = timestamp, Value = Temperature.FromDegreesCelsius(RequireValue(p.Value, nameof(p.Value)))}),

            ("TEMPERATURE_SNOW_SURFACE", (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
                m.SurfaceTemperature = new SlfStationDateTemperature { Date = timestamp, Value = Temperature.FromDegreesCelsius(RequireValue(p.Value, nameof(p.Value)))}),

            ("WIND_MEAN", (SlfProperties p, SlfStationMeasurement m, DateTime timestamp) =>
            {
                m.WindSpeedMean = new SlfStationDateSpeed { Date = timestamp, Value = Speed.FromKilometersPerHour(RequireValue(p.Velocity, nameof(p.Velocity))) };
                m.WindDirection = new SlfStationDateAngle { Date = timestamp, Value = Angle.FromDegrees(RequireValue(p.Direction, nameof(p.Direction))) };
            }),
        };

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

        public async Task<IEnumerable<SlfStationMeasurement>> GetLatestMeasurementsAsync()
        {
            this.logger.LogDebug($"GetLatestMeasurementsAsync");

            var parameterAndTasks = ValueMappings
                .Select(p => (p.QueryParameter, Task: this.GetLatestMeasurementsAsync(p.QueryParameter)))
                .ToArray();

            var responseTasks = parameterAndTasks
                .Select(x => x.Task)
                .ToArray();

            await Task.WhenAll(responseTasks);

            var responses = new List<(string QueryParameter, SlfStationMeasurementResponse Response)>();
            foreach (var parameterAndTask in parameterAndTasks)
            {
                var response = await parameterAndTask.Task;
                responses.Add(new(parameterAndTask.QueryParameter, response));
            }

            var measurements = new List<SlfStationMeasurement>();

            foreach (var response in responses)
            {
                var stationDataTimepointParameter = ValueMappings.Single(m => m.QueryParameter == response.QueryParameter);

                foreach (var feature in response.Response.Features)
                {
                    var measurement = measurements.SingleOrDefault(m => m.Station.Code == feature.Properties.Code);
                    if (measurement == null)
                    {
                        measurement = new SlfStationMeasurement();
                        measurements.Add(measurement);
                    }

                    measurement.Station = feature.Properties.Clone<SlfStation>();
                    measurement.Station.Location = new SlfLocation
                    {
                        Coordinates = feature.Geometry.Coordinates
                    };

                    if (feature.Properties.Timestamp is DateTime timestamp)
                    {
                        if (feature.Properties.Value != null || feature.Properties.Velocity != null)
                        {
                            stationDataTimepointParameter.AssignmentAction(feature.Properties, measurement, timestamp);
                        }
                        else
                        {
                            this.logger.LogWarning($"GetLatestMeasurementsAsync: Station {feature.Properties.Code} with feature.Properties.Type=\"{feature.Properties.Type}\" does not contain a valid measurement value");
                        }
                    }
                }
            }

            return measurements;
        }

        private async Task<SlfStationMeasurementResponse> GetLatestMeasurementsAsync(string parameter)
        {
            this.logger.LogDebug($"GetLatestMeasurementsAsync");

            var builder = new UriBuilder(SlfApiEndpoint)
            {
                Path = $"public/station-data/timepoint/{parameter}/current/geojson",
            };

            if (parameter == "WIND_MEAN")
            {
                builder.Query = "stationTypeFilter=WIND";
            }

            var uri = builder.ToString();
            this.logger.LogDebug($"GetLatestMeasurementsAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetLatestMeasurementsAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var slfStationMeasurementResponse = JsonSerializer.Deserialize<SlfStationMeasurementResponse>(responseJson, JsonSerialization.CreateOptions())!;
            return slfStationMeasurementResponse;
        }

        public async Task<Stream> GetMapTeaserImageAsync(string network, string stationCode)
        {
            this.logger.LogDebug($"GetMapTeaserImageAsync");

            var builder = new UriBuilder(WhiteRiskApiEndpoint)
            {
                Path = $"measurement-station-teaser/{network}-{stationCode}-no-marker-m.webp",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetMapTeaserImageAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return responseStream;
        }
    }
}
