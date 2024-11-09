using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public sealed class GetBusyCouriersQuery : IRequest<GetBusyCouriersResponse>;

internal sealed class GetBusyCouriersQueryHandler : IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>
{
    private const string SqlQuery = "SELECT id as Id, name as Name, location_x as X, location_y as Y, transport_id as TransportId FROM public.couriers";
    
    private readonly PostgresConnectionString _connectionString;

    public GetBusyCouriersQueryHandler(PostgresConnectionString connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<GetBusyCouriersResponse> Handle(GetBusyCouriersQuery request, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        
        var result = await conn.QueryAsync<CourierProjection>(SqlQuery, new { });
        var list = result.AsList();
        if (list.Count == 0)
        {
            return GetBusyCouriersResponse.Empty;
        }

        var couriers = list.Select(Map);
        return new GetBusyCouriersResponse(couriers);
    }
    
    private static Courier Map(CourierProjection result)
    {
        var location = new Location(result.X, result.Y);
        return new Courier(result.Id, result.Name, result.TransportId, location);
    }
}