using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public sealed class CreateOrderCommand : IRequest<UnitResult<Error>>
{
    public Guid BasketId { get; }
    public string Street { get; }

    public CreateOrderCommand(Guid basketId, string street)
    {
        BasketId = basketId;
        
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(street));
        }
        Street = street;
    }
}

internal sealed class CreateOrderCommandHandler : 
    IRequestHandler<CreateOrderCommand, UnitResult<Error>>
{
    private readonly IUnitOfWork _uow;
    private readonly IOrderRepository _orders;

    public CreateOrderCommandHandler(IUnitOfWork uow, IOrderRepository orders)
    {
        _uow = uow;
        _orders = orders;
    }

    public async Task<UnitResult<Error>> Handle(CreateOrderCommand request, CancellationToken ct = default)
    {
        var loc = CalculateLocation(request.Street);
        if (loc.IsFailure)
        {
            return UnitResult.Failure(loc.Error);
        }
        
        var order = Order.Create(request.BasketId, loc.Value);
        if (order.IsFailure)
        {
            return UnitResult.Failure(order.Error);
        }

        await _orders.AddAsync(order.Value, ct);
        await _uow.SaveChangesAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    // TODO: implement
    private Result<Location, Error> CalculateLocation(string street)
    {
        return Location.CreateRandom().Value;
    }
}