using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturationTN.Models.Enums;

namespace FacturationTN.Models;

public class Facture
{
    public int Id { get; set; }

    // ════════════════════════════════════════════════════════════
    // En-tête
    // ════════════════════════════════════════════════════════════

    /// <summary>Numéro auto-généré (FA-YYYY-NNNN).</summary>
    [Required(ErrorMessage = "Le numéro de facture est obligatoire.")]
    [MaxLength(20)]
    public string NumeroFacture { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date de facture est obligatoire.")]
    public DateTime DateFacture { get; set; } = DateTime.Today;

    public StatutFacture Statut { get; set; } = StatutFacture.Brouillon;

    // ── Client ──

    [Required(ErrorMessage = "Le client est obligatoire.")]
    [Range(1, int.MaxValue, ErrorMessage = "Le client est obligatoire.")]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    // ════════════════════════════════════════════════════════════
    // Montants
    // ════════════════════════════════════════════════════════════

    /// <summary>Timbre fiscal appliqué à cette facture.</summary>
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Le timbre fiscal ne peut pas être négatif.")]
    public decimal TimbreFiscal { get; set; } = 1.000m;

    // ── Propriétés calculées (non mappées) ──

    [NotMapped]
    public decimal TotalHT => Lignes.Sum(l => l.MontantHT);

    [NotMapped]
    public decimal TotalTVA => Lignes.Sum(l => l.MontantTva);

    [NotMapped]
    public decimal TotalTTC => TotalHT + TotalTVA + TimbreFiscal;

    // ════════════════════════════════════════════════════════════
    // Lignes de facture
    // ════════════════════════════════════════════════════════════

    public ICollection<LigneFacture> Lignes { get; set; } = new List<LigneFacture>();

    // ════════════════════════════════════════════════════════════
    // Audit
    // ════════════════════════════════════════════════════════════

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateModification { get; set; }
}
