using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public static class RangoAnual
    {
        public static SelectList Get(int i, bool ab)
        {
            var meses = ab ? DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames : DateTimeFormatInfo.CurrentInfo.MonthNames;
            return new SelectList(
                from int n in Enumerable.Range(0, 12 / i).ToArray()
                select new
                {
                    Id = i == 1 ? (n + 1).ToString() : (n * i + 1) + "-" + (n * i + i),
                    Name = i == 1 ? meses[n] : meses[n * i] + "-" + meses[n * i + i - 1]
                }, "Id", "Name");
        }
    }
}
