using System;
using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Models;

/// <summary>
/// Represents the weather conditions that are considered climbable.
/// </summary>
public class ClimbableConditions {
    [Key]
    public int Id { get; private set; }

    [Required]
    public DateTimeOffset Date { get; set; }

    [StringLength(200)]
    public required string Summary { get; set; }

    [Required]
    public int TempMin { get; set; }

    [Required]
    public int TempMax { get; set; }

    [Required]
    public int WindSpeed { get; set; }

    [Required]
    public int WindGust { get; set; }

    [Required]
    [Range(0, 360)]
    public int WindDegree { get; set; }

    [Required]
    [Range(0, 100)]
    public int Clouds { get; set; }

    [Required]
    [Range(0, 100)]
    public int Humidity { get; set; }

    [Required]
    [Range(0, 100)]
    public int Pop { get; set; }

    public double Rain { get; set; }

    public double Snow { get; set ;}
}
