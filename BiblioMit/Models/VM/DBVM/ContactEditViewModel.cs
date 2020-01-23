using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class ContactEditViewModel
    {
        public int ContactId { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Apellido")]
        public string Last { get; set; }

        [Display(Name = "Cargo")]
        public Position Position { get; set; }

        [Display(Name = "Descripción de Cargo")]
        public string Description { get; set; }

        [Display(Name = "Teléfono")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:+## # #### ####}")]
        public long Phone { get; set; }

        [Display(Name = "Horario")]
        [DisplayFormat(DataFormatString = "{0:H:mm}")]
        public DateTime OpenHr { get; set; }

        [Display(Name = "Horario")]
        [DisplayFormat(DataFormatString = "{0:H:mm}")]
        public DateTime CloseHr { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
