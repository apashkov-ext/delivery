namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

internal sealed class CourierProjection
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int TransportId { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
}