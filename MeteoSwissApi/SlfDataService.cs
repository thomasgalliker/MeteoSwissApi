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
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi
{
    public class SlfDataService : ISlfDataService
    {
        public const int PlzMinLength = 4;
        public const int PlzPaddingLength = 6;

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly Uri slfApiEndpoint;
        private readonly Uri whiteRiskApiEndpoint;
        private readonly bool verboseLogging;

        public SlfDataService(ILogger<SlfDataService> logger, ISlfDataServiceOptions options)
            : this(logger, new HttpClient(), options)
        {
        }

        public SlfDataService(ILogger<SlfDataService> logger, HttpClient httpClient, ISlfDataServiceOptions options)
        {
            this.logger = logger;
            this.slfApiEndpoint = new Uri(options.SlfApiEndpoint, UriKind.Absolute);
            this.whiteRiskApiEndpoint = new Uri(options.WhiteRiskApiEndpoint, UriKind.Absolute);
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

            var builder = new UriBuilder(this.slfApiEndpoint)
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

            var builder = new UriBuilder(this.slfApiEndpoint)
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

        private static readonly (string QueryParameter, Action<SlfProperties, SlfStationMeasurement> AssignmentAction)[] ValueMappings =
        {
            ("HEIGHT_NEW_SNOW_1D", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight1d = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("HEIGHT_NEW_SNOW_3D", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight3d = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("HEIGHT_NEW_SNOW_7D", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight7d = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("SNOW_HEIGHT", (SlfProperties p, SlfStationMeasurement m) =>
                m.SnowHeight = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("TEMPERATURE_AIR", (SlfProperties p, SlfStationMeasurement m) =>
                m.AirTemperature = new SlfStationMeasurementTemperatureValue { Date = p.Timestamp.Value, Value = Temperature.FromDegreesCelsius(p.Value.Value)}),

            ("TEMPERATURE_SNOW_SURFACE", (SlfProperties p, SlfStationMeasurement m) =>
                m.SurfaceTemperature = new SlfStationMeasurementTemperatureValue { Date = p.Timestamp.Value, Value = Temperature.FromDegreesCelsius(p.Value.Value)}),

            ("WIND_MEAN", (SlfProperties p, SlfStationMeasurement m) =>
            {
                m.WindSpeedMean = new SlfStationMeasurementSpeedValue{ Date = p.Timestamp.Value, Value = Speed.FromKilometersPerHour(p.Velocity.Value)};
                m.WindDirection = new SlfStationMeasurementAngleValue{ Date = p.Timestamp.Value, Value = Angle.FromDegrees(p.Direction.Value) };
            }),
        };

        /// <inheritdoc />
        public async Task<IEnumerable<SlfStationMeasurement>> GetLatestMeasurementsAsync()
        {
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
            this.logger.LogDebug($"GetStationDataTimepointAsync");

            var builder = new UriBuilder(this.slfApiEndpoint)
            {
                Path = $"public/station-data/timepoint/{parameter}/current/geojson",
            };

            if (parameter == "WIND_MEAN")
            {
                builder.Query = "stationTypeFilter=WIND";
            }

            var uri = builder.ToString();
            this.logger.LogDebug($"GetStationDataTimepointAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetStationDataTimepointAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var stationDataTimePointReponse = JsonConvert.DeserializeObject<SlfStationMeasurementResponse>(responseJson, this.serializerSettings);
            return stationDataTimePointReponse;
        }

        public async Task<Stream> GetMapTeaserImageAsync(string network, string stationCode)
        {
            this.logger.LogDebug($"GetMapTeaserImageAsync");

            var builder = new UriBuilder(this.whiteRiskApiEndpoint)
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
