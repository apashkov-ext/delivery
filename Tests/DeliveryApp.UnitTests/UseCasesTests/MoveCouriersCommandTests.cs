using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using Moq;
using Moq.AutoMock;

namespace DeliveryApp.UnitTests.UseCasesTests;

public class MoveCouriersCommandTests
{
    [Fact]
    public async Task AlreadyOnPosition_ShouldCompleteAndMakeFree()
    {
        var loc = Location.Create(5, 5).Value;
        var order = Order.Create(Guid.NewGuid(), loc).Value;
        var courier = Courier.Create("Name", Transport.Car, loc).Value;
        order.Assign(courier);
        courier.MakeBusy();
        
        var mocker = new AutoMocker();
        mocker.GetMock<IOrderRepository>()
            .Setup(x => x.FindAssignedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Order>([order]));
        mocker.GetMock<ICourierRepository>()
            .Setup(x => x.FindBusyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Courier>([courier]));
        var handler = mocker.CreateInstance<MoveCouriersCommandHandler>();

        var req = new MoveCouriersCommand();
        await handler.Handle(req);
        
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(CourierStatus.Free, courier.Status);
        
        mocker.GetMock<ICourierRepository>()
            .Verify(x => x.UpdateAsync(It.Is<Courier>(c => c.Status == CourierStatus.Free), It.IsAny<CancellationToken>()), Times.Once);        
        mocker.GetMock<IOrderRepository>()
            .Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task NearThePosition_ShouldCompleteAndMakeFree()
    {
        var orderLoc = Location.Create(5, 5).Value;
        var order = Order.Create(Guid.NewGuid(), orderLoc).Value;
        var courierLoc = Location.Create(4, 5).Value;
        var courier = Courier.Create("Name", Transport.Car, courierLoc).Value;
        order.Assign(courier);
        courier.MakeBusy();
        
        var mocker = new AutoMocker();
        mocker.GetMock<IOrderRepository>()
            .Setup(x => x.FindAssignedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Order>([order]));
        mocker.GetMock<ICourierRepository>()
            .Setup(x => x.FindBusyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Courier>([courier]));
        var handler = mocker.CreateInstance<MoveCouriersCommandHandler>();

        var req = new MoveCouriersCommand();
        await handler.Handle(req);
        
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(CourierStatus.Free, courier.Status);
        Assert.Equal(order.TargetLocation, courier.Location);
        
        mocker.GetMock<ICourierRepository>()
            .Verify(x => x.UpdateAsync(It.Is<Courier>(c => c.Status == CourierStatus.Free), It.IsAny<CancellationToken>()), Times.Once);        
        mocker.GetMock<IOrderRepository>()
            .Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }    
    
    [Fact]
    public async Task ShouldInvokeRepo()
    {
        var orderLoc = Location.Create(5, 5).Value;
        var order = Order.Create(Guid.NewGuid(), orderLoc).Value;
        var courierLoc = Location.Create(1, 1).Value;
        var courier = Courier.Create("Name", Transport.Pedestrian, courierLoc).Value;
        order.Assign(courier);
        courier.MakeBusy();
        
        var mocker = new AutoMocker();
        mocker.GetMock<IOrderRepository>()
            .Setup(x => x.FindAssignedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Order>([order]));
        mocker.GetMock<ICourierRepository>()
            .Setup(x => x.FindBusyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadOnlyCollection<Courier>([courier]));
        var handler = mocker.CreateInstance<MoveCouriersCommandHandler>();

        var req = new MoveCouriersCommand();
        await handler.Handle(req);
        
        mocker.GetMock<ICourierRepository>()
            .Verify(x => x.UpdateAsync(It.Is<Courier>(c => c.Status == CourierStatus.Busy), It.IsAny<CancellationToken>()), Times.Once);        
        mocker.GetMock<IOrderRepository>()
            .Verify(x => x.UpdateAsync(It.Is<Order>(o => o.Status == OrderStatus.Completed), It.IsAny<CancellationToken>()), Times.Never);
    }
}