namespace DashboardInvestimenti.Models
{
    public class HomeViewModel
    {
        public string ContractId { get; set; }
        public string ContractName { get; set; }
        public string DocDate { get; set; }
        public string Sottoscrizioni { get; set; }
        public string ValoreDisponibile { get; set; }
        public string ValoreQuota { get; set; }
        public string AverageValoreQuota { get; set; }
        public string GainLoss { get; set; }
        public string GainLossPercentage { get; set; }
    }
}