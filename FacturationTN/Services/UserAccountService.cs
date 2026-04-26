using System.ComponentModel.DataAnnotations;
using FacturationTN.Data;
using FacturationTN.Models;
using FacturationTN.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacturationTN.Services;

public class UserAccountService : IUserAccountService
{
    private readonly AppDbContext _dbContext;
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin",
        "User",
        "Manager",
        "Accountant",
        "Sales",
        "Supervisor",
        "Auditor",
        "Finance",
        "Support",
        "Viewer"
    };

    public UserAccountService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserAccount?> GetByIdAsync(int id)
    {
        return await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<UserAccount> UpdateProfileAsync(
        int id,
        string userName,
        string? fullName,
        string? email,
        string? phone,
        string role,
        string? profileImageDataUrl)
    {
        if (string.IsNullOrWhiteSpace(userName) || userName.Trim().Length < 3)
        {
            throw new ValidationException("Le nom d'utilisateur doit contenir au moins 3 caracteres.");
        }

        var user = await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Id == id && u.IsActive)
                   ?? throw new KeyNotFoundException("Utilisateur introuvable.");

        var normalizedUserName = userName.Trim().ToUpperInvariant();
        var exists = await _dbContext.UserAccounts.AnyAsync(u => u.Id != id && u.NormalizedUserName == normalizedUserName);
        if (exists)
        {
            throw new ValidationException("Ce nom d'utilisateur existe deja.");
        }

        var normalizedRole = string.IsNullOrWhiteSpace(role) ? string.Empty : role.Trim();
        if (!AllowedRoles.Contains(normalizedRole))
        {
            throw new ValidationException("Role invalide.");
        }

        user.UserName = userName.Trim();
        user.NormalizedUserName = normalizedUserName;
        user.FullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName.Trim();
        user.Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        user.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        user.Role = normalizedRole;
        user.ProfileImageDataUrl = string.IsNullOrWhiteSpace(profileImageDataUrl) ? null : profileImageDataUrl;

        await _dbContext.SaveChangesAsync();
        return user;
    }
}
