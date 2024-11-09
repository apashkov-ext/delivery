using System.Collections.ObjectModel;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public sealed class GetBusyCouriersResponse
{
    public ReadOnlyCollection<Courier> Couriers { get; }
    
    public static GetBusyCouriersResponse Empty = new([]);
    
    public GetBusyCouriersResponse(IEnumerable<Courier> couriers)
    {
        Couriers = new(couriers.ToArray());
    }
}

public sealed class Courier
{
    public Guid Id { get; }
    public string Name { get; }
    public int TransportId { get; }
    public Location Location { get; }

    public Courier(Guid id, string name, int transportId, Location location)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }
        Name = name;
        TransportId = transportId;
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }
}

public sealed class Location
{
    public int X { get; }
    public int Y { get; }

    public Location(int x, int y)
    {
        X = x;
        Y = y;
    }
}