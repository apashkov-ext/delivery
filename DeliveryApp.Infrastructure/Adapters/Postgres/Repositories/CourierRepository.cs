using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

internal class CourierRepository : ICourierRepository
{
    private readonly ApplicationDbContext _db;

    public CourierRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<UnitResult<Error>> AddAsync(Courier courier, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(courier);
        
        _db.Attach(courier.Status);
        _db.Attach(courier.Transport);
        await _db.Couriers.AddAsync(courier, ct);
        
        return UnitResult.Success<Error>();
    }

    public Task<UnitResult<Error>> UpdateAsync(Courier courier, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(courier);

        _db.Attach(courier.Status);
        _db.Attach(courier.Transport);
        _db.Couriers.Update(courier);
        
        return Task.FromResult(UnitResult.Success<Error>());
    }

    public async Task<Maybe<Courier>> FindByIdAsync(Guid courierId, CancellationToken ct = default)
    {
        var courier = await _db.Couriers
            .Include(x => x.Status)
            .Include(x => x.Transport)
            .FirstOrDefaultAsync(x => x.Id == courierId, ct);
        
        return courier;
    }

    public async Task<ReadOnlyCollection<Courier>> FindFreeAsync(CancellationToken ct = default)
    {
        var couriers = await _db.Couriers
            .Include(x => x.Status)
            .Include(x => x.Transport)
            .Where(x => x.Status == CourierStatus.Free)
            .ToListAsync(ct);

        return couriers.AsReadOnly();
    }

    public async Task<ReadOnlyCollection<Courier>> FindBusyAsync(CancellationToken ct = default)
    {
        var couriers = await _db.Couriers
            .Include(x => x.Status)
            .Include(x => x.Transport)
            .Where(x => x.Status == CourierStatus.Busy)
            .ToListAsync(ct);

        return couriers.AsReadOnly();
    }
}