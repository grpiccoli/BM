using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class EnsayoFito : Indexed
    {
        public override bool Equals(object obj)
        {
            return obj is EnsayoFito q && q.Id == Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        [Display(Name = "N° Formulario")]
        public int Id { get; set; }
        public string Estacion { get; set; }
        public string EntidadMuestreadora { get; set; }
        public DateTime FechaMuestreo { get; set; }
        public DateTime InicioAnalisis { get; set; }
        //public int Formulario { get; set; }
        public string Email { get; set; }
        //public int Codigo { get; set; }
        public DateTime Recepcion { get; set; }
        public DateTime FinAnalisis { get; set; }
        public string Laboratorio { get; set; }
        public string Telefono { get; set; }
        public int? CentreId { get; set; }
        public virtual Centre Centre { get; set; }
        public int PSMBId { get; set; }
        public virtual PSMB PSMB { get; set; }
        public int Muestras { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Analista { get; set; }
        public double? Temperatura { get; set; }
        public double? Oxigeno { get; set; }
        public double? Ph { get; set; }
        public double? Salinidad { get; set; }
        public virtual ICollection<Phytoplankton> Fitoplanctons { get; set; }
    }
}
