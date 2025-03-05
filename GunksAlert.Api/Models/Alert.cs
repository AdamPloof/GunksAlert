using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using GunksAlert.Api.Security;
using GunksAlert.Api.Services.Attributes;

namespace GunksAlert.Api.Models;

/// <summary>
/// A record for an alert that has been sent
/// </summary>
public class Alert {
    [Key]
    public int Id { get; private set; }

    [ForeignKey("User")]
    [NonZero]
    public required string UserId { get; set; }

    [Required]
    public required AppUser User { get; set; }

    [Required]
    public DateOnly SentOn { get; set; }

    [Required]
    public DateOnly ForecastDate { get; set; }

    [ForeignKey("Crag")]
    [NonZero]
    [Required]
    public int CragId { get; set; }

    [Required]
    public bool Canceled { get; set; } = false;
}
