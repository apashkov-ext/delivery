using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public interface IDispatchService
{
    Result<Courier, Error> Dispatch(Order order, ReadOnlyCollection<Courier> couriers);
}

public class DispatchService : IDispatchService
{
    public Result<Courier, Error> Dispatch(Order order, ReadOnlyCollection<Courier> couriers)
    {
        if (order is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(order));
        }

        if (couriers is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(couriers));
        }

        if (order.Status != OrderStatus.Created)
        {
            return Errors.InvalidOrderStatus();
        }

        if (couriers.Count == 0)
        {
            return Errors.NoAnyCouriers();
        }

        var freeCouriers = couriers.Where(x => x.Status == CourierStatus.Free).ToArray();
        if (freeCouriers.Length == 0)
        {
            return Errors.NoFreeCouriers();
        }

        var fastest = freeCouriers
            .Select(x => new
            {
                Courier = x,
                Steps = x.StepsToLocation(order.TargetLocation)
            })
            .Where(x => x.Steps.IsSuccess)
            .MinBy(x => x.Steps.Value);

        if (fastest is null)
        {
            return Errors.NoFreeCouriers();
        }

        var assign = order.Assign(fastest.Courier)
            .Tap(() => fastest.Courier.MakeBusy());

        if (assign.IsFailure)
        {
            return assign.Error;
        }

        return fastest.Courier;
    }

    public static class Errors
    {
        public static Error InvalidOrderStatus()
        {
            return new Error("invalid.order.status", $"Order is in unexpected status. Expected status: {OrderStatus.Created}");
        }
        
        public static Error NoAnyCouriers()
        {
            return new Error("no.any.couriers", "There is no Courier to process the Order");
        }
        
        public static Error NoFreeCouriers()
        {
            return new Error("no.free.couriers", "There is no Courier in the Free status who could process the Order");
        }    
    }
}