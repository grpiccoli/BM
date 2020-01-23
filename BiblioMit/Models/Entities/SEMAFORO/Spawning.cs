using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Spawning
    {
        public int Id { get; set; }
        public int CentreId { get; set; }
        public virtual Centre Centre { get; set; }
        public DateTime Date { get; set; }
        [Range(0,100)]
        [Display(Description = "%")]
        public int MaleProportion { get; set; }
        [Range(0, 100)]
        [Display(Description = "%")]
        public int FemaleProportion { get; set; }
        [Display(Description = "%")]
        public double MaleIG { get; set; }
        [Display(Description = "%")]
        public double FemaleIG { get; set; }
        public virtual ICollection<RepStage> Stage { get; set; }
    }
}
