using System;
using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Models;

/// <summary>
/// Represents a standarized weather condition summary for a day including an icon code.
/// </summary>
/// 
/// <seealso href="https://openweathermap.org/weather-conditions" />
public class DailyCondition {
    [Key]
    public int Id { get; private set; }

    [StringLength(200)]
    [Required]
    public required string Main { get; set; }

    [StringLength(200)]
    [Required]
    public required string Description { get; set; }

    [StringLength(3)]
    [Required]
    public required string Icon { get; set; }
}
