using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using Moq;
using Moq.AutoMock;

namespace DeliveryApp.UnitTests.UseCasesTests;

public class CreateOrderCommandTests
{
    [Fact]
    public async Task ShouldInvokeRepo()
    {
        var mocker = new AutoMocker();
        var handler = mocker.CreateInstance<CreateOrderCommandHandler>();

        var basketId = Guid.NewGuid();
        var street = "Street";
        
        var req = new CreateOrderCommand(basketId, street);
        var result = await handler.Handle(req);
        
        Assert.True(result.IsSuccess);
        mocker.GetMock<IOrderRepository>()
            .Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}