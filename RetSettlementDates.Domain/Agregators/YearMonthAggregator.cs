using RetSettlementDates.Domain.Abstractions;
using RetSettlementDates.Domain.DataObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RetSettlementDates.Domain.Agregators
{
    public interface IYearMonthAggregator
    {
        IEnumerable<(string location, AggSettlement month)> QueryByMonth(string location);
        IEnumerable<(string location, AggSettlement year)> QueryByYear(string location);
    }

    internal class YearMonthAggregator : IDataService, IYearMonthAggregator
    {
        private Dictionary<string, Dictionary<int, AggSettlement>> ByLocationYear { get; }
        private Dictionary<string, Dictionary<int, AggSettlement>> ByLocationMonth { get; }

        public YearMonthAggregator()
        {
            ByLocationYear = new Dictionary<string, Dictionary<int, AggSettlement>>();
            ByLocationMonth = new Dictionary<string, Dictionary<int, AggSettlement>>();
        }

        public Task ProcessSettlement(Settlement item, CancellationToken token)
        {
            var yearID = $"1/1/{item.DateOfService.Year}";
            var monthID = $"1/{item.DateOfService.Month}/{item.DateOfService.Year}";

            if (!ByLocationYear.TryGetValue(item.SettlementLocationName, out Dictionary<int, AggSettlement> timeIdx))
            {
                timeIdx = new Dictionary<int, AggSettlement>();
                ByLocationYear[item.SettlementLocationName] = timeIdx;
            }

            AddTimeAgg(timeIdx, yearID, item.DateOfService.Year, item);

            if (!ByLocationMonth.TryGetValue(item.SettlementLocationName, out timeIdx))
            {
                timeIdx = new Dictionary<int, AggSettlement>();
                ByLocationMonth[item.SettlementLocationName] = timeIdx;
            }

            AddTimeAgg(timeIdx, monthID, item.DateOfService.Year, item);

            return Task.CompletedTask;
        }

        public static void AddTimeAgg(Dictionary<int, AggSettlement> index, string aggID, int id, Settlement item)
        {
            if (!index.TryGetValue(id, out AggSettlement agg))
                agg = new AggSettlement(aggID);

            agg.AddOne(item);
            index[id] = agg;
        }

        public IEnumerable<(string location, AggSettlement year)> QueryByYear(string location) =>
            Query(ByLocationYear, location);

        public IEnumerable<(string location, AggSettlement month)> QueryByMonth(string location) =>
            Query(ByLocationMonth, location);

        public static IEnumerable<(string location, AggSettlement agg)> Query(
            Dictionary<string, Dictionary<int, AggSettlement>> idx, string location)
        {
            if (!string.IsNullOrEmpty(location))
                foreach (var locationIdx in idx)
                    foreach (var yearAgg in locationIdx.Value)
                        yield return (locationIdx.Key, yearAgg.Value);

            yield break;
        }
    }
}
