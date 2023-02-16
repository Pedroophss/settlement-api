using RetSettlementDates.Domain.Abstractions;
using RetSettlementDates.Domain.DataObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RetSettlementDates.Domain.Indexes
{
    public interface ILocationDateIndex
    {
        IEnumerable<Settlement> Query(string location, DateTime start, DateTime end);
    }

    public class LocationDateIndex : IDataService, ILocationDateIndex
    {
        Dictionary<string, Dictionary<DateTime, List<Settlement>>> ByLocationAndDate { get; }
        Dictionary<DateTime, List<Settlement>> ByDate { get; }

        public LocationDateIndex()
        {
            ByLocationAndDate = new Dictionary<string, Dictionary<DateTime, List<Settlement>>>();
            ByDate = new Dictionary<DateTime, List<Settlement>>();
        }

        public Task ProcessSettlement(Settlement item, CancellationToken token)
        {
            IndexLocationAndDate(item);
            IndexDates(ByDate, item);

            return Task.CompletedTask;
        }

        private void IndexLocationAndDate(Settlement item)
        {
            var name = item.SettlementLocationName;

            if (ByLocationAndDate.TryGetValue(name, out Dictionary<DateTime, List<Settlement>> dateIdx))
            {
                IndexDates(dateIdx, item);
                return;
            }

            ByLocationAndDate[name] = new Dictionary<DateTime, List<Settlement>>
            {
                { item.DateOfService, new List<Settlement> { item } }
            };
        }

        private static void IndexDates(Dictionary<DateTime, List<Settlement>> dates, Settlement item)
        {
            if (dates.TryGetValue(item.DateOfService, out List<Settlement> items))
            {
                items.Add(item);
                return;
            }

            dates[item.DateOfService] = new List<Settlement> { item };
        }

        public IEnumerable<Settlement> Query(string location, DateTime start, DateTime end)
        {
            if (string.IsNullOrEmpty(location))
                foreach (var locationIdx in ByLocationAndDate)
                    foreach(var item in QueryByDates(locationIdx.Value, start, end))
                        yield return item;


            else if (ByLocationAndDate.TryGetValue(location, out Dictionary<DateTime, List<Settlement>> dates))
                foreach (var item in QueryByDates(ByDate, start, end))
                    yield return item;

            yield break;
        }

        private static IEnumerable<Settlement> QueryByDates(
            Dictionary<DateTime, List<Settlement>> dates, DateTime start, DateTime end)
        {
            var sanitizedStart = new DateTime(start.Year, start.Month, start.Day);
            var sanitizedEnd = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);

            foreach(var date in dates)
            {
                if (date.Key >= sanitizedStart && date.Key <= sanitizedEnd)
                    foreach (var item in date.Value)
                        yield return item;
            }
        }
    }
}
