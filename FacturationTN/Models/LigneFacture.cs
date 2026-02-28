using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturationTN.Models;

public class LigneFacture
{
    public int Id { get; set; }

    // ── Parent facture ──

    [Required]
    public int FactureId { get; set; }
    public Facture Facture { get; set; } = null!;

    // ── Produit référencé ──

    [Required(ErrorMessage = "Le produit est obligatoire.")]
    [Range(1, int.MaxValue, ErrorMessage = "Le produit est obligatoire.")]
    public int ProduitId { get; set; }
    public Produit Produit { get; set; } = null!;

    // ── Valeurs figées au moment de la facturation ──

    /// <summary>Désignation copiée depuis le produit (snapshot).</summary>
    [Required]
    [MaxLength(200)]
    public string Designation { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être au moins 1.")]
    public int Quantite { get; set; } = 1;

    /// <summary>Prix unitaire HT figé au moment de la facturation.</summary>
    [Required]
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Le prix unitaire HT ne peut pas être négatif.")]
    public decimal PrixUnitaireHT { get; set; }

    /// <summary>Taux TVA figé au moment de la facturation (0, 7, 13, 19).</summary>
    [Range(0, 100)]
    public int TauxTva { get; set; } = 19;

    // ── Propriétés calculées (non mappées) ──

    [NotMapped]
    public decimal MontantHT => Quantite * PrixUnitaireHT;

    [NotMapped]
    public decimal MontantTva => MontantHT * TauxTva / 100m;

    [NotMapped]
    public decimal MontantTTC => MontantHT + MontantTva;
}
