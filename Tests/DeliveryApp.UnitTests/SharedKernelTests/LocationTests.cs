using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.UnitTests.SharedKernelTests;

public class LocationTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(11)]
    public void Create_InvalidX_ShouldExplode(int x)
    {
        var loc = Location.Create(x, 1);
        
        Assert.True(loc.IsFailure);
        Assert.NotNull(loc.Error);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_ValidX_ShouldNotExplode(int x)
    {
        var loc = Location.Create(x, 1);
        Assert.True(loc.IsSuccess);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(11)]
    public void Create_InvalidY_ShouldExplode(int y)
    {
        var loc = Location.Create(1, y);
        
        Assert.True(loc.IsFailure);
        Assert.NotNull(loc.Error);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_ValidY_ShouldNotExplode(int y)
    {
        var loc = Location.Create(1, y);
        Assert.True(loc.IsSuccess);
    }
    
    [Fact]
    public void Create_ShouldReturnInstance()
    {
        var loc = Location.Create(1, 1);
        Assert.NotNull(loc.GetValueOrDefault());
    }
    
    [Fact]
    public void CreateRandom_ShouldReturnInstance()
    {
        var loc = Location.CreateRandom();
        Assert.NotNull(loc.GetValueOrDefault());
    }
    
    [Fact]
    public void DistanceTo_NullArg_ShouldExplode()
    {
        var loc = Location.CreateRandom();

        var distance = loc.Value.DistanceTo(null);

        Assert.True(distance.IsFailure);
        Assert.NotNull(distance.Error);
    }
    
    [Fact]
    public void DistanceTo_TargetIsGreaterThanCurrent_ShouldReturnCorrectValue()
    {
        var current = Location.Create(2, 6);
        var target = Location.Create(4, 9);

        var distance = current.Value.DistanceTo(target.Value);
        
        Assert.Equal(5, distance.Value);
    }
    
    [Fact]
    public void DistanceTo_TargetIsLessThanCurrent_ShouldReturnCorrectValue()
    {
        var current = Location.Create(4, 9);
        var target = Location.Create(2, 6);

        var distance = current.Value.DistanceTo(target.Value);
        
        Assert.Equal(5, distance.Value);
    }

    [Fact]
    public void Equals_ShouldReturnFalse()
    {
        var left = Location.Create(2, 3);
        var right = Location.Create(3, 4);
        
        Assert.False(left.Value == right.Value);
    }
    
    [Fact]
    public void Equals_ShouldReturnTrue()
    {
        var left = Location.Create(3, 4);
        var right = Location.Create(3, 4);
        
        Assert.True(left.Value == right.Value);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        var loc = Location.Create(5, 9);
        Assert.Equal("{x: 5, y: 9}", loc.Value.ToString());
    }
}