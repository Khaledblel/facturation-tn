using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturationTN.Models;

/// <summary>
/// Paramètres généraux de l'application (singleton — une seule ligne en DB).
/// Regroupe le timbre fiscal et les informations de l'entreprise.
/// </summary>
public class Parametres
{
    public int Id { get; set; }

    // ════════════════════════════════════════════════════════════
    // Timbre fiscal
    // ════════════════════════════════════════════════════════════

    [Required]
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Le montant du timbre fiscal ne peut pas être négatif.")]
    public decimal MontantTimbreFiscal { get; set; } = 1.000m;

    // ════════════════════════════════════════════════════════════
    // Informations de l'entreprise
    // ════════════════════════════════════════════════════════════

    [Required(ErrorMessage = "La raison sociale est obligatoire.")]
    [MaxLength(200)]
    public string RaisonSociale { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le matricule fiscale est obligatoire.")]
    [MaxLength(30)]
    [RegularExpression(@"^\d{7}/[A-Z]/[A-Z]/[A-Z]\d{3}$",
        ErrorMessage = "Format attendu : 0000000/X/X/X000")]
    public string MatriculeFiscale { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est obligatoire.")]
    [MaxLength(400)]
    public string Adresse { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
    public string? Telephone { get; set; }

    [MaxLength(150)]
    [EmailAddress(ErrorMessage = "Adresse email invalide.")]
    public string? Email { get; set; }

    // ════════════════════════════════════════════════════════════
    // Logo
    // ════════════════════════════════════════════════════════════

    /// <summary>Logo stocké en base64 (PNG, JPG ou SVG). Max ~2 Mo.</summary>
    public string? LogoBase64 { get; set; }

    /// <summary>Type MIME du logo (image/png, image/jpeg, image/svg+xml).</summary>
    [MaxLength(30)]
    public string? LogoContentType { get; set; }
}
