using FacturationTN.Models;

namespace FacturationTN.Services.Interfaces;

public interface IFactureService
{
    // ── CRUD ──
    Task<Facture?> GetByIdAsync(int id);
    Task<Facture> CreateAsync(Facture facture);
    Task<Facture> UpdateAsync(Facture facture);
    Task DeleteAsync(int id);

    // ── Recherche & filtrage ──
    Task<(List<Facture> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int? clientId = null,
        int page = 1,
        int pageSize = 20);

    // ── Numérotation ──
    /// <summary>Génère le prochain numéro de facture (FA-YYYY-NNNN).</summary>
    Task<string> GenererNumeroAsync();

    // ── Statut ──
    Task ValiderAsync(int id);
    Task AnnulerAsync(int id);
    Task MarquerPayeeAsync(int id);
}
