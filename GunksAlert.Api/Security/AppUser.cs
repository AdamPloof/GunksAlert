using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using GunksAlert.Api.Models;

namespace GunksAlert.Api.Security;

/// <summary>
/// The User class for the app
/// </summary>
public class AppUser : IdentityUser {
    private List<AlertCriteria> _criterias = new List<AlertCriteria>();
    
    public IReadOnlyCollection<AlertCriteria> Criterias {
        get => _criterias.AsReadOnly();
        private set {
            _criterias = value?.ToList() ?? new List<AlertCriteria>();
        }
    }

    public void AddCriteria(AlertCriteria criteria) {
        if (!_criterias.Contains(criteria)) {
            _criterias.Add(criteria);
        }
    }

    public void RemoveCriteria(AlertCriteria criteria) {
        _criterias.Remove(criteria);
    }
}
