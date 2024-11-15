using Dapper;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;

public sealed class GetCreatedAndAssignedOrdersQuery : IRequest<GetCreatedAndAssignedOrdersResponse>;

internal sealed class GetCreatedAndAssignedOrdersQueryHandler
    : IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>
{
    private const string SqlQuery = "SELECT id as Id, target_location_x as X, target_location_y as Y FROM public.orders where status_id!=@status_id;";
    
    private readonly PostgresConnectionString _connectionString;

    public GetCreatedAndAssignedOrdersQueryHandler(PostgresConnectionString connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<GetCreatedAndAssignedOrdersResponse> Handle(GetCreatedAndAssignedOrdersQuery request, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        
        var result = await conn.QueryAsync<OrderProjection>(SqlQuery, new { status_id = OrderStatus.Completed.Id });
        var list = result.AsList();
        if (list.Count == 0)
        {
            return GetCreatedAndAssignedOrdersResponse.Empty;
        }

        var orders = list.Select(Map);
        return new GetCreatedAndAssignedOrdersResponse(orders);
    }
    private static Order Map(OrderProjection result)
    {
        var location = new Location(result.X, result.Y);
        return new Order(result.Id, location);
    }
}