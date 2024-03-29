﻿using System;

namespace MeteoSwissApi
{
    public class SwissMetNetOptions
    {
        /// <summary>
        /// The cache expiration time used for all service requests in <see cref="SwissMetNetService"/>.
        /// Default: <c>null</c> (caching is disabled).
        /// </summary>
        public TimeSpan? CacheExpiration { get; set; }
    }
}