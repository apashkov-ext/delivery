using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands.AssignOrder;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using Moq;
using Moq.AutoMock;

namespace DeliveryApp.UnitTests.UseCasesTests;

public class AssignOrderCommandTests
{
    [Fact]
    public async Task ShouldInvokeDispatcher()
    {
        var orderLoc = Location.Create(5, 5).Value;
        var order = Order.Create(Guid.NewGuid(), orderLoc).Value;
        var courierLoc = Location.Create(1, 1).Value;
        var courier = Courier.Create("Name", Transport.Car, courierLoc).Value;
        
        var mocker = new AutoMocker();
        mocker.GetMock<IOrderRepository>()
            .Setup(x => x.FindCreatedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Order>([order]));
        mocker.GetMock<ICourierRepository>()
            .Setup(x => x.FindFreeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Courier>([courier]));
        var handler = mocker.CreateInstance<AssignOrderCommandHandler>();
        
        var req = new AssignOrderCommand();
        await handler.Handle(req);
        
        mocker.GetMock<IDispatchService>()
            .Verify(x => x.Dispatch(It.Is<Order>(o => o == order), It.Is<ReadOnlyCollection<Courier>>(c => c.Contains(courier))), Times.Once);
    }    
    
    [Fact]
    public async Task DispatcherFail_ShouldNotInvokeRepos()
    {
        var orderLoc = Location.Create(5, 5).Value;
        var order = Order.Create(Guid.NewGuid(), orderLoc).Value;
        var courierLoc = Location.Create(1, 1).Value;
        var courier = Courier.Create("Name", Transport.Car, courierLoc).Value;
        
        var mocker = new AutoMocker();
        mocker.GetMock<IOrderRepository>()
            .Setup(x => x.FindCreatedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Order>([order]));
        mocker.GetMock<ICourierRepository>()
            .Setup(x => x.FindFreeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Courier>([courier]));
        mocker.GetMock<IDispatchService>()
            .Setup(x => x.Dispatch(It.IsAny<Order>(), It.IsAny<ReadOnlyCollection<Courier>>()))
            .Returns(DispatchService.Errors.InvalidOrderStatus());
        var handler = mocker.CreateInstance<AssignOrderCommandHandler>();
        
        var req = new AssignOrderCommand();
        await handler.Handle(req);
        
        mocker.GetMock<ICourierRepository>()
            .Verify(x => x.UpdateAsync(It.IsAny<Courier>(), It.IsAny<CancellationToken>()), Times.Never);        
        mocker.GetMock<IOrderRepository>()
            .Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }    
    
    [Fact]
    public async Task DispatcherSuccess_ShouldInvokeRepos()
    {
        var orderLoc = Location.Create(5, 5).Value;
        var order = Order.Create(Guid.NewGuid(), orderLoc).Value;
        var courierLoc = Location.Create(1, 1).Value;
        var courier = Courier.Create("Name", Transport.Car, courierLoc).Value;
        
        var mocker = new AutoMocker();
        mocker.GetMock<IOrderRepository>()
            .Setup(x => x.FindCreatedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Order>([order]));
        mocker.GetMock<ICourierRepository>()
            .Setup(x => x.FindFreeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Courier>([courier]));
        mocker.GetMock<IDispatchService>()
            .Setup(x => x.Dispatch(It.IsAny<Order>(), It.IsAny<ReadOnlyCollection<Courier>>()))
            .Returns(courier);
        var handler = mocker.CreateInstance<AssignOrderCommandHandler>();
        
        var req = new AssignOrderCommand();
        await handler.Handle(req);
        
        mocker.GetMock<ICourierRepository>()
            .Verify(x => x.UpdateAsync(It.IsAny<Courier>(), It.IsAny<CancellationToken>()), Times.Once);        
        mocker.GetMock<IOrderRepository>()
            .Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}