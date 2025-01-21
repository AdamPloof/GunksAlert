using System;
using System.IO;
using System.Collections.Generic;

using GunksAlert.Cli.Services;
using System.Globalization;

namespace GunksAlert.Cli;

/// <summary>
/// TODO: add remark about why actions return bools (should continue?)
/// </summary>
public class App {
    private WeatherManager _weatherManager;
    private Dictionary<string, Func<string?, bool>> _actions;
    private DateTime? _dateOpt;
    private bool _abortEarly;

    public App(WeatherManager weatherManager) {
        _weatherManager = weatherManager;
        _dateOpt = null;
        _abortEarly = false;

        // Note: order of options is important. Earlier options are given precedence
        // and may short circuit later options.
        _actions = new Dictionary<string, Func<string?, bool>>() {
            {"-h", Help},
            {"--help", Help},
            {"-d", StoreDateOpt},
            {"--date", StoreDateOpt},
            {"-u", HandleUpdate},
            {"--update", HandleUpdate},
            {"-c", HandleClear},
            {"--clear", HandleClear},
        };
    }

    public void Run(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("No arguments provided. Aborting.");
            return;
        }

        // TODO: handle options and values separated by spaces as well as '='
        foreach (string arg in args) {
            if (_abortEarly) {
                Console.WriteLine("Exiting early. Check arguments and try again.");
                return;
            }

            string[] keyVal = arg.Split("=", 2);
            string key = keyVal[0];
            string? val = keyVal.Length > 1 ? keyVal[1] : null;

            if (!_actions.TryGetValue(key, out Func<string?, bool>? action)) {
                Console.WriteLine($"Invalid argument: {key}");
                return;
            }

            bool shouldContinue = action.Invoke(val);
            if (!shouldContinue) {
                return;
            }
        }
    }

    private bool StoreDateOpt(string? date) {
        if (date == null) {
            _dateOpt = null;
        } else {
            DateTime dateVal;
            if (DateTime.TryParseExact(
                date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateVal
            )) {
                _dateOpt = dateVal;
            } else {
                _abortEarly = true;
                Console.WriteLine($"Date option must be in yyyy-MM-dd format. Got: {date}");
            }
        }

        return true;
    }

    private bool HandleUpdate(string? model) {
        if (model == null) {
            Console.WriteLine("Most provide a model to update. Null given");
            return false;
        }

        string normalizedModel = model.ToLower().Replace("_", string.Empty);
        if (normalizedModel == "weatherhistory") {
            _weatherManager.UpdateWeatherHistory(_dateOpt);
        } else if (normalizedModel == "forecast") {
            _weatherManager.UpdateForecast();
        }

        return false;
    }

    private bool HandleClear(string? model) {
        if (model == null) {
            Console.WriteLine("Most provide a model to clear. Null given");
            return false;
        }

        string normalizedModel = model.ToLower();
        if (normalizedModel == "weather-history") {
            _weatherManager.ClearWeatherHistory(_dateOpt);
        } else if (normalizedModel == "forecast") {
            _weatherManager.ClearForecasts();
        }

        return false;
    }

    private bool Help(string? _) {
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
        
        return false;
    }
}
