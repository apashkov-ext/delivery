using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface ICourierRepository : IRepository<Courier>
{
    Task AddAsync(Courier courier, CancellationToken ct = default);
    
    Task UpdateAsync(Courier courier, CancellationToken ct = default);
    
    Task<Maybe<Courier>> FindByIdAsync(Guid courierId, CancellationToken ct = default);
    
    Task<ReadOnlyCollection<Courier>> FindFree(CancellationToken ct = default);
}