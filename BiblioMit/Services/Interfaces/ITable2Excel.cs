using OfficeOpenXml;

namespace BiblioMit.Services
{
    interface ITable2Excel
    {
        void Process(string html, out ExcelPackage output);
    }
}
