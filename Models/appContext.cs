using Microsoft.EntityFrameworkCore;
using Alpaca.Markets;


namespace StockMarket.Models
{
    public class appContext : DbContext
    {
        public appContext(DbContextOptions<appContext> options) : base(options) { }
    }
}
