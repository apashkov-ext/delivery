namespace DeliveryApp.Core.Domain.SharedKernel;

public sealed class VoidResult
{
    private static readonly Lazy<VoidResult> _instance = new(() => new VoidResult());
    public static VoidResult Get => _instance.Value;
}