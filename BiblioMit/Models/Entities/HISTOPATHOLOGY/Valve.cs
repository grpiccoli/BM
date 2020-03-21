using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public enum ValveType
    {
        Epibiont, Encrusting
    }
    public class Valve
    {
        //Ids
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int IndividualId { get; set; }

        //Parent
        public virtual Individual Individual { get; set; }
        
        //ATT
        public ValveType ValveType { get; set; }

        public string Species { get; set; }

        public string Comment { get; set; }

        //Child
        public virtual ICollection<Photo> Photos { get; } = new List<Photo>();
    }
}
