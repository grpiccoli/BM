namespace BiblioMit.Models.ViewModels
{
    public class BoletinVM
    {
        public int TipoPeriodoId { get; set; }

        public int Year { get; set; }

        public int PeriodoId { get; set; }
    }

    public enum Periodo
    {
        Mensual = 1,
        Trimestral = 3,
        Anual = 12
    }
}
