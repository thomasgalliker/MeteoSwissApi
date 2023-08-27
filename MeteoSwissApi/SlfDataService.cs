using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using MeteoSwissApi.Models.Converters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MeteoSwissApi.Utils;
using System.Threading.Tasks;
using MeteoSwissApi.Models;
using System.Data.Common;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using UnitsNet;
using MeteoSwissApi.Extensions;
using System.Linq.Expressions;

namespace MeteoSwissApi
{
    public class SlfDataService : ISlfDataService
    {
        public const int PlzMinLength = 4;
        public const int PlzPaddingLength = 6;

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly Uri apiEndpoint;
        private readonly bool verboseLogging;

        public SlfDataService(ILogger<SlfDataService> logger, ISlfDataServiceOptions options)
            : this(logger, new HttpClient(), options)
        {
        }

        public SlfDataService(ILogger<SlfDataService> logger, HttpClient httpClient, ISlfDataServiceOptions options)
        {
            this.logger = logger;
            this.apiEndpoint = new Uri(options.ApiEndpoint, UriKind.Absolute);
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

            var builder = new UriBuilder(this.apiEndpoint)
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

            var builder = new UriBuilder(this.apiEndpoint)
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

        private static readonly (string QueryParameter, string PropertiesType, Action<SlfProperties, SlfStationMeasurement> AssignmentAction)[] ValueMappings =
        {
            ("HEIGHT_NEW_SNOW_1D", "SNOW_FLAT", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight1d = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("HEIGHT_NEW_SNOW_3D", "SNOW_FLAT", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight3d = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("HEIGHT_NEW_SNOW_7D", "SNOW_FLAT", (SlfProperties p, SlfStationMeasurement m) =>
                m.NewSnowHeight7d = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("SNOW_HEIGHT", "SNOW_FLAT", (SlfProperties p, SlfStationMeasurement m) =>
                m.SnowHeight = new SlfStationMeasurementLengthValue { Date = p.Timestamp.Value, Value = Length.FromCentimeters(p.Value.Value)}),

            ("TEMPERATURE_AIR", "SNOW_FLAT", (SlfProperties p, SlfStationMeasurement m) =>
                m.AirTemperature = new SlfStationMeasurementTemperatureValue { Date = p.Timestamp.Value, Value = Temperature.FromDegreesCelsius(p.Value.Value)}),

            ("TEMPERATURE_SNOW_SURFACE", "SNOW_FLAT", (SlfProperties p, SlfStationMeasurement m) =>
                m.SurfaceTemperature = new SlfStationMeasurementTemperatureValue { Date = p.Timestamp.Value, Value = Temperature.FromDegreesCelsius(p.Value.Value)}),

            ("WIND_MEAN", "WIND", (SlfProperties p, SlfStationMeasurement m) =>
            {
                m.WindSpeedMean = new SlfStationMeasurementSpeedValue{ Date = p.Timestamp.Value, Value = Speed.FromKilometersPerHour(p.Velocity.Value)};
                m.WindDirection = new SlfStationMeasurementAngleValue{ Date = p.Timestamp.Value, Value = Angle.FromDegrees(p.Direction.Value) };
            }),
        };

        /// <inheritdoc />
        public Task<IEnumerable<SlfStationMeasurement>> GetLatestMeasurementsAsync()
        {
            return this.GetLatestMeasurementsInternalAsync(null);
        }

        private async Task<IEnumerable<SlfStationMeasurement>> GetLatestMeasurementsInternalAsync(string stationCode)
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

                this.logger.LogDebug($"GetLatestMeasurementsAsync: Mapping response.QueryParameter=\"{response.QueryParameter}\" >> \"{stationDataTimepointParameter.PropertiesType}\"");

                IEnumerable<SlfFeature> features = response.Response.Features;
                if (!string.IsNullOrEmpty(stationCode))
                {
                    features = features.Where(f => f.Properties.Code == stationCode).ToList();
                }

                foreach (var feature in features)
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
                        //if (feature.Properties.Type == stationDataTimepointParameter.PropertiesType)
                        {
                            if (feature.Properties.Value != null || feature.Properties.Velocity != null)
                            {
                                stationDataTimepointParameter.AssignmentAction(feature.Properties, measurement);
                            }
                            else
                            {
                                this.logger.LogWarning($"GetLatestMeasurementsAsync: feature.Properties does not contain a valid measurement value");
                            }
                        }
                        //else
                        //{
                        //    this.logger.LogWarning($"GetLatestMeasurementsAsync: Station.Code={feature.Properties.Code} returned feature.Properties.Type=\"{feature.Properties.Type}\" (expected: \"{stationDataTimepointParameter.PropertiesType}\")");
                        //}
                    }
                }
            }

            return measurements;
        }

        private async Task<SlfStationMeasurementResponse> GetLatestMeasurementsAsync(string parameter)
        {
            this.logger.LogDebug($"GetStationDataTimepointAsync");

            var builder = new UriBuilder(this.apiEndpoint)
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
    }
}
