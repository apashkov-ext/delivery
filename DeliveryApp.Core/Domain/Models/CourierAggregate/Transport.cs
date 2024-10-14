using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

public sealed class Transport : Entity<int>
{
    public static readonly Transport Pedestrian = new(1, nameof(Pedestrian).ToLowerInvariant(), 1);
    public static readonly Transport Bicycle = new(1, nameof(Bicycle).ToLowerInvariant(), 2);
    public static readonly Transport Car = new(1, nameof(Car).ToLowerInvariant(), 3);
    
    public string Name { get; }
    public int Speed { get; }
    
    private Transport() { }

    private Transport(int id, string name, int speed)
    {
        Id = id;
        Name = name;
        Speed = speed;
    }

    public static IEnumerable<Transport> List()
    {
        yield return Pedestrian;
        yield return Bicycle;
        yield return Car;
    }

    public static Result<Transport, Error> FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return GeneralErrors.ValueIsInvalid(nameof(name));
        }
        
        var t = List().SingleOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        if (t is null)
        {
            return Errors.UnknownTransportId();
        }

        return t;
    }
    
    public static Result<Transport, Error> FromId(int id)
    {
        if (id < 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(id));
        }

        var t = List().SingleOrDefault(x => x.Id == id);
        if (t is null)
        {
            return Errors.UnknownTransportId();
        }

        return t;
    }
    
    internal static class Errors
    {
        public static Error UnknownTransportId()
        {
            return new Error("transport.id.is.unknown", "The transport ID is unknown");
        }
    }
}