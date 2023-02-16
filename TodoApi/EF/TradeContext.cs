using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.EF
{
    internal class TradeContext : DbContext
    {
        public TradeContext(DbContextOptions<TradeContext> options)
            : base(options)
        {
        }

        public DbSet<TradeEntity> Trades { get; set; }
    }
}