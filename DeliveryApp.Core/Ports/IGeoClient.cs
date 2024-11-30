using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    Task<Result<Location, Error>> GetGeolocationAsync(string street, CancellationToken cancellationToken);
}