using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiblioMit.Extensions
{
    public static partial class EnumUtils
    {
        public static IEnumerable<SelectListItem> Enum2Select<TEnum>(string name = null)
    where TEnum : struct, IConvertible, IFormattable
        {
            switch (name)
            {
                case "Name":
                    return ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new SelectListItem
                        {
                            Value = t.ToString("d", null),
                            Text = t.GetAttrName()
                        }).ToList();
                case "Description":
                    return ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new SelectListItem
                        {
                            Value = t.ToString("d", null),
                            Text = t.GetAttrDescription()
                        }).ToList();
                default:
                    return ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new SelectListItem
                        {
                            Value = t.ToString("d", null),
                            Text = t.ToString()
                        }).ToList();
            }
        }
        public static IEnumerable<TEnum> Enum2List<TEnum>()
where TEnum : struct, IConvertible, IFormattable
        {
            return ((TEnum[])Enum.GetValues(typeof(TEnum)))
                .Select(t => t).ToList();
        }
    }
}
