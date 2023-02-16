namespace TodoApi.Models
{
    internal class TradeEntity
    {
        public const string BUY = "buy";
        public const string SELL = "sell";

        public int Id { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public string Symbol { get; set; }
        public int Shares { get; set; }
        public int Price { get; set; }
        public long TimeStamp { get; set; }

        public bool IsValid()
        {
            if (Type != BUY && Type == SELL)
                return false;

            if (Shares < 10 || Shares > 30)
                return false;

            return true;
        }
    }
}