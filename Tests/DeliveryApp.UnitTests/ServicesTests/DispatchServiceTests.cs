using System;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.UnitTests.ServicesTests;

public class DispatchServiceTests
{
    [Fact]
    public void Dispatch_InvalidStatusOrder_ShouldReturnError()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        _ = order.Assign(courier);
        Courier[] couriers = [];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.True(dispatch.IsFailure);
        Assert.Equal(DispatchService.Errors.InvalidOrderStatus(), dispatch.Error);
    }    
    
    [Fact]
    public void Dispatch_EmptyCouriers_ShouldReturnError()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        Courier[] couriers = [];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.True(dispatch.IsFailure);
        Assert.Equal(DispatchService.Errors.NoAnyCouriers(), dispatch.Error);
    }    
    
    [Fact]
    public void Dispatch_NoAnyFreeCourier_ShouldReturnError()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        _ = courier.MakeBusy();
        Courier[] couriers = [ courier ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.True(dispatch.IsFailure);
        Assert.Equal(DispatchService.Errors.NoFreeCouriers(), dispatch.Error);
    }    
    
    [Fact]
    public void Dispatch_ShouldAssignCourierToOrder()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        Courier[] couriers = [ courier ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.True(dispatch.IsSuccess);
        Assert.Equal(courier.Id, order.CourierId);
    }      
    
    [Fact]
    public void Dispatch_ShouldSetAssignedOrderStatus()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        Courier[] couriers = [ courier ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.Equal(OrderStatus.Assigned, order.Status);
    }     
    
    [Fact]
    public void Dispatch_ShouldMakeCourierBusy()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        Courier[] couriers = [ courier ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.Equal(CourierStatus.Busy, courier.Status);
    }    
    
    [Fact]
    public void Dispatch_ShouldReturnAssignedCourier()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        Courier[] couriers = [ courier ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.Equal(courier, dispatch.Value);
    }   
    
    /*
        The fastest courier is C.
        A o o o o o o o o o
        o o o B o o o o o o
        o o o o o x o o o o
        o o o o o C o o o o
        o o o o o o o o o o
        o o o o o o o o o o
     */
    
    [Fact]
    public void Dispatch_ShouldSelectTheFastestCourierC()
    {
        var order = Order.Create(Guid.NewGuid(), Location.Create(6, 3).Value).Value;
        var courierA = Courier.Create("Courier A", Transport.Car, Location.Create(1, 1).Value).Value;
        var courierB = Courier.Create("Courier B", Transport.Bicycle, Location.Create(4, 2).Value).Value;
        var courierC = Courier.Create("Courier C", Transport.Pedestrian, Location.Create(6, 4).Value).Value;
        Courier[] couriers = [ courierA, courierB, courierC ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.Equal(courierC, dispatch.Value);
    }
    
    /*
        The fastest courier is A.
        o A o o o o o o o o
        o o o o o o o o o o
        o o o o o x o o o C
        o o o o o o o o o o
        o o o o o o o o o o
        o o o o o o o o o o
    */
    
    [Fact]
    public void Dispatch_ShouldSelectTheFastestCourierA()
    {
        var order = Order.Create(Guid.NewGuid(), Location.Create(6, 3).Value).Value;
        var courierA = Courier.Create("Courier A", Transport.Car, Location.Create(2, 1).Value).Value;
        var courierB = Courier.Create("Courier B", Transport.Pedestrian, Location.Create(10, 4).Value).Value;
        Courier[] couriers = [ courierA, courierB ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.Equal(courierA, dispatch.Value);
    }
    
    /*
        Fastest couriers are A and B.
        o o o o o A o o o o
        o o o o o o o o o o
        o B o o o x o o o o
        o o o o o o o o o o
        o o o o o o o o o o
        o o o o o o o o o o
    */
    [Fact]
    public void Dispatch_ShouldSelectFirstRelevantCourierA()
    {
        var order = Order.Create(Guid.NewGuid(), Location.Create(6, 3).Value).Value;
        var courierA = Courier.Create("Courier A", Transport.Pedestrian, Location.Create(6, 1).Value).Value;
        var courierB = Courier.Create("Courier B", Transport.Bicycle, Location.Create(2, 3).Value).Value;
        Courier[] couriers = [ courierA, courierB ];
        var srv = new DispatchService();

        var dispatch = srv.Dispatch(order, couriers.AsReadOnly());
        
        Assert.Equal(courierA, dispatch.Value);
    }
}