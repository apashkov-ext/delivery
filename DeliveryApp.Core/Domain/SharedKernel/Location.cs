using System.Diagnostics;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel;

[DebuggerDisplay(value: "{GetType()} {ToString()}")]
public sealed class Location : ValueObject
{
    private const int MinCoordValue = 1;
    private const int MaxCoordValue = 10;

    private static readonly Random _r = new();
    
    public int X { get; }
    public int Y { get; }

    public static Location MinLocation => new (MinCoordValue, MinCoordValue);
    public static Location MaxLocation => new (MaxCoordValue, MaxCoordValue);
    
    private Location() { }

    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Result<Location, Error> Create(int x, int y)
    {
        if (x < MinCoordValue || x > MaxCoordValue)
        {
            return Errors.LocationCoordinateValueIsInvalid(MinCoordValue, MaxCoordValue);
        }
        
        if (y < MinCoordValue || y > MaxCoordValue)
        {
            return Errors.LocationCoordinateValueIsInvalid(MinCoordValue, MaxCoordValue);
        }

        return new Location(x, y);
    }

    /// <summary>
    /// Returns a location with a random coordinates.
    /// </summary>
    /// <returns></returns>
    public static Result<Location> CreateRandom()
    {
        var x = _r.Next(MinCoordValue, MaxCoordValue + 1);
        var y = _r.Next(MinCoordValue, MaxCoordValue + 1);
        return new Location(x, y);
    }

    /// <summary>
    /// Returns a distance (in steps) from the current location to a specified target location.
    /// </summary>
    /// <param name="target">Target location.</param>
    /// <returns>Steps</returns>
    public Result<int, Error> DistanceTo(Location target)
    {
        if (target is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(target));
        }
        
        return Math.Abs(target.X - X) + Math.Abs(target.Y - Y);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }

    public override string ToString() => $"{{x: {X}, y: {Y}}}";
    
    internal static class Errors
    {
        public static Error LocationCoordinateValueIsInvalid(int from, int to)
        {
            return new Error($"{nameof(Location).ToLowerInvariant()}.coordinate.value.is.invalid", $"Coordinate value must be between {from} and {to} including boundaries");
        }
    }
}