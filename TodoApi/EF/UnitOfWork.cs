using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApi.EF
{
    internal interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken token);
        Task RollbackAsync(CancellationToken _);
    }

    internal class UnitOfWork : IUnitOfWork
    {
        public TradeContext Ctx { get; }

        public UnitOfWork(TradeContext ctx)
        {
            Ctx = ctx;
        }

        public async Task CommitAsync(CancellationToken token) =>
            await Ctx.SaveChangesAsync(token);

        public Task RollbackAsync(CancellationToken _)
        {
            foreach (var entry in Ctx.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
