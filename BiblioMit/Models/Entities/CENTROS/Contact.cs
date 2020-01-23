using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    #region snippet1
    public class Contact
    {
        public int ContactId { get; set; }

        [Display(Name = "Submitted by")]
        // user Id from AspNetUser table
        public string OwnerId { get; set; }

        [Display(Name = "Centre")]
        public int CentreId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Surname")]
        public string Last { get; set; }

        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:+## # #### ####}")]
        public long Phone { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Position")]
        public Position Position { get; set; }

        [Display(Name = "Working Hours")]
        [DisplayFormat(DataFormatString = "{0:H:mm}")]
        public DateTime OpenHr { get; set; }

        [Display(Name = "Horario")]
        [DisplayFormat(DataFormatString = "{0:H:mm}")]
        public DateTime CloseHr { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Estado")]
        public ContactStatus Status { get; set; }

        public virtual Centre Centre { get; set; }
    }

    public enum Position
    {
        Investigacion,
        Secretaria,
        Direccion
    }

    public enum ContactStatus
    {
        Ingresado,
        Aprobado,
        Rechazado
    }
    #endregion
}