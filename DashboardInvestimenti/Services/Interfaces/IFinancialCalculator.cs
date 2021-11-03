﻿using DashboardInvestimenti.Models;
using System.Collections.Generic;
using System.Globalization;

namespace DashboardInvestimenti.Services.Interfaces
{
    public interface IFinancialCalculator
    {
        string ToStringFormat { get; }
        CultureInfo CultureInfo { get; }
        int FractionalDigits { get; }

        string ToString(double value);

        double GetAverageValoreQuota(List<ChartModel> chartModels);

        string GetGuadagnoNetto(List<ChartModel> chartModels, out string coloreGuadagno);

        string GetGuadagnoPercentuale(List<ChartModel> chartModels);
    }
}