namespace StockDataExternalSource.Models
{

    public class Item
    {
        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volume { get; set; }

        public decimal Adj_High { get; set; }

        public decimal Adj_Low { get; set; }

        public decimal Adj_Close { get; set; }

        public decimal Adj_Open { get; set; }

        public decimal Adj_Volume { get; set; }

        public decimal Split_Factor { get; set; }

        public decimal Dividend { get; set; }

        public string Name { get; set; }

        public string Exchange_Code { get; set; }

        public string Asset_Type { get; set; }

        public string Price_Currency { get; set; }

        public string Symbol { get; set; }

        public string Exchange { get; set; }

        public DateTimeOffset Date { get; set; }
    }

}
