using System.Globalization;

namespace AirWaterStore.Web.Helper;

public static class Currency
{
    public const string CultureCode = "vi-VN";
    public static readonly CultureInfo VnCurrencyFormat = new CultureInfo(CultureCode);
    // public static string FormatCurrency(decimal amount)
    // {
    //     return amount.ToString("C", VnCurrencyFormat);
    // }
}
