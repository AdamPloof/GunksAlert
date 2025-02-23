using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

using GunksAlert.Api.Security;
using GunksAlert.Api.ViewModels;

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
    public async Task<IActionResult> Login(PasswordLoginViewModel model) {
        if (!ModelState.IsValid) {
            return View(model);
        }

        SignInResult result = await _authProvider.LoginAsync(model.Username, model.Password);
        if (result.Succeeded) {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt");

        return View(model);
    }

    [HttpGet("register", Name = "register")]
    [AllowAnonymous]
    public IActionResult Register() {
        return View();
    }

    [HttpPost("register", Name = "register")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(PasswordRegisterViewModel model) {
        if (!ModelState.IsValid) {
            return View(model);
        }

        IdentityResult result = await _authProvider.RegisterAsync(model.Username, model.Password);
        if (result.Succeeded) {
            return RedirectToAction("Index", "Home");
        }

        foreach (IdentityError err in result.Errors) {
            ModelState.AddModelError("", err.Description);
        }

        return View(model);
    }

    [HttpPost("logout", Name = "logout")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Logout() {
        await _authProvider.LogoutAsync();

        return RedirectToAction("Index", "Home");
    }
}
