using ItemBase.Resources;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItemBase.Core.Settings
{
    
    public enum Database
    {
        None,
        Mongo,
    }
    public class CoreSettings
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Database Database { get; init; }

        public MongoConnection MongoConnection { get; init; }

        public readonly static CoreSettings Default = new CoreSettings()
        {
            CacheSettings = CacheSettings.Default,
            MongoConnection = MongoConnection.Default,
            Database = Database.None,
            ResourcesSettings = ResourcesSettings.Default,
        };

        public  CacheSettings CacheSettings { get; init; }

        public ResourcesSettings ResourcesSettings { get; init; }

       
        

        

    }
}
