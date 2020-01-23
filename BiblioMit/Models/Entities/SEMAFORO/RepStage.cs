using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class RepStage
    {
        public int Id { get; set; }
        public int SpawningId { get; set; }
        public virtual Spawning Spawning { get; set; }
        public Stage Stage { get; set; }
        [Range(0,100)]
        [Display(Description = "%")]
        public int Proportion { get; set; }
    }
    public enum Stage
    {
        EnMadurez = 0,
        Maduro = 1,
        Desovado = 2,
        EnDesove = 3
    }
}
