using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public class MoveCouriersCommand : IRequest<UnitResult<Error>>;

internal sealed class MoveCouriersCommandHandler : IRequestHandler<MoveCouriersCommand, UnitResult<Error>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICourierRepository _couriers;
    private readonly IOrderRepository _orders;

    public MoveCouriersCommandHandler(IUnitOfWork uow,
        ICourierRepository couriers,
        IOrderRepository orders)
    {
        _uow = uow;
        _couriers = couriers;
        _orders = orders;
    }
    
    public async Task<UnitResult<Error>> Handle(MoveCouriersCommand request, CancellationToken ct = default)
    {
        var busyCouriers = await _couriers.FindBusyAsync(ct);
        if (busyCouriers.Count == 0)
        {
            return UnitResult.Success<Error>();
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
                order.Complete();
                courier.MakeFree();

                await _orders.UpdateAsync(order);
                await _couriers.UpdateAsync(courier);
                
                continue;
            }

            courier.Move(order.TargetLocation);
            if (courier.Location == order.TargetLocation)
            {
                order.Complete();
                courier.MakeFree();
                
                await _orders.UpdateAsync(order);
                await _couriers.UpdateAsync(courier);
                
                continue;
            }
            
            await _couriers.UpdateAsync(courier);
        }

        await _uow.SaveChangesAsync(ct);
        
        return UnitResult.Success<Error>();
    }
}