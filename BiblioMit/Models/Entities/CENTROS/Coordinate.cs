using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class Coordinate
    {
        //Ids
        public int Id { get; set; }

        [Display(Name = "Cuenca Id")]
        public int? CuencaId { get; set; }

        [Display(Name = "PSMB Id")]
        public int? PSMBId { get; set; }

        [Display(Name = "Concession Id")]
        public int? CentreId { get; set; }

        [Display(Name = "Polygon")]
        public int? PolygonId { get; set; }

        //Parents
        public virtual Cuenca Cuenca { get; set; }
        public virtual PSMB PSMB { get; set; }

        [Display(Name = "Farming Centre")]
        public virtual Centre Centre { get; set; }

        [Display(Name = "Polygon")]
        public virtual Polygon Polygon { get; set; }

        //ATT
        [Display(Name = "Latitude")]
        [Range(-50, -25)]
        public double Latitude { get; set; }

        [Display(Name = "Longitude")]
        [Range(-80, -60)]
        public double Longitude { get; set; }

        [Display(Name = "Vertex")]
        [Range(1,50)]
        public int Vertex { get; set; }
        //CHILD
    }
}
