using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

internal class OrderRepository : IOrderRepository
{
    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Maybe<Order>> FindByIdAsync(Guid orderId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ReadOnlyCollection<Order>> FindCreated(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ReadOnlyCollection<Order>> FindAssigned(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}