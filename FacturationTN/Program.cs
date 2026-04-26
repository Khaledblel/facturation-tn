using FacturationTN.Components;
using FacturationTN.Data;
using FacturationTN.Models;
using FacturationTN.Services;
using FacturationTN.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// ── Entity Framework Core (SQLite) ──
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<IFactureService, FactureService>();
builder.Services.AddScoped<IParametresService, ParametresService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();

var app = builder.Build();

// ── Auto-create / migrate database on startup ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!await db.UserAccounts.AnyAsync())
    {
        var admin = new UserAccount
        {
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Role = "Admin",
            FullName = "Administrateur",
            IsActive = true,
            DateCreation = DateTime.UtcNow
        };

        var hasher = new PasswordHasher<UserAccount>();
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

        db.UserAccounts.Add(admin);
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/login", async (HttpContext httpContext, AppDbContext dbContext) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var username = form["username"].ToString().Trim();
    var password = form["password"].ToString();
    var returnUrl = form["returnUrl"].ToString();

    if (string.IsNullOrWhiteSpace(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
    {
        returnUrl = "/";
    }

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        return Results.LocalRedirect($"/login?error={Uri.EscapeDataString("Nom d'utilisateur ou mot de passe manquant.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var normalizedUserName = username.ToUpperInvariant();
    var user = await dbContext.UserAccounts.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName && u.IsActive);

    if (user is null)
    {
        return Results.LocalRedirect($"/login?error={Uri.EscapeDataString("Identifiants invalides.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var hasher = new PasswordHasher<UserAccount>();
    var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
    if (verification == PasswordVerificationResult.Failed)
    {
        return Results.LocalRedirect($"/login?error={Uri.EscapeDataString("Identifiants invalides.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Role, user.Role)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.LocalRedirect(returnUrl);
}).DisableAntiforgery();

app.MapPost("/auth/register", async (HttpContext httpContext, AppDbContext dbContext) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var username = form["username"].ToString().Trim();
    var password = form["password"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();

    if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
    {
        return Results.LocalRedirect($"/register?error={Uri.EscapeDataString("Le nom d'utilisateur doit contenir au moins 3 caracteres.")}");
    }

    if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
    {
        return Results.LocalRedirect($"/register?error={Uri.EscapeDataString("Le mot de passe doit contenir au moins 8 caracteres.")}");
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        return Results.LocalRedirect($"/register?error={Uri.EscapeDataString("La confirmation du mot de passe est invalide.")}");
    }

    var normalizedUserName = username.ToUpperInvariant();
    var exists = await dbContext.UserAccounts.AnyAsync(u => u.NormalizedUserName == normalizedUserName);
    if (exists)
    {
        return Results.LocalRedirect($"/register?error={Uri.EscapeDataString("Ce nom d'utilisateur existe deja.")}");
    }

    var user = new UserAccount
    {
        UserName = username,
        NormalizedUserName = normalizedUserName,
        Role = "User",
        IsActive = true,
        DateCreation = DateTime.UtcNow
    };

    var hasher = new PasswordHasher<UserAccount>();
    user.PasswordHash = hasher.HashPassword(user, password);

    dbContext.UserAccounts.Add(user);
    await dbContext.SaveChangesAsync();

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Role, user.Role)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.LocalRedirect("/");
}).DisableAntiforgery();

app.MapPost("/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/login");
}).DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
