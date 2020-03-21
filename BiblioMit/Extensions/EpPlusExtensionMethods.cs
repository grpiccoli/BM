using OfficeOpenXml;
using System;
using System.Linq;

namespace BiblioMit.Extensions
{
    public static class EpPlusExtensionMethods
    {
        public static int GetColumnByName(this ExcelWorksheet ws, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            return ws.Cells["1:1"].FirstOrDefault(c => c.Value.ToString().Equals(columnName, StringComparison.InvariantCultureIgnoreCase)).Start.Column;
        }

        public static int GetRowByValue(this ExcelWorksheet ws, char col, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            return ws.Cells[$"{col}:{col}"].FirstOrDefault(c => c.Value.ToString().Equals(columnName, StringComparison.InvariantCultureIgnoreCase)).Start.Row;
        }
    }
}
