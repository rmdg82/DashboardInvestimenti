using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
using DashboardInvestimenti.Contracts;
using DashboardInvestimenti.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using DashboardInvestimenti.Helpers;
using MudBlazor;
using Microsoft.Extensions.Configuration;
using Blazored.SessionStorage;

namespace DashboardInvestimenti.Pages
{
    public partial class Prudente
    {
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }

        [Inject]
        public IExcelReader<ExcelModel> ExcelReader { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        public static string PrudenteSessionKey => "prudenteFileRows";

        public List<string> PeriodoTemporale { get; set; } = new();
        public List<double> ValoreQuote { get; set; } = new();
        public List<double> ValoreInvestimento { get; set; } = new();

        private LineConfig _config1;
        private LineConfig _config2;

        private bool _isFileLoaded;

        protected override async Task OnInitializedAsync()
        {
            if (await SessionStorageService.ContainKeyAsync(PrudenteSessionKey))
            {
                List<ExcelModel> fileRows =
                    await SessionStorageService.GetItemAsync<List<ExcelModel>>(PrudenteSessionKey);
                PopulateData(fileRows);
                GenerateCharts();
                StateHasChanged();
                _isFileLoaded = true;
            }
            else
            {
                _isFileLoaded = false;
            }
        }

        private List<ExcelModel> ReadContent(byte[] excelContent, bool reverse)
        {
            List<ExcelModel> fileRows = new();
            if (excelContent != null)
            {
                fileRows = ExcelReader.Read(excelContent).ToList();
            }

            if (reverse)
            {
                fileRows.Reverse();
            }

            return fileRows;
        }

        private async Task UploadFile(InputFileChangeEventArgs e)
        {
            byte[] fileContent;
            using (MemoryStream ms = new())
            {
                await e.File.OpenReadStream().CopyToAsync(ms);
                fileContent = ms.ToArray();
            }
            List<ExcelModel> fileRows = ReadContent(fileContent, reverse: true);
            PopulateData(fileRows);
            GenerateCharts();

            await SessionStorageService.SetItemAsync(PrudenteSessionKey, fileRows);
            _isFileLoaded = true;

            StateHasChanged();
        }

        private void GenerateCharts()
        {
            _config1 = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
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
                },
            };

            _config2 = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
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
                },
            };

            foreach (var periodo in PeriodoTemporale)
            {
                _config1.Data.Labels.Add(periodo);
                _config2.Data.Labels.Add(periodo);
            }

            IDataset<double> valoreQuoteDataSet = new LineDataset<double>(ValoreQuote)
            {
                Label = "Valore quote",
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Red),
                BorderColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Red),
                Fill = FillingMode.Disabled,
            };
            IDataset<double> valoreInvDataSet = new LineDataset<double>(ValoreInvestimento)
            {
                Label = "Valore Investimento",
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Blue),
                BorderColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Blue),
                Fill = FillingMode.Disabled
            };

            _config1.Data.Datasets.Add(valoreQuoteDataSet);
            _config2.Data.Datasets.Add(valoreInvDataSet);
        }

        private void PopulateData(List<ExcelModel> fileRows)
        {
            List<ChartModel> chartModels = DataMapperHelper.MapToChartModel(fileRows);

            foreach (var chartModel in chartModels)
            {
                PeriodoTemporale.Add(chartModel.Data);
                ValoreQuote.Add(chartModel.ValoreQuota);
                ValoreInvestimento.Add(chartModel.ValoreInvestimento);
            }
        }
    }
}