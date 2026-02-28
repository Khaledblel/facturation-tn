using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturationTN.Models.Enums;

namespace FacturationTN.Models;

public class Client
{
    public int Id { get; set; }

    // ════════════════════════════════════════════════════════════
    // Identification
    // ════════════════════════════════════════════════════════════

    [Required(ErrorMessage = "Le type de client est obligatoire.")]
    [Range(1, int.MaxValue, ErrorMessage = "Le type de client est obligatoire.")]
    public TypeClient TypeClient { get; set; }

    public StatutClient Statut { get; set; } = StatutClient.Actif;

    /// <summary>FK vers la catégorie dynamique (nullable).</summary>
    public int? CategorieClientId { get; set; }
    public CategorieClient? Categorie { get; set; }

    [Required(ErrorMessage = "Le nom / raison sociale est obligatoire.")]
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le matricule fiscale est obligatoire.")]
    [MaxLength(30)]
    [RegularExpression(@"^\d{7}/[A-Z]/[A-Z]/[A-Z]\d{3}$",
        ErrorMessage = "Format attendu : 0000000/X/X/X000")]
    public string MatriculeFiscale { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? NumeroRne { get; set; }

    public FormeJuridique? FormeJuridique { get; set; }

    // ════════════════════════════════════════════════════════════
    // Adresse & Contact
    // ════════════════════════════════════════════════════════════

    [MaxLength(300)]
    public string? Adresse { get; set; }

    [MaxLength(10)]
    public string? CodePostal { get; set; }

    [MaxLength(100)]
    public string? Ville { get; set; }

    [MaxLength(50)]
    public string? Gouvernorat { get; set; }

    [MaxLength(60)]
    public string Pays { get; set; } = "Tunisie";

    [MaxLength(20)]
    [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
    public string? Telephone { get; set; }

    [MaxLength(20)]
    [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
    public string? Telephone2 { get; set; }

    [MaxLength(20)]
    public string? Fax { get; set; }

    [MaxLength(150)]
    [EmailAddress(ErrorMessage = "Adresse email invalide.")]
    public string? Email { get; set; }

    [MaxLength(200)]
    [Url(ErrorMessage = "URL invalide.")]
    public string? SiteWeb { get; set; }

    [MaxLength(150)]
    public string? NomContact { get; set; }

    [MaxLength(100)]
    public string? PosteContact { get; set; }

    // ════════════════════════════════════════════════════════════
    // Commercial
    // ════════════════════════════════════════════════════════════

    [Column(TypeName = "decimal(18,3)")]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Le plafond de crédit ne peut pas être négatif.")]
    public decimal PlafondCredit { get; set; }

    public ModePaiement? ModePaiement { get; set; }

    [MaxLength(5)]
    public string Devise { get; set; } = "TND";

    /// <summary>Remise par défaut appliquée au client (%).</summary>
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 100, ErrorMessage = "La remise doit être entre 0 et 100 %.")]
    public decimal RemiseParDefaut { get; set; }

    // ════════════════════════════════════════════════════════════
    // Notes
    // ════════════════════════════════════════════════════════════

    [MaxLength(2000)]
    public string? Notes { get; set; }

    // ════════════════════════════════════════════════════════════
    // Audit
    // ════════════════════════════════════════════════════════════

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateModification { get; set; }
}
