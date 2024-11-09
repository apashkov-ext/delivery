namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;

internal sealed class OrderProjection
{
    public Guid Id { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
}