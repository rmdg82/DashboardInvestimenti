using System.Collections.Generic;

namespace DashboardInvestimenti.Services.Interfaces;

public interface IExcelReader<T> where T : class
{
    public string FolderPath { get; }

    IEnumerable<T> Read(string path);

    IEnumerable<T> Read(byte[] content);
}