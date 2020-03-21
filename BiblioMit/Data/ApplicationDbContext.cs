using Microsoft.EntityFrameworkCore;
using BiblioMit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BiblioMit.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder?.Entity<AppRole>()
                .HasMany(e => e.Users)
                .WithOne()
                .HasForeignKey(e => e.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CentreProducto>()
                .HasKey(c => new { c.CentreId, c.ProductoId });

            builder.Entity<PlataformaUser>()
                .HasKey(p => new { p.AppUserId, p.PlataformId });

            builder.Entity<AreaCodeProvincia>()
                .HasKey(p => new { p.AreaCodeId, p.ProvinciaId});

            //builder.Entity<Centre>()
            //    .HasMany(c => c.Abastecimientos)
            //    .WithOne()
            //    .HasForeignKey(a => a.CentreId)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaim { get; set; }

        public DbSet<IdentityUserRole<string>> IdentityUserRole { get; set; }
        public DbSet<Larvae> Larvae { get; set; }
        public DbSet<Polygon> Polygon { get; set; }
        public DbSet<RepStage> RepStage { get; set; }
        public DbSet<Specie> Specie { get; set; }
        public DbSet<SpecieSeed> SpecieSeed { get; set; }
        public DbSet<Talla> Talla { get; set; }
        public DbSet<Larva> Larva { get; set; }
        public DbSet<Seed> Seed { get; set; }
        public DbSet<Spawning> Spawning { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<Origen> Origen { get; set; }
        public DbSet<Cuenca> Cuenca { get; set; }
        public DbSet<PSMB> PSMB { get; set; }
        public DbSet<AppUserRole> AppUserRole { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<PlataformaUser> PlataformaUser { get; set; }
        public DbSet<Platform> Platform { get; set; }
        public DbSet<AppRole> AppRole { get; set; }
        public DbSet<Valve> Valve { get; set; }
        public DbSet<Soft> Soft { get; set; }
        public DbSet<Forum> Forum { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<PostReply> PostReply { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<Sampling> Sampling { get; set; }
        public DbSet<Individual> Individual { get; set; }
        public DbSet<Centre> Centre { get; set; }
        public DbSet<EnsayoFito> EnsayoFito { get; set; }
        public DbSet<Phytoplankton> Phytoplankton { get; set; }
        //public DbSet<Planta> Planta { get; set; }
        public DbSet<Producto> Producto { get; set; }
        //public DbSet<PlantaProducto> PlantaProducto { get; set; }
        public DbSet<CentreProducto> CentreProducto { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Coordinate> Coordinate { get; set; }
        //public DbSet<Production> Production { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Provincia> Provincia { get; set; }
        public DbSet<AreaCode> AreaCode { get; set; }
        public DbSet<AreaCodeProvincia> AreaCodeProvincia { get; set; }
        public DbSet<Comuna> Comuna { get; set; }
        public DbSet<Planilla> Planilla { get; set; }
        //public DbSet<Abastecimiento> Abastecimiento { get; set; }
        //public DbSet<Cosecha> Cosecha { get; set; }
        //public DbSet<MateriaPrima> MateriaPrima { get; set; }
        //public DbSet<Semilla> Semilla { get; set; }
        public DbSet<ExcelFile> ExcelFile { get; set; }
        public DbSet<Columna> Columna { get; set; }
        //public DbSet<Entry> Entries { get; set; }
        public DbSet<ProdEntry> ProdEntry { get; set; }
        //public DbSet<FormatosFecha> FormatosFecha { get; set; }
    }
}
