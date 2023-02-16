using System;

namespace RetSettlementDates.Domain.DataObjects
{
    public class Settlement
    {
        public static Settlement InvalidRef =
            new Settlement(string.Empty, 0, DateTime.MinValue, 0, 0, DateTime.MinValue, DateTime.MinValue);

        public string SettlementLocationName { get; }
        public int SettlementLocationID { get; }
        public DateTime DateOfService { get; }
        public float PricePerMWh { get; }
        public float VolumeMWh { get; }
        public DateTime InsertDate { get; }
        public DateTime ModifiedDate { get; }

        public Settlement
        (
            string settlementLocationName,
            int settlementLocationID, 
            DateTime dateOfService, 
            float pricePerMWh, 
            float volumeMWh, 
            DateTime insertDate, 
            DateTime modifiedDate
        )
        {
            SettlementLocationName = settlementLocationName;
            SettlementLocationID = settlementLocationID;
            DateOfService = dateOfService;
            PricePerMWh = pricePerMWh;
            VolumeMWh = volumeMWh;
            InsertDate = insertDate;
            ModifiedDate = modifiedDate;
        }
    }
}
