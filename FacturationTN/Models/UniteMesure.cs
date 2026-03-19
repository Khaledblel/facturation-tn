using System.ComponentModel.DataAnnotations;

namespace FacturationTN.Models;

/// <summary>
/// Unité de mesure créée par l'utilisateur (Pièce, Kg, Litre, Heure…).
/// Table de référence dynamique.
/// </summary>
public class UniteMesure
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom de l'unité est obligatoire.")]
    [MaxLength(20)]
    public string Nom { get; set; } = string.Empty;

    // ── Navigation ──
    public ICollection<Produit> Produits { get; set; } = [];
}
