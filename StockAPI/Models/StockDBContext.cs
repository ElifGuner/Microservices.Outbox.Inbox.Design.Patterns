using Microsoft.EntityFrameworkCore;
using StockAPI.Models.Entities;

namespace StockAPI.Models
{
    public class StockDBContext : DbContext
    {
        public StockDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<OrderInbox> OrderInboxes { get; set; }
    }
}
