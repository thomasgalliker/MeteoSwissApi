using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MeteoSwissApi.Extensions;
using MeteoSwissApi.Models;
using MeteoSwissApi.Models.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UnitsNet;

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
        private readonly JsonSerializerSettings serializerSettings;
        private readonly bool verboseLogging;


        /// <summary>
        /// Initializes a new instance of the <see cref="SlfDataService"/> class.
        /// </summary>
        public SlfDataService()
            : this(new NullLogger<SlfDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlfDataService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public SlfDataService(
            ILogger<SlfDataService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlfDataService"/> class.
        /// </summary>
        /// <param name="options">The service options.</param>
        public SlfDataService(
            IOptions<MeteoSwissApiOptions> options)
          : this(options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlfDataService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public SlfDataService(
            MeteoSwissApiOptions options)
          : this(new NullLogger<SlfDataService>(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlfDataService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The service options.</param>
        public SlfDataService(
            ILogger<SlfDataService> logger,
            IOptions<MeteoSwissApiOptions> options)
          : this(logger, options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlfDataService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The service options.</param>
        public SlfDataService(
            ILogger<SlfDataService> logger,
            MeteoSwissApiOptions options)
          : this(logger, new HttpClient(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlfDataService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="httpClient">The HttpClient instance.</param>
        /// <param name="options">The service options.</param>
        public SlfDataService(
            ILogger<SlfDataService> logger,
            HttpClient httpClient,
            MeteoSwissApiOptions options)
        {
            this.logger = logger;
            this.verboseLogging = options.VerboseLogging;
            this.httpClient = httpClient;
            this.serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            this.serializerSettings.Converters.Add(new TemperatureJsonConverter());
        }

        /// <inheritdoc />
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

            var stationInfo = JsonConvert.DeserializeObject<SlfStationInfo>(responseJson, this.serializerSettings);
            return stationInfo;
        }

        /// <inheritdoc />
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

            var measurement = JsonConvert.DeserializeObject<SlfStationMeasurement>(responseJson, this.serializerSettings);

            var stationinfo = await this.GetStationInfoAsync(network, code);
            measurement.Station = stationinfo;
            return measurement;
        }

        /// <inheritdoc />
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


            var timeseries = JsonConvert.DeserializeObject<SlfStationMeasurementsResponse>(responseJson, this.serializerSettings);

            var slfStationMeasurementItems = timeseries.TemperatureAir
                .Select(t =>
                {
                    var windVelocityMean = timeseries.WindVelocityMean.Single(w => w.Date == t.Date);
                    var windVelocityMax = timeseries.WindVelocityMax.Single(w => w.Date == t.Date);
                    var windDirectionMean = timeseries.WindDirectionMean.Single(w => w.Date == t.Date);

                    var slfWindInfo = new SlfWindInfo
                    {
                        VelocityMax = windVelocityMax.Value,
                        VelocityMean = windVelocityMean.Value,
                        Direction = windDirectionMean.Value,
                    };

                    return new SlfStationMeasurementItem
                    {
                        Date = t.Date,
                        TemperatureAir = t.Value,
                        Wind = slfWindInfo
                    };
                })
                .OrderBy(t => t.Date)
                .ToArray();

            return slfStationMeasurementItems;
        }

        private static readonly (string QueryParameter, Action<SlfProperties, SlfStationMeasurement> AssignmentAction)[] ValueMappings =
        {
            ("HEIGHT_NEW_SNOW_1D", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight1d = new SlfStationDateLength { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("HEIGHT_NEW_SNOW_3D", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight3d = new SlfStationDateLength { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("HEIGHT_NEW_SNOW_7D", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight7d = new SlfStationDateLength { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("SNOW_HEIGHT", (SlfProperties p, SlfStationMeasurement m) =>
                m.SnowHeight = new SlfStationDateLength { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("TEMPERATURE_AIR", (SlfProperties p, SlfStationMeasurement m) =>
                m.AirTemperature = new SlfStationDateTemperature { Date = p.Timestamp.Value, Value = Temperature.FromDegreesCelsius(p.Value.Value)}),

            ("TEMPERATURE_SNOW_SURFACE", (SlfProperties p, SlfStationMeasurement m) =>
                m.SurfaceTemperature = new SlfStationDateTemperature { Date = p.Timestamp.Value, Value = Temperature.FromDegreesCelsius(p.Value.Value)}),

            ("WIND_MEAN", (SlfProperties p, SlfStationMeasurement m) =>
            {
                m.WindSpeedMean = new SlfStationDateSpeed{ Date = p.Timestamp.Value, Value = Speed.FromKilometersPerHour(p.Velocity.Value)};
                m.WindDirection = new SlfStationDateAngle{ Date = p.Timestamp.Value, Value = Angle.FromDegrees(p.Direction.Value) };
            }),
        };

        /// <inheritdoc />
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

                    if (feature.Properties.Timestamp is not null)
                    {
                        if (feature.Properties.Value != null || feature.Properties.Velocity != null)
                        {
                            stationDataTimepointParameter.AssignmentAction(feature.Properties, measurement);
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

            var slfStationMeasurementResponse = JsonConvert.DeserializeObject<SlfStationMeasurementResponse>(responseJson, this.serializerSettings);
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
