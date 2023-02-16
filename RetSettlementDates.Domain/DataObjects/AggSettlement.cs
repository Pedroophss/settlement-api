using System;

namespace RetSettlementDates.Domain.DataObjects
{
    public class AggSettlement
    {
        public string ID { get; }
        public float MinPrice { get; private set; }
        public float MaxPrice { get; private set; }
        public int Count { get; private set; }
        public float SumPrice { get; private set; }
        public float SumVolume { get; private set; }

        public AggSettlement(string Id)
        {
            ID = Id;
            MinPrice = float.MaxValue;
            MaxPrice = float.MinValue;
        }

        public float AvgPrice => SumPrice / Count;
        public float AvgVolume => SumVolume / Count;
        public float AvgTotalDollars => AvgPrice * SumVolume;

        public void AddOne(Settlement item)
        {
            MinPrice = Math.Min(MinPrice, item.PricePerMWh);
            MaxPrice = Math.Max(MaxPrice, item.PricePerMWh);

            SumPrice += item.PricePerMWh;
            SumVolume += item.VolumeMWh;

            Count++;
        }
    }
}
