using System.ComponentModel.DataAnnotations;
using FacturationTN.Data;
using FacturationTN.Models;
using FacturationTN.Models.Enums;
using FacturationTN.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacturationTN.Services;

public class ClientService(AppDbContext dbContext) : IClientService
{
    private readonly AppDbContext _dbContext = dbContext;
    private static readonly HashSet<string> DefaultCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "VIP",
        "PME",
        "Particulier",
        "Export"
    };

    public async Task<List<Client>> GetAllAsync()
    {
        return await _dbContext.Clients
            .AsNoTracking()
            .Include(c => c.Categorie)
            .OrderByDescending(c => c.DateCreation)
            .ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _dbContext.Clients
            .AsNoTracking()
            .Include(c => c.Categorie)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client> CreateAsync(Client client)
    {
        try
        {
            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();

            return client;
        }
        catch (DbUpdateException ex) when (IsMatriculeUniqueViolation(ex))
        {
            throw new ValidationException("Ce matricule fiscale existe deja. Veuillez en utiliser un autre.");
        }
    }

    public async Task<Client> UpdateAsync(Client client)
    {
        var existing = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == client.Id)
            ?? throw new KeyNotFoundException("Client introuvable.");

        existing.TypeClient = client.TypeClient;
        existing.Statut = client.Statut;
        existing.CategorieClientId = client.CategorieClientId;
        existing.Nom = client.Nom;
        existing.MatriculeFiscale = client.MatriculeFiscale;
        existing.NumeroRne = client.NumeroRne;
        existing.FormeJuridique = client.FormeJuridique;
        existing.Adresse = client.Adresse;
        existing.CodePostal = client.CodePostal;
        existing.Ville = client.Ville;
        existing.Gouvernorat = client.Gouvernorat;
        existing.Pays = client.Pays;
        existing.Telephone = client.Telephone;
        existing.Telephone2 = client.Telephone2;
        existing.Fax = client.Fax;
        existing.Email = client.Email;
        existing.SiteWeb = client.SiteWeb;
        existing.NomContact = client.NomContact;
        existing.PosteContact = client.PosteContact;
        existing.PlafondCredit = client.PlafondCredit;
        existing.ModePaiement = client.ModePaiement;
        existing.Devise = client.Devise;
        existing.RemiseParDefaut = client.RemiseParDefaut;
        existing.Notes = client.Notes;
        existing.DateModification = DateTime.UtcNow;

        try
        {
            await _dbContext.SaveChangesAsync();
            return existing;
        }
        catch (DbUpdateException ex) when (IsMatriculeUniqueViolation(ex))
        {
            throw new ValidationException("Ce matricule fiscale existe deja. Veuillez en utiliser un autre.");
        }
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
        if (client is null)
        {
            return;
        }

        try
        {
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsDeleteConstraintViolation(ex))
        {
            throw new InvalidOperationException("Impossible de supprimer ce client car il est lie a des factures.");
        }
    }

    public async Task<(List<Client> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? categorie = null,
        string? statut = null,
        string? gouvernorat = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbContext.Clients
            .AsNoTracking()
            .Include(c => c.Categorie)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(c =>
                c.Nom.Contains(term) ||
                c.MatriculeFiscale.Contains(term) ||
                (c.Email != null && c.Email.Contains(term)) ||
                (c.Telephone != null && c.Telephone.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(categorie))
        {
            query = query.Where(c => c.Categorie != null && c.Categorie.Nom == categorie);
        }

        if (!string.IsNullOrWhiteSpace(statut) && Enum.TryParse<StatutClient>(statut, true, out var statutValue))
        {
            query = query.Where(c => c.Statut == statutValue);
        }

        if (!string.IsNullOrWhiteSpace(gouvernorat))
        {
            query = query.Where(c => c.Gouvernorat == gouvernorat);
        }

        var total = await query.CountAsync();

        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Max(pageSize, 1);
        var skip = (normalizedPage - 1) * normalizedPageSize;

        var items = await query
            .OrderByDescending(c => c.DateCreation)
            .Skip(skip)
            .Take(normalizedPageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<CategorieClient>> GetCategoriesAsync()
    {
        return await _dbContext.CategoriesClient
            .AsNoTracking()
            .OrderBy(c => c.Nom)
            .ToListAsync();
    }

    public async Task<CategorieClient> CreateCategorieAsync(string nom)
    {
        var normalizedName = nom.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ValidationException("Le nom de la categorie est obligatoire.");
        }

        var existing = await _dbContext.CategoriesClient
            .FirstOrDefaultAsync(c => c.Nom.ToLower() == normalizedName.ToLower());

        if (existing is not null)
        {
            return existing;
        }

        var categorie = new CategorieClient { Nom = normalizedName };
        _dbContext.CategoriesClient.Add(categorie);
        await _dbContext.SaveChangesAsync();

        return categorie;
    }

    public async Task DeleteCategorieAsync(int id)
    {
        var categorie = await _dbContext.CategoriesClient
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new KeyNotFoundException("Categorie introuvable.");

        if (DefaultCategories.Contains(categorie.Nom))
        {
            throw new ValidationException("Les categories par defaut ne peuvent pas etre supprimees.");
        }

        var isUsed = await _dbContext.Clients
            .AsNoTracking()
            .AnyAsync(c => c.CategorieClientId == id);

        if (isUsed)
        {
            throw new InvalidOperationException("Impossible de supprimer cette categorie car elle est utilisee par au moins un client.");
        }

        _dbContext.CategoriesClient.Remove(new CategorieClient { Id = id });
        await _dbContext.SaveChangesAsync();
    }

    private static bool IsMatriculeUniqueViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase)
               && message.Contains("Clients.MatriculeFiscale", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDeleteConstraintViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase)
               || message.Contains("constraint", StringComparison.OrdinalIgnoreCase)
               && message.Contains("Client", StringComparison.OrdinalIgnoreCase);
    }
}
