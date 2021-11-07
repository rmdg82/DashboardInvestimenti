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
        public void GetLastGuadagnoNetto_ListEmpty_ReturnZero()
        {
            var sut = _financialCalculator.GetLastGuadagnoNetto(new List<ChartModel>(), out string coloreGuadagno);
            Assert.Equal(string.Empty, sut);
            Assert.Equal(string.Empty, coloreGuadagno);
        }

        [Theory]
        [InlineData(125, 100)]
        [InlineData(90, 100)]
        public void GetLastGuadagnoNetto_ListNotEmpty_ReturnCorrectValues(double valore, double sottoscrizioni)
        {
            var chartModels = new List<ChartModel>
            {
                new ChartModel
                {
                    ValoreInvestimento = valore,
                    Sottoscrizioni = sottoscrizioni
                },
            };
            var guadagno = valore - sottoscrizioni;
            string segnoGuadagno = guadagno >= 0 ? "+ " : string.Empty;
            var expectedColoreGuadagno = guadagno >= 0 ? "green" : "red";
            string result = segnoGuadagno + _financialCalculator.ToString(guadagno);

            var sut = _financialCalculator.GetLastGuadagnoNetto(chartModels, out string coloreGuadagno);

            Assert.Equal(result, sut);
            Assert.Equal(expectedColoreGuadagno, coloreGuadagno);
        }

        [Theory]
        [InlineData(125, 100)]
        [InlineData(90, 100)]
        public void GetLastGuadagnoPercentuale_ListNotEmpty_ReturnCorrectValues(double valore, double sottoscrizioni)
        {
            var chartModels = new List<ChartModel>
            {
                new ChartModel
                {
                    ValoreInvestimento = valore,
                    Sottoscrizioni = sottoscrizioni
                },
            };

            var guadagno = valore - sottoscrizioni;
            var guadagnoPerc = guadagno / 100;
            string segnoGuadagno = guadagno >= 0 ? "+ " : string.Empty;
            string result = segnoGuadagno + guadagnoPerc.ToString("P", _financialCalculator.CultureInfo);

            var sut = _financialCalculator.GetLastGuadagnoPercentuale(chartModels);

            Assert.Equal(result, sut);
        }

        [Fact]
        public void GetLastGuadagnoPercentuale_ListEmpty_ReturnStringEmpty()
        {
            var sut = _financialCalculator.GetLastGuadagnoPercentuale(new List<ChartModel>());
            Assert.Equal(string.Empty, sut);
        }

        [Fact]
        public void GetGuadagnoPercentuale_NullValues_ReturnArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _financialCalculator.GetGuadagnoPercentuale(null));
        }

        [Fact]
        public void GetGuadagnoPercentuale_25UpValues_Return0_25Up()
        {
            var chartModel = new ChartModel
            {
                Sottoscrizioni = 100,
                ValoreInvestimento = 125,
            };

            var sut = _financialCalculator.GetGuadagnoPercentuale(chartModel);

            Assert.Equal(0.25, sut);
        }
    }
}