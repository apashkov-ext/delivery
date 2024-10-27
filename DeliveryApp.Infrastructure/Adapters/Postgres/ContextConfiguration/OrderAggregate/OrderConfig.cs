using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.ContextConfiguration.OrderAggregate;

internal class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.OwnsOne(x => x.TargetLocation, a =>
        {
            a.Property(p => p.X).HasColumnName("target_location_x").IsRequired();
            a.Property(p => p.Y).HasColumnName("target_location_y").IsRequired();
            a.WithOwner();
        });

        builder.HasOne(x => x.Status)
            .WithMany()
            .IsRequired()
            .HasForeignKey("status_id");

        builder.Property(x => x.CourierId)
            .HasColumnName("courier_id")
            .IsRequired(false);
    }
}