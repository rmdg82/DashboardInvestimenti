using DashboardInvestimenti.Models;
using DashboardInvestimenti.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DashboardInvestimenti.Services.Implementations
{
    public class FinancialCalculator : IFinancialCalculator
    {
        public string ToStringFormat { get; private set; } = "C";
        public CultureInfo CultureInfo { get; private set; } = CultureInfo.CreateSpecificCulture("it-IT");

        public int FractionalDigits { get; private set; } = 4;

        public double GetAverageValoreQuota(List<ChartModel> chartModels)
        {
            if (chartModels is null)
            {
                throw new ArgumentNullException(nameof(chartModels));
            }

            if (!chartModels.Any())
            {
                return 0;
            }

            var sumValoriQuota = chartModels.Sum(line => line.ValoreQuota);
            int numLines = chartModels.Count;

            return Math.Round(sumValoriQuota / numLines, FractionalDigits);
        }

        public string ToString(double value)
        {
            return value.ToString(ToStringFormat, CultureInfo);
        }
    }
}