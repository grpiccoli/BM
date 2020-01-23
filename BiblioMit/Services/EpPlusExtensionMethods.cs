using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public static class EpPlusExtensionMethods
    {
        public static int GetColumnByName(this ExcelWorksheet ws, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            return ws.Cells["1:1"].First(c => c.Value.ToString().ToLower() == columnName.ToLower()).Start.Column;
        }

        public static int GetRowByValue(this ExcelWorksheet ws, char col, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            return ws.Cells[$"{col}:{col}"].First(c => c.Value.ToString().ToLower() == columnName.ToLower()).Start.Row;
        }
    }
}
