using FacturationTN.Models;

namespace FacturationTN.Services.Interfaces;

public interface IParametresService
{
    // ── Paramètres généraux (singleton) ──
    Task<Parametres> GetAsync();
    Task<Parametres> UpdateAsync(Parametres parametres);

    // ── Timbre fiscal ──
    Task<decimal> GetTimbreFiscalAsync();
    Task UpdateTimbreFiscalAsync(decimal montant);

    // ── Logo ──
    Task UpdateLogoAsync(string base64, string contentType);
    Task RemoveLogoAsync();

    // ── Taux de TVA (lecture seule) ──
    Task<List<TauxTva>> GetTauxTvaAsync();
}
