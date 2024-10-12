using System;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.SharedKernelTests;

public class LocationTests
{
    [Fact]
    public void MinAndMax()
    {
        Assert.Equal(1, Location.MinComponentValue);
        Assert.Equal(10, Location.MaxComponentValue);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(11)]
    public void Create_InvalidX_ShouldExplode(int x)
    {
        var act = () => Location.Create(x, 1);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_ValidX_ShouldNotExplode(int x)
    {
        _ = Location.Create(x, 1);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(11)]
    public void Create_InvalidY_ShouldExplode(int y)
    {
        var act = () => Location.Create(1, y);
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_ValidY_ShouldNotExplode(int y)
    {
        _ = Location.Create(1, y);
    }
    
    [Fact]
    public void Create_ShouldReturnInstance()
    {
        var loc = Location.Create(1, 1);
        Assert.NotNull(loc);
    }
    
    [Fact]
    public void CreateRandom_ShouldReturnInstance()
    {
        var loc = Location.CreateRandom();
        Assert.NotNull(loc);
    }
    
    [Fact]
    public void CreateRandom_ShouldReturnInstanceWithInRangeValues()
    {
        var loc = Location.CreateRandom();
        
        Assert.InRange(loc.X, Location.MinComponentValue, Location.MaxComponentValue);
        Assert.InRange(loc.Y, Location.MinComponentValue, Location.MaxComponentValue);
    }
    
    [Fact]
    public void DistanceTo_NullArg_ShouldExplode()
    {
        var loc = Location.CreateRandom();

        Func<object> act = () => loc.DistanceTo(null);

        Assert.Throws<ArgumentNullException>(act);
    }
    
    [Fact]
    public void DistanceTo_TargetIsGreaterThanCurrent_ShouldReturnCorrectValue()
    {
        var current = Location.Create(2, 6);
        var target = Location.Create(4, 9);

        var distance = current.DistanceTo(target);
        
        Assert.Equal(5, distance);
    }
    
    [Fact]
    public void DistanceTo_TargetIsLessThanCurrent_ShouldReturnCorrectValue()
    {
        var current = Location.Create(4, 9);
        var target = Location.Create(2, 6);

        var distance = current.DistanceTo(target);
        
        Assert.Equal(5, distance);
    }

    [Fact]
    public void Equals_ShouldReturnFalse()
    {
        var left = Location.Create(2, 3);
        var right = Location.Create(3, 4);
        
        Assert.False(left == right);
    }
    
    [Fact]
    public void Equals_ShouldReturnTrue()
    {
        var left = Location.Create(3, 4);
        var right = Location.Create(3, 4);
        
        Assert.True(left == right);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        var loc = Location.Create(5, 9);
        Assert.Equal("{5, 9}", loc.ToString());
    }
}