using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GunksAlert.Models;

/// <summary>
/// Stores data for a single day's weather forecast. Is used by comparing to `ClimbableConditions`
/// to determine if an alert should be sent.
/// </summary>
/// 
/// <remarks>
/// The <see cref="DailyConditionId"/> property reference a DailyCondition summary which is
/// a standarized short summary for the day's weather.
/// </remarks>
public class Forecast {
    [Key]
    public int Id { get; private set; }

    [Required]
    public DateTimeOffset Date { get; set; }

    [StringLength(200)]
    public required string Summary { get; set; }

    [Required]
    public int TempLow { get; set; }

    [Required]
    public int TempHigh { get; set; }

    [Required]
    public int TempFeelsLike { get; set; }

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

    [ForeignKey("DailyCondition")]
    public int DailyConditionId { get; set; }

    public required DailyCondition DailyCondition { get; set; }
}
