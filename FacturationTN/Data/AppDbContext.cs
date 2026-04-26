using Microsoft.EntityFrameworkCore;
using FacturationTN.Models;
using FacturationTN.Models.Enums;

namespace FacturationTN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── Domain entities ──
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Produit> Produits => Set<Produit>();
    public DbSet<Facture> Factures => Set<Facture>();
    public DbSet<LigneFacture> LignesFacture => Set<LigneFacture>();

    // ── Lookup / reference tables ──
    public DbSet<CategorieClient> CategoriesClient => Set<CategorieClient>();
    public DbSet<CategorieProduit> CategoriesProduit => Set<CategorieProduit>();
    public DbSet<UniteMesure> UnitesMesure => Set<UniteMesure>();
    public DbSet<TauxTva> TauxTva => Set<TauxTva>();

    // ── Settings (singleton) ──
    public DbSet<Parametres> Parametres => Set<Parametres>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ════════════════════════════════════════════════════════
        // Client
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<Client>(e =>
        {
            e.HasIndex(c => c.MatriculeFiscale).IsUnique();
            e.HasOne(c => c.Categorie)
             .WithMany(cat => cat.Clients)
             .HasForeignKey(c => c.CategorieClientId)
             .OnDelete(DeleteBehavior.SetNull);

            // Store enums as strings for readability
            e.Property(c => c.TypeClient).HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.Statut).HasConversion<string>().HasMaxLength(15);
            e.Property(c => c.FormeJuridique).HasConversion<string>().HasMaxLength(10);
            e.Property(c => c.ModePaiement).HasConversion<string>().HasMaxLength(20);
        });

        // ════════════════════════════════════════════════════════
        // CategorieClient
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<CategorieClient>(e =>
        {
            e.HasIndex(c => c.Nom).IsUnique();
        });

        // ════════════════════════════════════════════════════════
        // Produit
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<Produit>(e =>
        {
            e.HasIndex(p => p.Reference).IsUnique();
            e.HasOne(p => p.Categorie)
             .WithMany(cat => cat.Produits)
             .HasForeignKey(p => p.CategorieProduitId)
             .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.UniteMesure)
             .WithMany(u => u.Produits)
             .HasForeignKey(p => p.UniteMesureId)
             .OnDelete(DeleteBehavior.SetNull);

            e.Property(p => p.TypeProduit).HasConversion<string>().HasMaxLength(10);
            e.Property(p => p.Statut).HasConversion<string>().HasMaxLength(15);
        });

        // ════════════════════════════════════════════════════════
        // CategorieProduit
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<CategorieProduit>(e =>
        {
            e.HasIndex(c => c.Nom).IsUnique();
        });

        // ════════════════════════════════════════════════════════
        // UniteMesure
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<UniteMesure>(e =>
        {
            e.HasIndex(u => u.Nom).IsUnique();
        });

        // ════════════════════════════════════════════════════════
        // Facture
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<Facture>(e =>
        {
            e.HasIndex(f => f.NumeroFacture).IsUnique();
            e.HasOne(f => f.Client)
             .WithMany()
             .HasForeignKey(f => f.ClientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(f => f.Statut).HasConversion<string>().HasMaxLength(15);
        });

        // ════════════════════════════════════════════════════════
        // LigneFacture
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<LigneFacture>(e =>
        {
            e.HasOne(l => l.Facture)
             .WithMany(f => f.Lignes)
             .HasForeignKey(l => l.FactureId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(l => l.Produit)
             .WithMany()
             .HasForeignKey(l => l.ProduitId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ════════════════════════════════════════════════════════
        // UserAccount
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<UserAccount>(e =>
        {
            e.HasIndex(u => u.NormalizedUserName).IsUnique();
        });

        // ════════════════════════════════════════════════════════
        // Seed Data
        // ════════════════════════════════════════════════════════

        // Taux de TVA tunisiens
        modelBuilder.Entity<TauxTva>().HasData(
            new TauxTva { Id = 1, Libelle = "Exonéré",       Taux = 0,  Actif = true },
            new TauxTva { Id = 2, Libelle = "Réduit",        Taux = 7,  Actif = true },
            new TauxTva { Id = 3, Libelle = "Intermédiaire", Taux = 13, Actif = true },
            new TauxTva { Id = 4, Libelle = "Normal",        Taux = 19, Actif = true }
        );

        // Paramètres par défaut
        modelBuilder.Entity<Parametres>().HasData(
            new Parametres
            {
                Id = 1,
                MontantTimbreFiscal = 1.000m,
                RaisonSociale = string.Empty,
                MatriculeFiscale = string.Empty,
                Adresse = string.Empty
            }
        );

        // Catégories client par défaut
        modelBuilder.Entity<CategorieClient>().HasData(
            new CategorieClient { Id = 1, Nom = "VIP" },
            new CategorieClient { Id = 2, Nom = "PME" },
            new CategorieClient { Id = 3, Nom = "Particulier" },
            new CategorieClient { Id = 4, Nom = "Export" }
        );

        // Catégories produit par défaut
        modelBuilder.Entity<CategorieProduit>().HasData(
            new CategorieProduit { Id = 1, Nom = "Alimentaire" },
            new CategorieProduit { Id = 2, Nom = "Électronique" },
            new CategorieProduit { Id = 3, Nom = "Textile" },
            new CategorieProduit { Id = 4, Nom = "Service conseil" },
            new CategorieProduit { Id = 5, Nom = "Bâtiment" },
            new CategorieProduit { Id = 6, Nom = "Informatique" }
        );

        // Unités de mesure par défaut
        modelBuilder.Entity<UniteMesure>().HasData(
            new UniteMesure { Id = 1, Nom = "Pièce" },
            new UniteMesure { Id = 2, Nom = "Kg" },
            new UniteMesure { Id = 3, Nom = "Litre" },
            new UniteMesure { Id = 4, Nom = "Mètre" },
            new UniteMesure { Id = 5, Nom = "m²" },
            new UniteMesure { Id = 6, Nom = "Heure" },
            new UniteMesure { Id = 7, Nom = "Jour" },
            new UniteMesure { Id = 8, Nom = "Forfait" }
        );
    }
}
