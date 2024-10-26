using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.UnitTests.ModelsTests;

public class OrderTests
{
    [Fact]
    public void Create_GuidEmpty_ShouldFail()
    {
        var order = Order.Create(Guid.Empty, Location.CreateRandom().Value);
        Assert.True(order.IsFailure);
    }
    
    [Fact]
    public void Create_ShouldBeInCreatedStatus()
    {
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        
        Assert.True(order.IsSuccess);
        Assert.Equal(OrderStatus.Created, order.Value.Status);
    }
    
    [Fact]
    public void Assign_NullCourier_ShouldFail()
    {
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        
        var assign = order.Value.Assign(null as Courier);
        
        Assert.True(assign.IsFailure);
        Assert.NotNull(assign.Error);
    }  
    
    [Fact]
    public void Assign_ShouldSuccess()
    {
        var courier = Courier.Create("Courier", Transport.Car, Location.CreateRandom().Value);
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        
        var assign = order.Value.Assign(courier.Value);
        
        Assert.True(assign.IsSuccess);
    }  
    
    [Fact]
    public void Assign_ShouldSetStatus()
    {
        var courier = Courier.Create("Courier", Transport.Car, Location.CreateRandom().Value);
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        
        _ = order.Value.Assign(courier.Value);
        
        Assert.Equal(OrderStatus.Assigned, order.Value.Status);
    }
    
    [Fact]
    public void Assign_ShouldSetCourierId()
    {
        var courier = Courier.Create("Courier", Transport.Car, Location.CreateRandom().Value);
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        
        _ = order.Value.Assign(courier.Value);
        
        Assert.Equal(order.Value.CourierId, courier.Value.Id);
    }    
    
    [Fact]
    public void Assign_Assigned_ShouldFail()
    {
        var courier = Courier.Create("Courier", Transport.Car, Location.CreateRandom().Value);
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        _ = order.Value.Assign(courier.Value);
        
        var assign = order.Value.Assign(courier.Value);
        
        Assert.True(assign.IsFailure);
    }
    
    [Fact]
    public void Complete_NotAssigned_ShouldFail()
    {
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        
        var complete = order.Value.Complete();
        
        Assert.True(complete.IsFailure);
    }
    
    [Fact]
    public void Complete_ShouldSuccess()
    {
        var courier = Courier.Create("Courier", Transport.Car, Location.CreateRandom().Value);
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        _ = order.Value.Assign(courier.Value);
        
        var complete = order.Value.Complete();
        
        Assert.True(complete.IsSuccess);
    }
    
    [Fact]
    public void Complete_ShouldSetStatusToCompleted()
    {
        var courier = Courier.Create("Courier", Transport.Car, Location.CreateRandom().Value);
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        _ = order.Value.Assign(courier.Value);
        
        _ = order.Value.Complete();
        
        Assert.Equal(OrderStatus.Completed, order.Value.Status);
    }
    
    [Fact]
    public void Complete_ShouldSetCourierIdToNull()
    {
        var courier = Courier.Create("Courier", Transport.Car, Location.CreateRandom().Value);
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value);
        _ = order.Value.Assign(courier.Value);
        
        _ = order.Value.Complete();
        
        Assert.Null(order.Value.CourierId);
    }
}