using BiblioMit.Data;
using BiblioMit.Extensions;
using BiblioMit.Models;
using Microsoft.AspNetCore.SignalR;
using NCalc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public static class Import
    {
        public static (Dictionary<string, Dictionary<string, object>>, string) Analyze<T>
            (
                string planilla,
                ApplicationDbContext context,
                BindingFlags bindingFlags,
                List<string> toskip = null
            )
        {
            var data = typeof(T).GetFields(bindingFlags).Concat(typeof(T).BaseType.GetFields(bindingFlags)).ToArray();

            var Id = context.Excel.SingleOrDefault(e => e.Name == planilla).Id;

            var ddata = new Dictionary<string, Dictionary<string, object>> { };

            foreach (var dt in data)
            {
                var name = Regex.Replace(dt.Name, "<([a-zA-Z0-9_]+)>.*", "$1");
                if (toskip != null && toskip.Any(s => s == name)) continue;
                Console.WriteLine($"{Id} {name}");
                var var = context.Columna.SingleOrDefault(c => c.ExcelId == Id && c.Name == name);
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
        
        public async static Task<string> Fito(ExcelPackage package,
        ApplicationDbContext context,
        List<string> toskip = null)
        {
            (Centre centre, EnsayoFito item, List<Phytoplankton> fitos, _, string error) = 
                await AmbAsync(package, context, toskip)
                .ConfigureAwait(false);
            if (!string.IsNullOrEmpty(error)) return error;
            if (centre != null) context.Centre.Update(centre);
            context.EnsayoFito.Add(item);
            context.Phytoplankton.AddRange(fitos.ToArray());
            await context.SaveChangesAsync();
            return null;
        }

        public async static Task<(Centre, EnsayoFito, List<Phytoplankton>, List<Groups>, string)> AmbAsync(ExcelPackage package,
        ApplicationDbContext context,
        List<string> toskip = null)
        {
            var planilla = nameof(EnsayoFito);

            var fitos = new List<Phytoplankton>();
            var groups = new List<Groups>();

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            (Dictionary<string, Dictionary<string, object>> tdata, string error)
                = Analyze<EnsayoFito>(planilla, context, bindingFlags, toskip);

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

            var find = await context.FindAsync<EnsayoFito>(item.Id)
                .ConfigureAwait(false);
            if (find != null) return (null, null, null, null, "Archivo ya ingresado");

            //Get Fitos
            var Id = context.Excel.SingleOrDefault(e => e.Name == planilla).Id;
            var fin = context.Columna.SingleOrDefault(c => c.ExcelId == Id && c.Name == "TEXTO FIN RESULTADOS DE MUESTRAS DE AGUA");
            var end = worksheet.GetRowByValue('A', fin.Description);
            var inicio = context.Columna.SingleOrDefault(c => c.ExcelId == Id && c.Name == "CELDA INICIO RESULTADOS DE MUESTRAS DE AGUA");
            int.TryParse(Regex.Replace(inicio.Description, "[A-Z]+", ""), out int start);

            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;

            var gen = Regex.Replace(textInfo.ToTitleCase(worksheet.Cells[$"A{start+1}"].Value.ToString().ToLower()), " .*", "");

            for (int row = start+2; row < end; row++)
            {
                var sp = worksheet.Cells[row, 1].Value.ToString();

                if (sp.ToLower().Contains("total")) continue;

                var c = Regex.Replace(worksheet.Cells[row, 3].Value.ToString(), @",([0-9]{3})", @"$1");

                if (string.IsNullOrWhiteSpace(c))
                {
                    gen = Regex.Replace(textInfo.ToTitleCase(worksheet.Cells[row, 1].Value.ToString().ToLower()), " .*", "");
                    continue;
                }

                var e = worksheet.Cells[row, 2].Value.ToString();
                var ce = Convert.ToDouble(c, CultureInfo.CreateSpecificCulture("es-CL"));
                var group = new Groups();
                try
                {
                    group = context.Groups.SingleOrDefault(g => g.Name == gen);
                }
                catch
                {
                    return (null, null, null, null, $"Grupo {gen} no encontrado");
                }
                if (group == null)
                {
                    group = new Groups
                    {
                        Id = context.Groups.Max(g => g.Id),
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
                    fito.EAR = (EAR)Convert.ToInt16(e);
                }
                fitos.Add(fito);
            }

            if (item.CentreId.HasValue)
            {
                var centre = await context.FindAsync<Centre>(item.CentreId.Value)
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

            var psmb = await context.FindAsync<PSMB>(item.PSMBId)
                .ConfigureAwait(false);
            if (psmb == null)
                return (null, null, null, null,
$"PSMB {item.PSMBId} de la declaración {item.Id} con fecha {item.FechaMuestreo} de entidad {item.EntidadMuestreadora} no existe en base de datos, favor verificar dato");

            return (null, item, fitos, groups, null);
        }

        public async static Task Read<T>(ExcelPackage package, ProdEntry entry,
        ApplicationDbContext context, IHubContext<EntryHub> hubContext,
        List<string> toskip = null) where T : Planilla
        {
            double pgr = 0;
            var planilla = entry.Reportes.ToString();
            var planillas = new List<T> { };

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var msg = string.Empty;

            (Dictionary<string, Dictionary<string, object>> tdata, string error)
                = Analyze<T>(planilla, context, bindingFlags, toskip);

            if (error != null)
            {
                msg =
$@">ERROR: El valor para la columna {error} no fue encontrada en la base de datos.
0 registros procesados. Verificar archivo.";

                entry.OutPut += msg;

                context.Update(entry);
                context.BulkSaveChanges();

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

                        await hubContext
                            .Clients.All
                            .SendAsync("Update", "log", msg);

                        status = "warning";
                        await hubContext
                            .Clients.All
                            .SendAsync("Update", "status", status);

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

                            context.Update(entry);
                            context.BulkSaveChanges();

                            await hubContext
                            .Clients.All
                            .SendAsync("Update", "log", msg);

                            await hubContext
                                .Clients.All
                                .SendAsync("Update", "status", status);

                            return;
                        }
                        if ((string)d.Value["opt"] != null)
                        {
                            Expression e = new Expression((value + (string)d.Value["opt"]).Replace(",", "."));
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

                    var test = (item.Fecha).ToString("MMyyyy");

                    var n = item.Declaracion;

                    item.Dato = entry.Reportes;

                    var dt = (int)item.Dato;

                    var test2 = string.Format("{0}{1}{2}",
                        n, dt, test);

                    item.Id = Convert.ToInt64(test2);

                    planillas.Add(item);

                    pgr += pgrRow;

                    await hubContext
                        .Clients.All
                        .SendAsync("Update", "progress", pgr);
                    await hubContext
                        .Clients.All
                        .SendAsync("Update", "status", status);

                    System.Diagnostics.Debug.WriteLine($"row:{item.Row} sheet:{item.Sheet}");
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

                var find = context.Find<T>(dato.Id);

                if (find == null)
                {
                    if (dato.Dato == Tipo.Semilla && !origenes.Any(o => o.Id == dato.OrigenId))
                    {
                        var orig = context.Find<Origen>(dato.OrigenId);

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

                                await hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg);

                                status = "warning";
                                await hubContext
                                    .Clients.All
                                    .SendAsync("Update", "status", status);

                                continue;
                            }
                        }
                    }

                    if (!centros.Any(o => o.Id == dato.CentreId))
                    {
                        var parent = context.Find<Centre>(dato.CentreId);

                        if (parent == null)
                        {
                            try
                            {
                                var comuna = context.Comuna.FirstOrDefault(c => c.Name.ToLower() == dato.NombreComuna.ToString().ToLower());
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

                                await hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg);

                                status = "warning";
                                await hubContext
                                    .Clients.All
                                    .SendAsync("Update", "status", status);

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
                        await hubContext
                            .Clients.All
                            .SendAsync("Update", "agregada", entry.Agregadas);
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

                        await hubContext
                            .Clients.All
                        .SendAsync("Update", "log", msg);

                        entry.OutPut += msg;

                        var centre = context.Centre.Find(dato.CentreId);
                        if (centre == null)
                        {
                            msg = $">Centro {dato.CentreId} no existe en base de datos.";

                            await hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg);

                            entry.OutPut += msg;
                        }

                        entry.OutPut += msg;
                        entry.Observaciones++;

                        status = "warning";
                        await hubContext
                            .Clients.All
                            .SendAsync("Update", "status", status);
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
                            await hubContext
                                .Clients.All
                                .SendAsync("Update", "agregada", entry.Agregadas);
                            //context.Entries.Update(entry);
                        }
                        catch
                        {
                            msg = $">Unknown error: Declaración de {dato.Dato} N°{dato.Id}, con fecha {dato.Fecha}, en hoja {dato.Sheet}, filas {dato.Rows} no pudieron ser procesadas. Verificar archivo.";

                            await hubContext
                                .Clients.All
                                .SendAsync("Update", "log", msg);

                            entry.OutPut += msg;
                            entry.Observaciones++;

                            status = "warning";
                            await hubContext
                                .Clients.All
                                .SendAsync("Update", "status", status);
                            continue;
                        }
                    }
                }

                pgr += pgrP;

                await hubContext.Clients.All
                    .SendAsync("Update", "progress", pgr);
                await hubContext.Clients.All
                    .SendAsync("Update", "status", status);
            }

            context.Centre.AddRange(centros);
            context.Origen.AddRange(origenes);
            context.Planilla.AddRange(datos);
            context.Planilla.UpdateRange(updates);

            //context.BulkInsert(centros);
            //context.BulkInsert(origenes);
            //context.BulkInsert(datos);
            //context.BulkUpdate(updates);

            status = "success";

            await hubContext.Clients.All
                .SendAsync("Update", "progress", 100);
            await hubContext.Clients.All
                .SendAsync("Update", "status", status);

            msg = $">{entry.Agregadas} añadidos" + (entry.Actualizadas != 0 ? $"y {entry.Actualizadas} registros actualizados " : " ") + "exitosamente.";
            entry.OutPut += msg;
            entry.Success = true;
            await hubContext.Clients.All.SendAsync("Update", "log", msg);

            context.ProdEntry.Update(entry);

            context.SaveChanges();
        }

        public static T AddChanges<T>(T val1, T val2) where T : class, IHasBasicIndexer
        {
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
                        value = Convert.ToInt32(num);
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
                        var culture = CultureInfo.CreateSpecificCulture(num.Contains(',') ? "es-CL" : "en-GB");
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
                    if (val.ToString().ToLower().Contains(tipo.ToString().ToLower()))
                    {
                        Enum.TryParse(tipo, out ProductionType production);
                        value = production;
                        break;
                    }
                }
                if (value == null) value = ProductionType.Desconocido;
            }
            else if (type == typeof(Item?) || type == typeof(Item))
            {
                foreach (var tipo in Enum.GetNames(typeof(Item)))
                {
                    if (val.ToString().ToLower()[0] == tipo.ToString().ToLower()[0])
                    {
                        Enum.TryParse(tipo, out Item production);
                        value = production;
                        break;
                    }
                }
            }
            else if (type == typeof(Tipo))
            {
                value = val.ToString().ToLower().Contains("m") ? Tipo.MateriaPrima : Tipo.Producción;
            }
            else if (type == typeof(Enum))
            {
                value = Enum.Parse(DataTableExtensions.GetEnumType("BiblioMit.Models." + typeof(T).ToString()), val.ToString());
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