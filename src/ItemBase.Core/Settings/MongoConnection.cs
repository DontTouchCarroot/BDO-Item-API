using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core.Settings
{
    public class MongoConnection
    {
        public readonly static MongoConnection Default = new MongoConnection()
        { 
            ConnectionString = "",
            DatabaseName = "",
        };
        public required string ConnectionString { get; init; }

        public required string DatabaseName { get; init; }

    }
}
