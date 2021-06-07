using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
using DashboardInvestimenti.Helpers;
using DashboardInvestimenti.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardInvestimenti.Components
{
    public partial class CustomChart
    {
        [Parameter]
        public string HeaderTitle { get; set; }

        [Parameter]
        public List<string> Labels { get; set; }

        [Parameter]
        public List<double> ChartValues { get; set; }

        [Parameter]
        public Color GraphColor { get; set; } = Color.Red;

        public bool IsFileLoaded { get; set; } = false;

        public LineConfig ChartConfig { get; set; } = new()
        {
            Options = new LineOptions
            {
                Responsive = true,
                MaintainAspectRatio = true,
                Tooltips = new Tooltips
                {
                    Mode = InteractionMode.Nearest,
                    Intersect = true
                },
                Hover = new Hover
                {
                    Mode = InteractionMode.Nearest,
                    Intersect = true
                },
                Legend = new Legend() { Display = false }
            }
        };

        protected override void OnInitialized()
        {
            ClearChartsData();
            foreach (var label in Labels)
            {
                ChartConfig.Data.Labels.Add(label);
            }

            IDataset<double> values = new LineDataset<double>(ChartValues)
            {
                BackgroundColor = ColorUtil.FromDrawingColor(GraphColor),
                BorderColor = ColorUtil.FromDrawingColor(GraphColor),
                Fill = FillingMode.Disabled,
            };

            ChartConfig.Data.Datasets.Add(values);
            IsFileLoaded = true;
        }

        private void ClearChartsData()
        {
            ChartConfig.Data.Labels.Clear();
            ChartConfig.Data.Datasets.Clear();
        }
    }
}