using DeliveryApp.Core.Domain.Models.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.ContextConfiguration.CourierAggregate;

internal class CourierStatusEntityConfig : IEntityTypeConfiguration<CourierStatus>
{
    public void Configure(EntityTypeBuilder<CourierStatus> builder)
    {
        builder.ToTable("courier_statuses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("name")
            .IsRequired();
    }
}