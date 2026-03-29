using System.ComponentModel.DataAnnotations;
using FacturationTN.Data;
using FacturationTN.Models;
using FacturationTN.Models.Enums;
using FacturationTN.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacturationTN.Services;

public class ProduitService(AppDbContext dbContext) : IProduitService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<List<Produit>> GetAllAsync()
    {
        return await _dbContext.Produits
            .AsNoTracking()
            .Include(p => p.Categorie)
            .Include(p => p.UniteMesure)
            .OrderByDescending(p => p.DateCreation)
            .ToListAsync();
    }

    public async Task<Produit?> GetByIdAsync(int id)
    {
        return await _dbContext.Produits
            .AsNoTracking()
            .Include(p => p.Categorie)
            .Include(p => p.UniteMesure)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produit> CreateAsync(Produit produit)
    {
        try
        {
            _dbContext.Produits.Add(produit);
            await _dbContext.SaveChangesAsync();

            return produit;
        }
        catch (DbUpdateException ex) when (IsReferenceUniqueViolation(ex))
        {
            throw new ValidationException("Cette reference existe deja. Veuillez en utiliser une autre.");
        }
    }

    public async Task<Produit> UpdateAsync(Produit produit)
    {
        var existing = await _dbContext.Produits.FirstOrDefaultAsync(p => p.Id == produit.Id)
            ?? throw new KeyNotFoundException("Produit introuvable.");

        existing.TypeProduit = produit.TypeProduit;
        existing.Statut = produit.Statut;
        existing.CategorieProduitId = produit.CategorieProduitId;
        existing.Reference = produit.Reference;
        existing.Designation = produit.Designation;
        existing.Marque = produit.Marque;
        existing.UniteMesureId = produit.UniteMesureId;
        existing.PrixAchatHT = produit.PrixAchatHT;
        existing.PrixVenteHT = produit.PrixVenteHT;
        existing.TauxTva = produit.TauxTva;
        existing.RemiseMax = produit.RemiseMax;
        existing.SuiviStock = produit.SuiviStock;
        existing.QuantiteStock = produit.QuantiteStock;
        existing.SeuilAlerte = produit.SeuilAlerte;
        existing.Description = produit.Description;
        existing.Notes = produit.Notes;
        existing.DateModification = DateTime.UtcNow;

        try
        {
            await _dbContext.SaveChangesAsync();
            return existing;
        }
        catch (DbUpdateException ex) when (IsReferenceUniqueViolation(ex))
        {
            throw new ValidationException("Cette reference existe deja. Veuillez en utiliser une autre.");
        }
    }

    public async Task DeleteAsync(int id)
    {
        var produit = await _dbContext.Produits.FirstOrDefaultAsync(p => p.Id == id);
        if (produit is null)
        {
            return;
        }

        try
        {
            _dbContext.Produits.Remove(produit);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsDeleteConstraintViolation(ex))
        {
            throw new InvalidOperationException("Impossible de supprimer ce produit car il est lie a des factures.");
        }
    }

    public async Task<(List<Produit> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? type = null,
        string? categorie = null,
        string? statut = null,
        int? tauxTva = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbContext.Produits
            .AsNoTracking()
            .Include(p => p.Categorie)
            .Include(p => p.UniteMesure)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                p.Reference.Contains(term) ||
                p.Designation.Contains(term) ||
                (p.Marque != null && p.Marque.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<TypeProduit>(type, true, out var typeValue))
        {
            query = query.Where(p => p.TypeProduit == typeValue);
        }

        if (!string.IsNullOrWhiteSpace(categorie))
        {
            query = query.Where(p => p.Categorie != null && p.Categorie.Nom == categorie);
        }

        if (!string.IsNullOrWhiteSpace(statut))
        {
            var normalizedStatut = statut.Trim().Replace("é", "e", StringComparison.OrdinalIgnoreCase);
            if (Enum.TryParse<StatutProduit>(normalizedStatut, true, out var statutValue))
            {
                query = query.Where(p => p.Statut == statutValue);
            }
        }

        if (tauxTva.HasValue)
        {
            query = query.Where(p => p.TauxTva == tauxTva.Value);
        }

        var total = await query.CountAsync();

        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Max(pageSize, 1);
        var skip = (normalizedPage - 1) * normalizedPageSize;

        var items = await query
            .OrderByDescending(p => p.DateCreation)
            .Skip(skip)
            .Take(normalizedPageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<CategorieProduit>> GetCategoriesAsync()
    {
        return await _dbContext.CategoriesProduit
            .AsNoTracking()
            .OrderBy(c => c.Nom)
            .ToListAsync();
    }

    public async Task<CategorieProduit> CreateCategorieAsync(string nom)
    {
        var normalizedName = nom.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ValidationException("Le nom de la categorie est obligatoire.");
        }

        if (normalizedName.Length > 30)
        {
            throw new ValidationException("Le nom de la categorie ne doit pas dépasser 30 caractères.");
        }

        var existing = await _dbContext.CategoriesProduit
            .FirstOrDefaultAsync(c => c.Nom.ToLower() == normalizedName.ToLower());

        if (existing is not null)
        {
            return existing;
        }

        var categorie = new CategorieProduit { Nom = normalizedName };
        _dbContext.CategoriesProduit.Add(categorie);
        await _dbContext.SaveChangesAsync();

        return categorie;
    }

    public async Task<List<UniteMesure>> GetUnitesAsync()
    {
        return await _dbContext.UnitesMesure
            .AsNoTracking()
            .OrderBy(u => u.Nom)
            .ToListAsync();
    }

    public async Task<UniteMesure> CreateUniteAsync(string nom)
    {
        var normalizedName = nom.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ValidationException("Le nom de l'unite est obligatoire.");
        }

        if (normalizedName.Length > 20)
        {
            throw new ValidationException("Le nom de l'unite ne doit pas dépasser 20 caractères.");
        }

        var existing = await _dbContext.UnitesMesure
            .FirstOrDefaultAsync(u => u.Nom.ToLower() == normalizedName.ToLower());

        if (existing is not null)
        {
            return existing;
        }

        var unite = new UniteMesure { Nom = normalizedName };
        _dbContext.UnitesMesure.Add(unite);
        await _dbContext.SaveChangesAsync();

        return unite;
    }

    private static bool IsReferenceUniqueViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase)
               && message.Contains("Produits.Reference", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDeleteConstraintViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase)
               || message.Contains("constraint", StringComparison.OrdinalIgnoreCase)
               && message.Contains("Produit", StringComparison.OrdinalIgnoreCase);
    }
}