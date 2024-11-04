using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public sealed class GetBusyCouriersQuery : IRequest<GetBusyCouriersResponse>;

internal sealed class GetBusyCouriersQueryHandler : IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>
{
    private const string SqlQuery = "SELECT id, name, location_x, location_y, status_id, transport_id FROM public.couriers";
    
    private readonly PostgresConnectionString _connectionString;

    public GetBusyCouriersQueryHandler(PostgresConnectionString connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<GetBusyCouriersResponse> Handle(GetBusyCouriersQuery request, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        
        var result = await conn.QueryAsync<dynamic>(SqlQuery, new { });
        var list = result.AsList();
        if (list.Count == 0)
        {
            return GetBusyCouriersResponse.Empty;
        }

        var couriers = list.Select(Map);
        return new GetBusyCouriersResponse(couriers);
    }
    
    private static Courier Map(dynamic result)
    {
        var location = new Location(result.location_x, result.location_y);
        return new Courier(result.id, result.name, result.transport_id, location);
    }
}