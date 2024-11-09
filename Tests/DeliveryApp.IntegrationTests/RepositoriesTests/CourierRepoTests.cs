using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.RepositoriesTests;

public class CourierRepoTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private ApplicationDbContext _db;
    
    public CourierRepoTests()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("courier")
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
    public async Task Add_ShouldInsert()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        
        var repo = new CourierRepository(_db);
        var add = await repo.AddAsync(courier);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();
        
        Assert.True(add.IsSuccess);
        
        var freshCourier = await repo.FindByIdAsync(courier.Id);
        Assert.True(freshCourier.HasValue);
        Assert.Equivalent(courier, freshCourier.Value);
    }    
    
    [Fact]
    public async Task Update_ShouldUpdate()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        
        var repo = new CourierRepository(_db);
        _ = await repo.AddAsync(courier);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();

        courier.MakeBusy();
        var update = await repo.UpdateAsync(courier);
        
        Assert.True(update.IsSuccess);
        var freshCourier = await repo.FindByIdAsync(courier.Id);
        Assert.True(freshCourier.HasValue);
        Assert.Equivalent(courier, freshCourier.Value);
    }    
    
    [Fact]
    public async Task FindByIdAsync_ShouldHasNoValue()
    {
        var repo = new CourierRepository(_db);
        var courier = await repo.FindByIdAsync(Guid.NewGuid());
        
        Assert.True(courier.HasNoValue);
    }    
    
    [Fact]
    public async Task FindByIdAsync_ShouldReturnAggregate()
    {
        var courier = Courier.Create("Name", Transport.Car, Location.MinLocation).Value;
        
        var repo = new CourierRepository(_db);
        _ = await repo.AddAsync(courier);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();
        
        var find = await repo.FindByIdAsync(courier.Id);
        
        Assert.True(find.HasValue);
        Assert.Equivalent(courier, find.Value);
    }    
    
    [Fact]
    public async Task FindFree_ShouldReturnEmpty()
    {
        var repo = new CourierRepository(_db);

        var free = await repo.FindFreeAsync();
        
        Assert.Empty(free);
    }    
    
    [Fact]
    public async Task FindFree_ShouldReturnFree()
    {
        var courier1 = Courier.Create("Name1", Transport.Car, Location.MinLocation).Value;
        var courier2 = Courier.Create("Name2", Transport.Car, Location.MinLocation).Value;
        var courier3 = Courier.Create("Name3", Transport.Car, Location.MinLocation).Value;
        courier3.MakeBusy();
        
        var repo = new CourierRepository(_db);
        _ = await repo.AddAsync(courier1);
        _ = await repo.AddAsync(courier2);
        _ = await repo.AddAsync(courier3);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();

        var free = await repo.FindFreeAsync();
        
        Assert.Equal(2, free.Count);
        Assert.Contains(courier1, free);
        Assert.Contains(courier2, free);
    }    
    
    [Fact]
    public async Task FindBusy_ShouldReturnEmpty()
    {
        var repo = new CourierRepository(_db);

        var busy = await repo.FindBusyAsync();
        
        Assert.Empty(busy);
    }    
    
    [Fact]
    public async Task FindBusy_ShouldReturnBusy()
    {
        var courier1 = Courier.Create("Name1", Transport.Car, Location.MinLocation).Value;
        var courier2 = Courier.Create("Name2", Transport.Car, Location.MinLocation).Value;
        var courier3 = Courier.Create("Name3", Transport.Car, Location.MinLocation).Value;
        courier3.MakeBusy();
        
        var repo = new CourierRepository(_db);
        _ = await repo.AddAsync(courier1);
        _ = await repo.AddAsync(courier2);
        _ = await repo.AddAsync(courier3);
        var uow = new UnitOfWork(_db);
        await uow.SaveChangesAsync();

        var busy = await repo.FindBusyAsync();
        
        Assert.Single(busy);
        Assert.DoesNotContain(courier1, busy);
        Assert.DoesNotContain(courier2, busy);
    } 
}