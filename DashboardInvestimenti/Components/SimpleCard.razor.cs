using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardInvestimenti.Components
{
    public partial class SimpleCard
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public string Value { get; set; }
    }
}