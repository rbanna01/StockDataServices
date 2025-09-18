namespace StockDataExternalSource.Models
{
    public class EODResponse
    {
        public Pagination Pagination { get; set; }

        public Item[] Data { get; set; }

    }
}
