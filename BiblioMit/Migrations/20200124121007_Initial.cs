using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BiblioMit.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreaCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Last = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    ProfileImageUrl = table.Column<string>(nullable: true),
                    MemberSince = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    BsnssName = table.Column<string>(nullable: true),
                    Acronym = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cuenca",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuenca", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Excel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forum",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forum", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Origen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Origen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platform",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Plataforma = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platform", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Surface = table.Column<int>(nullable: false),
                    Pop2002 = table.Column<int>(nullable: false),
                    Pop2010 = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specie",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Sp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    AppRoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetRoles_AppRoleId",
                        column: x => x.AppRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    RoleAssigner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppUserId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    OutPut = table.Column<string>(nullable: true),
                    IP = table.Column<string>(nullable: true),
                    Actualizadas = table.Column<int>(nullable: false),
                    Agregadas = table.Column<int>(nullable: false),
                    Observaciones = table.Column<int>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    Min = table.Column<DateTime>(nullable: false),
                    Max = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    Reportes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdEntry_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Columna",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ExcelId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Operation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Columna_Excel_ExcelId",
                        column: x => x.ExcelId,
                        principalTable: "Excel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ForumId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Forum_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Forum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlataformaUser",
                columns: table => new
                {
                    AppUserId = table.Column<string>(nullable: false),
                    PlataformId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlataformaUser", x => new { x.AppUserId, x.PlataformId });
                    table.ForeignKey(
                        name: "FK_PlataformaUser_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlataformaUser_Platform_PlataformId",
                        column: x => x.PlataformId,
                        principalTable: "Platform",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Surface = table.Column<int>(nullable: false),
                    Population = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provincia_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostReply",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    PostId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReply_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostReply_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AreaCodeProvincia",
                columns: table => new
                {
                    ProvinciaId = table.Column<int>(nullable: false),
                    AreaCodeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaCodeProvincia", x => new { x.AreaCodeId, x.ProvinciaId });
                    table.ForeignKey(
                        name: "FK_AreaCodeProvincia_AreaCode_AreaCodeId",
                        column: x => x.AreaCodeId,
                        principalTable: "AreaCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaCodeProvincia_Provincia_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comuna",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ProvinciaId = table.Column<int>(nullable: false),
                    CuencaId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DE = table.Column<int>(nullable: false),
                    CS = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comuna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comuna_Cuenca_CuencaId",
                        column: x => x.CuencaId,
                        principalTable: "Cuenca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comuna_Provincia_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Polygon",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ComunaId = table.Column<int>(nullable: true),
                    ProvinciaId = table.Column<int>(nullable: true),
                    RegionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polygon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Polygon_Comuna_ComunaId",
                        column: x => x.ComunaId,
                        principalTable: "Comuna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Polygon_Provincia_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Polygon_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PSMB",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ComunaId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSMB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PSMB_Comuna_ComunaId",
                        column: x => x.ComunaId,
                        principalTable: "Comuna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Centre",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    ComunaId = table.Column<int>(nullable: true),
                    PSMBId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Acronym = table.Column<string>(nullable: true),
                    FolioRNA = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    CuerpoAgua = table.Column<int>(nullable: true),
                    Certificable = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Centre_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Centre_Comuna_ComunaId",
                        column: x => x.ComunaId,
                        principalTable: "Comuna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Centre_PSMB_PSMBId",
                        column: x => x.PSMBId,
                        principalTable: "PSMB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Analysis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CentreId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analysis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analysis_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CentreProducto",
                columns: table => new
                {
                    CentreId = table.Column<int>(nullable: false),
                    ProductoId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentreProducto", x => new { x.CentreId, x.ProductoId });
                    table.ForeignKey(
                        name: "FK_CentreProducto_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CentreProducto_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    ContactId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<string>(nullable: true),
                    CentreId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Last = table.Column<string>(nullable: true),
                    Phone = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    OpenHr = table.Column<DateTime>(nullable: false),
                    CloseHr = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_Contact_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coordinate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CuencaId = table.Column<int>(nullable: true),
                    PSMBId = table.Column<int>(nullable: true),
                    CentreId = table.Column<int>(nullable: true),
                    PolygonId = table.Column<int>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Vertex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coordinate_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coordinate_Cuenca_CuencaId",
                        column: x => x.CuencaId,
                        principalTable: "Cuenca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coordinate_PSMB_PSMBId",
                        column: x => x.PSMBId,
                        principalTable: "PSMB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coordinate_Polygon_PolygonId",
                        column: x => x.PolygonId,
                        principalTable: "Polygon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnsayoFito",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Estacion = table.Column<string>(nullable: true),
                    EntidadMuestreadora = table.Column<string>(nullable: true),
                    FechaMuestreo = table.Column<DateTime>(nullable: false),
                    InicioAnalisis = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Recepcion = table.Column<DateTime>(nullable: false),
                    FinAnalisis = table.Column<DateTime>(nullable: false),
                    Laboratorio = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    CentreId = table.Column<int>(nullable: true),
                    PSMBId = table.Column<int>(nullable: false),
                    Muestras = table.Column<int>(nullable: false),
                    FechaEnvio = table.Column<DateTime>(nullable: false),
                    Analista = table.Column<string>(nullable: true),
                    Temperatura = table.Column<double>(nullable: true),
                    Oxigeno = table.Column<double>(nullable: true),
                    Ph = table.Column<double>(nullable: true),
                    Salinidad = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnsayoFito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnsayoFito_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnsayoFito_PSMB_PSMBId",
                        column: x => x.PSMBId,
                        principalTable: "PSMB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Larvae",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CentreId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Larvae", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Larvae_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Planilla",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Declaracion = table.Column<int>(nullable: false),
                    CentreId = table.Column<int>(nullable: false),
                    Dato = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Peso = table.Column<double>(nullable: false),
                    TipoProduccion = table.Column<int>(nullable: true),
                    TipoItemProduccion = table.Column<int>(nullable: true),
                    OrigenId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Planilla_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Planilla_Origen_OrigenId",
                        column: x => x.OrigenId,
                        principalTable: "Origen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sampling",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CentreId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Salinity = table.Column<int>(nullable: true),
                    Temp = table.Column<double>(nullable: true),
                    O2 = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sampling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sampling_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CentreId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DateCuelga = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seed_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spawning",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CentreId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    MaleProportion = table.Column<int>(nullable: false),
                    FemaleProportion = table.Column<int>(nullable: false),
                    MaleIG = table.Column<double>(nullable: false),
                    FemaleIG = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spawning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spawning_Centre_CentreId",
                        column: x => x.CentreId,
                        principalTable: "Centre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phytoplankton",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EnsayoFitoId = table.Column<int>(nullable: false),
                    Species = table.Column<string>(nullable: true),
                    EAR = table.Column<int>(nullable: true),
                    C = table.Column<double>(nullable: false),
                    GroupsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phytoplankton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phytoplankton_EnsayoFito_EnsayoFitoId",
                        column: x => x.EnsayoFitoId,
                        principalTable: "EnsayoFito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Phytoplankton_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Larva",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LarvaeId = table.Column<int>(nullable: false),
                    SpecieId = table.Column<int>(nullable: false),
                    LarvaType = table.Column<int>(nullable: false),
                    Count = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Larva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Larva_Larvae_LarvaeId",
                        column: x => x.LarvaeId,
                        principalTable: "Larvae",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Larva_Specie_SpecieId",
                        column: x => x.SpecieId,
                        principalTable: "Specie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Individual",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    SamplingId = table.Column<int>(nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    Maturity = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Number = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(nullable: true),
                    Depth = table.Column<int>(nullable: true),
                    ADG = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Individual", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Individual_Sampling_SamplingId",
                        column: x => x.SamplingId,
                        principalTable: "Sampling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecieSeed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpecieId = table.Column<int>(nullable: false),
                    SeedId = table.Column<int>(nullable: false),
                    Capture = table.Column<int>(nullable: false),
                    Proportion = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecieSeed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecieSeed_Seed_SeedId",
                        column: x => x.SeedId,
                        principalTable: "Seed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecieSeed_Specie_SpecieId",
                        column: x => x.SpecieId,
                        principalTable: "Specie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepStage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpawningId = table.Column<int>(nullable: false),
                    Stage = table.Column<int>(nullable: false),
                    Proportion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepStage_Spawning_SpawningId",
                        column: x => x.SpawningId,
                        principalTable: "Spawning",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Soft",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IndividualId = table.Column<int>(nullable: false),
                    SoftType = table.Column<int>(nullable: false),
                    Tissue = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: true),
                    Degree = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Soft_Individual_IndividualId",
                        column: x => x.IndividualId,
                        principalTable: "Individual",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Valve",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IndividualId = table.Column<int>(nullable: false),
                    ValveType = table.Column<int>(nullable: false),
                    Species = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Valve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Valve_Individual_IndividualId",
                        column: x => x.IndividualId,
                        principalTable: "Individual",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Talla",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpecieSeedId = table.Column<int>(nullable: false),
                    Range = table.Column<int>(nullable: false),
                    Proportion = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Talla_SpecieSeed_SpecieSeedId",
                        column: x => x.SpecieSeedId,
                        principalTable: "SpecieSeed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IndividualId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Magnification = table.Column<int>(nullable: false),
                    SoftId = table.Column<int>(nullable: true),
                    ValveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photo_Individual_IndividualId",
                        column: x => x.IndividualId,
                        principalTable: "Individual",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Photo_Soft_SoftId",
                        column: x => x.SoftId,
                        principalTable: "Soft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Photo_Valve_ValveId",
                        column: x => x.ValveId,
                        principalTable: "Valve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analysis_CentreId",
                table: "Analysis",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaCodeProvincia_ProvinciaId",
                table: "AreaCodeProvincia",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_AppRoleId",
                table: "AspNetUserClaims",
                column: "AppRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Centre_CompanyId",
                table: "Centre",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Centre_ComunaId",
                table: "Centre",
                column: "ComunaId");

            migrationBuilder.CreateIndex(
                name: "IX_Centre_PSMBId",
                table: "Centre",
                column: "PSMBId");

            migrationBuilder.CreateIndex(
                name: "IX_CentreProducto_ProductoId",
                table: "CentreProducto",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Columna_ExcelId",
                table: "Columna",
                column: "ExcelId");

            migrationBuilder.CreateIndex(
                name: "IX_Comuna_CuencaId",
                table: "Comuna",
                column: "CuencaId");

            migrationBuilder.CreateIndex(
                name: "IX_Comuna_ProvinciaId",
                table: "Comuna",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_CentreId",
                table: "Contact",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinate_CentreId",
                table: "Coordinate",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinate_CuencaId",
                table: "Coordinate",
                column: "CuencaId");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinate_PSMBId",
                table: "Coordinate",
                column: "PSMBId");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinate_PolygonId",
                table: "Coordinate",
                column: "PolygonId");

            migrationBuilder.CreateIndex(
                name: "IX_EnsayoFito_CentreId",
                table: "EnsayoFito",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_EnsayoFito_PSMBId",
                table: "EnsayoFito",
                column: "PSMBId");

            migrationBuilder.CreateIndex(
                name: "IX_Individual_SamplingId",
                table: "Individual",
                column: "SamplingId");

            migrationBuilder.CreateIndex(
                name: "IX_Larva_LarvaeId",
                table: "Larva",
                column: "LarvaeId");

            migrationBuilder.CreateIndex(
                name: "IX_Larva_SpecieId",
                table: "Larva",
                column: "SpecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Larvae_CentreId",
                table: "Larvae",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_IndividualId",
                table: "Photo",
                column: "IndividualId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_SoftId",
                table: "Photo",
                column: "SoftId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_ValveId",
                table: "Photo",
                column: "ValveId");

            migrationBuilder.CreateIndex(
                name: "IX_Phytoplankton_EnsayoFitoId",
                table: "Phytoplankton",
                column: "EnsayoFitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Phytoplankton_GroupsId",
                table: "Phytoplankton",
                column: "GroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_Planilla_CentreId",
                table: "Planilla",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_Planilla_OrigenId",
                table: "Planilla",
                column: "OrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_PlataformaUser_PlataformId",
                table: "PlataformaUser",
                column: "PlataformId");

            migrationBuilder.CreateIndex(
                name: "IX_Polygon_ComunaId",
                table: "Polygon",
                column: "ComunaId");

            migrationBuilder.CreateIndex(
                name: "IX_Polygon_ProvinciaId",
                table: "Polygon",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Polygon_RegionId",
                table: "Polygon",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ForumId",
                table: "Post",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId",
                table: "Post",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReply_PostId",
                table: "PostReply",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReply_UserId",
                table: "PostReply",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdEntry_AppUserId",
                table: "ProdEntry",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Provincia_RegionId",
                table: "Provincia",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_PSMB_ComunaId",
                table: "PSMB",
                column: "ComunaId");

            migrationBuilder.CreateIndex(
                name: "IX_RepStage_SpawningId",
                table: "RepStage",
                column: "SpawningId");

            migrationBuilder.CreateIndex(
                name: "IX_Sampling_CentreId",
                table: "Sampling",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_Seed_CentreId",
                table: "Seed",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_Soft_IndividualId",
                table: "Soft",
                column: "IndividualId");

            migrationBuilder.CreateIndex(
                name: "IX_Spawning_CentreId",
                table: "Spawning",
                column: "CentreId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecieSeed_SeedId",
                table: "SpecieSeed",
                column: "SeedId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecieSeed_SpecieId",
                table: "SpecieSeed",
                column: "SpecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Talla_SpecieSeedId",
                table: "Talla",
                column: "SpecieSeedId");

            migrationBuilder.CreateIndex(
                name: "IX_Valve_IndividualId",
                table: "Valve",
                column: "IndividualId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analysis");

            migrationBuilder.DropTable(
                name: "AreaCodeProvincia");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CentreProducto");

            migrationBuilder.DropTable(
                name: "Columna");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Coordinate");

            migrationBuilder.DropTable(
                name: "Larva");

            migrationBuilder.DropTable(
                name: "Photo");

            migrationBuilder.DropTable(
                name: "Phytoplankton");

            migrationBuilder.DropTable(
                name: "Planilla");

            migrationBuilder.DropTable(
                name: "PlataformaUser");

            migrationBuilder.DropTable(
                name: "PostReply");

            migrationBuilder.DropTable(
                name: "ProdEntry");

            migrationBuilder.DropTable(
                name: "RepStage");

            migrationBuilder.DropTable(
                name: "Talla");

            migrationBuilder.DropTable(
                name: "AreaCode");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Excel");

            migrationBuilder.DropTable(
                name: "Polygon");

            migrationBuilder.DropTable(
                name: "Larvae");

            migrationBuilder.DropTable(
                name: "Soft");

            migrationBuilder.DropTable(
                name: "Valve");

            migrationBuilder.DropTable(
                name: "EnsayoFito");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Origen");

            migrationBuilder.DropTable(
                name: "Platform");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Spawning");

            migrationBuilder.DropTable(
                name: "SpecieSeed");

            migrationBuilder.DropTable(
                name: "Individual");

            migrationBuilder.DropTable(
                name: "Forum");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Seed");

            migrationBuilder.DropTable(
                name: "Specie");

            migrationBuilder.DropTable(
                name: "Sampling");

            migrationBuilder.DropTable(
                name: "Centre");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "PSMB");

            migrationBuilder.DropTable(
                name: "Comuna");

            migrationBuilder.DropTable(
                name: "Cuenca");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
