using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacturationTN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriesClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesClient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesProduit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesProduit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parametres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MontantTimbreFiscal = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RaisonSociale = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MatriculeFiscale = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    LogoBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    LogoContentType = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TauxTva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Libelle = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Taux = table.Column<int>(type: "INTEGER", nullable: false),
                    Actif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TauxTva", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitesMesure",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitesMesure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeClient = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Statut = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    CategorieClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MatriculeFiscale = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    NumeroRne = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FormeJuridique = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Adresse = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    CodePostal = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Ville = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Gouvernorat = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Pays = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Telephone2 = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Fax = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    SiteWeb = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NomContact = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    PosteContact = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PlafondCredit = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ModePaiement = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Devise = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    RemiseParDefaut = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_CategoriesClient_CategorieClientId",
                        column: x => x.CategorieClientId,
                        principalTable: "CategoriesClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Produits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeProduit = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Statut = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    CategorieProduitId = table.Column<int>(type: "INTEGER", nullable: true),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Marque = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UniteMesureId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrixAchatHT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PrixVenteHT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TauxTva = table.Column<int>(type: "INTEGER", nullable: false),
                    RemiseMax = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SuiviStock = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuantiteStock = table.Column<int>(type: "INTEGER", nullable: false),
                    SeuilAlerte = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produits_CategoriesProduit_CategorieProduitId",
                        column: x => x.CategorieProduitId,
                        principalTable: "CategoriesProduit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Produits_UnitesMesure_UniteMesureId",
                        column: x => x.UniteMesureId,
                        principalTable: "UnitesMesure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Factures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroFacture = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DateFacture = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Statut = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimbreFiscal = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factures_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LignesFacture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FactureId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProduitId = table.Column<int>(type: "INTEGER", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    PrixUnitaireHT = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TauxTva = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesFacture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LignesFacture_Factures_FactureId",
                        column: x => x.FactureId,
                        principalTable: "Factures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LignesFacture_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CategoriesClient",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "VIP" },
                    { 2, "PME" },
                    { 3, "Particulier" },
                    { 4, "Export" }
                });

            migrationBuilder.InsertData(
                table: "CategoriesProduit",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "Alimentaire" },
                    { 2, "Électronique" },
                    { 3, "Textile" },
                    { 4, "Service conseil" },
                    { 5, "Bâtiment" },
                    { 6, "Informatique" }
                });

            migrationBuilder.InsertData(
                table: "Parametres",
                columns: new[] { "Id", "Adresse", "Email", "LogoBase64", "LogoContentType", "MatriculeFiscale", "MontantTimbreFiscal", "RaisonSociale", "Telephone" },
                values: new object[] { 1, "", null, null, null, "", 1.000m, "", null });

            migrationBuilder.InsertData(
                table: "TauxTva",
                columns: new[] { "Id", "Actif", "Libelle", "Taux" },
                values: new object[,]
                {
                    { 1, true, "Exonéré", 0 },
                    { 2, true, "Réduit", 7 },
                    { 3, true, "Intermédiaire", 13 },
                    { 4, true, "Normal", 19 }
                });

            migrationBuilder.InsertData(
                table: "UnitesMesure",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "Pièce" },
                    { 2, "Kg" },
                    { 3, "Litre" },
                    { 4, "Mètre" },
                    { 5, "m²" },
                    { 6, "Heure" },
                    { 7, "Jour" },
                    { 8, "Forfait" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesClient_Nom",
                table: "CategoriesClient",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesProduit_Nom",
                table: "CategoriesProduit",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CategorieClientId",
                table: "Clients",
                column: "CategorieClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_MatriculeFiscale",
                table: "Clients",
                column: "MatriculeFiscale",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factures_ClientId",
                table: "Factures",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Factures_NumeroFacture",
                table: "Factures",
                column: "NumeroFacture",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LignesFacture_FactureId",
                table: "LignesFacture",
                column: "FactureId");

            migrationBuilder.CreateIndex(
                name: "IX_LignesFacture_ProduitId",
                table: "LignesFacture",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_CategorieProduitId",
                table: "Produits",
                column: "CategorieProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_Reference",
                table: "Produits",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produits_UniteMesureId",
                table: "Produits",
                column: "UniteMesureId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitesMesure_Nom",
                table: "UnitesMesure",
                column: "Nom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LignesFacture");

            migrationBuilder.DropTable(
                name: "Parametres");

            migrationBuilder.DropTable(
                name: "TauxTva");

            migrationBuilder.DropTable(
                name: "Factures");

            migrationBuilder.DropTable(
                name: "Produits");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "CategoriesProduit");

            migrationBuilder.DropTable(
                name: "UnitesMesure");

            migrationBuilder.DropTable(
                name: "CategoriesClient");
        }
    }
}
