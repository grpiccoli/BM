using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Entry : Indexed
    {
        public int Id { get; set; }

        [Display(Name = "Usuario")]
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

        [Display(Name = "Fecha de Subida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string OutPut { get; set; }

        public string IP { get; set; }

        public int Actualizadas { get; set; }

        public int Agregadas { get; set; }

        public int Observaciones { get; set; }

        public bool Success { get; set; }

        [Display(Name ="Rango")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Min { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Max { get; set; }

        //public Informe Informe { get; set; }
    }
    //public enum Informe
    //{
    //    [Display(Name = "Productivo")]
    //    Productivo = 1,
    //    [Display(Name = "Ambiental")]
    //    Ambiental = 2,
    //    [Display(Name = "Semáforo")]
    //    Semaforo = 3
    //}
}
