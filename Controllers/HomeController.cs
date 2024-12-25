using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

using GunksAlert.Models;

namespace GunksAlert.Controllers;

public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger) {
        _logger = logger;
    }

    [Route("/", Name = "Home")]
    public IActionResult Index() {
        return View();
    }

    [Route("/foo/{name}", Name = "Foo")]
    public IActionResult Foo(string name) {
        ViewData["name"] = name;

        return View();
    }

    [Route("/privacy", Name = "Privacy")]
    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
