using RetSettlementDates.Domain.DataObjects;
using System.Threading;
using System.Threading.Tasks;

namespace RetSettlementDates.Domain.Abstractions
{
    /// <summary>
    /// Data Services will keep some settlement data stored on memory in specific way
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Process each settlement
        /// </summary>
        /// <param name="item">the settlement that will be processed by this DS (data service)</param>
        /// <returns>async tast</returns>
        public Task ProcessSettlement(Settlement item, CancellationToken token);
    }
}
