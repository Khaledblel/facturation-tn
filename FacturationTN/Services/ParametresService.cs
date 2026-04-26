using FacturationTN.Data;
using FacturationTN.Models;
using FacturationTN.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacturationTN.Services;

/// <summary>
/// Accès aux paramètres globaux (ligne singleton en base, Id = 1 après seed).
/// </summary>
public class ParametresService : IParametresService
{
    private readonly AppDbContext _dbContext;

    public ParametresService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Parametres> GetAsync()
    {
        var row = await _dbContext.Parametres.AsNoTracking().FirstOrDefaultAsync();
        if (row is not null)
        {
            return row;
        }

        // Sécurité si la base est vide (pas de seed)
        var created = new Parametres
        {
            MontantTimbreFiscal = 1.000m,
            RaisonSociale = string.Empty,
            MatriculeFiscale = string.Empty,
            Adresse = string.Empty
        };
        _dbContext.Parametres.Add(created);
        await _dbContext.SaveChangesAsync();
        return created;
    }

    public async Task<Parametres> UpdateAsync(Parametres parametres)
    {
        var existing = await _dbContext.Parametres.FirstOrDefaultAsync(p => p.Id == parametres.Id)
                       ?? await _dbContext.Parametres.FirstOrDefaultAsync()
                       ?? throw new InvalidOperationException("Aucune ligne Parametres en base.");

        existing.MontantTimbreFiscal = parametres.MontantTimbreFiscal;
        existing.RaisonSociale = parametres.RaisonSociale;
        existing.MatriculeFiscale = parametres.MatriculeFiscale;
        existing.Adresse = parametres.Adresse;
        existing.Telephone = parametres.Telephone;
        existing.Email = parametres.Email;
        existing.LogoBase64 = parametres.LogoBase64;
        existing.LogoContentType = parametres.LogoContentType;

        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<decimal> GetTimbreFiscalAsync()
    {
        var p = await GetAsync();
        return p.MontantTimbreFiscal;
    }

    public async Task UpdateTimbreFiscalAsync(decimal montant)
    {
        if (montant < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(montant), "Le timbre fiscal ne peut pas être négatif.");
        }

        var existing = await _dbContext.Parametres.FirstOrDefaultAsync()
                       ?? throw new InvalidOperationException("Aucune ligne Parametres en base.");

        existing.MontantTimbreFiscal = montant;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLogoAsync(string base64, string contentType)
    {
        var existing = await _dbContext.Parametres.FirstOrDefaultAsync()
                       ?? throw new InvalidOperationException("Aucune ligne Parametres en base.");

        existing.LogoBase64 = base64;
        existing.LogoContentType = contentType;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveLogoAsync()
    {
        var existing = await _dbContext.Parametres.FirstOrDefaultAsync()
                       ?? throw new InvalidOperationException("Aucune ligne Parametres en base.");

        existing.LogoBase64 = null;
        existing.LogoContentType = null;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<TauxTva>> GetTauxTvaAsync()
    {
        return await _dbContext.TauxTva
            .AsNoTracking()
            .Where(t => t.Actif)
            .OrderBy(t => t.Taux)
            .ToListAsync();
    }
}
