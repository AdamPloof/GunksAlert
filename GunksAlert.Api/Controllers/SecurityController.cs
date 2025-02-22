using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

using GunksAlert.Api.Security;

namespace GunksAlert.Api.Controllers;

public class SecurityController : Controller {
    private readonly IAuthenticationProvider _authProvider;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(
        IAuthenticationProvider authProvider,
        ILogger<SecurityController> logger
    ) {
        _authProvider = authProvider;
        _logger = logger;
    }

    [HttpGet("login", Name = "login")]
    [AllowAnonymous]
    public IActionResult Login() {        
        return View();
    }

    [HttpPost("login", Name = "login")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string identifier, string secret) {
        SignInResult result = await _authProvider.LoginAsync(identifier, secret);
        if (result.Succeeded) {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt");

        return View();
    }

    [HttpGet("register", Name = "register")]
    [AllowAnonymous]
    public IActionResult Register() {
        return View();
    }

    [HttpPost("register", Name = "register")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string identifier, string secret) {
        IdentityResult result = await _authProvider.RegisterAsync(identifier, secret);
        if (result.Succeeded) {
            return RedirectToAction("Index", "Home");
        }

        foreach (IdentityError err in result.Errors) {
            ModelState.AddModelError("", err.Description);
        }

        return View();
    }

    [HttpPost("logout", Name = "logout")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Logout() {
        await _authProvider.LogoutAsync();

        return RedirectToAction("Index", "Home");
    }
}
