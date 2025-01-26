using System;
using System.Collections.Generic;
using Xunit;

using GunksAlert.Cli;
using GunksAlert.Cli.Entities;

namespace GunksAlert.Tests;

public class AppTests {
    [Fact]
    public void ArgsAreEmpty() {
        string[] args = new string[] {};
        App app = new();
        Queue<AppAction> actions = app.GetActionQueue(args);

        Assert.Empty(actions);
    }

    [Fact]
    public void InvalidOptionsThrow() {
        string[] args = new string[] {
            "--date",
            "--bar",
            "--baz",
        };
        App app = new();
        Assert.Throws<ArgumentException>(() => app.GetActionQueue(args));
    }

    [Fact]
    public void OptionsHaveNoValues() {
        string[] args = new string[] {
            "--date",
            "--update",
            "-h",
        };
        App app = new();
        Queue<AppAction> actions = app.GetActionQueue(args);

        foreach (AppAction action in actions) {
            Assert.Null(action.Value);
        }
    }

    [Fact]
    public void OptionValuesEqualsDelimited() {
        string[] args = new string[] {
            "--date=2024-11-01",
            "--update=weather-history",
            "-h",
        };
        App app = new();
        Queue<AppAction> actions = app.GetActionQueue(args);

        AppAction date = actions.Dequeue();
        Assert.Equal("--date", date.LongOpt);
        Assert.Equal("2024-11-01", date.Value);

        AppAction update = actions.Dequeue();
        Assert.Equal("--update", update.LongOpt);
        Assert.Equal("weather-history", update.Value);

        AppAction help = actions.Dequeue();
        Assert.Equal("-h", help.ShortOpt);
        Assert.Null(help.Value);
    }

    [Fact]
    public void OptionValuesSpaceDelimited() {
        string[] args = new string[] {
            "--date",
            "2024-11-01",
            "--update",
            "weather-history",
            "-h",
        };
        App app = new();
        Queue<AppAction> actions = app.GetActionQueue(args);

        AppAction date = actions.Dequeue();
        Assert.Equal("--date", date.LongOpt);
        Assert.Equal("2024-11-01", date.Value);

        AppAction update = actions.Dequeue();
        Assert.Equal("--update", update.LongOpt);
        Assert.Equal("weather-history", update.Value);

        AppAction help = actions.Dequeue();
        Assert.Equal("-h", help.ShortOpt);
        Assert.Null(help.Value);
    }

    [Fact]
    public void OptionValuesMixedDelimited() {
        string[] args = new string[] {
            "--date=2024-11-01",
            "--update",
            "weather-history",
            "-h",
        };
        App app = new();
        Queue<AppAction> actions = app.GetActionQueue(args);

        AppAction date = actions.Dequeue();
        Assert.Equal("--date", date.LongOpt);
        Assert.Equal("2024-11-01", date.Value);

        AppAction update = actions.Dequeue();
        Assert.Equal("--update", update.LongOpt);
        Assert.Equal("weather-history", update.Value);

        AppAction help = actions.Dequeue();
        Assert.Equal("-h", help.ShortOpt);
        Assert.Null(help.Value);
    }

    [Fact]
    public void OptionsInOrderWithValues() {
        string[] args = new string[] {
            "-h",
            "--date=2024-11-01",
            "--start_date",
            "2024-12-01",
            "-e=2025-01-01",
            "--update",
            "weather-history",
            "--clear=forecast",
        };
        App app = new();
        Queue<AppAction> actions = app.GetActionQueue(args);

        AppAction help = actions.Dequeue();
        Assert.Equal("-h", help.ShortOpt);
        Assert.Null(help.Value);

        AppAction date = actions.Dequeue();
        Assert.Equal("--date", date.LongOpt);
        Assert.Equal("2024-11-01", date.Value);

        AppAction startDate = actions.Dequeue();
        Assert.Equal("--start_date", startDate.LongOpt);
        Assert.Equal("2024-12-01", startDate.Value);

        AppAction endDate = actions.Dequeue();
        Assert.Equal("-e", endDate.ShortOpt);
        Assert.Equal("2025-01-01", endDate.Value);

        AppAction update = actions.Dequeue();
        Assert.Equal("--update", update.LongOpt);
        Assert.Equal("weather-history", update.Value);

        AppAction clear = actions.Dequeue();
        Assert.Equal("--clear", clear.LongOpt);
        Assert.Equal("forecast", clear.Value);
    }

    [Fact]
    public void OptionsOutOfOrderWithValues() {
        string[] args = new string[] {
            "--start_date",
            "2024-12-01",
            "--date=2024-11-01",
            "-h",
            "--clear",
            "forecast",
            "--update=weather-history",
            "-e=2025-01-01",
        };
        App app = new();
        Queue<AppAction> actions = app.GetActionQueue(args);

        AppAction help = actions.Dequeue();
        Assert.Equal("-h", help.ShortOpt);
        Assert.Null(help.Value);

        AppAction date = actions.Dequeue();
        Assert.Equal("--date", date.LongOpt);
        Assert.Equal("2024-11-01", date.Value);

        AppAction startDate = actions.Dequeue();
        Assert.Equal("--start_date", startDate.LongOpt);
        Assert.Equal("2024-12-01", startDate.Value);

        AppAction endDate = actions.Dequeue();
        Assert.Equal("-e", endDate.ShortOpt);
        Assert.Equal("2025-01-01", endDate.Value);

        AppAction update = actions.Dequeue();
        Assert.Equal("--update", update.LongOpt);
        Assert.Equal("weather-history", update.Value);

        AppAction clear = actions.Dequeue();
        Assert.Equal("--clear", clear.LongOpt);
        Assert.Equal("forecast", clear.Value);
    }

    [Fact]
    public void DuplicateOptionsThrows() {
        string[] args = new string[] {
            "--start_date",
            "2024-04-05",
            "--start_date",
            "2024-04-07",
            "--update=forecast",
        };
        App app = new();
        Assert.Throws<ArgumentException>(() => app.GetActionQueue(args));
    }
}
