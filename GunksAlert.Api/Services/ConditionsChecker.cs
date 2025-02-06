using System;
using System.Linq;
using System.Diagnostics;
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
    private static readonly double DryRate = 0.05;
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
    /// This is very rough guess especially since we're dealing with trying to use weather history
    /// to predict snow pack and melt. Would be better if there was a current snow pack endpoint we
    /// could use, but haven't found one.
    /// </remarks
    /// <param name="recentWeather"></param>
    /// <param name="upcomingWeather"></param>
    /// <param name="targetDate">Assumed to always be later than today</param>
    /// <returns>
    /// Likelyhood of being dry, between 0-1 with 1 being definitely dry and 0 being definitely wet
    /// </returns>
    public static double CragWillBeDry(
        List<WeatherHistory> recentWeather,
        List<Forecast> upcomingWeather,
        DateOnly currentDate,
        DateOnly targetDate
    ) {
        if (targetDate < currentDate) {
            throw new ArgumentException("Target date is before current date");
        }
        
        double chanceDryDayOf = ChanceDry(recentWeather, upcomingWeather, currentDate, targetDate);
        Forecast targetForecast = upcomingWeather.Where(
            f => DateOnly.FromDateTime(f.Date.Date) == targetDate
        ).First();
        double chancePrecipitation = targetForecast.Pop + chanceDryDayOf;
        double chanceDry =  1.0 - chancePrecipitation;

        return chanceDry > 0.0 ? chanceDry : 0.0;
    }

    public static double ChanceDry(
        List<WeatherHistory> recentWeather,
        List<Forecast> upcomingWeather,
        DateOnly currentDate,
        DateOnly targetDate
    ) {
        double chanceDry = 1.0;
        double precipitation = PrecipitaionTotal(recentWeather, upcomingWeather, currentDate, targetDate);
        chanceDry -= precipitation * DryRate;

        double snowpack = EstimatedSnowpack(recentWeather);
        chanceDry -= snowpack * DryRate;
        
        double precipitationDayBefore = 0.0;
        if (targetDate == currentDate) {
            WeatherHistory yesterday = recentWeather.Where(
                w => w.Date == currentDate.AddDays(-1)
            ).First();
            precipitationDayBefore = yesterday.Precipitation;
        } else {
            Forecast dayBefore = upcomingWeather.Where(
                f => DateOnly.FromDateTime(f.Date.Date) == targetDate.AddDays(-1)
            ).First();
            precipitationDayBefore = dayBefore.Rain;
        }

        // Additional penality if the rain happens the day before
        chanceDry -= precipitationDayBefore * DryRate;

        return chanceDry > 0.0 ? chanceDry : 0.0;
    }

    public static double PrecipitaionTotal(
        List<WeatherHistory> recentWeather,
        List<Forecast> upcomingWeather,
        DateOnly currentDate,
        DateOnly targetDate
    ) {
        int forecastDaysToReview = targetDate.DayNumber - currentDate.DayNumber;
        int historyDaysToReview = 3 - forecastDaysToReview;
        historyDaysToReview = historyDaysToReview > 0 ? historyDaysToReview : 0;
        double totalPrecipitaition = 0.0;
        if (historyDaysToReview > 0) {
            IEnumerable<WeatherHistory> historiesToReview = recentWeather.Where(w => {
               return w.Date.DayNumber >= currentDate.DayNumber - historyDaysToReview; 
            });
            Debug.Assert(
                historyDaysToReview == historiesToReview.Count(),
                $"Histories needed to predict crag dryness not present. Expected {historiesToReview}, got {historiesToReview.Count()}"
            );
            totalPrecipitaition += PrecipitationAmount(historiesToReview.ToList());
        }

        IEnumerable<Forecast> forecastsToReview = upcomingWeather.Where(f => {
            int forecastDayNum = DateOnly.FromDateTime(f.Date.Date).DayNumber;
            return forecastDayNum >= targetDate.DayNumber - forecastDaysToReview;
        });
        Debug.Assert(
            forecastDaysToReview <= forecastsToReview.Count(),
            $"Forecasts needed to predict crag dryness not present. Expected at least {forecastDaysToReview}, got {forecastsToReview.Count()}"
        );

        totalPrecipitaition += PrecipitationAmount(forecastsToReview.ToList());

        return totalPrecipitaition;
    }

    /// <summary>
    /// Get the accumulated precipition amount from a list of weather histories
    /// </summary>
    /// <param name="recentWeather"></param>
    /// <returns></returns>
    public static double PrecipitationAmount(List<WeatherHistory> recentWeather) {
        double precip = 0.0;
        foreach (WeatherHistory history in recentWeather) {
            precip += history.Precipitation;
        }

        return precip;
    }

    /// <summary>
    /// Get the accumulated precipitation amount from a list of forecasts
    /// </summary>
    /// <param name="upcomingWeather"></param>
    /// <returns></returns>
    public static double PrecipitationAmount(List<Forecast> upcomingWeather) {
        double precip = 0.0;
        foreach (Forecast forecast in upcomingWeather) {
            precip += forecast.Rain;
            precip += forecast.Snow;
        }

        return precip;
    }

    /// <summary>
    /// Make an estimated guess about how much snow is on the ground as of current date based on snow in the past 90
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

    public bool DayIsClimbable(DateOnly day, Crag crag, ClimbableConditions conditions) {

        return true;
    }
}
