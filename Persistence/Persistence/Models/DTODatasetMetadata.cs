using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    public class DTODatasetMetadata
    {
        public List<string> Symbols { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateFrom { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateTo { get; set; }
    }
}
