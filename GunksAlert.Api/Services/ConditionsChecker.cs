using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Models;
using GunksAlert.Api.Data;

namespace GunksAlert.Api.Services;

/// <summary>
/// ConditionsChecker is responsible for inspecting the recent weather history of a crag and the
/// upcomming forecast and making the call on whether an alert should be sent to notify climbers
/// of climbable conditions.
/// </summary>
public class ConditionsChecker {
    private readonly GunksDbContext _context;

    public ConditionsChecker(GunksDbContext context) {
        _context = context;
    }

    public static bool ForecastLooksGood(
        Forecast forecast,
        ClimbableConditions conditions
    ) {


        return true;
    }

    /// <summary>
    /// Look at the recent weather history to get a sense of how likely the crag is to be dry.
    /// </summary>
    /// <remarks>
    /// This is very rough guess especially if you we're dealing with trying to use weather history
    /// to predict snow pack and melt. Would be better if there was a current snow pack endpoint we
    /// could use, but haven't found one.
    /// </remarks
    /// <param name="recentWeather"></param>
    /// <param name="upcomingWeather"></param>
    /// <param name="targetDate"></param>
    /// <returns>
    /// Likelyhood of being dry, between 0-1 with 1 being definitely dry and 0 being definitely wet
    /// </returns>
    public static double CragIsDry(
        List<WeatherHistory> recentWeather,
        List<Forecast> upcomingWeather,
        DateOnly targetDate
    ) {
        double snowpack = EstimatedSnowpack(recentWeather);

        return 1.0;
    }

    /// <summary>
    /// Make an estimated guess about how much snow is on the ground as of today based on snow in the past 90
    /// days minus melting.
    /// </summary>
    /// <param name="seasonHistory">The season's weather history, up to 90 days</param>
    /// <returns></returns>
    public static double EstimatedSnowpack(List<WeatherHistory> seasonHistory) {
        double snowpack = 0.0;
        foreach (WeatherHistory weather in seasonHistory) {
            double snowfall = 0.0;
            if (weather.TempHigh < 35.6) {
                // assume 10:1 snow-water ratio
                snowfall = weather.Precipitation * 10;
            } else if (weather.TempHigh < 41) {
                // mix snow/rain
                snowfall = (weather.Precipitation * 0.5) * 10;
            } else {
                snowfall = 0.0;
            }

            double meltFactor = 0.0;
            if (weather.TempHigh > 32) {
                // Base melt rate = 0.02 inches melt per day for every degree above freezing
                meltFactor = (weather.TempHigh - 32) * 0.02;
                meltFactor *= 1 - (weather.Clouds / 100); // more clouds, less melt
                meltFactor *= 1 - (weather.Humidity / 100); // more humidity, less melt
                meltFactor *= 1 + (weather.WindSpeed / 100); // more wind, more melt
            } else {
                meltFactor = 0.0;
            }

            snowpack += snowfall;
            snowpack -= meltFactor;
            snowpack = snowpack < 0.0 ? 0.0 : snowpack;
        }

        return snowpack;
    }

    /// <summary>
    /// Get the number of recent days that likely had dry/drying conditions since
    /// - yesterday
    /// - last three days
    /// - last week
    /// - last month
    /// 
    /// <remarks>
    /// Returns as dict of the date ranges and dryness count as a double. Uses double instead of int because
    /// a likely dry day counts as 1, a *very* dry day (low humidity, high wind, full sun, etc) counts as 1.5
    /// </remarks>
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, double> GetDryDayCounts(List<WeatherHistory> recentWeather) {
        Dictionary<string, double> dryDays = new Dictionary<string, double>() {
            {"yesterday", 0.0},
            {"lastThreeDays", 0.0},
            {"lastWeek", 0.0},
            {"lastMonth", 0.0},
        };
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        foreach (WeatherHistory history in recentWeather) {
            int historyAge = today.DayNumber - history.Date.DayNumber;
            if (historyAge > 30) {
                // Only interested in the past 30 days
                break;
            }
            if (historyAge == 1) {

            } else if (historyAge <= 3) {

            } else if (historyAge <= 7 && history.TempHigh < 34.0) {
                
            } else if (history.TempHigh < 34.0) {

            }
        }

        return dryDays;
    }

    public bool DayIsClimbable(DateOnly day, Crag crag, ClimbableConditions conditions) {

        return true;
    }
}
