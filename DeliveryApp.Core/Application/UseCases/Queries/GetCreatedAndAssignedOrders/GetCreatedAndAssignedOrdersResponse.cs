using System.Collections.ObjectModel;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;

public sealed class GetCreatedAndAssignedOrdersResponse
{
    public ReadOnlyCollection<Order> Orders { get; }

    public static GetCreatedAndAssignedOrdersResponse Empty { get; } = new([]);
    
    public GetCreatedAndAssignedOrdersResponse(IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        Orders = new(orders.ToArray());
    }
}

public sealed class Order
{
    public Guid Id { get; }
    public Location Location { get; }

    public Order(Guid id, Location location)
    {
        Id = id;
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
