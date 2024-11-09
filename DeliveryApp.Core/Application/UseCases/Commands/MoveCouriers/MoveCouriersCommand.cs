using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public class MoveCouriersCommand : IRequest;

internal sealed class MoveCouriersCommandHandler : IRequestHandler<MoveCouriersCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICourierRepository _couriers;
    private readonly IOrderRepository _orders;
    private readonly ILogger<MoveCouriersCommandHandler> _logger;

    public MoveCouriersCommandHandler(IUnitOfWork uow,
        ICourierRepository couriers,
        IOrderRepository orders,
        ILogger<MoveCouriersCommandHandler> logger)
    {
        _uow = uow;
        _couriers = couriers;
        _orders = orders;
        _logger = logger;
    }
    
    public async Task Handle(MoveCouriersCommand request, CancellationToken ct = default)
    {
        var busyCouriers = await _couriers.FindBusyAsync(ct);
        if (busyCouriers.Count == 0)
        {
            return;
        }

        var orders = await _orders.FindAssignedAsync(ct);
        foreach (var courier in busyCouriers)
        {
            var order = orders.FirstOrDefault(x => x.CourierId == courier.Id);
            if (order is null)
            {
                continue;
            }

            // already on recipient position.
            if (courier.Location == order.TargetLocation)
            {
                await CompleteOrder(order, courier);
                continue;
            }

            courier.Move(order.TargetLocation);
            if (courier.Location == order.TargetLocation)
            {
                await CompleteOrder(order, courier);
                continue;
            }
            
            await _couriers.UpdateAsync(courier);
        }

        await _uow.SaveChangesAsync(ct);
    }

    private async Task CompleteOrder(Order order, Courier courier)
    {
        using var loggerScope = _logger.BeginScope(new Dictionary<string, object>
        {
            { "OrderId", order.Id },
            { "CourierId", courier.Id }
        });
        
        var complete = order.Complete();
        if (complete.IsFailure)
        { 
            _logger.LogWarning("Failed to complete order: {Error}", complete.Error);
            return;
        }
        
        var free = courier.MakeFree();
        if (free.IsFailure)
        { 
            _logger.LogWarning("Failed to complete order: {Error}", free.Error);
            return;
        }

        await _orders.UpdateAsync(order);
        await _couriers.UpdateAsync(courier);
        await _uow.SaveChangesAsync();
    }
}