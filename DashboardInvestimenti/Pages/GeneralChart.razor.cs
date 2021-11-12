using Blazored.SessionStorage;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.Common.Handlers;
using ChartJs.Blazor.Interop;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
using DashboardInvestimenti.Helpers;
using DashboardInvestimenti.Models;
using DashboardInvestimenti.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardInvestimenti.Pages
{
    public partial class GeneralChart
    {
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        [Inject]
        public IExcelReader<ExcelModel> ExcelReader { get; set; }

        [Inject]
        public IFinancialCalculator Calculator { get; set; }

        public List<string> PeriodoTemporale { get; set; } = new();
        public List<double> Chart1Data { get; set; } = new();
        public List<double> Chart2Data { get; set; } = new();

        public string ContractId { get; set; }
        public string ContractName { get; set; }

        private readonly LineConfig _chart1Config = new()
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

        private readonly LineConfig _chart2Config = new()
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
                Legend = new Legend() { Display = false },
                Scales = new Scales
                {
                    YAxes = new List<CartesianAxis>
                    {
                        new LinearCartesianAxis
                        {
                            Ticks = new LinearCartesianTicks
                            {
                                SuggestedMax = 14,
                                SuggestedMin = 10,
                                StepSize = 2,
                            }
                        }
                    },
                }
            },
        };

        private bool _isFileLoaded = false;
        private string _dataDocumento = string.Empty;
        private string _ultimoValoreQuota = string.Empty;
        private string _mediaValoreQuota = string.Empty;
        private double _mediaValoreQuotaValue;
        private string _guadagnoNetto = string.Empty;
        private string _guadagnoPercentuale = string.Empty;
        private string _coloreGuadagno = string.Empty;
        private string _totInvestiti = string.Empty;

        private List<ExcelModel> ReadContent(byte[] excelContent, bool reverseRows)
        {
            List<ExcelModel> fileRows = new();
            if (excelContent != null)
            {
                fileRows = ExcelReader.Read(excelContent).ToList();
            }

            if (reverseRows)
            {
                fileRows.Reverse();
            }

            return fileRows;
        }

        private async Task UploadFile(InputFileChangeEventArgs e)
        {
            if (!await IsFileNameCorrect(e))
            {
                return;
            }

            (ContractId, ContractName) = GetContractInfos(e.File.Name);

            var fileRows = await GenerateRowsFromFile(e);
            PopulateData(fileRows);
            GenerateCharts();
            _isFileLoaded = true;
            StateHasChanged();
        }

        private (string contractId, string contractName) GetContractInfos(string fileName)
        {
            var splittedName = fileName.Split('_');
            var contractId = splittedName[0];
            var contractName = GetContractName(contractId);

            return (contractId, contractName);

            string GetContractName(string contractId)
            {
                var contractName = Configuration[$"Contracts:{contractId}"];
                return contractName;
            }
        }

        private async Task<List<ExcelModel>> GenerateRowsFromFile(InputFileChangeEventArgs e)
        {
            byte[] fileContent;
            using (MemoryStream ms = new())
            {
                await e.File.OpenReadStream().CopyToAsync(ms);
                fileContent = ms.ToArray();
            }

            return ReadContent(fileContent, reverseRows: true);
        }

        private async Task<bool> IsFileNameCorrect(InputFileChangeEventArgs e)
        {
            var splittedName = e.File.Name.Split('_');
            if (splittedName.Length != 4)
            {
                await DialogService.ShowMessageBox("Attenzione", $"Il nome del file '{e.File.Name}' non è corretto!");
                return false;
            }

            try
            {
                var dateFromFile = DateTime.Parse(splittedName[2]);
                _dataDocumento = dateFromFile.ToString("d/M/yyyy");
            }
            catch (FormatException)
            {
                await DialogService.ShowMessageBox("Attenzione", $"Il nome del file '{e.File.Name}' non ha una data leggibile!");
                return false;
            }

            return true;
        }

        private void ClearOldData()
        {
            PeriodoTemporale.Clear();
            Chart1Data.Clear();
            Chart2Data.Clear();
            ClearChartsData();
        }

        private void GenerateCharts()
        {
            foreach (var periodo in PeriodoTemporale)
            {
                _chart1Config.Data.Labels.Add(periodo);
                _chart2Config.Data.Labels.Add(periodo);
            }

            // Quote value on chart1
            IDataset<double> valoreQuoteDataSet = new LineDataset<double>(Chart1Data)
            {
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Red),
                BorderColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Red),
                Fill = FillingMode.Disabled,
            };

            // Average quote on chart1
            IDataset<double> valoreQuotaAverageDataSet = new LineDataset<double>(Enumerable.Repeat<double>(_mediaValoreQuotaValue, Chart1Data.Count))
            {
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.PaleGreen),
                Fill = FillingMode.Disabled,
            };

            // Total Investment value on chart2
            IDataset<double> valoreInvDataSet = new LineDataset<double>(Chart2Data)
            {
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Blue),
                BorderColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Blue),
                Fill = FillingMode.Disabled
            };

            _chart1Config.Data.Datasets.Add(valoreQuoteDataSet);
            _chart1Config.Data.Datasets.Add(valoreQuotaAverageDataSet);
            _chart2Config.Data.Datasets.Add(valoreInvDataSet);
        }

        private void ClearChartsData()
        {
            _chart1Config.Data.Labels.Clear();
            _chart1Config.Data.Datasets.Clear();
            _chart2Config.Data.Labels.Clear();
            _chart2Config.Data.Datasets.Clear();
        }

        private void PopulateData(List<ExcelModel> fileRows)
        {
            List<ChartModel> chartModels = DataMapperHelper.MapToChartModel(fileRows);

            _ultimoValoreQuota = Calculator.ToString(chartModels.Last().ValoreQuota);
            _mediaValoreQuotaValue = Calculator.GetAverageValoreQuota(chartModels);
            _mediaValoreQuota = Calculator.ToString(_mediaValoreQuotaValue);

            _guadagnoNetto = Calculator.GetLastGuadagnoNetto(chartModels, out _coloreGuadagno);
            _guadagnoPercentuale = Calculator.GetLastGuadagnoPercentuale(chartModels);
            _totInvestiti = Calculator.ToString(chartModels.Last().Sottoscrizioni);

            ClearOldData();
            foreach (var chartModel in chartModels)
            {
                PeriodoTemporale.Add(chartModel.Data);
                Chart1Data.Add(chartModel.ValoreQuota);
                //Chart2Data.Add(chartModel.ValoreInvestimento);
                Chart2Data.Add(Math.Round((Calculator.GetGuadagnoPercentuale(chartModel) * 100), 2));
            }
        }
    }
}