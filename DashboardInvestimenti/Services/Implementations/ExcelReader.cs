using DashboardInvestimenti.Models;
using DashboardInvestimenti.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DashboardInvestimenti.Services.Implementations
{
    public class ExcelReader : IExcelReader<ExcelModel>
    {
        private readonly IConfiguration _configuration;

        public ExcelReader(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string FolderPath => _configuration["ExcelFolder:path"];

        public IEnumerable<ExcelModel> Read(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' non può essere Null o uno spazio vuoto.", nameof(path));
            }

            var mapper = new Mapper(path);
            return mapper.Take<ExcelModel>()
                .Where(m => m.Value.IdContratto != null)
                .Select(m => m.Value);
        }

        public IEnumerable<ExcelModel> Read(byte[] content)
        {
            var result = new List<ExcelModel>();

            if (content is not null)
            {
                using var ms = new MemoryStream(content);
                var mapper = new Mapper(ms);
                result = mapper.Take<ExcelModel>()
                    .Where(m => m.Value.IdContratto != null)
                    .Select(m => m.Value)
                    .ToList();
            }

            return result;
        }
    }
}