using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using GunksAlert.Api.Models;

namespace GunksAlert.Api.Security;

/// <summary>
/// The User class for the app
/// </summary>
public class AppUser : IdentityUser {
    public List<AlertCriteria> Criterias { get; } = new List<AlertCriteria>();
}
