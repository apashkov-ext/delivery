using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

public sealed class Location : ValueObject
{
    public const int MinComponentValue = 1;
    public const int MaxComponentValue = 10;

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
        if (x < MinComponentValue || x > MaxComponentValue)
        {
            ThrowRangeEx(nameof(x));
        }
        
        if (y < MinComponentValue || y > MaxComponentValue)
        {
            ThrowRangeEx(nameof(y));
        }

        return new(x, y);
    }

    public static Location CreateRandom()
    {
        var x = _r.Next(MinComponentValue, MaxComponentValue + 1);
        var y = _r.Next(MinComponentValue, MaxComponentValue + 1);
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
        throw new ArgumentOutOfRangeException($"The '{paramName}' value must be between {MinComponentValue} and {MaxComponentValue} including boundaries", paramName);
    }

    public override string ToString() => $"{{{X}, {Y}}}";
}