using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using GunksAlert.Models;
using GunksAlert.Data;
using Microsoft.EntityFrameworkCore;

namespace GunksAlert.Controllers;

public class CragController : Controller {
    private readonly GunksDbContext _context;
    private readonly ILogger<CragController> _logger;

    public CragController(GunksDbContext context, ILogger<CragController> logger) {
        _context = context;
        _logger = logger;
    }

    [Route("/crag/list", Name = "Crag List")]
    public async Task<IActionResult> List() {        
        return View(await _context.Crags.ToListAsync());
    }
}
