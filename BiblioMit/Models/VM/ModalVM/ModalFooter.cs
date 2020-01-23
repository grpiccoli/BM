using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class ModalFooter
    {
        [Display(Name="Save")]
        public string SubmitButtonText { get; set; } = "Save";
        [Display(Name="Cancel")]
        public string CancelButtonText { get; set; } = "Cancel";
        public string SubmitButtonId { get; set; } = "btn-submit";
        public string CancelButtonId { get; set; } = "btn-cancel";
        public bool OnlyCancelButton { get; set; }
    }
}