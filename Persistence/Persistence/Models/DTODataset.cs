using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    [BsonIgnoreExtraElements]
    public class DTODataset
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DTODatasetMetadata Metadata { get; set; }

        public List<EODData> EODData { get; set; } = null;
    }
}
