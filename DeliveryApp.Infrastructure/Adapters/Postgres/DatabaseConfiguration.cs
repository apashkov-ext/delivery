using System.ComponentModel.DataAnnotations;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class DatabaseConfiguration
{
    [Required]
    public string CONNECTION_STRING { get; set; }
}