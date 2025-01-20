using System;
using System.Threading.Tasks;
using GunksAlert.Cli.Services;

namespace GunksAlert.Cli;

internal static class Program {
    static void Main(string[] args) {
        ApiBridge api = new ApiBridge();
        WeatherManager weatherManager = new WeatherManager(api);
        App app = new App(weatherManager);
        app.Run(args);
    }
}
