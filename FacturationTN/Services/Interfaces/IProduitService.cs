using FacturationTN.Models;

namespace FacturationTN.Services.Interfaces;

public interface IProduitService
{
    // ── CRUD ──
    Task<List<Produit>> GetAllAsync();
    Task<Produit?> GetByIdAsync(int id);
    Task<Produit> CreateAsync(Produit produit);
    Task<Produit> UpdateAsync(Produit produit);
    Task DeleteAsync(int id);

    // ── Recherche & filtrage ──
    Task<(List<Produit> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? type = null,
        string? categorie = null,
        string? statut = null,
        int? tauxTva = null,
        int page = 1,
        int pageSize = 20);

    // ── Catégories dynamiques ──
    Task<List<CategorieProduit>> GetCategoriesAsync();
    Task<CategorieProduit> CreateCategorieAsync(string nom);

    // ── Unités de mesure dynamiques ──
    Task<List<UniteMesure>> GetUnitesAsync();
    Task<UniteMesure> CreateUniteAsync(string nom);
}
