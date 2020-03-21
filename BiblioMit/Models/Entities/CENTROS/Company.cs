using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Company
    {
        [Display(Name = "RUT")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        [DisplayFormat(DataFormatString = "{0,9:N0}" )]
        public int Id { get; set; }

        [Display(Name = "Business Name")]
        public string BsnssName { get; set; }

        [Display(Name = "Acronym")]
        public string Acronym { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Farming Centres")]
        public virtual ICollection<Centre> Centres { get; } = new List<Centre>();
    }
}