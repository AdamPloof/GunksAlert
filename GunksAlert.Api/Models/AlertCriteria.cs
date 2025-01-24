using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GunksAlert.Api.Models;

/// <summary>
/// Represents the criteria for sending an alert.
/// </summary>
/// <remarks>
/// Criteria is composed of
///   - The crag
///   - The conditions
///   - The period during which alerts should be sent
/// </remarks>
public class AlertCriteria {
    [Key]
    public int Id { get; private set; }

    [Required]
    [ForeignKey("Crag")]
    public int CragId { get; set; }

    public required Crag Crag { get; set; }

    [Required]
    [ForeignKey("ClimbableConditions")]
    public int ClimbableConditionsId { get; set; }

    public required ClimbableConditions ClimbableConditions { get; set; }

    [Required]
    [ForeignKey("AlertPeriod")]
    public int AlertPeriodId { get; set; }

    public required AlertPeriod AlertPeriod { get; set; }
}
