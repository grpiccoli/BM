using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public enum SoftType
    {
        [Display(Name = "Haemocytic Infiltration")]
        HaemocyticInfiltration,

        [Display(Name = "Rickettsia Like Organisms")]
        RLO,

        [Display(Name = "Microsporidia")]
        Microsporidia,

        [Display(Name = "Neoplasia")]
        DN,

        [Display(Name = "Abscess")]
        Abscess,

        [Display(Name = "Lipofuscin")]
        Lipofuscin,

        [Display(Name ="Gonadal atresia")]
        Atresia,

        [Display(Name = "Ciliates")]
        Ciliates,

        [Display(Name = "Trematode")]
        Trematode,

        [Display(Name = "Copepod")]
        Copepod,

        [Display(Name = "Ancistrum")]
        Ancistrum
    }

    public enum Tissue
    {
        [Display(Name = "Nephridium")]
        Nephridium,
        [Display(Name = "Intestine")]
        Intestine,
        [Display(Name = "Style sac")]
        StyleSac,
        [Display(Name = "Gonad")]
        Gonad,
        [Display(Name = "Tubules")]
        Tubules,
        [Display(Name = "Gill")]
        Gill,
        [Display(Name = "Digestive gland")]
        DigestiveGland,
        [Display(Name = "Mantle")]
        Mantle,
        [Display(Name = "Foot")]
        Foot,
        [Display(Name = "Plicate membrane")]
        PlicateMembrane
    }

    public class Soft
    {
        public int Id { get; set; }

        [Required]
        public int IndividualId { get; set; }
        //Parent
        public virtual Individual Individual { get; set; }
        //ATT
        [Required]
        [Display(Name = "Symbiont or Condition")]
        public SoftType SoftType { get; set; }

        [Display(Name = "Tissue")]
        public Tissue Tissue { get; set; }
        //Child
        public virtual ICollection<Photo> Photos { get; } = new List<Photo>();

        [Display(Name = "Count")]
        [Range(1,1000, ErrorMessage = "This value must be between {0} and {1}")]
        public int? Count { get; set; }

        [Display(Name = "Degree")]
        public Degree? Degree { get; set; }
    }

    public enum Degree
    {
        [Display(Name = "0 Abcense", Description = "No presence observed")]
        d0,
        [Display(Name = "1 Incipient", Description = "Sparse presence observed")]
        d1,
        [Display(Name = "2 Frequent", Description ="Visible in several locations in the histological plate")]
        d2,
        [Display(Name = "3 Disseminated", Description = "Present in multiple tissues")]
        d3
    }
}
