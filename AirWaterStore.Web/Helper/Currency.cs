using System;
using System.Globalization;

namespace AirWaterStore.Web.Helper;

public static class Currency
{
    public static CultureInfo VnCurrencyFormat()
    {
        string cultureCode = "vi-VN"; // Replace with the desired culture code
        CultureInfo culture = new CultureInfo(cultureCode);

        // Get the currency format information
        // NumberFormatInfo currencyFormat = culture.NumberFormat;
        return culture;
    }

}
