using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardInvestimenti.Models
{
    public class ExcelModel
    {
        public string Data { get; set; }
        public string IdContratto { get; set; }
        public string NumeroQuote { get; set; }
        public string ValoreQuota { get; set; }
        public string ValoreDisponibile { get; set; }
        public string Sottoscrizioni { get; set; }
        public string Rimborsi { get; set; }
    }
}