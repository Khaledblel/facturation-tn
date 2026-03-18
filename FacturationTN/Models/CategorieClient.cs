using System.ComponentModel.DataAnnotations;

namespace FacturationTN.Models;

/// <summary>
/// Catégorie client créée par l'utilisateur (VIP, PME, Particulier, Export…).
/// Table de référence dynamique.
/// </summary>
public class CategorieClient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom de la catégorie est obligatoire.")]
    [MaxLength(30)]
    public string Nom { get; set; } = string.Empty;

    // ── Navigation ──
    public ICollection<Client> Clients { get; set; } = [];
}
