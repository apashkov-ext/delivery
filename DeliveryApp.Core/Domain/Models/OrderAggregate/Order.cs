using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

public sealed class Order : Aggregate
{
    public Guid? CourierId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Location TargetLocation { get; }
    
    private Order() { }

    private Order(Guid id, Location targetLocation, OrderStatus status)
    {
        Id = id;
        TargetLocation = targetLocation;
        Status = status;
    }

    /// <summary>
    /// Creates an order instance.
    /// </summary>
    /// <param name="basketId">Basket identifier.</param>
    /// <param name="targetLocation">Target location.</param>
    /// <returns></returns>
    public static Result<Order, Error> Create(Guid basketId, 
        Location targetLocation)
    {
        if (basketId == Guid.Empty)
        {
            return GeneralErrors.ValueIsInvalid(nameof(basketId));
        }

        if (targetLocation is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(targetLocation));
        }
        
        return new Order(basketId, 
            targetLocation, 
            OrderStatus.Created);
    }
    
    public static Result<Order, Error> Create(Guid basketId, 
        Result<Location> targetLocation)
    {
        if (basketId == Guid.Empty)
        {
            return GeneralErrors.ValueIsInvalid(nameof(basketId));
        }

        if (targetLocation.Value is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(targetLocation));
        }
        
        return new Order(basketId, 
            targetLocation.Value, 
            OrderStatus.Created);
    }

    public UnitResult<Error> Assign(Courier courier)
    {
        if (courier is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(courier));
        }

        if (Status == OrderStatus.Assigned)
        {
            return Errors.OrderAlreadyAssigned();
        }

        CourierId = courier.Id;
        Status = OrderStatus.Assigned;

        return UnitResult.Success<Error>();
    }    
    
    public UnitResult<Error> Complete()
    {
        if (Status != OrderStatus.Assigned)
        {
            return Errors.ImpossibleToComplete();
        }

        CourierId = null;
        Status = OrderStatus.Completed;
        
        return UnitResult.Success<Error>();
    }
    
    public class Errors
    {
        public static Error OrderAlreadyAssigned()
        {
            return new Error("order.already.assigned", "The Order has already been assigned");
        }             
        
        public static Error ImpossibleToComplete()
        {
            return new Error("order.impossible.to.complete", "It is impossible to complete an Order: it is not in an Assigned status");
        }        
    }
}