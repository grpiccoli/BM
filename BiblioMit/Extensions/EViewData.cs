//using BiblioMit.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace BiblioMit.Extensions
{
    public static class EViewData
    {
        public static MultiSelectList Enum2Select<TEnum>(
            IDictionary<string, List<string>> Filters = null,
            string name = null)
        {
            var tipo = typeof(TEnum);
            var nombre = tipo.ToString().Split('.').Last();
            switch (name)
            {
                case "Description":
                    {
                        return new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Id = s, Name = s.GetAttrDescription() },
                            "Id", "Name",
                            Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null);
                    }
                case "Name":
                    {
                        return new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Id = s, Name = s.GetAttrName() },
                            "Id", "Name",
                            Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null);
                    }
                default:
                    {
                        return new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Id = s, Name = s.ToString() },
                            "Id", "Name",
                            Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null);
                    }
            }
        }

        public static string GetAttrDescription<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
              .FirstOrDefault()
              ?.GetCustomAttribute<DisplayAttribute>(false)
              ?.Description
              ?? e.ToString();
        }

        public static string GetAttrPrompt<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
              .FirstOrDefault()
              ?.GetCustomAttribute<DisplayAttribute>(false)
              ?.Prompt
              ?? e.ToString();
        }
        public static string GetAttrName<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DisplayAttribute>(false)
                ?.Name
                ?? e.ToString();
        }
        public static string GetAttrName<TEnum>(this TEnum e, string lang)
        {
            switch (lang)
            {
                default:
                case "es":
                    //var rm = new ResourceManager(typeof(EnumResources));
                    //var name = e.GetType().Name + "_" + e;
                    //var resourceDisplayName = rm.GetString(name, CultureInfo.InvariantCulture);
                    //return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format(CultureInfo.InvariantCulture, "{0}", e) : resourceDisplayName;
                    return string.Format(CultureInfo.InvariantCulture, "{0}", e);
            }
        }
        public static string GetAttrGroupName<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
              .FirstOrDefault()
              ?.GetCustomAttribute<DisplayAttribute>(false)
              ?.GroupName
              ?? e.ToString();
        }
    }
}
