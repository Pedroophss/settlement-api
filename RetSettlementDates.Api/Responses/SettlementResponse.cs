using RetSettlementDates.Domain.DataObjects;
using System;

namespace RetSettlementDates.Api.Responses
{
    public class SettlementResponse
    {
        public string SettlementLocationName { get; }
        public DateTime DateOfService { get; }
        public float PricePerMWh { get; }
        public float VolumeMWh { get; }

        public SettlementResponse(Settlement item)
        {
            SettlementLocationName = item.SettlementLocationName;
            DateOfService = item.DateOfService;
            PricePerMWh = item.PricePerMWh;
            VolumeMWh = item.VolumeMWh;
        }

        public static implicit operator SettlementResponse(Settlement item) =>
            new SettlementResponse(item);
    }
}
