using System.Diagnostics;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

[DebuggerDisplay(value: "{GetType()} {ToString()}")]
public sealed class Courier: Aggregate
{
    public string Name { get; }
    public Transport Transport { get; }
    public Location Location { get; private set; }
    public CourierStatus Status { get; private set; }
    
    private Courier() { }

    public Courier(Guid id, 
        string name, 
        Transport transport, 
        Location location, 
        CourierStatus status)
    {
        Name = name;
        Transport = transport;
        Location = location;
        Status = status;
        Id = id;
    }

    public static Result<Courier, Error> Create(string name, 
        Transport transport, 
        Location location)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return GeneralErrors.ValueIsRequired(nameof(name));
        }

        if (transport == null)
        {
            return GeneralErrors.ValueIsRequired(nameof(transport));
        }
        
        if (location == null)
        {
            return GeneralErrors.ValueIsRequired(nameof(location));
        }
        
        var id = Guid.NewGuid();
        return new Courier(id, 
            name, 
            transport, 
            location, 
            CourierStatus.Free);
    }

    public UnitResult<Error> MakeBusy()
    {
        if (Status != CourierStatus.Free)
        {
            return Errors.CourierAlreadyBusy();
        }

        Status = CourierStatus.Busy;
        
        return UnitResult.Success<Error>();
    }
    
    public UnitResult<Error> MakeFree()
    {
        if (Status == CourierStatus.Free)
        {
            return Errors.CourierAlreadyFree();
        }
        
        Status = CourierStatus.Free;
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Move(Location targetLocation)
    {
        if (targetLocation is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(targetLocation));
        }

        if (Location == targetLocation)
        {
            return UnitResult.Success<Error>();
        }

        var cells = Transport.Speed;
        
        var stepsX = Location.X - targetLocation.X;
        var deltaX = Math.Abs(stepsX);
        
        var stepsY = Location.Y - targetLocation.Y;
        var deltaY = Math.Abs(stepsY);

        var loc = Result.Of<Location, Error>(Location);

        // to the RIGHT
        if (stepsX < 0)
        {
            if (deltaX >= cells)
            {
                return Location.Create(Location.X + cells, Location.Y)
                    .Tap(x => Location = x)
                    .Map(_ => UnitResult.Success<Error>());
            }
            
            Location.Create(Location.X + deltaX, Location.Y)
                .Tap(x => Location = x);
            
            if (loc.IsFailure)
            {
                return loc.Error;
            }

            if (Location == targetLocation)
            {
                return UnitResult.Success<Error>();
            }
            
            cells -= deltaX;
        }
        
        // to the LEFT
        if (stepsX > 0)
        {
            if (deltaX >= cells)
            {
                return Location.Create(Location.X - cells, Location.Y)
                    .Tap(x => Location = x)
                    .Map(_ => UnitResult.Success<Error>());
            }
            
            Location.Create(Location.X - deltaX, Location.Y)
                .Tap(x => Location = x);
            
            if (loc.IsFailure)
            {
                return loc.Error;
            }
            
            if (Location == targetLocation)
            {
                return UnitResult.Success<Error>();
            }
            
            cells -= deltaX;
        }
        
        // to the BOTTOM
        if (stepsY < 0)
        {
            if (deltaY >= cells)
            {
                return Location.Create(Location.X, Location.Y + cells)
                    .Tap(x => Location = x)
                    .Map(_ => UnitResult.Success<Error>());
            }
            
            Location.Create(Location.X, Location.Y + deltaY)
                .Tap(x => Location = x);
            
            if (loc.IsFailure)
            {
                return loc.Error;
            }
            
            if (Location == targetLocation)
            {
                return UnitResult.Success<Error>();
            }
            
            cells -= deltaY;
        }
        
        // to the TOP
        if (stepsY > 0)
        {
            if (deltaY >= cells)
            {
                return Location.Create(Location.X, Location.Y - cells)
                    .Tap(x => Location = x)
                    .Map(_ => UnitResult.Success<Error>());
            }
            
            loc = Location.Create(Location.X, Location.Y - deltaY)
                .Tap(x => Location = x);
            
            if (loc.IsFailure)
            {
                return loc.Error;
            }
            
            if (Location == targetLocation)
            {
                return UnitResult.Success<Error>();
            }
            
            cells -= deltaY;
        }
        
        return UnitResult.Success<Error>();
    }

    public Result<double, Error> StepsToLocation(Location targetLocation)
    {
        if (targetLocation is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(targetLocation));
        }

        var dist = Location.DistanceTo(targetLocation);
        if (dist.IsFailure)
        {
            return dist.Error;
        }

        return (double) dist.Value / Transport.Speed;
    }

    public override string ToString() => $"Name: {Name}, Transport: {Transport}, Status: {Status}";

    public static class Errors
    {
        public static Error CourierAlreadyBusy()
        {
            return new Error("courier.is.not.free", "Courier is not free now");
        }  
        
        public static Error CourierAlreadyFree()
        {
            return new Error("courier.is.free", "Courier is already free");
        }  
    }
}