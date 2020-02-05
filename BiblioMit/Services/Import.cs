using BiblioMit.Data;
using BiblioMit.Extensions;
using BiblioMit.Models;
using Microsoft.AspNetCore.SignalR;
using NCalc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class Import : IImport
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<EntryHub> _hubContext;
        public Import(ApplicationDbContext context,
            IHubContext<EntryHub> hubContext)
        {
            _hubContext = hubContext;
            _context = context;
        }
        public (Dictionary<string, Dictionary<string, object>>, string) Analyze<T>
            (
                string planilla,
                BindingFlags bindingFlags,
                List<string> toskip = null
            )
        {
            var data = typeof(T).GetFields(bindingFlags).Concat(typeof(T).BaseType.GetFields(bindingFlags)).ToArray();

            var Id = _context.Excel.SingleOrDefault(e => e.Name == planilla).Id;

            var ddata = new Dictionary<string, Dictionary<string, object>> { };

            foreach (var dt in data)
            {
                var name = Regex.Replace(dt.Name, "<([a-zA-Z0-9_]+)>.*", "$1");
                if (toskip != null && toskip.Any(s => s == name)) continue;
                Console.WriteLine($"{Id} {name}");
                var var = _context.Columna.SingleOrDefault(c => c.ExcelId == Id && c.Name == name);
                var tmp = new Dictionary<string, object>
                {
                    { "type", dt.FieldType },
                };
                if (var == null)
                {
                    return (null, name);
                }
                if (string.IsNullOrWhiteSpace(var.Description)) continue;
                tmp.Add("var", var.Description);
                tmp.Add("opt", var.Operation);
                ddata.Add(name, tmp);
            }
            return (ddata, null);
        }
        
        public async Task<string> Fito(ExcelPackage package,
        List<string> toskip = null)
        {
            (Centre centre, EnsayoFito item, List<Phytoplankton> fitos, _, string error) = 
                await AmbAsync(package, toskip)
                .ConfigureAwait(false);

            if (!string.IsNullOrEmpty(error)) return error;

            if (centre != null) _context.Centre.Update(centre);
            await _context.EnsayoFito.AddAsync(item)
                .ConfigureAwait(false);
            await _context.Phytoplankton.AddRangeAsync(fitos.ToArray())
                .ConfigureAwait(false);
            await _context.SaveChangesAsync()
                .ConfigureAwait(false);
            return null;
        }

        public async Task<(Centre, EnsayoFito, List<Phytoplankton>, List<Groups>, string)> AmbAsync(ExcelPackage package,
        List<string> toskip = null)
        {
            if (package == null) return (null, null, null, null, "missing arguments");
            var planilla = nameof(EnsayoFito);

            var fitos = new List<Phytoplankton>();
            var groups = new List<Groups>();

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            (Dictionary<string, Dictionary<string, object>> tdata, string error)
                = Analyze<EnsayoFito>(planilla, bindingFlags, toskip);

            if (error != null) return (null, null, null, null, error);

            var worksheet = package.Workbook.Worksheets.First();
            if (worksheet == null) return (null, null, null, null, "No existe worksheet");

            var item = Activator.CreateInstance<EnsayoFito>();

            foreach (var d in tdata)
            {
                MethodInfo method = typeof(Import).GetMethod("GetFromExcel")
                    .MakeGenericMethod(new Type[] { (Type)d.Value["type"] });
                object value = null;
                value = method.Invoke(value, new object[] { worksheet, (string)d.Value["var"], null });
                if(d.Key == "Id" && value == null)
                {
                    return (null, null, null, null, "Archivo presenta errores no se encontró Id");
                }
                if(d.Key == "PSMBId" && value == null)
                {
                    continue;
                }
                else
                {
                    item[d.Key] = value;
                }
            }

            var find = await _context.FindAsync<EnsayoFito>(item.Id)
                .ConfigureAwait(false);
            if (find != null) return (null, null, null, null, "Archivo ya ingresado");

            //Get Fitos
            var Id = _context.Excel.SingleOrDefault(e => e.Name == planilla).Id;
            var fin = _context.Columna.SingleOrDefault(c => c.ExcelId == Id && c.Name == "TEXTO FIN RESULTADOS DE MUESTRAS DE AGUA");
            var end = worksheet.GetRowByValue('A', fin.Description);
            var inicio = _context.Columna.SingleOrDefault(c => c.ExcelId == Id && c.Name == "CELDA INICIO RESULTADOS DE MUESTRAS DE AGUA");
            var parsed = int.TryParse(Regex.Replace(inicio.Description, "[A-Z]+", ""), out int start);

            if (!parsed) return (null, null, null, null, "error parsing start");

            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;

            var gen = Regex.Replace(textInfo
                .ToTitleCase(worksheet.Cells[$"A{start+1}"].Value.ToString().ToUpperInvariant()), " .*", "");

            for (int row = start+2; row < end; row++)
            {
                var sp = worksheet.Cells[row, 1].Value.ToString();

                if (sp.ToUpperInvariant().Contains("TOTAL", StringComparison.InvariantCultureIgnoreCase)) continue;

                var c = Regex.Replace(worksheet.Cells[row, 3].Value.ToString(), @",([0-9]{3})", @"$1");

                if (string.IsNullOrWhiteSpace(c))
                {
                    gen = Regex.Replace(textInfo
                        .ToTitleCase(worksheet.Cells[row, 1].Value.ToString().ToUpperInvariant()), " .*", "");
                    continue;
                }

                var e = worksheet.Cells[row, 2].Value.ToString();
                var ce = Convert.ToDouble(c, CultureInfo.CreateSpecificCulture("es-CL"));
                var group = new Groups();
                try
                {
                    group = _context.Groups.SingleOrDefault(g => g.Name == gen);
                }
                catch
                {
                    return (null, null, null, null, $"Grupo {gen} no encontrado");
                }
                if (group == null)
                {
                    group = new Groups
                    {
                        Id = _context.Groups.Max(g => g.Id),
                        Name = gen
                    };
                    groups.Add(group);
                }
                var fito = new Phytoplankton
                {
                    GroupsId = group.Id,
                    Species = sp,
                    C = ce,
                    EnsayoFitoId = item.Id
                };
                if (!string.IsNullOrWhiteSpace(e))
                {
                    fito.EAR = (EAR)Convert.ToInt16(e, new CultureInfo("es-CL"));
                }
                fitos.Add(fito);
            }

            if (item.CentreId.HasValue)
            {
                var centre = await _context.FindAsync<Centre>(item.CentreId.Value)
                    .ConfigureAwait(false);

                if (centre == null)
                {
                    if (item.CentreId.Value == item.PSMBId)
                    {
                        //return "No se especificó Centro";
                        item.CentreId = null;
                    }
                    else
                    {
                        return (null, null, null, null,
$"El centro {item.CentreId} de la declaración {item.Id} con fecha {item.FechaMuestreo} no existe en la base de datos, favor verificar centro");
                    }
                }
                else
                {
                    if(centre.PSMBId.HasValue) item.PSMBId = centre.PSMBId.Value;
                }
            }
            else
            {
                return (null, null, null, null,
"No se especifica número de Centro válido y no se puede deducir a partir del número de centro");
            }

            var psmb = await _context.FindAsync<PSMB>(item.PSMBId)
                .ConfigureAwait(false);
            if (psmb == null)
                return (null, null, null, null,
$"PSMB {item.PSMBId} de la declaración {item.Id} con fecha {item.FechaMuestreo} de entidad {item.EntidadMuestreadora} no existe en base de datos, favor verificar dato");

            return (null, item, fitos, groups, null);
        }

        public async Task Read<T>(ExcelPackage package, ProdEntry entry,
        List<string> toskip = null) where T : Planilla
        {
            if (entry == null || package == null) return;
            double pgr = 0;
            var planilla = entry.Reportes.ToString();
            var planillas = new List<T> { };

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var msg = string.Empty;

            (Dictionary<string, Dictionary<string, object>> tdata, string error)
                = Analyze<T>(planilla, bindingFlags, toskip);

            if (error != null)
            {
                msg =
$@">ERROR: El valor para la columna {error} no fue encontrada en la base de datos.
0 registros procesados. Verificar archivo.";

                entry.OutPut += msg;

                _context.Update(entry);
                await _context.SaveChangesAsync()
                    .ConfigureAwait(false);

                return;
            };

            var pgrTotal = 100;

            pgr += 4;

            var pgrReadWrite = (pgrTotal - pgr) / 6;

            var pgrRow = pgrReadWrite / package.Workbook.Worksheets.Where(w => w.Dimension != null).Sum(w => w.Dimension.Rows);

            var status = "info";

            foreach (var worksheet in package.Workbook.Worksheets.Where(w => w.Dimension != null))
            {
                if (worksheet == null) break;
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null)
                    {
                        msg = $">W: Fila '{row}' en hoja '{worksheet.Name}' Está vacía.";

                        entry.OutPut += msg;

                        await _hubContext
                            .Clients.All
                            .SendAsync("Update", "log", msg)
                            .ConfigureAwait(false);

                        status = "warning";
                        await _hubContext
                            .Clients.All
                            .SendAsync("Update", "status", status)
                            .ConfigureAwait(false);

                        continue;
                    }
                    var item = Activator.CreateInstance<T>();

                    item.Row = row;

                    item.Sheet = worksheet.Name;

                    foreach (var d in tdata)
                    {
                        MethodInfo method = typeof(Import).GetMethod("GetFromExcel")
                            .MakeGenericMethod(new Type[] { (Type)d.Value["type"] });
                        object value = null;
                        value = method.Invoke(value, new object[] { worksheet, (string)d.Value["var"], row });
                        if (value == null)
                        {
                            status = "danger";

                            msg =
                                $">ERROR: Columna '{(string)d.Value["var"]}' no encontrada en hoja '{worksheet.Name}'. Verificar archivo.\n0 registros procesados.";

                            entry.OutPut += msg;

                            _context.Update(entry);
                            await _context.SaveChangesAsync()
                                .ConfigureAwait(false);

                            await _hubContext
                            .Clients.All
                            .SendAsync("Update", "log", msg)
                            .ConfigureAwait(false);

                            await _hubContext
                                .Clients.All
                                .SendAsync("Update", "status", status)
                                .ConfigureAwait(false);

                            return;
                        }
                        if ((string)d.Value["opt"] != null)
                        {
                            Expression e = new Expression((value + (string)d.Value["opt"]).Replace(",", ".", StringComparison.InvariantCultureIgnoreCase));
                            item[d.Key] = e.Evaluate();
                        }
                        else
                        {
                            item[d.Key] = value;
                        }
                        System.Diagnostics.Debug.WriteLine($"column:{d.Key}");
                    }

                    //EXTRA STEPS
                    if (item.Fecha == DateTime.MinValue)
                    {
                        item.Fecha = new DateTime(item.Year, item.Month, 1);
                    }

                    var test = (item.Fecha).ToString("MMyyyy", new CultureInfo("es-CL"));

                    var n = item.Declaracion;

                    item.Dato = entry.Reportes;

                    var dt = (int)item.Dato;

                    var test2 = string.Format(new CultureInfo("es-CL"), "{0}{1}{2}",
                        n, dt, test);

                    item.Id = Convert.ToInt64(test2, new CultureInfo("es-CL"));

                    planillas.Add(item);

                    pgr += pgrRow;

                    await _hubContext
                        .Clients.All
                        .SendAsync("Update", "progress", pgr)
                        .ConfigureAwait(false);
                    await _hubContext
                        .Clients.All
                        .SendAsync("Update", "status", status)
                        .ConfigureAwait(false);

                    Debug.WriteLine($"row:{item.Row} sheet:{item.Sheet}");
                }
            }

            entry.Min = planillas.Min(p => p.Fecha);
            entry.Max = planillas.Max(p => p.Fecha);

            var registros = planillas.GroupBy(p => p.Id);

            var pgrP = pgrReadWrite * 5 / registros.Count();

            var datos = new List<T>();
            var origenes = new List<Origen>();
            var centros = new List<Centre>();
            var updates = new List<T>();

            foreach (var r in registros)
            {
                var dato = r.First();
                dato.Dato = dato.Dato > 0 ? dato.Dato : entry.Reportes;
                dato.Peso = r.Sum(p => p.Peso);
                dato.Row = r.Sum(p => p.Row);

                var find = await _context.FindAsync<T>(dato.Id).ConfigureAwait(false);

                if (find == null)
                {
                    if (dato.Dato == Tipo.Semilla && !origenes.Any(o => o.Id == dato.OrigenId))
                    {
                        var orig = await _context.FindAsync<Origen>(dato.OrigenId).ConfigureAwait(false);

                        if (orig == null)
                        {
                            try
                            {
                                var origen = new Origen
                                {
                                    Id = dato.OrigenId.Value,
                                    Name = dato.Origen
                                };

                                origenes.Add(origen);
                                //context.Origen.Add(origen);
                                //context.SaveChanges();
                            }
                            catch
                            {
                                msg = $">W: Origen no existe en archivo." +
                                    $">Declaración de {dato.Dato} N°{dato.Id}, con fecha {dato.Fecha}, " +
                                    $"en hoja {dato.Sheet}, filas {dato.Rows} no pudieron ser procesadas.\n" +
                                    $">Verificar archivo.";

                                entry.OutPut += msg;
                                entry.Observaciones++;

                                await _hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg)
                                .ConfigureAwait(false);

                                status = "warning";
                                await _hubContext
                                    .Clients.All
                                    .SendAsync("Update", "status", status)
                                    .ConfigureAwait(false);

                                continue;
                            }
                        }
                    }

                    if (!centros.Any(o => o.Id == dato.CentreId))
                    {
                        var parent = await _context.FindAsync<Centre>(dato.CentreId)
                            .ConfigureAwait(false);

                        if (parent == null)
                        {
                            try
                            {
                                var comuna = _context.Comuna.FirstOrDefault(c => 
                                c.Name.ToUpperInvariant() == 
                                dato.NombreComuna.ToString(new CultureInfo("es-CL")).ToUpperInvariant());
                                var centre = new Centre
                                {
                                    Id = dato.CentreId,
                                    ComunaId = comuna.Id,
                                    CompanyId = 0,
                                    PSMBId = 0,
                                    Type = (CentreType)2,
                                    CuerpoAgua = CuerpoAgua.Mar
                                };

                                centros.Add(centre);
                                //context.Centre.Add(centre);
                                //context.SaveChanges();
                            }
                            catch
                            {
                                msg = $">W: Comuna {dato.NombreComuna} no existe en base de datos." +
                                    $">Declaración de {dato.Dato} N°{dato.Id}, con fecha {dato.Fecha}, " +
                                    $"en hoja {dato.Sheet}, filas {dato.Rows} no pudieron ser procesadas.\n" +
                                    $">Verificar archivo.";

                                entry.OutPut += msg;
                                entry.Observaciones++;

                                await _hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg)
                                .ConfigureAwait(false);

                                status = "warning";
                                await _hubContext
                                    .Clients.All
                                    .SendAsync("Update", "status", status)
                                    .ConfigureAwait(false);

                                continue;
                            }
                        }
                    }

                    datos.Add(dato);
                    //context.Add(dato);
                    try
                    {
                        //context.SaveChanges();
                        entry.Agregadas++;
                        await _hubContext
                            .Clients.All
                            .SendAsync("Update", "agregada", entry.Agregadas)
                            .ConfigureAwait(false);
                        //context.Entries.Update(entry);
                    }
                    catch
                    {
                        //Buscadores de Empresas
                        //http://www.genealog.cl
                        //https://www.mercantil.com
                        //https://www.smartx.cl
                        //Buscador de personas
                        //https://www.nombrerutyfirma.cl

                        msg = $">W: Declaración de {dato.Dato} N°{dato.Id}, con fecha {dato.Fecha}, en hoja {dato.Sheet}, filas {dato.Rows} no pudieron ser procesadas. Verificar archivo.";

                        await _hubContext
                            .Clients.All
                        .SendAsync("Update", "log", msg)
                        .ConfigureAwait(false);

                        entry.OutPut += msg;

                        var centre = await _context.Centre.FindAsync(dato.CentreId)
                            .ConfigureAwait(false);
                        if (centre == null)
                        {
                            msg = $">Centro {dato.CentreId} no existe en base de datos.";

                            await _hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg)
                                .ConfigureAwait(false);

                            entry.OutPut += msg;
                        }

                        entry.OutPut += msg;
                        entry.Observaciones++;

                        status = "warning";
                        await _hubContext
                            .Clients.All
                            .SendAsync("Update", "status", status)
                            .ConfigureAwait(false);
                        continue;
                    }
                }
                else
                {
                    var updated = AddChanges(find, dato);
                    if (updated != find)
                    {
                        updates.Add(updated);
                        //context.Update(updated);
                        try
                        {
                            //context.SaveChanges();
                            entry.Actualizadas++;
                            await _hubContext
                                .Clients.All
                                .SendAsync("Update", "agregada", entry.Agregadas)
                                .ConfigureAwait(false);
                            //context.Entries.Update(entry);
                        }
                        catch
                        {
                            msg = $">Unknown error: Declaración de {dato.Dato} N°{dato.Id}, con fecha {dato.Fecha}, en hoja {dato.Sheet}, filas {dato.Rows} no pudieron ser procesadas. Verificar archivo.";

                            await _hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg)
                                .ConfigureAwait(false);

                            entry.OutPut += msg;
                            entry.Observaciones++;

                            status = "warning";
                            await _hubContext
                                .Clients.All
                                .SendAsync("Update", "status", status)
                                .ConfigureAwait(false);
                            continue;
                        }
                    }
                }

                pgr += pgrP;

                await _hubContext.Clients.All
                    .SendAsync("Update", "progress", pgr)
                    .ConfigureAwait(false);
                await _hubContext.Clients.All
                    .SendAsync("Update", "status", status)
                    .ConfigureAwait(false);
            }

            await _context.Centre.AddRangeAsync(centros).ConfigureAwait(false);
            await _context.Origen.AddRangeAsync(origenes).ConfigureAwait(false);
            await _context.Planilla.AddRangeAsync(datos).ConfigureAwait(false);
            _context.Planilla.UpdateRange(updates);

            //context.BulkInsert(centros);
            //context.BulkInsert(origenes);
            //context.BulkInsert(datos);
            //context.BulkUpdate(updates);

            status = "success";

            await _hubContext.Clients.All
                .SendAsync("Update", "progress", 100)
                .ConfigureAwait(false);
            await _hubContext.Clients.All
                .SendAsync("Update", "status", status)
                .ConfigureAwait(false);

            msg = $">{entry.Agregadas} añadidos" + (entry.Actualizadas != 0 ? $"y {entry.Actualizadas} registros actualizados " : " ") + "exitosamente.";
            entry.OutPut += msg;
            entry.Success = true;
            await _hubContext.Clients.All.SendAsync("Update", "log", msg)
                .ConfigureAwait(false);

            _context.ProdEntry.Update(entry);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public static T AddChanges<T>(T val1, T val2) where T : class, IHasBasicIndexer
        {
            if(val1 != null && val2 != null){
                FieldInfo[] fi = val1.GetType().GetFields();

                foreach (FieldInfo f in fi)
                {
                    var var1 = f.GetValue(val1);
                    var var2 = f.GetValue(val2);
                    if (var1 != var2)
                    {
                        val1[f.Name] = val2[f.Name];
                    }
                }
            }
            return val1;
        }

        public static bool IsBoxed<T>(T value)
        {
            return
                (typeof(T).IsInterface || typeof(T) == typeof(object)) &&
                value != null &&
                value.GetType().IsValueType;
        }

        public static object GetFromExcel<T>(ExcelWorksheet worksheet, string var, int? row)
        {
            if (worksheet == null) return null;
            int? col;
            object value = null;
            var type = typeof(T);
            if (string.IsNullOrWhiteSpace(var)) return null;

            object val;
            if (row.HasValue)
            {
                try
                {
                    col = worksheet.GetColumnByName(var);
                    val = worksheet.Cells[row.Value, col.Value].Value;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                val = worksheet.Cells[var].Value;
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                if (val != null)
                {
                    var num = Regex.Replace(Regex.Replace(val.ToString(), @"(-.*|\.+)$", ""), @"[^0-9]", "");
                    if (!string.IsNullOrWhiteSpace(num))
                    {
                        value = Convert.ToInt32(num, CultureInfo.InvariantCulture);
                    }
                }
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                if (val != null)
                {
                    var num = Regex.Replace(Regex.Replace(val.ToString(), @"(?<=^.+)(-.*|\.+)$", ""), @"[^0-9\.,-]", "");
                    if (!string.IsNullOrWhiteSpace(num))
                    {
                        var culture = CultureInfo.CreateSpecificCulture(num.Contains(',', StringComparison.InvariantCultureIgnoreCase) ? "es-CL" : "en-GB");
                        value = Convert.ToDouble(num, culture);
                    }
                }
            }
            else if (type == typeof(string))
            {
                value = val != null ?
                                val.ToString() : "";
            }
            else if (type == typeof(bool))
            {
                value = val != null &&
                    val.ToString() == "Si" || val.ToString() == "Yes";
            }
            else if (type == typeof(DateTime))
            {
                var sDate = val.ToString();
                var formats = new string[] { "yyyyMMdd", "yyyy-MM-dd", "dd-MM-yyyy HH:mm", "dd-MM-yyyy", "dd-MM-yyyy HH:mm'&nbsp;'" };
                DateTime.TryParseExact(sDate, formats, new CultureInfo("es-CL"), DateTimeStyles.None, out DateTime date);
                //DateTime.TryParse(sDate, out DateTime date);
                value = date;
            }
            else if (type == typeof(ProductionType?) || type == typeof(ProductionType))
            {
                foreach (var tipo in Enum.GetNames(typeof(ProductionType)))
                {
                    if (val.ToString().ToUpperInvariant()
                        .Contains(tipo.ToString(CultureInfo.InvariantCulture)
                        .ToUpperInvariant(),
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        var parsed = Enum.TryParse(tipo, out ProductionType production);
                        if (parsed)
                        {
                            value = production;
                            break;
                        }
                    }
                }
                if (value == null) value = ProductionType.Desconocido;
            }
            else if (type == typeof(Item?) || type == typeof(Item))
            {
                foreach (var tipo in Enum.GetNames(typeof(Item)))
                {
                    if (val.ToString().ToUpperInvariant()[0] == 
                        tipo.ToString(CultureInfo.InvariantCulture).ToUpperInvariant()[0])
                    {
                        var parsed = Enum.TryParse(tipo, out Item production);
                        if (parsed)
                        {
                            value = production;
                            break;
                        }
                    }
                }
            }
            else if (type == typeof(Tipo))
            {
                value = val.ToString().ToUpperInvariant()
                    .Contains("M", StringComparison.InvariantCultureIgnoreCase) ? Tipo.MateriaPrima : Tipo.Producción;
            }
            else if (type == typeof(Enum))
            {
                value = Enum.Parse(DataTableExtensions
                    .GetEnumType("BiblioMit.Models." + typeof(T).ToString()), val.ToString());
            }
            try
            {
                return (T)value;
            }
            catch
            {
                return null;
            }
        }
    }
}