﻿using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

/*
 *
    Система сама распределяет заказы на курьеров, она берёт любой заказ в статусе Created (не распределённый) и ищет самого подходящего курьера.
    Алгоритм работы:
        За 1 раз мы диспетчеризуем только 1 заказ
        Логика диспетчеризации:
            Берём 1 заказ со статусом Created
            Берём всех курьеров в статусе Free
            Считаем время доставки заказа для каждого курьера, учитывая его текущее местоположение
            Побеждает курьер, который потенциально быстрее всего доставит заказ, его и назначаем на заказ

    Допущения:
        Считаем, что посылка (заказ) появляется у курьера сразу после назначения, ему не надо ехать на склад и потом к клиенту.
        Курьер начинает доставку из его текущего Location и завершает в Location заказа
 */
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