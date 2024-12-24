using System;
using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Models;

/// <summary>
/// Represents the date range for which alerts should be monitored and sent.
/// </summary>
public class AlertPeriod {
    [Key]
    public int Id { get; private set; }

    [Required]
    public DateTimeOffset StartDate { get; set; }

    [Required]
    public DateTimeOffset EndDate { get; set; }
}
