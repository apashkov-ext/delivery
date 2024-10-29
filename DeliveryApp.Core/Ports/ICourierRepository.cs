using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface ICourierRepository : IRepository<Courier>
{
    Task<UnitResult<Error>> AddAsync(Courier courier, CancellationToken ct = default);
    
    Task<UnitResult<Error>> UpdateAsync(Courier courier, CancellationToken ct = default);
    
    Task<Result<Maybe<Courier>, Error>> FindByIdAsync(Guid courierId, CancellationToken ct = default);
    
    Task<ReadOnlyCollection<Courier>> FindFreeAsync(CancellationToken ct = default);
}