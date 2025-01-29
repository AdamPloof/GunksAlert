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
    /// <returns>
    /// Likelyhood of being dry, between 0-1 with 1 being definitely dry and 0 being definitely wet
    /// </returns>
    public static double CragIsDry(List<WeatherHistory> recentWeather) {
        Dictionary<string, double> precipTotals = GetPrecipitationTotals(recentWeather);

        return 1.0;
    }

    /// <summary>
    /// Get preciptiation totals from weather histories that includes
    /// - snowLastWeek
    /// - snowLastMonth
    /// - rainLastThreeDays
    /// - rainYesterday
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, double> GetPrecipitationTotals(List<WeatherHistory> recentWeather) {
        Dictionary<string, double> precipTotals = new Dictionary<string, double>() {
            {"snowLastWeek", 0.0},
            {"snowLastMonth", 0.0},
            {"rainLastThreeDays", 0.0},
            {"rainYesterday", 0.0}
        };
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        foreach (WeatherHistory history in recentWeather) {
            int historyAge = today.DayNumber - history.Date.DayNumber;
            if (historyAge > 30) {
                // Only interested in the past 30 days
                break;
            }
            if (historyAge == 1) {
                if (history.TempHigh < 34.0) {
                    // assume snow
                    precipTotals["snowLastWeek"] += history.Precipitation;
                    precipTotals["snowLastMonth"] += history.Precipitation;
                } else {
                    precipTotals["rainYesterday"] += history.Precipitation;
                }
            } else if (historyAge <= 3) {
                if (history.TempHigh < 34.0) {
                    // assume snow
                    precipTotals["snowLastWeek"] += history.Precipitation;
                    precipTotals["snowLastMonth"] += history.Precipitation;
                } else {
                    precipTotals["rainLastThreeDays"] += history.Precipitation;
                }
            } else if (historyAge <= 7 && history.TempHigh < 34.0) {
                // assume snow
                precipTotals["snowLastWeek"] += history.Precipitation;
                precipTotals["snowLastMonth"] += history.Precipitation;
            } else if (history.TempHigh < 34.0) {
                precipTotals["snowLastMonth"] += history.Precipitation;
            }
        }

        return precipTotals;
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
