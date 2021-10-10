namespace DashboardInvestimenti.Models
{
    /// <summary>
    /// Model used when visualizing the charts
    /// </summary>
    public class ChartModel
    {
        public string Data { get; set; }
        public double ValoreQuota { get; set; }
        public double ValoreInvestimento { get; set; }
        public double Sottoscrizioni { get; set; }
    }
}