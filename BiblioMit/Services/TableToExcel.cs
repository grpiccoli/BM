using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace BiblioMit.Services
{
    public class TableToExcel
    {
        readonly ExcelPackage excel = new ExcelPackage();
        readonly ExcelWorksheet sheet;
        private int maxRow = 0;
        //Dictionary<string, object> cellsOccupied = new Dictionary<string, object>();

        public TableToExcel()
        {
            sheet = excel.Workbook.Worksheets.Add("sheet1");
        }

        public void Process(string html, out ExcelPackage output)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            var elements = document.All.Where(e => (e.LocalName == "tr" && !e.InnerHtml.Contains("<tr")) || e.LocalName == "br");
            foreach (var e in elements)
            {
                ProcessRows(e);
            }
            try
            {
                output = excel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ProcessRows(IElement row)
        {
            int rowIndex = 1;
            int colIndex;
            if (maxRow > 0)
            {
                rowIndex = maxRow;
            }
            if (string.IsNullOrWhiteSpace(row.InnerHtml))
            {
                colIndex = 1;
                sheet.Cells[rowIndex, colIndex].Value = string.Empty;
                ++rowIndex;
                if (rowIndex > maxRow)
                {
                    maxRow = rowIndex;
                }
                return;
            }

            colIndex = 1;
            foreach (var td in row.QuerySelectorAll("td"))
            {
                var text = td.TextContent.Replace("\n","").Replace("\r","").Replace("\r\n","").Trim();
                    sheet.Cells[rowIndex, colIndex].Value = text;
                ++colIndex;
            }
            ++rowIndex;
            if (rowIndex > maxRow)
            {
                maxRow = rowIndex;
            }
        }

        //private void SpanRow(int rowIndex, int colIndex, int rowSpan)
        //{
        //    sheet.Cells[rowIndex, colIndex, rowIndex + rowSpan - 1, colIndex].Merge = true;
        //    for (int i = 0; i < rowSpan; i++)
        //    {
        //        cellsOccupied.Add((rowIndex + i) + "_" + colIndex, true);
        //    }
        //    if (rowIndex + rowSpan - 1 > maxRow)
        //    {
        //        maxRow = rowIndex + rowSpan - 1;
        //    }
        //}

        //private void SpanCol(int rowIndex, int colIndex, int colSpan)
        //{
        //    sheet.Cells[rowIndex, colIndex, rowIndex, colIndex + colSpan - 1].Merge = true;
        //}

        //private void SpanRowAndCol(int rowIndex, int colIndex, int rowSpan, int colSpan)
        //{
        //    sheet.Cells[rowIndex, colIndex, rowIndex + rowSpan - 1, colIndex + colSpan - 1].Merge = true;
        //    for (int i = 0; i < rowSpan; i++)
        //    {
        //        for (int j = 0; j < colSpan; j++)
        //        {
        //            cellsOccupied.Add((rowIndex + i) + "_" + (colIndex + j), true);
        //        }
        //    }
        //    if (rowIndex + rowSpan - 1 > maxRow)
        //    {
        //        maxRow = rowIndex + rowSpan - 1;
        //    }
        //}

        //private int GetSpan(string html, int spanType = 0)
        //{
        //    string spanTypeText;

        //    if (spanType == 0)
        //    {
        //        spanTypeText = "row";
        //    }
        //    else
        //    {
        //        spanTypeText = "col";
        //    }
        //    string equation = Regex.Match(html.ToLower(), spanTypeText + @"span=.*?\d{1,}").ToString();
        //    if (!int.TryParse(Regex.Match(equation, @"\d{1,}").ToString(), out int span))
        //    {
        //        span = 1;
        //    }

        //    return span;
        //}
    }
}
