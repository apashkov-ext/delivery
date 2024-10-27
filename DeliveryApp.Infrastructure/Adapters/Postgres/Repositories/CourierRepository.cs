using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Ports;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

internal class CourierRepository : ICourierRepository
{
    public Task AddAsync(Courier courier, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Courier courier, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Maybe<Courier>> FindByIdAsync(Guid courierId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ReadOnlyCollection<Courier>> FindFree(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}