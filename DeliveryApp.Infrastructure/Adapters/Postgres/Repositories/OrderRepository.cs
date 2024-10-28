using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<UnitResult<Error>> AddAsync(Order order, CancellationToken ct = default)
    {
        if (order is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(order));
        }

        _db.Attach(order.Status);
        await _db.Orders.AddAsync(order, ct);
        
        return UnitResult.Success<Error>();
    }

    public Task<UnitResult<Error>> UpdateAsync(Order order, CancellationToken ct = default)
    {
        if (order is null)
        {
            return Task.FromResult<UnitResult<Error>>(GeneralErrors.ValueIsRequired(nameof(order)));
        }
        
        _db.Attach(order.Status);
        _db.Orders.Update(order);
        
        return Task.FromResult(UnitResult.Success<Error>());
    }

    public Task<Result<Maybe<Order>, Error>> FindByIdAsync(Guid orderId, CancellationToken ct = default)
    {
        if (orderId == Guid.Empty)
        {
            return Task.FromResult<Result<Maybe<Order>, Error>>(GeneralErrors.ValueIsInvalid(nameof(orderId)));
        }

        var task = _db.Orders
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == orderId, ct)
            .AsMaybe();
        
        return Result.Of<Maybe<Order>, Error>(() => task);
    }

    public async Task<ReadOnlyCollection<Order>> FindCreatedAsync(CancellationToken ct = default)
    {
        var orders = await _db.Orders
            .Include(x => x.Status)
            .Where(x => x.Status == OrderStatus.Created)
            .ToListAsync(ct);

        return orders.AsReadOnly();
    }

    public async Task<ReadOnlyCollection<Order>> FindAssignedAsync(CancellationToken ct = default)
    {
        var orders = await _db.Orders
            .Include(x => x.Status)
            .Where(x => x.Status == OrderStatus.Assigned)
            .ToListAsync(ct);

        return orders.AsReadOnly();
    }
}