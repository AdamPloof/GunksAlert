using System;
using System.Threading.Tasks;
using GunksAlert.Cli.Services;

namespace GunksAlert.Cli;

internal static class Program {
    static void Main(string[] args) {
        MainAsync(args).Wait();
    }

    static async Task MainAsync(string[] args) {
        App app = new App();
        await app.Run(args);
    }
}
