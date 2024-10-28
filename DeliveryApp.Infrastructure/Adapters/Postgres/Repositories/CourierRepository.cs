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
        if (courier is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(courier));
        }

        _db.Attach(courier.Status);
        _db.Attach(courier.Transport);
        await _db.Couriers.AddAsync(courier, ct);
        
        return UnitResult.Success<Error>();
    }

    public Task<UnitResult<Error>> UpdateAsync(Courier courier, CancellationToken ct = default)
    {
        if (courier is null)
        {
            return Task.FromResult<UnitResult<Error>>(GeneralErrors.ValueIsRequired(nameof(courier)));
        }
        
        _db.Attach(courier.Status);
        _db.Attach(courier.Transport);
        _db.Couriers.Update(courier);
        
        return Task.FromResult(UnitResult.Success<Error>());
    }

    public Task<Result<Maybe<Courier>, Error>> FindByIdAsync(Guid courierId, CancellationToken ct = default)
    {
        if (courierId == Guid.Empty)
        {
            return Task.FromResult<Result<Maybe<Courier>, Error>>(GeneralErrors.ValueIsInvalid(nameof(courierId)));
        }

        var task = _db.Couriers
            .Include(x => x.Status)
            .Include(x => x.Transport)
            .FirstOrDefaultAsync(x => x.Id == courierId, ct)
            .AsMaybe();
        
        return Result.Of<Maybe<Courier>, Error>(() => task);
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
}