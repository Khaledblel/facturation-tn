using System.ComponentModel.DataAnnotations;

namespace FacturationTN.Models;

/// <summary>
/// Taux de TVA en vigueur (table de référence en lecture seule).
/// Pré-remplie par seed : 0 %, 7 %, 13 %, 19 %.
/// </summary>
public class TauxTva
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Libelle { get; set; } = string.Empty;

    /// <summary>Valeur du taux (ex : 0, 7, 13, 19).</summary>
    [Required]
    [Range(0, 100)]
    public int Taux { get; set; }

    public bool Actif { get; set; } = true;
}
