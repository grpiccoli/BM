using BiblioMit.Data;
using BiblioMit.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public interface IImport
    {
        Task<string> Fito(ExcelPackage package, List<string> toskip = null);
        Task Read<T>(ExcelPackage package, ProdEntry entry,
        List<string> toskip = null) where T : Planilla;
        Task<(Centre, EnsayoFito, List<Phytoplankton>, List<Groups>, string)> AmbAsync(ExcelPackage package,
        List<string> toskip = null);
    }
}
