using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public enum Sex
    {
        [Display(Name = "Male")]
        Male,
        [Display(Name = "Female")]
        Female,
        [Display(Name = "Hermaphrodite")]
        Hermaphrodite,
        [Display(Name = "Undetermined")]
        Undetermined
    }

    public enum Maturity
    {
        [Display(Name = "Developing")]
        Developing,

        [Display(Name = "Mature")]
        Mature,

        [Display(Name = "Spawning")]
        Spawning,

        [Display(Name = "Post-spawning")]
        PostSpawning,

        [Display(Name = "Undetermined")]
        Undetermined
    }

    public class Individual
    {
        //Ids
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        [Display(Name = "No")]
        public int Id { get; set; }

        [Display(Name = "Sample Id")]
        public int SamplingId { get; set; }

        //Parent
        [Display(Name = "Sample")]
        public virtual Sampling Sampling { get; set; }

        //ATT
        [Display(Name = "Sex")]
        public Sex Sex { get; set; }

        [Display(Name = "Maturity")]
        public Maturity Maturity { get; set; }

        [Display(Name = "Length (mm)")]
        public int Length { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Number")]
        public int Number { get; set; }

        [Display(Name = "Tag")]
        public string Tag { get; set; }

        [Display(Name = "Depth")]
        public int? Depth { get; set; }

        [Display(Name = "Adipogranular cells")]
        public ADG? ADG { get; set; }

        //CHILD
        [Display(Name = "Valves")]
        public ICollection<Valve> Valves { get; set; }

        [Display(Name = "Soft Tissue")]
        public ICollection<Soft> Softs { get; set; }
    }
    public enum ADG
    {
        [Display(Name = "1 Present", Description = "Sparse adipogranular cells observed")]
        Intensity1,

        [Display(Name = "2 Frequent", Description = "Dispersed throughout mantle tissues")]
        Intensity2,

        [Display(Name = "3 Abundant", Description = "Adipogranular cells (ADG) constitute the vast mayority of the vesiculosus tissue total volume")]
        Intensity3
    }
}
