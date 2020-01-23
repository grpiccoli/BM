using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models.ViewModels
{
    public class TissueView
    {
        [Display(Name = "Count")]
        [Range(1, 1000, ErrorMessage = "This value must be between {0} and {1}")]
        public int? Count { get; set; }
        public Degree? Degree { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public bool Check { get; set; }
    }
}
