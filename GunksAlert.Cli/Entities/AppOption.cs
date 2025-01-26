using System;
using System.Collections.Generic;
using System.Threading;

namespace GunksAlert.Cli.Entities;

/// <summary>
/// The options accepted as command line args
/// </summary>
public class AppAction {
    public required string ShortOpt { get; init; }
    public required string LongOpt { get; init; }

    /// <summary>
    /// Does this action require a value
    /// </summary>
    public required bool ValueRequired { get; init; }
    
    /// <summary>
    /// Should execution continue after the action has been called
    /// </summary>
    public required bool ShouldContinue { get; init; }
    public required Func<string?, Task> ActionFunc { get; init; }

    public string? Value { get; set; } = null;
}
