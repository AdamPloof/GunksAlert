using Microsoft.AspNetCore.Identity;

namespace GunksAlert.Api.Security;

/// <summary>
/// Provides authentication using good old fashioned username and password
/// </summary>
public class PasswordAuthenticationProvider : IAuthenticationProvider {
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public PasswordAuthenticationProvider(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager
    ) {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<SignInResult> LoginAsync(string userIdentifier, string password) {
        return await _signInManager.PasswordSignInAsync(userIdentifier, password, false, false);
    }

    public async Task LogoutAsync() {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> RegisterAsync(string userIdentifier, string password) {
        var user = new AppUser() {UserName=userIdentifier};
        IdentityResult res = await _userManager.CreateAsync(user, password);
        if (res.Succeeded) {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return res;
    }
}
