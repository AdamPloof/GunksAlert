using System;
using System.IO;

namespace GunksAlert.Cli.Services;

public static class Logger {
    private static readonly string LogPath = Path.Combine(
        AppContext.BaseDirectory, "./log/app.log"
    );

    public enum LogLevel {
        Info,
        Warning,
        Error
    }

    public static void Info(string msg) {
        Log(LogLevel.Info, msg);
    }

    public static void Warning(string msg) {
        Log(LogLevel.Warning, msg);
    }

    public static void Error(string msg) {
        Log(LogLevel.Error, msg);
    }

    private static void Log(LogLevel level, string msg) {
        DateTime now = DateTime.Now;
        string logTime = now.ToString("yyyy-MM-dd");
        string entry = $"{logTime} [{level}]: {msg}";
        try {
            File.AppendAllText(LogPath, entry + Environment.NewLine);
        } catch (IOException e) {
            Console.WriteLine($"Failed to write to log: {e.Message}");
        }
    }
}
