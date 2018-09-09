

using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FlightBooking.Server.Data
{
    public class FlightBookingDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public FlightBookingDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration["db-connection-string"];
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    public class FlightBookingDbContextFactory : IDesignTimeDbContextFactory<FlightBookingDbContext>
    {   
        FlightBookingDbContext IDesignTimeDbContextFactory<FlightBookingDbContext>.CreateDbContext(string[] args)
        {
            var container = IoC.Configure();
            var lifetimeScope = container.BeginLifetimeScope();
            return lifetimeScope.Resolve<FlightBookingDbContext>();
               
        }
    }
}