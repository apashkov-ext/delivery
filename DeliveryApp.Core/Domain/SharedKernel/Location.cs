using System.Diagnostics;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

[DebuggerDisplay(value: "{GetType()} {ToString()}")]
public sealed class Location : ValueObject
{
    private const int MinCoordValue = 1;
    private const int MaxCoordValue = 10;

    private static readonly Random _r = new();
    
    public int X { get; }
    public int Y { get; }
    
    private Location() { }

    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Location Create(int x, int y)
    {
        if (x < MinCoordValue || x > MaxCoordValue)
        {
            ThrowRangeEx(nameof(x));
        }
        
        if (y < MinCoordValue || y > MaxCoordValue)
        {
            ThrowRangeEx(nameof(y));
        }

        return new(x, y);
    }

    public static Location CreateRandom()
    {
        var x = _r.Next(MinCoordValue, MaxCoordValue + 1);
        var y = _r.Next(MinCoordValue, MaxCoordValue + 1);
        return new(x, y);
    }

    /// <summary>
    /// Returns a distance (in steps) from the current location to a specified target location.
    /// </summary>
    /// <param name="target">Target location.</param>
    /// <returns>Steps</returns>
    public int DistanceTo(Location target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return Math.Abs(target.X - X) + Math.Abs(target.Y - Y);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }

    private static void ThrowRangeEx(string paramName)
    {
        throw new ArgumentOutOfRangeException($"The '{paramName}' value must be between {MinCoordValue} and {MaxCoordValue} including boundaries", paramName);
    }

    public override string ToString() => $"{{x: {X}, y: {Y}}}";
}