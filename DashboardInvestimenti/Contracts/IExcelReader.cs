using DashboardInvestimenti.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardInvestimenti.Contracts
{
    public interface IExcelReader<T> where T : class
    {
        public string FolderPath { get; }

        IEnumerable<T> Read(string path);

        IEnumerable<T> Read(byte[] content);
    }
}