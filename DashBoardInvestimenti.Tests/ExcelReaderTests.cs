using DashboardInvestimenti.Models;
using DashboardInvestimenti.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DashBoardInvestimenti.Tests;

public class ExcelReaderTests
{
    private readonly ExcelReader _excelReader;
    private readonly string _filePath;

    public ExcelReaderTests()
    {
        Mock<IConfiguration> mockConfiguration = new();
        mockConfiguration.Setup(x => x["ExcelFolder:path"])
            .Returns("/wwwroot/data/excel");
        IConfiguration configuration = mockConfiguration.Object;

        _excelReader = new ExcelReader(configuration);

        var currentDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        var combinedPath = Path.Combine(currentDir, "ExcelFiles");

        _filePath = Directory.GetFiles(combinedPath).SingleOrDefault();
    }

    [Fact]
    public void WhenCalledReadFromPath_ReturnCorrectNumberValues()
    {
        var result = _excelReader.Read(_filePath);

        Assert.IsAssignableFrom<IEnumerable<ExcelModel>>(result);
        Assert.NotNull(result);
        Assert.Equal(130, result.Count());
    }

    [Fact]
    public void WhenCalledReadFromByteArray_ReturnCorrectNumberValues()
    {
        var fileContent = File.ReadAllBytes(_filePath);

        var result = _excelReader.Read(fileContent);

        Assert.IsAssignableFrom<IEnumerable<ExcelModel>>(result);
        Assert.NotNull(result);
        Assert.Equal(130, result.Count());
    }
}