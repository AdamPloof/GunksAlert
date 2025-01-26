using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using GunksAlert.Cli.Services;
using GunksAlert.Cli.Entities;

namespace GunksAlert.Cli;

/// <summary>
/// The main app class. Parses arguments and calls the actions. The _actions list maps command line
/// args to methods.
/// </summary>
/// <remarks>
/// All actions return a bool to indicate whether the app should continue executing other actions
/// once complete with that one. 
/// </remarks>
public class App {
    public record CliOption {
        public required string Name { get; init; }
        public string? Value { get; init; }
    }

    private List<AppAction> _options;
    private DateTime? _dateOpt;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private bool _abortEarly;

    public App() {
        _dateOpt = null;
        _startDate = null;
        _endDate = null;
        _abortEarly = false;

        // Note: order of options is important. Earlier options are given precedence
        // and may short circuit later options.
        _options = new List<AppAction>() {
            new AppAction() {
                ShortOpt = "-h",
                LongOpt ="--help",
                ValueRequired = false,
                ShouldContinue = false,
                ActionFunc = _ => { Help(_); return Task.CompletedTask; } 
            },
            new AppAction() {
                ShortOpt = "-d",
                LongOpt ="--date",
                ValueRequired = true,
                ShouldContinue = true,
                ActionFunc = date => { StoreDateOpt(date); return Task.CompletedTask; }
            },
            new AppAction() {
                ShortOpt = "-s",
                LongOpt ="--start_date",
                ValueRequired = true,
                ShouldContinue = true,
                ActionFunc = startDate => { StoreStartDate(startDate); return Task.CompletedTask; }
            },
            new AppAction() {
                ShortOpt = "-e",
                LongOpt ="--end_date",
                ValueRequired = true,
                ShouldContinue = true,
                ActionFunc = endDate => { StoreEndDate(endDate); return Task.CompletedTask; }
            },
            new AppAction() {
                ShortOpt = "-u",
                LongOpt ="--update",
                ValueRequired = true,
                ShouldContinue = false,
                ActionFunc = HandleUpdate
            },
            new AppAction() {
                ShortOpt = "-c",
                LongOpt ="--clear",
                ValueRequired = true,
                ShouldContinue = false,
                ActionFunc = HandleClear
            },
        };
    }

    public async Task Run(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("No arguments provided. Aborting.");
            return;
        }

        Queue<AppAction> actionsToRun = GetActionQueue(args);
        foreach (AppAction action in actionsToRun) {
            if (_abortEarly) {
                Console.WriteLine("Exiting early. Check arguments and try again.");
                return;
            }

            await action.ActionFunc.Invoke(action.Value);
            if (!action.ShouldContinue) {
                return;
            }
        }
    }

    /// <summary>
    /// Parses command line args and returns the actions to be called in the appropriate order.
    /// 
    /// TODO: this should also check for invalid args
    /// </summary>
    /// <param name="args"></param>
    /// <returns>The actions to be invoked</returns>
    public Queue<AppAction> GetActionQueue(string[] args) {
        Queue<AppAction> actionQueue = new Queue<AppAction>();
        List<CliOption> opts = ParseOptions(args);
        foreach (AppAction action in _options) {
            // Note: ValidateOptions checks for dups so this should only ever return one option
            CliOption? opt = opts.Where(
                o => o.Name == action.ShortOpt || o.Name == action.LongOpt
            ).FirstOrDefault();
            if (opt != null) {
                actionQueue.Enqueue(action);
            }
        }

        return actionQueue;
    }

    private List<CliOption> ParseOptions(string[] args) {
        List<CliOption> options = new();
        bool skipNext = false;
        int argIdx = 0;
        foreach (string arg in args) {
            if (skipNext) {
                skipNext = false;
                argIdx++;
                continue;
            }

            string key;
            string? val = null;
            if (arg.Contains('=')) {
                // "=" delimited
                string[] keyVal = arg.Split("=", 2);
                key = keyVal[0];
                val = keyVal[1];
            } else {
                key = arg;
            }

            if ((val == null) && (argIdx < args.Length - 1)) {
                // Check if the next arg is the value (space delimited)
                string nextArg = args[argIdx + 1];
                if (!nextArg.StartsWith('-')) {
                    val = nextArg;
                    skipNext = true;
                }
            }
            
            options.Add(new CliOption() {Name = key, Value = val});
            argIdx++;
        }

        return options;
    }

    /// <summary>
    /// Ensure that all options passed actually exist. Check that options with
    /// required values have values set.
    /// 
    /// Throws ArgumentException if not valid
    /// </summary>
    /// <param name="options"></param>
    private void ValidateOptions(List<CliOption> options) {

    }

    private static DateTime ParseDateOpt(string opt) {
        if (!DateTime.TryParseExact(
            opt,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime date
        )) {
            throw new ArgumentException($"Date option must be in yyyy-MM-dd format. Got: {opt}");
        }

        return date;
    }

    private void StoreDateOpt(string? date) {
        if (date == null) {
            _dateOpt = null;
        } else {
            try {
                _dateOpt = ParseDateOpt(date);
            } catch (ArgumentException e) {
                _abortEarly = true;
                Console.WriteLine(e.Message);
            }
        }
    }

    private void StoreStartDate(string? date) {
        if (date == null) {
            _startDate = null;
        } else {
            try {
                _startDate = ParseDateOpt(date);
            } catch (ArgumentException e) {
                _abortEarly = true;
                Console.WriteLine(e.Message);
            }
        }
    }

    private void StoreEndDate(string? date) {
        if (date == null) {
            _endDate = null;
        } else {
            try {
                _endDate = ParseDateOpt(date);
            } catch (ArgumentException e) {
                _abortEarly = true;
                Console.WriteLine(e.Message);
            }
        }
    }

    private async Task HandleUpdate(string? model) {
        if (model == null) {
            Console.WriteLine("Most provide a model to update. Null given");
            return;
        }

        bool isSuccess = false;
        string normalizedModel = model.ToLower().Replace("_", string.Empty);
        if (normalizedModel == "weatherhistory") {
            if (_startDate != null && _endDate != null ){
                DateTime start = (DateTime)_startDate;
                DateTime end = (DateTime)_endDate;
                string startStr = start.ToString("yyyy-MM-dd");
                string endStr = end.ToString("yyyy-MM-dd");
                Console.WriteLine($"Updating WeatherHistroy for range. Start date: {startStr}, End date: {endStr}...");

                isSuccess = await WeatherManager.UpdateWeatherHistory(start, end);
            } else {
                string dateStr = _dateOpt == null ? "yesterday" : _dateOpt?.ToString("yyyy-MM-dd")!;
                Console.WriteLine($"Updating weather history for date: {dateStr}...");

                isSuccess = await WeatherManager.UpdateWeatherHistory(_dateOpt);
            }
        } else if (normalizedModel == "forecast") {
            Console.WriteLine("Updating forecast...");
            isSuccess = await WeatherManager.UpdateForecast();
        }

         if (isSuccess) {
            Console.WriteLine("Update successful");
         } else {
            Console.WriteLine("Update failed. See logs for details");
         }
    }

    private async Task HandleClear(string? model) {
        if (model == null) {
            Console.WriteLine("Most provide a model to clear. Null given");
            return;
        }

        bool isSuccess = false;
        string normalizedModel = model.ToLower();
        if (normalizedModel == "weather-history") {
            string dateStr = _dateOpt == null ? "today" : _dateOpt?.ToString("yyyy-MM-dd")!;
            Console.WriteLine($"Clearing weather history through date: {dateStr}...");

            isSuccess = await WeatherManager.ClearWeatherHistory(_dateOpt);
        } else if (normalizedModel == "forecast") {
            Console.WriteLine("Clearing forecasts...");
            isSuccess = await WeatherManager.ClearForecasts();
        }

        if (isSuccess) {
            Console.WriteLine("Clear successful");
         } else {
            Console.WriteLine("Clear failed. See logs for details");
         }
    }

    private void Help(string? _) {
        string help = """
            GunksAlert CLI - Maintenance Tool
            =================================

            Description:
            Execute maintenance tasks via the GunksAlert API for keeping weather history 
            and forecast data up to date.

            Usage:
            gunks [options]

            Examples:
            gunks --update=weather-history
            gunks --update=weather-history --date 2025-01-11
            gunks --clear=forecast

            Options:
            -h, --help              Show this help message and exit.

            -d, --date <DATE>       Specify the date for operations like updating weather history
                                    or clearing weather history. The date must be in the format
                                    `yyyy-MM-dd`.

            -s, --start_date <DATE> Specify the start date for range-based tasks like updating
                                    updating or clearing weather history. The date must be in the format
                                    `yyyy-MM-dd`.

            -e, --end_date <DATE>   Specify the end date for range-based tasks like updating
                                    updating or clearing weather history. The date must be in the format
                                    `yyyy-MM-dd`.

            -u, --update <VALUE>    Update forecast or weather history. Required value:
                                    - `forecast`
                                    - `weather-history`
                                    Values are case-insensitive.

            -c, --clear <VALUE>     Clear weather history or forecast. Required value:
                                    - `forecast`
                                    - `weather-history`
                                    Values are case-insensitive.
            """;
        Console.WriteLine(help);
    }
}
