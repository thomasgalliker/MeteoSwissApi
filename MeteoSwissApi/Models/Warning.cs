﻿using System;
using System.Collections.Generic;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class Warning
    {
        public Warning()
        {
            this.Links = new List<Link>();
        }

        [JsonProperty("warnType")]
        [JsonConverter(typeof(WarnTypeJsonConverter))]
        public WarnType WarnType { get; set; }

        [JsonProperty("warnLevel")]
        [JsonConverter(typeof(WarnLevelJsonConverter))]
        public WarnLevel WarnLevel { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("validFrom")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime ValidFrom { get; set; }

        [JsonProperty("validTo")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? ValidTo { get; set; }

        [JsonProperty("ordering")]
        public string Ordering { get; set; }

        [JsonProperty("htmlText")]
        public string HtmlText { get; set; }

        [JsonProperty("outlook")]
        public bool Outlook { get; set; }

        [JsonProperty("links")]
        public IReadOnlyCollection<Link> Links { get; set; }

        public override string ToString()
        {
            return
                $"WarnType={this.WarnType}, WarnLevel={this.WarnType.ToString(this.WarnLevel)}, " +
                $"Validity={this.ValidFrom}{(this.ValidTo is DateTime validTo ? $"-{validTo}" : "")} {(this.Text is string text ? $" ({text})" : "")}";
        }
    }
}