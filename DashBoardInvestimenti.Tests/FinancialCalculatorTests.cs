using DashboardInvestimenti.Models;
using DashboardInvestimenti.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DashBoardInvestimenti.Tests
{
    public class FinancialCalculatorTests
    {
        private readonly FinancialCalculator _financialCalculator;

        public FinancialCalculatorTests()
        {
            _financialCalculator = new FinancialCalculator();
        }

        [Fact]
        public void GetAverageValoreQuota_NullValue_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _financialCalculator.GetAverageValoreQuota(null));
        }

        [Fact]
        public void GetAverageValoreQuota_ListEmpty_ReturnZero()
        {
            var sut = _financialCalculator.GetAverageValoreQuota(new List<ChartModel>());

            Assert.Equal(0, sut);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(1.1, 2.2, 3.3)]
        [InlineData(1.2, 2.4, 3.6)]
        public void GetAverageValoreQuota_ListNotEmpty_ReturnCorrectValues(double int1, double int2, double int3)
        {
            var chartModels = new List<ChartModel>
            {
                new ChartModel
                {
                    ValoreQuota = int1
                },
                new ChartModel
                {
                    ValoreQuota = int2
                },
                new ChartModel
                {
                    ValoreQuota = int3
                }
            };

            var sut = _financialCalculator.GetAverageValoreQuota(chartModels);
            double expected = Math.Round((int1 + int2 + int3) / 3, _financialCalculator.FractionalDigits);

            Assert.Equal(expected, sut);
        }

        [Fact]
        public void GetGuadagnoNetto_ListEmpty_ReturnZero()
        {
            var sut = _financialCalculator.GetGuadagnoNetto(new List<ChartModel>(), out string coloreGuadagno);
            Assert.Equal(string.Empty, sut);
            Assert.Equal(string.Empty, coloreGuadagno);
        }

        [Theory]
        [InlineData(1, 2)]
        public void GetGuadagnoNetto_ListNotEmpty_ReturnCorrectValues(double int1, double int2)
        {
            var chartModels = new List<ChartModel>
            {
                new ChartModel
                {
                    ValoreInvestimento = int1,
                    Sottoscrizioni = int2
                },
            };

            var sut = _financialCalculator.GetGuadagnoNetto(chartModels, out string coloreGuadagno);
            string result = "-1,00 €";

            Assert.Equal(result, sut);
        }
    }
}