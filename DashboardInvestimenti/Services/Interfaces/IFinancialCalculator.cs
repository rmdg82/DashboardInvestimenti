using DashboardInvestimenti.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardInvestimenti.Services.Interfaces
{
    public interface IFinancialCalculator
    {
        string ToStringFormat { get; }
        CultureInfo CultureInfo { get; }
        int FractionalDigits { get; }

        string ToString(double value);

        double GetAverageValoreQuota(List<ChartModel> chartModels);
    }
}