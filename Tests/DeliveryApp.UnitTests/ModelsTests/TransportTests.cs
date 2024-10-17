using System.Linq;
using DeliveryApp.Core.Domain.Models.CourierAggregate;

namespace DeliveryApp.UnitTests.ModelsTests;

public class TransportTests
{
    [Fact]
    public void Pedestrian_ShouldHasCorrectValues()
    {
        var pedestrian = Transport.Pedestrian;
        
        Assert.Equal(1, pedestrian.Speed);
        Assert.Equal("pedestrian", pedestrian.Name);
    }
    
    [Fact]
    public void Bicycle_ShouldHasCorrectValues()
    {
        var bicycle = Transport.Bicycle;
        
        Assert.Equal(2, bicycle.Speed);
        Assert.Equal("bicycle", bicycle.Name);
    }
    
    [Fact]
    public void Car_ShouldHasCorrectValues()
    {
        var car = Transport.Car;
        
        Assert.Equal(3, car.Speed);
        Assert.Equal("car", car.Name);
    }

    [Theory]
    [InlineData("Pedestrian")]
    [InlineData("Bicycle")]
    [InlineData("Car")]
    public void FromName_ShouldReturnTransportByCaseInsensitiveName(string name)
    {
        var tr = Transport.FromName(name);
        Assert.NotNull(tr.GetValueOrDefault());
    }   
    
    [Theory]
    [InlineData("pedestrian", 1)]
    [InlineData("bicycle", 2)]
    [InlineData("car", 3)]
    public void FromName_ShouldReturnTransportWithCorrectId(string name, int expectedId)
    {
        var tr = Transport.FromName(name);
        Assert.Equal(expectedId, tr.Value.Id);
    }
    
    [Fact]
    public void FromName_UnknownName_ShouldReturnError()
    {
        var tr = Transport.FromName("myname");
        
        Assert.True(tr.IsFailure);
        Assert.Equal(Transport.Errors.UnknownTransportName(), tr.Error);
    }
    
    [Theory]
    [InlineData(1, "pedestrian")]
    [InlineData(2, "bicycle")]
    [InlineData(3, "car")]
    public void FromId_ShouldReturnTransportWithCorrectName(int id, string expectedName)
    {
        var tr = Transport.FromId(id);
        Assert.Equal(expectedName, tr.Value.Name);
    }    
    
    [Fact]
    public void FromId_UnknownId_ShouldReturnError()
    {
        var tr = Transport.FromId(888);
        
        Assert.True(tr.IsFailure);
        Assert.Equal(Transport.Errors.UnknownTransportId(), tr.Error);
    }
    
    [Fact]
    public void Equals_ShouldReturnTrue()
    {
        var car1 = Transport.FromName("car");
        var car2 = Transport.FromName("car");
        Assert.Equal(car1, car2);
    }

    [Fact]
    public void List_ShouldReturnKnownKindsOfTransport()
    {
        var list = Transport.List().ToArray();

        Assert.Contains(Transport.Pedestrian, list);
        Assert.Contains(Transport.Bicycle, list);
        Assert.Contains(Transport.Car, list);
    }
}