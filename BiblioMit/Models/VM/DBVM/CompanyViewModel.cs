using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class CompanyViewModel
    {
        [Required]
        [Display(Name = "RUT")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayFormat(DataFormatString = "{0:###'.'###'.'###}")]
        [RegularExpression(@"^[0-9]{4,9}[0-9Kk]$", ErrorMessage = "RUT must contain only numbers including Verification Digit")]
        public string RUT { get; set; }

        [Required]
        [Display(Name = "Razón Social")]
        public string BsnssName { get; set; }

        [Display(Name = "Sigla o Acrónimo")]
        public string Acronym { get; set; }
    }
}