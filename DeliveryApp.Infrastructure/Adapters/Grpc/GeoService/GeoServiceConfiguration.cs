using System.ComponentModel.DataAnnotations;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;

public class GeoServiceConfiguration
{
    [Required]
    public string GEO_SERVICE_GRPC_HOST { get; set; }
}