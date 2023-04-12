using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItemBase.Core.Settings
{
    public enum Cache { Redis, Memory }
    public class CacheSettings
    {

        public readonly static CacheSettings Default = new CacheSettings() {
            Cache = Cache.Memory,
            MemoryDistributedCacheOptions = new MemoryDistributedCacheOptions()
            {
                CompactionPercentage = 0,
                ExpirationScanFrequency = TimeSpan.FromSeconds(10),
                SizeLimit = 1000,
                TrackStatistics = true,
                TrackLinkedCacheEntries = true,
            },
            RedisCacheOptions = new RedisCacheOptions()
            {
                Configuration = "",
                InstanceName = "",
                ConfigurationOptions = null,
                ConnectionMultiplexerFactory= null,
                ProfilingSession = null,
            }
        };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Cache Cache { get; set; }
        public MemoryDistributedCacheOptions MemoryDistributedCacheOptions { get; set; }
        public RedisCacheOptions RedisCacheOptions { get; set; }
    }
}
