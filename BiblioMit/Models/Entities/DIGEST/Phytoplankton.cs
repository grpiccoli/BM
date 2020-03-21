using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Phytoplankton
    {
        public override bool Equals(object obj)
        {
            return obj is Phytoplankton q && q.EnsayoFitoId == EnsayoFitoId && q.Species == Species;
        }
        public override int GetHashCode()
        {
            return EnsayoFitoId.GetHashCode()*Species.GetHashCode(System.StringComparison.InvariantCultureIgnoreCase);
        }
        public int Id { get; set; }
        public int EnsayoFitoId { get; set; }
        public virtual EnsayoFito EnsayoFito { get; set; }
        public string Species { get; set; }
        [Display(Name = "Escala de Abundancia Relativa", ShortName = "E.A.R.")]
        public EAR? EAR { get; set; }
        [Display(Name = "Concentración Tóxica", Description = "cel/mL", ShortName = "C.")]
        public double C { get; set; }
        public int GroupsId { get; set; }
        public virtual Groups Groups { get; set; }
    }

    public enum EAR
    {
        Ausente = 0,
        Raro = 1,
        Escaso = 2,
        Regular = 3,
        Abundante = 4,
        Muy = 5,
        Extremadamente = 6,
        Hiper = 7
    }
}
