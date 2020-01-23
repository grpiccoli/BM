using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Models.ViewModels
{
    public class SoftTissuesData
    {
        public static List<Tissue> Tissues { get; set; } = new List<Tissue>
                                                            {
                                                                Tissue.DigestiveGland,
                                                                Tissue.Foot,
                                                                Tissue.Gill,
                                                                Tissue.Gonad,
                                                                Tissue.Intestine,
                                                                Tissue.Mantle,
                                                                Tissue.Nephridium,
                                                                Tissue.PlicateMembrane,
                                                                Tissue.StyleSac,
                                                                Tissue.Tubules
                                                            };
    }
}
