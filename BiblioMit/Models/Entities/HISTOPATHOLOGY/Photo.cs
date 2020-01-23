using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Photo
    {
        public int Id { get; set; }
        //Parent Id
        public int IndividualId { get; set; }

        //Related Parent Entities
        public virtual Individual Individual { get; set; }
        //ATT
        public string Key { get; set; }

        public string Comment { get; set; }

        public Magnification Magnification { get; set; }
    }

    public enum Magnification
    {
        [Display(Name = "5X")]
        mag5x,
        [Display(Name = "10X")]
        mag10x,
        [Display(Name = "20X")]
        mag20x,
        [Display(Name = "40X")]
        mag40x,
        [Display(Name = "100X")]
        mag100x
    }
}
