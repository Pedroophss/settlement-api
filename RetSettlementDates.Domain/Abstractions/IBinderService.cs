using System.Threading;
using System.Threading.Tasks;

namespace RetSettlementDates.Domain.Abstractions
{
    /// <summary>
    /// Binders will provide settlement for the data services
    /// </summary>
    public interface IBinderService
    {
        /// <summary>
        /// Bind the domain using some source
        /// </summary>
        Task Bind(CancellationToken token);
    }
}
