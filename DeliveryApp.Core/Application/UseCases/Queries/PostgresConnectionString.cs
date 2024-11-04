namespace DeliveryApp.Core.Application.UseCases.Queries;

public sealed class PostgresConnectionString
{
    public string Value { get; }
    
    public PostgresConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        }

        Value = connectionString;
    }

    public static implicit operator string(PostgresConnectionString connectionString)
    {
        if (connectionString is null)
        {
            throw new InvalidCastException("Connection string is null");
        }

        return connectionString.Value;
    }
}