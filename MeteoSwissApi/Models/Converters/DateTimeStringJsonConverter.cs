﻿using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeatherDisplay.Model.Wiewarm.Converters
{
    internal class DateTimeStringJsonConverter : DateTimeConverterBase
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateTime = (DateTime)value;
            writer.WriteValue(dateTime.ToString(DateFormat, CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string stringValue)
            {
                var dateTime = DateTime.ParseExact(stringValue, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                return dateTime;
            }

            return default(DateTime);
        }
    }
}
