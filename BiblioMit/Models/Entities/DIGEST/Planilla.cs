using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioMit.Models
{
    public class Planilla : Indexed
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [InsertOff]
        public long Id { get; set; }

        public int Declaracion { get; set; }

        public int CentreId { get; set; }
        public virtual Centre Centre { get; set; }

        public Tipo Dato { get; set; }

        public DateTime Fecha { get; set; }

        public double Peso { get; set; }

        public ProductionType? TipoProduccion { get; set; }

        public Item? TipoItemProduccion { get; set; }

        [NotMapped]
        public string NombreComuna { get; set; }

        [NotMapped]
        public string CompanyName { get; set; }

        [NotMapped]
        public int Year { get; set; }

        [NotMapped]
        public int Row { get; set; }

        [NotMapped]
        public string Rows { get; set; }

        [NotMapped]
        public string Sheet { get; set; }

        [NotMapped]
        public int Month { get; set; }

        [NotMapped]
        public string Origen { get; set; }

        public int? OrigenId { get; set; }
        public virtual Origen Origin { get; set; }
    }

    public enum ProductionType
    {
        [Display(Name = "No Informado", GroupName = "Reportes Plantas", Description = "")]
        Desconocido = 0,
        [Display(Name = "Congelados", GroupName = "Reportes Plantas", Description = "")]
        Congelado = 1,
        [Display(Name = "Conservas", GroupName = "Reportes Plantas", Description = "")]
        Conserva = 2,
        [Display(Name = "Refrigerados", GroupName = "Reportes Plantas", Description = "")]
        Refrigerado = 3
    }

    public enum Item
    {
        Producto, Recurso
    }

    public enum Tipo
    {
        [Display(GroupName = "Plantas", Name = "Materia Prima", Prompt = "Toneladas")]
        MateriaPrima = 0,

        [Display(Description = "Action whose purpose is the fixation of invertebrate larvae through the arrangement of collectors.",
            GroupName = "Farm", 
            Name = "Seed Uptake", 
            Prompt = "Tons")]
        Semilla = 1,

        [Display(Description = "Extractive activity that takes place in aquaculture centers in order to obtain a product for subsequent commercialization.",
            GroupName = "Farm",
            Name = "Adult Harvest", 
            Prompt = "Tons")]
        Cosecha = 2,

        [Display(Description = "Resource that an agent enters and is available for commercialization and / or transformation.",
            GroupName = "Plant",
            Name = "Plant Supply",
            Prompt = "Tons")]
        Abastecimiento = 3,

        [Display(Description = "Net weight of products obtained from a raw material, through a manufacturing process.",
            GroupName = "Plant", 
            Name = "Production of Manufacturing Plants", 
            Prompt = "Tons")]
        Producción = 4,

        [Display(Description = "Corresponds to the interface of heat exchange between the atmosphere and the ocean. In practical terms, the meaning of 'surface' will vary according to the measurement method used to determine the temperature.",
            GroupName = "Environmental",
            Name = "Surface Temperature", 
            Prompt = "SST - Sea Surface Temperature")]
        Temperatura = 5,

        [Display(Description = "Relative amount of dissolved salts (gr) in a body of water (L) measured in Practical Salinity Units (PSU) defined as 35 g of salt / Lt of water.",
            GroupName = "Environmental",
            Name = "Salinity", 
            Prompt = "PSU - Practical Salinity Units")]
        Salinidad = 6,
    }
}
