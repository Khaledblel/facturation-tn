using System.ComponentModel.DataAnnotations;
using FacturationTN.Data;
using FacturationTN.Models;
using FacturationTN.Models.Enums;
using FacturationTN.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacturationTN.Services;

public class FactureService(AppDbContext dbContext) : IFactureService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Facture?> GetByIdAsync(int id)
    {
        return await _dbContext.Factures
            .AsNoTracking()
            .Include(f => f.Client)
            .Include(f => f.Lignes)
            .ThenInclude(l => l.Produit)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Facture> CreateAsync(Facture facture)
    {
        if (facture.Lignes is null || facture.Lignes.Count == 0)
        {
            throw new ValidationException("Une facture doit contenir au moins une ligne.");
        }

        var clientExists = await _dbContext.Clients.AnyAsync(c => c.Id == facture.ClientId);
        if (!clientExists)
        {
            throw new ValidationException("Client introuvable.");
        }

        var entity = new Facture
        {
            NumeroFacture = facture.NumeroFacture,
            DateFacture = facture.DateFacture,
            Statut = facture.Statut,
            ClientId = facture.ClientId,
            TimbreFiscal = facture.TimbreFiscal,
            DateCreation = DateTime.UtcNow,
            DateModification = null
        };

        foreach (var ligne in facture.Lignes)
        {
            var produitOk = await _dbContext.Produits.AnyAsync(p => p.Id == ligne.ProduitId);
            if (!produitOk)
            {
                throw new ValidationException($"Produit introuvable (Id {ligne.ProduitId}).");
            }

            entity.Lignes.Add(new LigneFacture
            {
                ProduitId = ligne.ProduitId,
                Designation = ligne.Designation,
                Quantite = ligne.Quantite,
                PrixUnitaireHT = ligne.PrixUnitaireHT,
                TauxTva = ligne.TauxTva
            });
        }

        try
        {
            _dbContext.Factures.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        catch (DbUpdateException ex) when (IsNumeroFactureUniqueViolation(ex))
        {
            throw new ValidationException("Ce numero de facture existe deja.");
        }
    }

    public async Task<Facture> UpdateAsync(Facture facture)
    {
        var existing = await _dbContext.Factures
            .Include(f => f.Lignes)
            .FirstOrDefaultAsync(f => f.Id == facture.Id)
            ?? throw new KeyNotFoundException("Facture introuvable.");

        if (existing.Statut == StatutFacture.Payee || existing.Statut == StatutFacture.Annulee)
        {
            throw new InvalidOperationException("Impossible de modifier une facture payee ou annulee.");
        }

        var clientExists = await _dbContext.Clients.AnyAsync(c => c.Id == facture.ClientId);
        if (!clientExists)
        {
            throw new ValidationException("Client introuvable.");
        }

        if (facture.Lignes is null || facture.Lignes.Count == 0)
        {
            throw new ValidationException("Une facture doit contenir au moins une ligne.");
        }

        existing.NumeroFacture = facture.NumeroFacture;
        existing.DateFacture = facture.DateFacture;
        existing.Statut = facture.Statut;
        existing.ClientId = facture.ClientId;
        existing.TimbreFiscal = facture.TimbreFiscal;
        existing.DateModification = DateTime.UtcNow;

        var oldLignes = existing.Lignes.ToList();
        _dbContext.LignesFacture.RemoveRange(oldLignes);

        foreach (var ligne in facture.Lignes)
        {
            var produitOk = await _dbContext.Produits.AnyAsync(p => p.Id == ligne.ProduitId);
            if (!produitOk)
            {
                throw new ValidationException($"Produit introuvable (Id {ligne.ProduitId}).");
            }

            _dbContext.LignesFacture.Add(new LigneFacture
            {
                FactureId = existing.Id,
                ProduitId = ligne.ProduitId,
                Designation = ligne.Designation,
                Quantite = ligne.Quantite,
                PrixUnitaireHT = ligne.PrixUnitaireHT,
                TauxTva = ligne.TauxTva
            });
        }

        try
        {
            await _dbContext.SaveChangesAsync();
            return existing;
        }
        catch (DbUpdateException ex) when (IsNumeroFactureUniqueViolation(ex))
        {
            throw new ValidationException("Ce numero de facture existe deja.");
        }
    }

    public async Task DeleteAsync(int id)
    {
        var facture = await _dbContext.Factures.FirstOrDefaultAsync(f => f.Id == id);
        if (facture is null)
        {
            return;
        }

        if (facture.Statut == StatutFacture.Validee || facture.Statut == StatutFacture.Payee)
        {
            throw new InvalidOperationException("Impossible de supprimer une facture validee ou payee.");
        }

        _dbContext.Factures.Remove(facture);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<(List<Facture> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int? clientId = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbContext.Factures
            .AsNoTracking()
            .Include(f => f.Client)
            .Include(f => f.Lignes)
            .ThenInclude(l => l.Produit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(f =>
                f.NumeroFacture.Contains(term) ||
                (f.Client != null && f.Client.Nom.Contains(term)));
        }

        if (dateFrom.HasValue)
        {
            var from = dateFrom.Value.Date;
            query = query.Where(f => f.DateFacture >= from);
        }

        if (dateTo.HasValue)
        {
            var to = dateTo.Value.Date.AddDays(1);
            query = query.Where(f => f.DateFacture < to);
        }

        if (clientId is int cid && cid > 0)
        {
            query = query.Where(f => f.ClientId == cid);
        }

        var total = await query.CountAsync();

        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Max(pageSize, 1);
        var skip = (normalizedPage - 1) * normalizedPageSize;

        var items = await query
            .OrderByDescending(f => f.DateFacture)
            .ThenByDescending(f => f.Id)
            .Skip(skip)
            .Take(normalizedPageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<string> GenererNumeroAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"FA-{year}-";

        var numeros = await _dbContext.Factures
            .Where(f => f.NumeroFacture.StartsWith(prefix))
            .Select(f => f.NumeroFacture)
            .ToListAsync();

        var max = 0;
        foreach (var numero in numeros)
        {
            if (numero.Length <= prefix.Length)
            {
                continue;
            }

            var suffix = numero[prefix.Length..];
            if (int.TryParse(suffix, out var seq))
            {
                max = Math.Max(max, seq);
            }
        }

        return $"{prefix}{(max + 1):D4}";
    }

    public async Task ValiderAsync(int id)
    {
        var facture = await _dbContext.Factures.FirstOrDefaultAsync(f => f.Id == id)
            ?? throw new KeyNotFoundException("Facture introuvable.");

        if (facture.Statut != StatutFacture.Brouillon)
        {
            throw new InvalidOperationException("Seules les factures en brouillon peuvent etre validees.");
        }

        facture.Statut = StatutFacture.Validee;
        facture.DateModification = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task AnnulerAsync(int id)
    {
        var facture = await _dbContext.Factures.FirstOrDefaultAsync(f => f.Id == id)
            ?? throw new KeyNotFoundException("Facture introuvable.");

        if (facture.Statut == StatutFacture.Payee)
        {
            throw new InvalidOperationException("Impossible d'annuler une facture deja payee.");
        }

        facture.Statut = StatutFacture.Annulee;
        facture.DateModification = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task MarquerPayeeAsync(int id)
    {
        var facture = await _dbContext.Factures.FirstOrDefaultAsync(f => f.Id == id)
            ?? throw new KeyNotFoundException("Facture introuvable.");

        if (facture.Statut != StatutFacture.Validee)
        {
            throw new InvalidOperationException("Seules les factures validees peuvent etre marquees comme payees.");
        }

        facture.Statut = StatutFacture.Payee;
        facture.DateModification = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    private static bool IsNumeroFactureUniqueViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase)
               && message.Contains("Factures.NumeroFacture", StringComparison.OrdinalIgnoreCase);
    }
}
