using CustomDataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Sampling
    {
        //Ids
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        [Display(Name = "Sampling Id")]
        public int Id { get; set; }

        [Display(Name = "Farming Centre")]
        public int CentreId { get; set; }
        //public int SubmitterId { get; set; }
        //Parent
        //public virtual Submitter Submitter { get; set; }
        [Display(Name = "Farming Centre")]
        public virtual Centre Centre { get; set; }
        //ATT
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        [PastDate(ErrorMessage = "Date must be in the past")]
        public DateTime Date { get; set; }

        [Display(Name = "Salinity (ppt)")]          //salinity in ppt (parts per thousand)
        public int? Salinity { get; set; }

        [Display(Name = "Temp (°C)")]               //temperature °C
        public double? Temp { get; set; }

        [Display(Name = "Dissolved Oxigen (mg/L)")] //oxigen level in mg/L
        public double? O2 { get; set; }
        //CHILDS
        [Display(Name = "Subjects")]
        public virtual ICollection<Individual> Individuals { get; } = new List<Individual>();
    }
}
