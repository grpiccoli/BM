using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Extensions
{
    public static class TableFilter
    {
        public static IQueryable<TSource> Pre<TSource>(this IEnumerable<TSource> contexto)
        {
            var pre = contexto as IQueryable<TSource>;
            return pre;
        }
        public static PropertyDescriptor FilterSort<TSource>(this IEnumerable<TSource> contexto, string srt = "Id")
        {
            if (String.IsNullOrWhiteSpace(srt)) srt = "Id";
            var sorts = srt.Split(',').ToList();
            PropertyDescriptor sort = TypeDescriptor.GetProperties(typeof(TSource)).Find(srt, false);

            return sort;
        }
        public static ViewDataDictionary ViewData<TSource>(this IEnumerable<TSource> contexto,
            IQueryable<TSource> pre, int? pg = 1, int? rpp = 20, string srt = "Id",
            bool? asc = true, string[] val = null)
        {
            Dictionary<string, List<string>> Filters = new Dictionary<string, List<string>> { };

            if (val != null && val.Count() > 0)
            {
                foreach (var filter in val)
                {
                    var method = filter.Split(':').Take(2).ToArray();
                    Filters[method[0]] = method[1].Split(',').ToList();
                }

                foreach (var filter in Filters)
                {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(TSource)).Find(filter.Key, false);

                    pre = prop.PropertyType == typeof(DateTime) ?
                        pre.Where(x => filter.Value
                        .Contains(String.Format("{0:dd-MM-yyyy}", prop.GetValue(x)))) :
                        pre.Where(x => filter.Value
                        .Contains(prop.GetValue(x).ToString()));
                }
            }

            var ViewData = new ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
            { };
            ViewData["last"] = (int)Math.Ceiling((double)((decimal)contexto.Count() / rpp));
            ViewData["srt"] = srt;
            ViewData["val"] = val;
            ViewData["Filters"] = Filters;
            ViewData["asc"] = asc.Value;
            ViewData["pg"] = pg.Value;
            ViewData["rpp"] = rpp.Value;

            return ViewData;
        }

        public static IDictionary<string, List<string>> Filters<TSource>(this IEnumerable<TSource> contexto, IQueryable<TSource> pre, string val = null)
        {
            Dictionary<string, List<string>> Filters = new Dictionary<string, List<string>> { };

            if (!String.IsNullOrEmpty(val))
            {
                var filters = val.Split(';');
                foreach (var filter in filters)
                {
                    var method = filter.Split(':').Take(2).ToArray();
                    Filters[method[0]] = method[1].Split(',').ToList();
                }

                foreach (var filter in Filters)
                {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(TSource)).Find(filter.Key, false);

                    pre = prop.PropertyType == typeof(DateTime) ?
                        pre.Where(x => filter.Value
                        .Contains(String.Format("{0:dd-MM-yyyy}", prop.GetValue(x)))) :
                        pre.Where(x => filter.Value
                        .Contains(prop.GetValue(x).ToString()));
                }
            }
            return Filters;
        }

        public static List<object> Variables(
            int? pg = 1, int? rpp = 20, string srt = "Id",
            bool? asc = true)
        {
            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (String.IsNullOrEmpty(srt)) srt = "Date";
            if (asc == null) asc = false;

            bool _asc = asc.Value;

            return new List<object> { pg, rpp, srt, _asc };
        }
    }
}
