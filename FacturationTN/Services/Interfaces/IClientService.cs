using FacturationTN.Models;

namespace FacturationTN.Services.Interfaces;

public interface IClientService
{
    // ── CRUD ──
    Task<List<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
    Task<Client> CreateAsync(Client client);
    Task<Client> UpdateAsync(Client client);
    Task DeleteAsync(int id);

    // ── Recherche & filtrage ──
    Task<(List<Client> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? categorie = null,
        string? statut = null,
        string? gouvernorat = null,
        int page = 1,
        int pageSize = 20);

    // ── Catégories dynamiques ──
    Task<List<CategorieClient>> GetCategoriesAsync();
    Task<CategorieClient> CreateCategorieAsync(string nom);
    Task DeleteCategorieAsync(int id);
}
