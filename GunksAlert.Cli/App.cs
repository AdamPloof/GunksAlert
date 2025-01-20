using System;
using System.IO;
using System.Collections.Generic;

using GunksAlert.Cli.Services;

namespace GunksAlert.Cli;

/// <summary>
/// TODO: add remark about why actions return bools (should continue?)
/// </summary>
public class App {
    private WeatherManager _weatherManager;
    private Dictionary<string, Func<string?, bool>> _actions;
    private string? _dateOpt;

    public App(WeatherManager weatherManager) {
        _weatherManager = weatherManager;
        _dateOpt = null;

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
        _dateOpt = date;

        return true;
    }

    private bool HandleUpdate(string? model) {
        if (model == null) {
            Console.WriteLine("Most provide a model to update. Null given");
            return false;
        }

        string normalizedModel = model.ToLower().Replace("_", string.Empty);
        if (normalizedModel == "weatherhistory") {
            _weatherManager.UpdateWeatherHistory();
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

        string normalizedModel = model.ToLower().Replace("_", string.Empty);
        if (normalizedModel == "weatherhistory") {
            _weatherManager.ClearWeatherHistory();
        } else if (normalizedModel == "forecast") {
            _weatherManager.ClearForecasts();
        }

        return false;
    }

    private bool Help(string? _) {
        // TODO: Usage
        
        return false;
    }
}
