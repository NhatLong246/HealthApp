using System.Threading.Tasks;

namespace HealthApp.Services.Interfaces
{
    public interface IExportService
    {
        Task<bool> ExportToExcelAsync(string filePath, object data);
    }
}

