using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GunksAlert.Models;

/// <summary>
/// Represents the weather for a single day in the past.
/// </summary>
public class WeatherHistory {
    [Key]
    public int Id { get; private set; }

    [Required]
    public DateTimeOffset Date { get; set; }

    [Required]
    public int Temp { get; set; }

    [Required]
    public int WindSpeed { get; set; }

    [Required]
    [Range(0, 360)]
    public int WindDegree { get; set; }

    [Required]
    [Range(0, 100)]
    public int Clouds { get; set; }

    [Required]
    [Range(0, 100)]
    public int Humidity { get; set; }

    public double Rain { get; set; }

    public double Snow { get; set ;}

    [ForeignKey("DailyCondition")]
    public int DailyConditionId { get; set; }

    public required DailyCondition DailyCondition { get; set; }
}
