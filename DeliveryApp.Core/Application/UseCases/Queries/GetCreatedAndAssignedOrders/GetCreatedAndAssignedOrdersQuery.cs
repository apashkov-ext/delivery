using Dapper;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;

public sealed class GetCreatedAndAssignedOrdersQuery : IRequest<GetCreatedAndAssignedOrdersResponse>;

internal sealed class GetCreatedAndAssignedOrdersQueryHandler
    : IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>
{
    private const string SqlQuery =
        "SELECT id, courier_id, location_x, location_y, status_id FROM public.orders where status_id!=@status_id;";
    
    private readonly PostgresConnectionString _connectionString;

    public GetCreatedAndAssignedOrdersQueryHandler(PostgresConnectionString connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<GetCreatedAndAssignedOrdersResponse> Handle(GetCreatedAndAssignedOrdersQuery request, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        
        var result = await conn.QueryAsync<dynamic>(SqlQuery, new { status_id = OrderStatus.Completed.Id });
        var list = result.AsList();
        if (list.Count == 0)
        {
            return GetCreatedAndAssignedOrdersResponse.Empty;
        }

        var orders = list.Select(Map);
        return new GetCreatedAndAssignedOrdersResponse(orders);
    }
    private static Order Map(dynamic result)
    {
        var location = new Location(result.location_x, result.location_y);
        return new Order(result.id, location);
    }
}
