using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models.SamplingViewModels
{
    public class SamplingViewModel
    {
        [Display(Name = "Sampling Id")]
        public int Id { get; set; }

        [Display(Name = "Culturing Centre")]
        public int CentreId { get; set; }

        public string Season { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime Date { get; set; }

        [Display(Name = "Salinity (ppt)")]          //salinity in ppt (parts per thousand)
        public int Salinity { get; set; }

        [Display(Name = "Temp (°C)")]               //temperature °C
        public decimal Temp { get; set; }

        [Display(Name = "Dissolved Oxigen (mg/L)")] //oxigen level in mg/L
        public decimal O2 { get; set; }

        public ICollection<IndividualViewModel> Individuals { get; set; }
    }
}
