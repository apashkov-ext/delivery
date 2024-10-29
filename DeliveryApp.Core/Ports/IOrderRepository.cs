using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface IOrderRepository : IRepository<Order>
{
    Task<UnitResult<Error>> AddAsync(Order order, CancellationToken ct = default);
    
    Task<UnitResult<Error>> UpdateAsync(Order order, CancellationToken ct = default);
    
    Task<Maybe<Order>> FindByIdAsync(Guid orderId, CancellationToken ct = default);
    
    Task<ReadOnlyCollection<Order>> FindCreatedAsync(CancellationToken ct = default);
    
    Task<ReadOnlyCollection<Order>> FindAssignedAsync(CancellationToken ct = default);
}