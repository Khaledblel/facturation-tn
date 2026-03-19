using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturationTN.Models.Enums;

namespace FacturationTN.Models;

public class Produit
{
    public int Id { get; set; }

    // ════════════════════════════════════════════════════════════
    // Identification
    // ════════════════════════════════════════════════════════════

    [Required(ErrorMessage = "Le type de produit est obligatoire.")]
    [Range(1, int.MaxValue, ErrorMessage = "Le type de produit est obligatoire.")]
    public TypeProduit TypeProduit { get; set; }

    public StatutProduit Statut { get; set; } = StatutProduit.Actif;

    /// <summary>FK vers la catégorie dynamique (nullable).</summary>
    public int? CategorieProduitId { get; set; }
    public CategorieProduit? Categorie { get; set; }

    [Required(ErrorMessage = "La référence est obligatoire.")]
    [MaxLength(50)]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "La désignation est obligatoire.")]
    [MaxLength(50)]
    public string Designation { get; set; } = string.Empty;

    /// <summary>Non applicable pour les services.</summary>
    [MaxLength(30)]
    public string? Marque { get; set; }

    /// <summary>FK vers l'unité de mesure dynamique.</summary>
    public int? UniteMesureId { get; set; }
    public UniteMesure? UniteMesure { get; set; }

    // ════════════════════════════════════════════════════════════
    // Tarification
    // ════════════════════════════════════════════════════════════

    /// <summary>Prix d'achat HT (produit) ou coût de revient HT (service).</summary>
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, 9999999.999, ErrorMessage = "Le prix d'achat HT doit être entre 0 et 9 999 999.999.")]
    public decimal PrixAchatHT { get; set; }

    /// <summary>Prix de vente HT (produit) ou tarif HT (service).</summary>
    [Required(ErrorMessage = "Le prix de vente HT est obligatoire.")]
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, 9999999.999, ErrorMessage = "Le prix de vente HT doit être entre 0 et 9 999 999.999.")]
    public decimal PrixVenteHT { get; set; }

    /// <summary>Taux TVA applicable (0, 7, 13 ou 19).</summary>
    [Range(0, 100, ErrorMessage = "Le taux TVA doit être entre 0 et 100 %.")]
    public int TauxTva { get; set; } = 19;

    /// <summary>Remise maximale autorisée (%).</summary>
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 100, ErrorMessage = "La remise maximale doit être entre 0 et 100 %.")]
    public decimal RemiseMax { get; set; }

    // ── Propriétés calculées (non mappées en DB) ──

    [NotMapped]
    public decimal PrixTTC => PrixVenteHT * (1 + TauxTva / 100m);

    [NotMapped]
    public decimal MargeBrute => PrixVenteHT - PrixAchatHT;

    // ════════════════════════════════════════════════════════════
    // Stock (ignoré pour les services)
    // ════════════════════════════════════════════════════════════

    public bool SuiviStock { get; set; }

    [Range(0, 1000000, ErrorMessage = "La quantité en stock doit être entre 0 et 1 000 000.")]
    public int QuantiteStock { get; set; }

    [Range(0, 1000000, ErrorMessage = "Le seuil d'alerte doit être entre 0 et 1 000 000.")]
    public int SeuilAlerte { get; set; }

    // ════════════════════════════════════════════════════════════
    // Description & Notes
    // ════════════════════════════════════════════════════════════

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // ════════════════════════════════════════════════════════════
    // Audit
    // ════════════════════════════════════════════════════════════

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateModification { get; set; }
}
