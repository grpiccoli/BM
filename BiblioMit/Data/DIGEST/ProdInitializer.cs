using BiblioMit.Models;
using BiblioMit.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Data
{
    public static class ProdInitializer
    {
        public async static Task Initialize(ApplicationDbContext context, string adminId)
        {
            #region Entries Produccion
            if (!context.ProdEntry.Any())
            {
                var Entradas = new List<ProdEntry>()
                {
//Semillas
new ProdEntry { Id = 116, AppUserId = adminId, Date = DateTime.Today, OutPut = ">513 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 513, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2016-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2016-12-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2016 REPORTE ABASTECIMIENTO SEMILLAS 2016.xlsx", Reportes = Tipo.Semilla },
new ProdEntry { Id = 117, AppUserId = adminId, Date = DateTime.Today, OutPut = ">752 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 752, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2017-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2017-12-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2017 ABASTECIMIENTO DE SEMILLAS 2017 (DATOS POR MES).xlsx", Reportes = Tipo.Semilla },
new ProdEntry { Id = 118, AppUserId = adminId, Date = DateTime.Today, OutPut = ">239 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 239, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-03-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_1 REPORTE ABASTECIMIENTO SEMILLAS  1ER TRIMESTRE 2018 (DATOS POR MES).xlsx", Reportes = Tipo.Semilla },
new ProdEntry { Id = 119, AppUserId = adminId, Date = DateTime.Today, OutPut = ">393 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 393, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-06-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_2 REPORTE ABASTECIMIENTO SEMILLAS 2018 (AL 2DO TRIMESTRE).xlsx", Reportes = Tipo.Semilla },
new ProdEntry { Id = 120, AppUserId = adminId, Date = DateTime.Today, OutPut = ">138 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 138, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-09-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_3 REPORTE ABASTECIMIENTO SEMILLAS 2018 (AL 3ER TRIMESTRE).xlsx", Reportes = Tipo.Semilla },
//Cosechas
new ProdEntry { Id = 223, AppUserId = adminId, Date = DateTime.Today, OutPut = ">2845 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 2845, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2016-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2016-12-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2016 REPORTE COSECHAS 2016.xlsx", Reportes = Tipo.Cosecha },
new ProdEntry { Id = 224, AppUserId = adminId, Date = DateTime.Today, OutPut = ">11848 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 11848, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2017-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2017-12-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2017 COSECHAS 2017 (DATOS POR MES).xlsx", Reportes = Tipo.Cosecha },
new ProdEntry { Id = 225, AppUserId = adminId, Date = DateTime.Today, OutPut = ">4061 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 4061, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-03-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_1 REPORTE COSECHAS 1ER TRIMESTRE 2018 (DATOS POR MES).xlsx", Reportes = Tipo.Cosecha },
new ProdEntry { Id = 226, AppUserId = adminId, Date = DateTime.Today, OutPut = ">3892 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 3892, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-06-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_2 REPORTE COSECHAS 2018 (AL 2DO TRIMESTRE ).xlsx", Reportes = Tipo.Cosecha },
new ProdEntry { Id = 227, AppUserId = adminId, Date = DateTime.Today, OutPut = ">2915 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 2915, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-09-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_3 REPORTE COSECHAS 2018 (AL 3er TRIMESTRE ).xlsx", Reportes = Tipo.Cosecha },
//Abastecimientos
new ProdEntry { Id = 239, AppUserId = adminId, Date = DateTime.Today, OutPut = ">4565 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 4565, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2016-01-02","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2016-12-31","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2016 ABASTECIMIENTO PLANTAS 2016.xlsx", Reportes = Tipo.Abastecimiento },
new ProdEntry { Id = 240, AppUserId = adminId, Date = DateTime.Today, OutPut = ">4397 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 4397, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2017-01-02","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2017-12-30","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2017 ABASTECIMIENTO PLANTAS 2017 (DATOS POR MES).xlsx", Reportes = Tipo.Abastecimiento },
new ProdEntry { Id = 241, AppUserId = adminId, Date = DateTime.Today, OutPut = ">1294 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 1294, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-02","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-03-29","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_1 ABASTECIMIENTO PLANTAS 1ER TRIMESTE 2018 (DATOS POR MES).xlsx", Reportes = Tipo.Abastecimiento },
new ProdEntry { Id = 242, AppUserId = adminId, Date = DateTime.Today, OutPut = ">1281 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 1281, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-02","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-06-30","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_2 ABASTECIMIENTO PLANTAS 2018 (AL 2DO TRIMESTE).xlsx", Reportes = Tipo.Abastecimiento },
new ProdEntry { Id = 243, AppUserId = adminId, Date = DateTime.Today, OutPut = ">1509 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 1509, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-02","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-09-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_3 ABASTECIMIENTO PLANTAS 2018 (AL 3ER TRIMESTE).xlsx", Reportes = Tipo.Abastecimiento },
//Produccion
new ProdEntry { Id = 437, AppUserId = adminId, Date = DateTime.Today, OutPut = ">5740 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 5740, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2016-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2016-12-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2016 MATERIA PRIMA Y PRODUCCION  2016.xlsx", Reportes = Tipo.Producción },
new ProdEntry { Id = 438, AppUserId = adminId, Date = DateTime.Today, OutPut = ">5865 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 5865, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2017-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2017-12-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2017 MATERIAS PRIMAS Y PRODUCCION 2017 (DATOS POR MES).xlsx", Reportes = Tipo.Producción },
new ProdEntry { Id = 439, AppUserId = adminId, Date = DateTime.Today, OutPut = ">1661 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 1661, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-03-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_1 MATERIA PRIMA Y PRODUCCION 1ER TRIMESTRE 2018 (DATOS POR MES).xlsx", Reportes = Tipo.Producción },
new ProdEntry { Id = 440, AppUserId = adminId, Date = DateTime.Today, OutPut = ">1636 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 1636, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-06-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_2 MATERIA PRIMA Y PRODUCCION  2018 (AL 2do TRIMESTRE).xlsx", Reportes = Tipo.Producción },
new ProdEntry { Id = 481, AppUserId = adminId, Date = DateTime.Today, OutPut = ">2327 añadidos exitosamente.", IP = "1.1.1.1", Actualizadas = 0, Agregadas = 2327, Observaciones = 0, Success = true, Min = DateTime.ParseExact("2018-01-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), Max = DateTime.ParseExact("2018-09-01","yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture), FileName = "2018_3 MATERIA PRIMA Y PRODUCCION  2018 (AL 3ER  TRIMESTRE).xlsx", Reportes = Tipo.Producción },
                };
                await context.BulkInsertAsync(Entradas).ConfigureAwait(false);
            }
            #endregion

            #region Read Excel
            //var sep = "/";

            //var rs = (Tipo[])Enum.GetValues(typeof(Tipo));
            //for (int i = 5; i < 5; i++)
            //{
            //    var r = rs[i];

            //    var pwd = string.Join(sep, path, "..", "Boletin", "DBproduccion", r.ToString());

            //    foreach (var f in Directory.GetFiles(pwd).OrderBy(f => f))
            //    {
            //        if (context.ProdEntry.Any(e => e.FileName == Path.GetFileName(f))) continue;
            //        var p = new ProdEntry
            //        {
            //            AppUserId = adminId,
            //            IP = "1.1.1.1",
            //            Date = DateTime.Now,
            //            Success = false,
            //            Reportes = r,
            //            FileName = Path.GetFileName(f)
            //        };

            //        context.ProdEntry.Add(p);
            //        context.SaveChanges();

            //        var toskip = new List<string> { "Row", "Sheet", "Centre", "Rows", "Id", "Origin" };

            //        if (r != Tipo.Producción)
            //        {
            //            toskip.Add("TipoProduccion");
            //            toskip.Add("TipoItemProduccion");
            //            toskip.Add("Dato");
            //        }

            //        if (r != Tipo.Semilla)
            //        {
            //            toskip.Add("OrigenId");
            //            toskip.Add("Origen");
            //        }

            //        Stream stream = new StreamReader(f).BaseStream;
            //        ExcelPackage package = new ExcelPackage(stream);
            //        stream.Close();
            //        await Import.Read<Planilla>(package, p, context, hub, toskip);
            //    }
            //}
            #endregion 16 16
        }
    }
}
