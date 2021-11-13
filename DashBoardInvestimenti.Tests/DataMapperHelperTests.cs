using DashboardInvestimenti.Helpers;
using DashboardInvestimenti.Models;
using Xunit;

namespace DashBoardInvestimenti.Tests;

public class DataMapperHelperTests
{
    [Fact]
    public void MapToChartModel_CorrectValues_ReturnCorrectChartModel()
    {
        var excelModel = new ExcelModel
        {
            Data = "27/05/2021",
            ValoreQuota = "7.60 €",
            ValoreDisponibile = "902.42 €"
        };

        var result = DataMapperHelper.MapToChartModel(excelModel);

        Assert.Equal("27/05/2021", result.Data);
        Assert.Equal(7.60, result.ValoreQuota);
        Assert.Equal(902.42, result.ValoreInvestimento);
    }

    [Fact]
    public void MapToChartModel_UnparsableValues_ReturnDefaultValue()
    {
        var excelModel = new ExcelModel
        {
            Data = "27/05/2021",
            ValoreQuota = "€8,89,23€",
            ValoreDisponibile = "902,42 €"
        };

        var result = DataMapperHelper.MapToChartModel(excelModel);

        Assert.Equal("27/05/2021", result.Data);
        Assert.Equal(0, result.ValoreQuota);
        Assert.Equal(0, result.ValoreInvestimento);
    }
}