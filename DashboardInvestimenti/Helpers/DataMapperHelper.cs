using DashboardInvestimenti.Models;
using System.Collections.Generic;
using System.Globalization;

namespace DashboardInvestimenti.Helpers;

public static class DataMapperHelper
{
    public static string DataFileCulture { get; private set; } = "en-US";

    public static List<ChartModel> MapToChartModel(List<ExcelModel> models)
    {
        var result = new List<ChartModel>();
        foreach (var model in models)
        {
            result.Add(MapToChartModel(model));
        }

        return result;
    }

    public static ChartModel MapToChartModel(ExcelModel model)
    {
        var result = new ChartModel
        {
            Data = model.Data
        };

        var cleanedValoreQuota = CleanString(model.ValoreQuota);
        var cleanedValoreInvestimento = CleanString(model.ValoreDisponibile);
        var cleanedSottiscrizioni = CleanString(model.Sottoscrizioni);

        if (double.TryParse(cleanedValoreQuota, NumberStyles.Float, new CultureInfo(DataFileCulture), out double parsedValoreQuota))
        {
            result.ValoreQuota = parsedValoreQuota;
        }

        if (double.TryParse(cleanedValoreInvestimento, NumberStyles.Float, new CultureInfo(DataFileCulture), out double parsedValoreInvestimento))
        {
            result.ValoreInvestimento = parsedValoreInvestimento;
        }

        if (double.TryParse(cleanedSottiscrizioni, NumberStyles.Float, new CultureInfo(DataFileCulture), out double parsedSottoscrizioni))
        {
            result.Sottoscrizioni = parsedSottoscrizioni;
        }

        return result;
    }

    private static string CleanString(string input)
    {
        return input?.Replace("€", string.Empty).Trim();
    }
}