using RetSettlementDates.Domain.DataObjects;

namespace RetSettlementDates.Api.Responses
{
    public class GroupSettlementResponse
    {
        public string SettlementLocation { get; }
        public string YearDate { get; }
        public float AvgPrice { get; }
        public float AvgVolume { get; }
        public float AvgTotalDollars { get; }
        public float MinPrice { get; }
        public float MaxPrice { get; }

        public GroupSettlementResponse(string location, AggSettlement group)
        {
            SettlementLocation = location;
            YearDate = group.ID;
            AvgPrice = group.AvgPrice;
            AvgVolume = group.AvgVolume;
            AvgTotalDollars = group.AvgTotalDollars;
            MinPrice = group.MinPrice;
            MaxPrice = group.MaxPrice;
        }
    }
}
