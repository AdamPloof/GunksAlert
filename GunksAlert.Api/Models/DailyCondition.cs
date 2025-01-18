using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GunksAlert.Models;

/// <summary>
/// Represents a standarized weather condition summary for a day including an icon code.
/// </summary>
/// <remarks>
/// NOTE: It is possible to meet more than one weather condition for a requested location.
/// The first weather condition in API respond is primary.
/// </remarks>
/// <seealso href="https://openweathermap.org/weather-conditions" />
public class DailyCondition {
    private string _iconDay = "";
    private string _iconNight = "";

    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [StringLength(200)]
    [Required]
    [JsonPropertyName("main")]
    public string Main { get; set; } = "";

    [StringLength(200)]
    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [NotMapped]
    [JsonPropertyName("icon")]
    public string Icon {
        get => _iconDay;
        set {
            _iconDay = value[2] == 'n' ? value.Substring(0, 2) + "d" : value;
            _iconNight = value[2] == 'd' ? value.Substring(0, 2) + "n" : value;
        }
    }

    [StringLength(3)]
    [Required]
    [JsonIgnore]
    public string IconDay {
        get => _iconDay;
        set {
            _iconDay = value;
        }
    }

    [StringLength(3)]
    [Required]
    [JsonIgnore]
    public string IconNight {
        get => _iconNight;
        set {
            _iconNight = value;
        }
    }
}
