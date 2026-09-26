namespace MeteoSwissApi.Models
{
    public abstract class SlfStationDateValue<TValue>
    {
        [JsonProperty("timestamp")]
        public DateTime Date { get; set; }

        public abstract TValue Value { get; set; }

        public override string ToString()
        {
            return $"{{{this.Date}, {this.Value}}}";
        }
    }

    public class SlfStationDateLength : SlfStationDateValue<Length>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(CentimeterLengthJsonConverter))]
        public override Length Value { get; set; }
    }

    public class SlfStationDateTemperature : SlfStationDateValue<Temperature>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public override Temperature Value { get; set; }
    }

    public class SlfStationDateSpeed : SlfStationDateValue<Speed>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(SpeedJsonConverter))]
        public override Speed Value { get; set; }
    }

    public class SlfStationDateAngle : SlfStationDateValue<Angle>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(DegreeAngleJsonConverter))]
        public override Angle Value { get; set; }
    }

    /// <summary>
    /// Timeseries variant of <see cref="SlfStationDateSpeed"/> which keeps a JSON <c>null</c> value as <c>null</c>
    /// (System.Text.Json only forwards non-null tokens to <see cref="SpeedJsonConverter"/>).
    /// </summary>
    internal class SlfStationDateNullableSpeed : SlfStationDateValue<Speed?>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(SpeedJsonConverter))]
        public override Speed? Value { get; set; }
    }

    /// <summary>
    /// Timeseries variant of <see cref="SlfStationDateAngle"/> which keeps a JSON <c>null</c> value as <c>null</c>
    /// (System.Text.Json only forwards non-null tokens to <see cref="DegreeAngleJsonConverter"/>).
    /// </summary>
    internal class SlfStationDateNullableAngle : SlfStationDateValue<Angle?>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(DegreeAngleJsonConverter))]
        public override Angle? Value { get; set; }
    }
}
