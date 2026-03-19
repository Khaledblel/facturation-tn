using System.ComponentModel.DataAnnotations;

namespace FacturationTN.Models;

/// <summary>
/// Catégorie produit créée par l'utilisateur (Alimentaire, Électronique, Textile…).
/// Table de référence dynamique.
/// </summary>
public class CategorieProduit
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom de la catégorie est obligatoire.")]
    [MaxLength(30)]
    public string Nom { get; set; } = string.Empty;

    // ── Navigation ──
    public ICollection<Produit> Produits { get; set; } = [];
}
