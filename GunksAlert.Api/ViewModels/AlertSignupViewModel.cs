using System.ComponentModel.DataAnnotations;
using GunksAlert.Api.Models;

namespace GunksAlert.Api.ViewModels;

public class AlertSignupViewModel {
    // [Required]
    // public required Crag Crag { get; set; }

    // [Required]
    // public required ClimbableConditions Conditions { get; set; }

    public bool Sunday { get; set; } = false;
    public bool Monday { get; set; } = false;
    public bool Tuesday { get; set; } = false;
    public bool Wednesday { get; set; } = false;
    public bool Thursday { get; set; } = false;
    public bool Friday { get; set; } = false;
    public bool Saturday { get; set; } = false;

    public bool January { get; set; } = false;
    public bool February { get; set; } = false;
    public bool March { get; set; } = false;
    public bool April { get; set; } = false;
    public bool May { get; set; } = false;
    public bool June { get; set; } = false;
    public bool July { get; set; } = false;
    public bool August { get; set; } = false;
    public bool September { get; set; } = false;
    public bool October { get; set; } = false;
    public bool November { get; set; } = false;
    public bool December { get; set; } = false;
}
