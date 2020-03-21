using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using BiblioMit.Services;
//using Amazon.S3;
//using Amazon.S3.Model;
using BiblioMit.Models.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class PhotosController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly string _accessKey;
        //private readonly string _secretKey;
        //private readonly string _bucket;

        public PhotosController(ApplicationDbContext context)
        {
            _context = context;
            //_accessKey = configuration.GetValue<string>("S3_Id");
            //_secretKey = configuration.GetValue<string>("S3_KEY");
            //_accessKey = configuration["Authentication:AWS:S3:Id"];
            //_secretKey = configuration["Authentication:AWS:S3:Key"];
            //_bucket = "mytilidb";
        }

        [AllowAnonymous]
        public IActionResult Gallery()
        {
            var builder = new UriBuilder
            {
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                Path = "colecci-n-virtual/index.html"
            };
            return Redirect(builder.Uri.ToString());
        }

        public IActionResult GetImg(string f, string d)
        {
            var name = Regex.Replace(f, ".*/", "");

            var full = Path.Combine(Directory.GetCurrentDirectory(),
                                    d, name);

            return PhysicalFile(full, "image/jpg");
        }

        // GET: Photos
        public async Task <IActionResult> Index()
        {
            var photos = await _context.Photo
                            .Include(p => p.Individual)
                            .OrderBy(p => p.Individual.SamplingId)
                            .AsNoTracking()
                            .ToListAsync().ConfigureAwait(false);

            //var client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.SAEast1);

            var photosView = new List<UploadPhotoViewModel>();

            for (int i = 0; i < photos.Count; i++)
            {
                var url = new Uri (Url.Action("GetImg", "Photos",
                    new { f = photos[i].Key, d = "DB", th = false },
                    HttpContext.Request.Scheme));
                //var url = client.GetPreSignedURL(new GetPreSignedUrlRequest
                //{
                //    BucketName = _bucket,
                //    Key = photos[i].Key,
                //    Expires = DateTime.UtcNow.AddMinutes(30)
                //});

                //var thumb = client.GetPreSignedURL(new GetPreSignedUrlRequest
                //{
                //    BucketName = _bucket+"resized",
                //    Key = "resized-"+photos[i].Key,
                //    Expires = DateTime.UtcNow.AddMinutes(30)
                //});

                var thumb = Url.Action("GetImg", "Photos",
                new { f = photos[i].Key, d = Path.Combine("DB", "Thumbs"), th = true },
                HttpContext.Request.Scheme);

                var feature = HttpContext.Features.Get<IRequestCultureFeature>();
                var lang = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToUpperInvariant();

                string comment;

                //try
                //{
                //    comment = (lang == "es") ? photos[i].Comment : await nodeServices.InvokeAsync<string>("./wwwroot/js/translate.js", photos[i].Comment, lang);
                //}
                //catch
                //{
                comment = photos[i].Comment;
                //}

                photosView.Add(new UploadPhotoViewModel
                {
                    IndividualId = photos[i].IndividualId,
                    Comment = comment,
                    SampleId = photos[i].Individual.SamplingId,
                    Url = url,
                    Thumb = thumb,
                    Magnification = photos[i].Magnification,
                    PhId = photos[i].Id
                });
            }

            var grouped = photosView.GroupBy(t => t.SampleId);

            return View(grouped);
        }

        // GET: Photos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photo
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // GET: Photos/Create
        public IActionResult Create()
        {
            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToUpperInvariant();

            ViewData["IndividualId"] = _context.Individual.GroupBy(i => i.SamplingId);

            var mags = from Magnification e in Enum.GetValues(typeof(Magnification))
                        select new { Id = e, Name = e.GetDisplayName(lang) };
            ViewData["Magnification"] = new SelectList(mags, "Id", "Name");

            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IndividualId,File,Comment,Magnification")] UploadPhotoViewModel uploadPhoto)
        {
            if (uploadPhoto == null) return NotFound();
            //var client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.SAEast1);

            //var contentDisposition = ContentDispositionHeaderValue.Parse(uploadPhoto.File.ContentDisposition);

            //var filename = contentDisposition.FileName.Trim('"');

            //var stream = uploadPhoto.File.OpenReadStream();

            using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(),
                        "DB", uploadPhoto.File.FileName), FileMode.Create))
            {
                await uploadPhoto.File.CopyToAsync(stream).ConfigureAwait(false);
            }

            //var request = new PutObjectRequest
            //{
            //    BucketName = _bucket,
            //    Key = uploadPhoto.IndividualId.ToString()+"/"+filename,
            //    InputStream = stream,
            //    CannedACL = S3CannedACL.Private
            //};

            //var response = await client.PutObjectAsync(request);

            var photo = new Photo
            {
                IndividualId = uploadPhoto.IndividualId,
                Key = uploadPhoto.File.FileName,
                Comment = uploadPhoto.Comment,
                Magnification = uploadPhoto.Magnification
            };

            if (ModelState.IsValid)
            {
                _context.Add(photo);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index");
            }
            return View(photo);
        }

        //public IActionResult Tmp()
        //{
        //    //var client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.SAEast1);

        //    var photos = _context.Photo.Include(p => p.Individual).Where(p => p.Individual.SamplingId > 953).Skip(6);

        //    foreach (var photo in photos)
        //    {
        //        var file = Regex.Replace(photo.Key, @"^.*/", "");

        //        //var request = new PutObjectRequest
        //        //{
        //        //    BucketName = _bucket,
        //        //    Key = photo.Key,
        //        //    FilePath = "NewDB/" + file,
        //        //    CannedACL = S3CannedACL.Private
        //        //};

        //        //var response = await client.PutObjectAsync(request);
        //    }

        //    return RedirectToAction("Index");
        //}

        // GET: Photos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photo.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (photo == null)
            {
                return NotFound();
            }
            ViewData["IndividualId"] = new SelectList(_context.Set<Individual>(), "Id", "Id", photo.IndividualId);
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IndividualId,Comment,Magnification,Key")] Photo photo)
        {
            if (photo == null || id != photo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photo);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(photo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["IndividualId"] = new SelectList(_context.Set<Individual>(), "Id", "Id", photo.IndividualId);
            return View(photo);
        }

        // GET: Photos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photo
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photo.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool PhotoExists(int id)
        {
            return _context.Photo.Any(e => e.Id == id);
        }
    }
}
