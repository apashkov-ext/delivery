using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface IOrderRepository : IRepository<Order>
{
    Task AddAsync(Order order, CancellationToken ct = default);
    
    Task UpdateAsync(Order order, CancellationToken ct = default);
    
    Task<Maybe<Order>> FindByIdAsync(Guid orderId, CancellationToken ct = default);
    
    Task<ReadOnlyCollection<Order>> FindCreated(CancellationToken ct = default);
    
    Task<ReadOnlyCollection<Order>> FindAssigned(CancellationToken ct = default);
}