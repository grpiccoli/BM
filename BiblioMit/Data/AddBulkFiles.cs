using BiblioMit.Models;
using BiblioMit.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioMit.Data
{
    public static class AddBulkFiles
    {
        public async static Task Add(ApplicationDbContext context, string path)
        {
            var sep = "/";

            var boletin = string.Join(sep, path, "..", "UNSYNC/DB");

            var pwd = string.Join(sep, "..", "UNSYNC/DB", "plankton");
            //var pwd = string.Join(sep, boletin, "TEST");
            if (!Directory.Exists(pwd)) return;
            var error = string.Join(sep, boletin, "ERROR");
            var ok = string.Join(sep, boletin, "OK");

            var entries = new HashSet<EnsayoFito>();
            var centres = new HashSet<Centre>();
            var fitos = new HashSet<Phytoplankton>();
            var groups = new HashSet<Groups>();
            using(var log = new StreamWriter(pwd + "/BulkPlankton.log"))
            {
                foreach (var d in Directory.GetDirectories(pwd))
                {
                    foreach (var f in Directory.GetFiles(d))
                    {
                        var toskip = new List<string> { "Centre", "Fitoplanctons", "PSMB" };

                        var stream = new StreamReader(f, Encoding.GetEncoding("Windows-1252"), true);
                        var html = stream.ReadToEnd();
                        stream.Close();
                        var temp = new TableToExcel();
                        var xlsx = new ExcelPackage();
                        temp.Process(html, out xlsx);
                        (Centre centre, EnsayoFito item, List<Phytoplankton> fito, List<Groups> group, string err) = 
                            await Import.AmbAsync(xlsx, context, toskip)
                            .ConfigureAwait(false);
                        if (item != null && !fito.Any())
                        {
                            err = "fito empty";
                        }
                        var dest = err == null || err == "Archivo ya ingresado" ? ok : error;
                        var dir = new DirectoryInfo(d);
                        var targetPath = string.Join(sep, dest, dir.Name);
                        if (!Directory.Exists(targetPath))
                        {
                            Directory.CreateDirectory(targetPath);
                        }
                        var file = string.Join(sep, path, f);
                        var newf = string.Join(sep, targetPath, Path.GetFileName(f));
                        try
                        {
                            File.Move(file, newf);
                        }
                        catch (Exception e)
                        {
                            log.WriteLine(e);
                        }

                        if (!string.IsNullOrEmpty(err))
                        {
                            log.WriteLine(err);
                            continue;
                        }

                        if (centre != null) centres.UnionWith(new List<Centre> { centre });
                        if (item != null) entries.UnionWith(new List<EnsayoFito> { item });
                        if (fito != null) fitos.UnionWith(fito);
                        if (group != null) groups.UnionWith(group);
                    }
                }
                log.Close();
            }
            foreach(var c in centres)
            {
                var centre = await context.FindAsync<Centre>(c.Id)
                    .ConfigureAwait(false);
                if(centre == null)
                {
                    await context.Centre.AddAsync(c).ConfigureAwait(false);
                }
                else
                {
                    context.Centre.Update(c);
                }
            }
            //context.BulkInsert(centres);
            foreach (var e in entries)
            {
                var entrie = await context.FindAsync<EnsayoFito>(e.Id)
                    .ConfigureAwait(false);
                if (entrie == null)
                {
                    await context.EnsayoFito.AddAsync(e)
                        .ConfigureAwait(false);
                }
            }

            //context.BulkInsert(entries);
            foreach (var f in fitos)
            {
                var fito = context.Phytoplankton.SingleOrDefault(p => p.EnsayoFitoId == f.EnsayoFitoId && p.Species == f.Species);
                if (fito == null)
                {
                    await context.Phytoplankton.AddAsync(f)
                        .ConfigureAwait(false);
                }
            }
            await context.SaveChangesAsync()
                .ConfigureAwait(false);
            //context.BulkInsert(fitos);
            await context.BulkInsertAsync(groups)
                .ConfigureAwait(false);
        }
    }
}
