using AirWaterStore.Data.Models;

namespace AirWaterStore.Business.Library;

public static class VnPayHelper
{
    private static string idPrefix = "AirWater";

    //Generate unique id for vnpay side to track
    public static string GenerateUniqueId(Order order)
    {
        return $"{idPrefix}_{order.OrderId.ToString()}_{order.OrderDate.Ticks.ToString()}";
    }

    public static int ExtractOrderId(string generatedId)
    {
        if (string.IsNullOrWhiteSpace(generatedId))
            throw new ArgumentException("Input cannot be null or empty", nameof(generatedId));

        var parts = generatedId.Split('_');

        if (parts.Length != 3 || parts[0] != idPrefix)
            throw new FormatException("Invalid ID format");

        if (!int.TryParse(parts[1], out var orderId))
            throw new FormatException("Order ID is not a valid integer");

        return orderId;
    }
}
