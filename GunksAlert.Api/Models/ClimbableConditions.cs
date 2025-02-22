using System;
using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Api.Models;

/// <summary>
/// Represents the weather conditions that are considered climbable.
/// 
/// TODO:
///     - Rethink how to represent ideal conditions since good conditions is a
///       combination/calcualtion of multiple fields. For example, 40 degrees and very
///       windy with clouds not good. 40 degrees with full sun and no wind, pretty good.
/// </summary>
public class ClimbableConditions {
    [Key]
    public int Id { get; private set; }

    [StringLength(200)]
    public required string Summary { get; set; }

    [Required]
    public double TempMin { get; set; }

    [Required]
    public double TempMax { get; set; }

    [Required]
    public double WindSpeed { get; set; }

    [Required]
    public double WindGust { get; set; }

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
    public double Pop { get; set; }

    public double Rain { get; set; }

    public double Snow { get; set ;}
}
