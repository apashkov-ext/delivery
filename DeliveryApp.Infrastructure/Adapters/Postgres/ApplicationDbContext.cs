using System.Reflection;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

internal class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<Courier> Couriers { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<OrderStatus>(x =>
        {
            var statuses = OrderStatus.List();
            x.HasData(statuses.Select(s => new { s.Id, s.Name }));
        });
        
        modelBuilder.Entity<CourierStatus>(x =>
        {
            var statuses = CourierStatus.List();
            x.HasData(statuses.Select(s => new { s.Id, s.Name }));
        });        
        
        modelBuilder.Entity<Transport>(x =>
        {
            var transports = Transport.List();
            x.HasData(transports.Select(s => new { s.Id, s.Name, s.Speed }));
        });
    }
}