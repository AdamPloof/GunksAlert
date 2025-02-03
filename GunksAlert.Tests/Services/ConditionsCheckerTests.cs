using System;
using System.Collections.Generic;
using Xunit;

using GunksAlert.Api.Services;
using GunksAlert.Api.Models;

public class ConditionsCheckerTests {
    private static readonly Random _random = new Random();

    [Fact]
    public void NoPrecipitionEverIsDry() {

    }

    [Fact]
    public void MeltedSnowNoRainIsDry() {

    }

    [Fact]
    public void NoSnowSomeRainLotsOfSunIsDry() {

    }

    [Fact]
    public void LotsOfSnowMonthsOfMeltAndSunIsDry() {

    }

    [Fact]
    public void NoPrecipHistoryLightPrecipForecastWithSunBeforeIsDry() {

    }

    [Fact]
    public void NoPrecipHistoryRainDayOfIsWet() {

    }

    [Fact]
    public void LotsOfRainWeekOfDryTargetDateIsWet() {

    }

    [Fact]
    public void LotsOfSnowNotMeltedIsWet() {

    }

    [Fact]
    public void NotEnoughHistoryThrows() {

    }

    [Fact]
    public void TargetDateInThePastThrows() {

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
        List<WeatherHistory> histories = new();
        for (int i = 0; i < numDays; i++) {
            DateOnly date = startDate.AddDays(i);
            if (snow > 0.0) {
                // Snow
                double dailySnow = RandDouble(0.0, snow);
                snow -= dailySnow;
                histories.Add(MakeHistory(date, dailySnow, RandDouble(0, 32), RandDouble(0, 28)));
            } else if (rain > 0.0) {
                // Rain
                double dailyRain = RandDouble(0.0, rain);
                rain -= dailyRain;
                histories.Add(MakeHistory(date, dailyRain, RandDouble(37, 75), RandDouble(37, 67)));
            } else {
                // Dry
                histories.Add(MakeHistory(date, 0.0, RandDouble(37, 75), RandDouble(37, 67)));
            }
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
}
