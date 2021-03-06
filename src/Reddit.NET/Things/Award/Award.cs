﻿using Newtonsoft.Json;
using System;

namespace Reddit.Things
{
    [Serializable]
    public class Award
    {
        [JsonProperty("icon_70")]
        public string Icon70;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("url")]
        public string URL;

        [JsonProperty("icon_40")]
        public string Icon40;

        [JsonProperty("award_id")]
        public string AwardId;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("description")]
        public string Description;
    }
}
