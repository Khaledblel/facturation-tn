using FacturationTN.Models;

namespace FacturationTN.Services.Interfaces;

public interface IUserAccountService
{
    Task<UserAccount?> GetByIdAsync(int id);
    Task<UserAccount> UpdateProfileAsync(
        int id,
        string userName,
        string? fullName,
        string? email,
        string? phone,
        string role,
        string? profileImageDataUrl);
}
