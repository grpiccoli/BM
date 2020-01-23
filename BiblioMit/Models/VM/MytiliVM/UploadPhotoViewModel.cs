using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models.ViewModels
{
    public class UploadPhotoViewModel
    {
        public int PhId { get; set; }

        [Display(Name ="Subject")]
        public int IndividualId { get; set; }

        [Display(Name = "Image File")]
        public IFormFile File { get; set; }

        [Display(Name = "Comments")]
        public string Comment { get; set; }

        [Display(Name = "Sample")]
        public int SampleId { get; set; }

        [Display(Name = "Image Url")]
        public string Url { get; set; }

        [Display(Name = "Image Thumbnail")]
        public string Thumb { get; set; }

        [Display(Name = "Microscope Magnification Factor")]
        public Magnification Magnification { get; set; }
    }
}
