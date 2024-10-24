using System;
using System.Collections;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.UnitTests.ModelsTests;

public class CourierTests
{
    [Fact]
    public void Create_StateShouldBeCorrect()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value);
        
        Assert.True(courier.IsSuccess);
        Assert.Equal("Name", courier.Value.Name);
        Assert.NotEqual(Guid.Empty, courier.Value.Id);
        Assert.Equal(CourierStatus.Free, courier.Value.Status);
        Assert.NotNull(courier.Value.Transport);
        Assert.NotNull(courier.Value.Location);
    }    
    
    [Fact]
    public void MakeBusy_ShouldSuccess()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value);
        
        var busy = courier.Value.MakeBusy();
        
        Assert.True(busy.IsSuccess);
    }
    
    [Fact]
    public void MakeBusy_DoesNotFree_ShouldFail()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value);
        _ = courier.Value.MakeBusy();
        
        var busy = courier.Value.MakeBusy();
        
        Assert.True(busy.IsFailure);
    }       
    
    [Fact]
    public void MakeBusy_ShouldSetBusyStatus()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value);
        
        _ = courier.Value.MakeBusy();
        
        Assert.Equal(CourierStatus.Busy, courier.Value.Status);
    }    
    
    [Fact]
    public void MakeFree_ShouldSuccess()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value);
        _ = courier.Value.MakeBusy();
        
        var free = courier.Value.MakeFree();
        
        Assert.True(free.IsSuccess);
    }
    
    [Fact]
    public void MakeFree_DoesNotBusy_ShouldFail()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value);
        
        var free = courier.Value.MakeFree();
        
        Assert.True(free.IsFailure);
    }  
    
    [Fact]
    public void MakeFree_ShouldSetFreeStatus()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value);
        _ = courier.Value.MakeBusy();
        
        _ = courier.Value.MakeFree();
        
        Assert.Equal(CourierStatus.Free, courier.Value.Status);
    }
    
    [Fact]
    public void StepsToLocation_NullLocation_ShouldFail()
    {
        var loc = Location.Create(1, 1).Value;
        var courier = Courier.Create("Name", Transport.Bicycle, loc).Value;

        var dist = courier.StepsToLocation(null as Location);
        
        Assert.True(dist.IsFailure);
        Assert.NotNull(dist.Error);
    }
    
    [Fact]
    public void StepsToLocation_Bicycle_ShouldReturnCorrectValue()
    {
        var loc = Location.Create(1, 1).Value;
        var courier = Courier.Create("Name", Transport.Bicycle, loc).Value;
        var targetLocation = Location.Create(5, 5).Value;

        var dist = courier.StepsToLocation(targetLocation);
        
        Assert.Equal(4d, dist.Value);
    }

    [Fact]
    public void Move_NullLocation_ShouldFail()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.CreateRandom().Value).Value;

        var move = courier.Move(null as Location);
        
        Assert.True(move.IsFailure);
        Assert.NotNull(move.Error);
    }

    [Theory]
    [ClassData(typeof(MoveMatrixPedestrian))]
    public void Move_Pedestrian_ShouldSetExpectedCurrentLocation(Location current, Location target, Location expected)
    {
        var courier = Courier.Create("Name", Transport.Pedestrian, current).Value;

        var move = courier.Move(target);
        
        Assert.True(move.IsSuccess);
        Assert.Equal(expected, courier.Location);
    }    
    
    [Theory]
    [ClassData(typeof(MoveMatrixBicycle))]
    public void Move_Bicycle_ShouldSetExpectedCurrentLocation(Location current, Location target, Location expected)
    {
        var courier = Courier.Create("Name", Transport.Bicycle, current).Value;

        var move = courier.Move(target);
        
        Assert.True(move.IsSuccess);
        Assert.Equal(expected, courier.Location);
    }
}

/// <summary>
/// https://gitlab.com/microarch-ru/ddd-in-practice/templates/net/delivery-template/-/blob/module-4/Tests/DeliveryApp.UnitTests/Domain/Model/CourierAggregate/CourierTest.cs
/// </summary>
internal class MoveMatrixPedestrian : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
       // Пешеход, заказ X:совпадает, Y: совпадает
        yield return
        [
            Location.Create(1, 1).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value
        ];
        yield return
        [
            Location.Create(5, 5).Value, Location.Create(5, 5).Value, Location.Create(5, 5).Value
        ];

        // Пешеход, заказ X:совпадает, Y: выше
        yield return
        [
            Location.Create(1, 1).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value
        ];
        yield return
        [
            Location.Create(1, 1).Value, Location.Create(1, 5).Value, Location.Create(1, 2).Value
        ];

        // Пешеход, заказ X:правее, Y: совпадает
        yield return
        [
            Location.Create(2, 2).Value, Location.Create(3, 2).Value, Location.Create(3, 2).Value
        ];
        yield return
        [
            Location.Create(5, 5).Value, Location.Create(6, 5).Value, Location.Create(6, 5).Value
        ];

        // Пешеход, заказ X:правее, Y: выше
        yield return
        [
           Location.Create(2, 2).Value, Location.Create(3, 3).Value, Location.Create(3, 2).Value
        ];
        yield return
        [
            Location.Create(1, 1).Value, Location.Create(5, 5).Value, Location.Create(2, 1).Value
        ];

        // Пешеход, заказ X:совпадает, Y: ниже
        yield return
        [
            Location.Create(1, 2).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value
        ];
        yield return
        [
            Location.Create(5, 5).Value, Location.Create(5, 1).Value, Location.Create(5, 4).Value
        ];

        // Пешеход, заказ X:левее, Y: совпадает
        yield return
        [
            Location.Create(2, 2).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value
        ];
        yield return
        [
            Location.Create(5, 5).Value, Location.Create(1, 5).Value, Location.Create(4, 5).Value
        ];

        // Пешеход, заказ X:левее, Y: ниже
        yield return
        [Location.Create(2, 2).Value, Location.Create(1, 1).Value, Location.Create(1, 2).Value];
        yield return
        [Location.Create(5, 5).Value, Location.Create(1, 1).Value, Location.Create(4, 5).Value];

    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// https://gitlab.com/microarch-ru/ddd-in-practice/templates/net/delivery-template/-/blob/module-4/Tests/DeliveryApp.UnitTests/Domain/Model/CourierAggregate/CourierTest.cs
/// </summary>
internal class MoveMatrixBicycle : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [Location.Create(1, 1).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(5, 5).Value, Location.Create(5, 5).Value];

        // Велосипедист, заказ X:совпадает, Y: выше
        yield return [Location.Create(1, 1).Value, Location.Create(1, 3).Value, Location.Create(1, 3).Value];
        yield return [Location.Create(1, 1).Value, Location.Create(1, 5).Value, Location.Create(1, 3).Value];

        // Велосипедист, заказ X:правее, Y: совпадает
        yield return [Location.Create(2, 2).Value, Location.Create(4, 2).Value, Location.Create(4, 2).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(8, 5).Value, Location.Create(7, 5).Value];

        // Велосипедист, заказ X:правее, Y: выше
        yield return [Location.Create(2, 2).Value, Location.Create(4, 4).Value, Location.Create(4, 2).Value];
        yield return [Location.Create(1, 1).Value, Location.Create(5, 5).Value, Location.Create(3, 1).Value];

        // Велосипедист, заказ X:совпадает, Y: ниже
        yield return [Location.Create(1, 3).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(5, 1).Value, Location.Create(5, 3).Value];

        // Велосипедист, заказ X:левее, Y: совпадает
        yield return [Location.Create(3, 2).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(1, 5).Value, Location.Create(3, 5).Value];

        // Велосипедист, заказ X:левее, Y: ниже
        yield return [Location.Create(3, 3).Value, Location.Create(1, 1).Value, Location.Create(1, 3).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(1, 1).Value, Location.Create(3, 5).Value];

        // Велосипедист, заказ ближе чем скорость
        yield return [Location.Create(1, 1).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value];
        yield return [Location.Create(1, 1).Value, Location.Create(2, 1).Value, Location.Create(2, 1).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(5, 4).Value, Location.Create(5, 4).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(4, 5).Value, Location.Create(4, 5).Value];

        // Велосипедист, заказ с шагами по 2 осям
        yield return [Location.Create(1, 1).Value, Location.Create(2, 2).Value, Location.Create(2, 2).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(4, 4).Value, Location.Create(4, 4).Value];
        yield return [Location.Create(1, 1).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value];
        yield return [Location.Create(5, 5).Value, Location.Create(5, 4).Value, Location.Create(5, 4).Value];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}