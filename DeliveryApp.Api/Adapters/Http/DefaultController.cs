using Api.Controllers;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Courier = Api.Models.Courier;
using Location = Api.Models.Location;
using Order = Api.Models.Order;

namespace DeliveryApp.Api.Adapters.Http;

public class DefaultController : DefaultApiController
{
    private readonly IMediator _mediator;

    public DefaultController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override async Task<IActionResult> CreateOrder()
    {
        var basketId = Guid.NewGuid();
        
        var cmd = new CreateOrderCommand(basketId, "Street");
        var response = await _mediator.Send(cmd, CancellationToken.None);

        if (response.IsFailure)
        {
            return UnprocessableEntity();
        }

        return Ok();
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var query = new GetBusyCouriersQuery();
        var response = await _mediator.Send(query, CancellationToken.None);

        var dto = response.Couriers.Select(x => new Courier
        {
            Id = x.Id,
            Name = x.Name,
            Location = new Location
            {
                X = x.Location.X,
                Y = x.Location.Y
            }
        });
        return Ok(dto);
    }

    public override async Task<IActionResult> GetOrders()
    {
        var query = new GetCreatedAndAssignedOrdersQuery();
        var response = await _mediator.Send(query, CancellationToken.None);

        var dto = response.Orders.Select(x => new Order
        {
            Id = x.Id,
            Location = new Location
            {
                X = x.Location.X,
                Y = x.Location.Y
            }
        });
        return Ok(dto);
    }
}