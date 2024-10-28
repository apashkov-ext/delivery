using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.RepositoriesTests;

public class OrderRepoTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private ApplicationDbContext _db;
    
    public OrderRepoTests()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("order")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var conn = _postgreSqlContainer.GetConnectionString();
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(conn,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
                }).Options;
        _db = new ApplicationDbContext(contextOptions);
        await _db.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task Add_NullOrder_ShouldFail()
    {
        var repo = new OrderRepository(_db);
        var add = await repo.AddAsync(null as Order);
        
        Assert.True(add.IsFailure);
    }    
    
    [Fact]
    public async Task Add_ShouldInsert()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        
        var repo = new OrderRepository(_db);
        var add = await repo.AddAsync(order);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();
        
        Assert.True(add.IsSuccess);
        
        var freshOrder = await repo.FindByIdAsync(order.Id);
        Assert.True(freshOrder.Value.HasValue);
        Assert.Equivalent(order, freshOrder.Value.Value);
    }    
    
    [Fact]
    public async Task Update_NullOrder_ShouldFail()
    {
        var repo = new OrderRepository(_db);
        var update = await repo.UpdateAsync(null as Order);
        
        Assert.True(update.IsFailure);    
    }    
    
    [Fact]
    public async Task Update_ShouldUpdate()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        
        var repo = new OrderRepository(_db);
        _ = await repo.AddAsync(order);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();

        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        order.Assign(courier);
        var update = await repo.UpdateAsync(order);
        
        Assert.True(update.IsSuccess);
        var freshOrder = await repo.FindByIdAsync(order.Id);
        Assert.True(freshOrder.Value.HasValue);
        Assert.Equivalent(order, freshOrder.Value.Value);
    }    
    
    [Fact]
    public async Task FindByIdAsync_ShouldHasNoValue()
    {
        var repo = new OrderRepository(_db);
        var order = await repo.FindByIdAsync(Guid.NewGuid());
        
        Assert.True(order.IsSuccess);
        Assert.True(order.Value.HasNoValue);
    }    
    
    [Fact]
    public async Task FindByIdAsync_ShouldReturnAggregate()
    {
        var order = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        
        var repo = new OrderRepository(_db);
        _ = await repo.AddAsync(order);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();
        
        var find = await repo.FindByIdAsync(order.Id);
        
        Assert.True(find.IsSuccess);
        Assert.True(find.Value.HasValue);
        Assert.Equivalent(order, find.Value.Value);
    }    
    
    [Fact]
    public async Task FindCreated_ShouldReturnEmpty()
    {
        var repo = new OrderRepository(_db);

        var created = await repo.FindCreatedAsync();
        
        Assert.Empty(created);
    }    
    
    [Fact]
    public async Task FindCreated_ShouldReturnCreated()
    {
        var order1 = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var order2 = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var order3 = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        order3.Assign(courier);
        
        var repo = new OrderRepository(_db);
        _ = await repo.AddAsync(order1);
        _ = await repo.AddAsync(order2);
        _ = await repo.AddAsync(order3);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();

        var created = await repo.FindCreatedAsync();
        
        Assert.Equal(2, created.Count);
        Assert.Contains(order1, created);
        Assert.Contains(order2, created);
    }    
    
    [Fact]
    public async Task FindAssigned_ShouldReturnEmpty()
    {
        var repo = new OrderRepository(_db);

        var assigned = await repo.FindAssignedAsync();
        
        Assert.Empty(assigned);    
    }    
    
    [Fact]
    public async Task FindAssigned_ShouldReturnAssigned()
    {
        var order1 = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier1 = Courier.Create("Name1", Transport.Car, Location.MinLocation).Value;
        order1.Assign(courier1);
        var order2 = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        var courier2 = Courier.Create("Name2", Transport.Car, Location.MinLocation).Value;
        order2.Assign(courier2);
        var order3 = Order.Create(Guid.NewGuid(), Location.MinLocation).Value;
        
        var repo = new OrderRepository(_db);
        _ = await repo.AddAsync(order1);
        _ = await repo.AddAsync(order2);
        _ = await repo.AddAsync(order3);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();

        var assigned = await repo.FindAssignedAsync();
        
        Assert.Equal(2, assigned.Count);
        Assert.Contains(order1, assigned);
        Assert.Contains(order2, assigned);
    }
}