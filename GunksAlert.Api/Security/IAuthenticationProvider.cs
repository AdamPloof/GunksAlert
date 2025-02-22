using System;
using Microsoft.AspNetCore.Identity;

namespace GunksAlert.Api.Security;

/// <summary>
/// The interface that services managing authentication must implement 
/// </summary>
public interface IAuthenticationProvider {
    /// <summary>
    /// Login the user identified by identifier using the provided secret
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    public Task<SignInResult> LoginAsync(string identifier, string secret);
    
    /// <summary>
    /// Logout the current active user
    /// </summary>
    /// <returns></returns>
    public Task LogoutAsync();

    /// <summary>
    /// Register the user using the identifier and confirmed with the provided secret
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    public Task<IdentityResult> RegisterAsync(string identifier, string secret);
}
