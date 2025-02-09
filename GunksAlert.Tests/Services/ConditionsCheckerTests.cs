using System;
using System.Collections.Generic;
using Xunit;

using GunksAlert.Api.Services;
using GunksAlert.Api.Models;

public class ConditionsCheckerTests {
    private static readonly Random _random = new Random();

    [Fact]
    public void NoPrecipitionEverIsDry() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<Forecast> forecasts = MakeForecasts(new DateOnly(2025, 4, 1), 10, 0.0, 0.0);
        List<WeatherHistory> histories = MakeHistories(new DateOnly(2025, 1, 1), 90, 0.0, 0.0);

        Assert.True(ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate) > 0.99);
    }

    [Fact]
    public void MeltedSnowNoRainIsDry() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<Forecast> forecasts = MakeForecasts(new DateOnly(2025, 4, 1), 10, 0.0, 0.0);
        List<WeatherHistory> janHistory = MakeHistories(new DateOnly(2025, 1, 1), 31, 0.0, 6.0);
        List<WeatherHistory> febMarHistory = MakeHistories(new DateOnly(2025, 2, 1), 59, 0.0, 0.0);
        List<WeatherHistory> histories = janHistory.Concat(febMarHistory).ToList();

        Assert.True(ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate) > 0.99);
    }

    [Fact]
    public void NoSnowSomeRainLotsOfSunIsDry() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<Forecast> forecasts = MakeForecasts(new DateOnly(2025, 4, 1), 10, 0.0, 0.0);
        List<WeatherHistory> janFebHistory = MakeHistories(new DateOnly(2025, 1, 1), 31, 2.0, 0.0);
        List<WeatherHistory> marHistory = MakeHistories(new DateOnly(2025, 2, 1), 59, 0.0, 0.0);
        List<WeatherHistory> histories = janFebHistory.Concat(marHistory).ToList();

        Assert.True(ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate) > 0.99);
    }

    [Fact]
    public void LotsOfSnowMonthsOfMeltAndSunIsDry() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<Forecast> forecasts = MakeForecasts(new DateOnly(2025, 4, 1), 10, 0.0, 0.0);
        List<WeatherHistory> janStart = MakeHistories(new DateOnly(2025, 1, 1), 2, 0.0, 24);
        List<WeatherHistory> restHistory = MakeHistories(new DateOnly(2025, 1, 3), 88, 0.0, 0.0);
        List<WeatherHistory> histories = janStart.Concat(restHistory).ToList();

        Assert.True(ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate) > 0.99);
    }

    [Fact]
    public void NoPrecipHistoryLightPrecipForecastWithSunBeforeIsDry() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<WeatherHistory> histories = MakeHistories(new DateOnly(2025, 1, 1), 90, 0.0, 0.0);
        List<Forecast> earlyWeek = MakeForecasts(new DateOnly(2025, 4, 1), 2, 0.5, 0.0);
        List<Forecast> restOfWeek = MakeForecasts(new DateOnly(2025, 4, 3), 8, 0.0, 0.0);
        List<Forecast> forecasts = earlyWeek.Concat(restOfWeek).ToList();

        Assert.True(ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate) > 0.99);
    }

    [Fact]
    public void NoPrecipHistoryRainDayOfIsWet() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<WeatherHistory> histories = MakeHistories(new DateOnly(2025, 1, 1), 90, 0.0, 0.0);
        List<Forecast> mostOfWeek = MakeForecasts(new DateOnly(2025, 4, 1), 9, 0.0, 0.0);
        List<Forecast> dayOf = MakeForecasts(new DateOnly(2025, 4, 10), 1, 1.0, 0.0);
        List<Forecast> forecasts = mostOfWeek.Concat(dayOf).ToList();

        Assert.Equal(0.0, ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate));
    }

    [Fact]
    public void LotsOfRainWeekOfDryTargetDateIsWet() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<WeatherHistory> histories = MakeHistories(new DateOnly(2025, 1, 1), 90, 0.0, 0.0);
        List<Forecast> beginningOfWeek = MakeForecasts(new DateOnly(2025, 4, 1), 6, 1.0, 0.0);
        List<Forecast> endOfWeek = MakeForecasts(new DateOnly(2025, 4, 7), 3, 3.5, 0.0);
        Forecast dayOf = MakeForecast(new DateOnly(2025, 4, 10), 0.0, 50.0, 45.0);
        List<Forecast> forecasts = beginningOfWeek.Concat(endOfWeek).ToList();
        forecasts.Add(dayOf);

        Assert.True(ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate) < 0.68);
    }

    [Fact]
    public void LotsOfSnowNotMeltedIsWet() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<WeatherHistory> histories = MakeHistories(new DateOnly(2025, 1, 1), 90, 0.0, 0.0);
        List<Forecast> mostOfWeek = MakeForecasts(new DateOnly(2025, 4, 1), 8, 0.0, 12.0);
        List<Forecast> twoDaysOf = MakeForecasts(new DateOnly(2025, 4, 9), 2, 0.0, 0.0);
        List<Forecast> forecasts = mostOfWeek.Concat(twoDaysOf).ToList();

        Assert.True(ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate) < 0.3);
    }

    [Fact]
    public void NotEnoughHistoryThrows() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2025, 4, 10);
        List<Forecast> forecasts = MakeForecasts(new DateOnly(2025, 4, 1), 10, 0.0, 0.0);
        List<WeatherHistory> histories = MakeHistories(new DateOnly(2025, 3, 1), 30, 0.0, 0.0);

        Assert.Throws<ArgumentException>(
            () => ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate)
        );
    }

    [Fact]
    public void TargetDateInThePastThrows() {
        DateOnly currentDate = new DateOnly(2025, 4, 1);
        DateOnly targetDate = new DateOnly(2023, 4, 10);
        List<Forecast> forecasts = MakeForecasts(new DateOnly(2023, 4, 1), 10, 0.0, 0.0);
        List<WeatherHistory> histories = MakeHistories(new DateOnly(2023, 1, 1), 90, 0.0, 0.0);

        Assert.Throws<ArgumentException>(
            () => ConditionsChecker.CragWillBeDry(histories, forecasts, currentDate, targetDate)
        );
    }

    private static double RandDouble(double min, double max) {
        if (min > max) {
            throw new ArgumentException("Minimum in random range must be less than the max");
        }

        return min + (_random.NextDouble() * (max - min));
    }

    /// <summary>
    /// Return a set of WeatherHistory with required total precipition
    /// </summary>
    /// <returns></returns>
    private static List<WeatherHistory> MakeHistories(
        DateOnly startDate,
        int numDays,
        double rain,
        double snow
    ) {
        snow /= 10; // snow-water ratio
        List<WeatherHistory> histories = new();
        for (int i = 0; i < numDays; i++) {
            DateOnly date = startDate.AddDays(i);
            if (snow > 0.0) {
                // Snow
                double dailySnow = snow < 0.1 ? snow : RandDouble(0.0, snow);
                snow -= dailySnow;
                histories.Add(MakeHistory(date, dailySnow, RandDouble(0, 32), RandDouble(0, 28)));
            } else if (rain > 0.0) {
                // Rain
                double dailyRain = rain < 0.1 ? rain : RandDouble(0.0, rain);
                rain -= dailyRain;
                histories.Add(MakeHistory(date, dailyRain, RandDouble(37, 75), RandDouble(37, 67)));
            } else {
                // Dry
                histories.Add(MakeHistory(date, 0.0, RandDouble(37, 75), RandDouble(37, 67)));
            }
        }

        // not the most realistic way of adding remaining precip, but it'll do
        if (snow > 0.0) {
            histories[histories.Count - 1].Precipitation += snow;
        }

        if (rain > 0.0) {
            histories[histories.Count - 1].Precipitation += rain;
        }

        return histories;
    }

    private static WeatherHistory MakeHistory(DateOnly date, double precip, double tempHigh, double tempLow) {
        WeatherHistory history = new WeatherHistory() {
            Date = date,
            TempHigh = tempHigh,
            TempLow = tempLow,
            Clouds = _random.NextDouble(),
            Humidity = _random.NextDouble(),
            Precipitation = precip,
            WindSpeed = RandDouble(0.0, 15.0),
            WindDegree = RandDouble(0.0, 360.0)
        };

        return history;
    }

    /// <summary>
    /// Return a set of Forecasts with required total precipition
    /// </summary>
    /// <returns></returns>
    private static List<Forecast> MakeForecasts(
        DateOnly startDate,
        int numDays,
        double rain,
        double snow
    ) {
        List<Forecast> forecasts = new();
        for (int i = 0; i < numDays; i++) {
            DateOnly date = startDate.AddDays(i);
            if (snow > 0.0) {
                // Snow
                double dailySnow = snow < 0.1 ? snow : RandDouble(0.0, snow);
                snow -= dailySnow;
                forecasts.Add(MakeForecast(date, dailySnow, RandDouble(0, 32), RandDouble(0, 28)));
            } else if (rain > 0.0) {
                // Rain
                double dailyRain = rain < 0.1 ? rain : RandDouble(0.0, rain);
                rain -= dailyRain;
                forecasts.Add(MakeForecast(date, dailyRain, RandDouble(37, 75), RandDouble(37, 67)));
            } else {
                // Dry
                forecasts.Add(MakeForecast(date, 0.0, RandDouble(37, 75), RandDouble(37, 67)));
            }
        }

        // not the most realistic way of adding remaining precip, but it'll do
        if (snow > 0.0) {
            forecasts[forecasts.Count - 1].Snow += snow;
        }

        if (rain > 0.0) {
            forecasts[forecasts.Count - 1].Rain += rain;
        }

        return forecasts;
    }

    private static Forecast MakeForecast(DateOnly date, double precip, double tempHigh, double tempLow) {
        Forecast forecast = new Forecast() {
            Date = new DateTimeOffset(date, TimeOnly.MinValue, TimeSpan.Zero),
            Summary = "Test day",
            TempHigh = tempHigh,
            TempLow = tempLow,
            Clouds = _random.Next(0, 100),
            Humidity = _random.Next(0, 100),
            Rain = tempHigh < 36 ? 0.0 : precip,
            Snow = tempHigh < 36 ? precip : 0.0,
            Pop = precip > 0.0 ? 1.0 : 0.0,
            WindSpeed = RandDouble(0.0, 15.0),
            WindDegree = _random.Next(0, 360)
        };

        return forecast;
    }
}
