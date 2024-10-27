using DeliveryApp.Core.Domain.Models.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.ContextConfiguration.CourierAggregate;

internal class CourierConfig : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.ToTable("couriers");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("name")
            .IsRequired();

        builder.OwnsOne(x => x.Location, a =>
        {
            a.Property(p => p.X).HasColumnName("location_x").IsRequired();
            a.Property(p => p.Y).HasColumnName("location_y").IsRequired();
            a.WithOwner();
        });
        
        builder.HasOne(x => x.Status)
            .WithMany()
            .IsRequired()
            .HasForeignKey("status_id");
        
        builder.HasOne(x => x.Transport)
            .WithMany()
            .IsRequired()
            .HasForeignKey("transport_id");
    }
}