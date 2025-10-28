using Persistence.Interfaces;

namespace Persistence.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string CollectionName { get; set; }

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string GetCollectionName() => CollectionName;

        public string GetConnectionString() => ConnectionString;

        public string GetDatabaseName() => DatabaseName;
    }
}
