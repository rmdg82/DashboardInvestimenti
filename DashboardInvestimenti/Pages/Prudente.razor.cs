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
using System.Collections.Generic;
using DashboardInvestimenti.Helpers;
using MudBlazor;
using Microsoft.Extensions.Configuration;
using Blazored.SessionStorage;
using System;
using System.Globalization;

namespace DashboardInvestimenti.Pages
{
    public partial class Prudente
    {
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }

        [Inject]
        public IExcelReader<ExcelModel> ExcelReader { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        public List<string> PeriodoTemporale { get; set; } = new();
        public List<double> ValoreQuote { get; set; } = new();
        public List<double> ValoreInvestimento { get; set; } = new();

        private readonly string _prudenteFileRowsSessionKey = "prudenteFileRows";
        private readonly string _prudenteDocDataSessionKey = "prudenteDataDoc";

        private string NomeContratto => Configuration["IdContratti:prudente"];

        private readonly LineConfig _config1 = new()
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

        private readonly LineConfig _config2 = new()
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

        private bool _isFileLoaded;
        private string _dataDocumento = string.Empty;
        private string _ultimoValoreQuota = string.Empty;
        private string _mediaValoreQuota = string.Empty;
        private double _mediaValoreQuotaValue;
        private string _guadagno = string.Empty;
        private string _coloreGuadagno = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (await SessionStorageService.ContainKeyAsync(_prudenteFileRowsSessionKey))
            {
                List<ExcelModel> fileRows =
                    await SessionStorageService.GetItemAsync<List<ExcelModel>>(_prudenteFileRowsSessionKey);
                if (await SessionStorageService.ContainKeyAsync(_prudenteDocDataSessionKey))
                {
                    _dataDocumento = await SessionStorageService.GetItemAsync<string>(_prudenteDocDataSessionKey);
                }

                PopulateData(fileRows);
                GenerateCharts();
                _isFileLoaded = true;
                StateHasChanged();
            }
            else
            {
                _isFileLoaded = false;
            }
        }

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
            if (!(await IsFileNameCorrect(e)))
            {
                return;
            }
            List<ExcelModel> fileRows = await GenerateRowsFromFile(e);
            PopulateData(fileRows);
            GenerateCharts();

            await SessionStorageService.SetItemAsync(_prudenteFileRowsSessionKey, fileRows);
            await SessionStorageService.SetItemAsync(_prudenteDocDataSessionKey, _dataDocumento);

            _isFileLoaded = true;

            StateHasChanged();
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

            if (splittedName[0] != NomeContratto)
            {
                await DialogService.ShowMessageBox("Attenzione", $"Il nome del file '{e.File.Name}' non è compatibile con il tipo di contratto '{NomeContratto}'!");
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
            ValoreQuote.Clear();
            ValoreInvestimento.Clear();
            ClearChartsData();
        }

        private void GenerateCharts()
        {
            foreach (var periodo in PeriodoTemporale)
            {
                _config1.Data.Labels.Add(periodo);
                _config2.Data.Labels.Add(periodo);
            }

            IDataset<double> valoreQuoteDataSet = new LineDataset<double>(ValoreQuote)
            {
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Red),
                BorderColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Red),
                Fill = FillingMode.Disabled,
            };

            IDataset<double> valoreQuotaAverageDataSet = new LineDataset<double>(Enumerable.Repeat<double>(_mediaValoreQuotaValue, ValoreQuote.Count))
            {
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.PaleGreen),
                Fill = FillingMode.Disabled,
            };

            IDataset<double> valoreInvDataSet = new LineDataset<double>(ValoreInvestimento)
            {
                BackgroundColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Blue),
                BorderColor = ColorUtil.FromDrawingColor(System.Drawing.Color.Blue),
                Fill = FillingMode.Disabled
            };

            _config1.Data.Datasets.Add(valoreQuoteDataSet);
            _config1.Data.Datasets.Add(valoreQuotaAverageDataSet);
            _config2.Data.Datasets.Add(valoreInvDataSet);
        }

        private void ClearChartsData()
        {
            _config1.Data.Labels.Clear();
            _config1.Data.Datasets.Clear();
            _config2.Data.Labels.Clear();
            _config2.Data.Datasets.Clear();
        }

        private void PopulateData(List<ExcelModel> fileRows)
        {
            List<ChartModel> chartModels = DataMapperHelper.MapToChartModel(fileRows);

            var lastRow = chartModels.Last();
            _ultimoValoreQuota = lastRow.ValoreQuota.ToString("C", CultureInfo.CreateSpecificCulture("it-IT"));

            _mediaValoreQuotaValue = GetAverageValoreQuota(chartModels);
            _mediaValoreQuota = _mediaValoreQuotaValue.ToString("C", CultureInfo.CreateSpecificCulture("it-IT"));

            double guadagno = lastRow.ValoreInvestimento - lastRow.Sottoscrizioni;
            string segnoGuadagno = guadagno >= 0 ? "+ " : string.Empty;
            _coloreGuadagno = guadagno >= 0 ? "green" : "red";
            _guadagno = segnoGuadagno + guadagno.ToString("C", CultureInfo.CreateSpecificCulture("it-IT"));

            ClearOldData();
            foreach (var chartModel in chartModels)
            {
                PeriodoTemporale.Add(chartModel.Data);
                ValoreQuote.Add(chartModel.ValoreQuota);
                ValoreInvestimento.Add(chartModel.ValoreInvestimento);
            }
        }

        private static double GetAverageValoreQuota(List<ChartModel> chartModels)
        {
            var sumValoriQuota = chartModels.Sum(line => line.ValoreQuota);
            int numLines = chartModels.Count;

            return Math.Round((sumValoriQuota / numLines), 4);
        }
    }
}