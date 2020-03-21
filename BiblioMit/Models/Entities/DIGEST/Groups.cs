using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Groups
    {
        public override bool Equals(object obj)
        {
            return obj is Groups q && q.Id == Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Phytoplankton> Phytoplanktons { get; } = new List<Phytoplankton>();
    }
}
