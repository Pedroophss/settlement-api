using System.Threading.Tasks;
using TodoApi.EF;

namespace TodoApi.Repositories
{
    internal class TradeRepository
    {
        public TradeContext Ctx { get; }

        public TradeRepository(TradeContext ctx)
        {
            Ctx = ctx;
        }


    }
}
