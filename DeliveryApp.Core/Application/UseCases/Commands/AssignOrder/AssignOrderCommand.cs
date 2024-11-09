using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrder;

public class AssignOrderCommand : IRequest;

internal sealed class AssignOrderCommandHandler : IRequestHandler<AssignOrderCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly IOrderRepository _orders;
    private readonly ICourierRepository _couriers;
    private readonly IDispatchService _dispatchService;
    private readonly ILogger<AssignOrderCommandHandler> _logger;

    public AssignOrderCommandHandler(IUnitOfWork uow,
        IOrderRepository orders, 
        ICourierRepository couriers, 
        IDispatchService dispatchService,
        ILogger<AssignOrderCommandHandler> logger)
    {
        _uow = uow;
        _orders = orders;
        _couriers = couriers;
        _dispatchService = dispatchService;
        _logger = logger;
    }
    
    public async Task Handle(AssignOrderCommand request, CancellationToken ct = default)
    {
        var orders = await _orders.FindCreatedAsync(ct);
        if (orders.Count == 0)
        {
            return;
        }
        
        foreach (var order in orders)
        {
            using var loggerScope = _logger.BeginScope(new Dictionary<string, object>
            {
                { "OrderId", order.Id }
            });
            
            var freeCouriers = await _couriers.FindFreeAsync(ct);
            var assign = _dispatchService.Dispatch(order, freeCouriers);
            if (assign.IsFailure)
            {
                _logger.LogWarning("Failed to assign order: {Error}", assign.Error);
                continue;
            }

            await _orders.UpdateAsync(order, CancellationToken.None);
            await _couriers.UpdateAsync(assign.Value, CancellationToken.None);
            await _uow.SaveChangesAsync(ct);
        }
    }
}