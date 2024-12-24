using System;
using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Models;

/// <summary>
/// The location of the crag to send alerts about. Hold the data needed to fetch 
/// localized weather forecasts and history.
/// </summary>
public class Crag {
    [Key]
    public int Id { get; private set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public required string City { get; set; }

    [Required]
    public required string StateProvince { get; set; }

    [Required]
    public required string Country { get; set; }
}
