using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using GunksAlert.Api.Models;
using GunksAlert.Api.Data;

namespace GunksAlert.Api.Services;

/// <summary>
/// ConditionsChecker is responsible for inspecting the recent weather history of a crag and the
/// upcoming forecast and making the call on whether an alert should be sent to notify climbers
/// of climbable conditions.
/// </summary>
public class ConditionsChecker {
    private static readonly double DryRate = 0.1;
    private readonly GunksDbContext _context;

    public ConditionsChecker(GunksDbContext context) {
        _context = context;
    }

    /// <summary>
    /// Check the conditions for a given day and return a report on whether that day
    /// will be climbable. Fetches required forecasts and weather history from the database.
    /// </summary>
    /// <param name="conditions"></param>
    /// <param name="currentDate"></param>
    /// <param name="targetDate"></param>
    /// <returns></returns>
    public ConditionsReport CheckConditions(
        Crag crag,
        ClimbableConditions conditions,
        DateOnly currentDate,
        DateOnly targetDate
    ) {
        DateTimeOffset currentDt = new DateTimeOffset(
            currentDate.ToDateTime(new TimeOnly(0, 0)),
            TimeSpan.Zero
        );
        DateTimeOffset targetDt = new DateTimeOffset(
            targetDate.ToDateTime(new TimeOnly(23, 59)),
            TimeSpan.Zero
        );

        List<Forecast> upcomingWeather = _context.Forecasts.Where(f =>
            f.Date.Date >= currentDt.Date && f.Date.Date <= targetDt.Date
        ).ToList();
        DateOnly startHistory = currentDate.AddDays(-90);
        List<WeatherHistory> recentWeather = _context.WeatherHistories.Where(
            h => h.Date >= startHistory
        ).ToList();

        ConditionsReport report = new ConditionsReport() {
            Date = targetDate,
            CragId = crag.Id
        };
        DateTimeOffset dt = new DateTimeOffset(
            targetDate.ToDateTime(new TimeOnly(0, 0)),
            TimeSpan.Zero
        );
        Forecast targetForecast = upcomingWeather.Where(f =>
            f.Date.Date == dt.Date
        ).First();

        // TODO: could probably get components of dryness (snowpack, rain preceding days, etc.) separately
        // and pass into ChanceDry rather than having a side effect of modifying the report.
        report.ChanceDry = ChanceDry(recentWeather, upcomingWeather, currentDate, targetDate, ref report);
        report.TempFeelsLike = targetForecast.TempFeelsLikeDay;
        report.WindSpeed = targetForecast.WindSpeed;
        report.WindDegree = targetForecast.WindDegree;
        report.Humidity = targetForecast.Humidity;
        report.TempMin = conditions.TempMin;
        report.TempMax = conditions.TempMax;

        return report;
    }

    /// <summary>
    /// Get the wind chill temperature for a forecast.
    /// </summary>
    /// <remarks>
    /// Only relevant to temperatures at or below 50 degrees and wind speeds
    /// greater than 3mph.
    /// <see href="https://www.weather.gov/media/epz/wxcalc/windChill.pdf"/>
    /// </remarks>
    /// <param name="forecast"></param>
    /// <returns></returns>
    public static double WindChill(Forecast forecast) {
        if (forecast.TempHigh > 50.0 || forecast.WindSpeed <= 3.0) {
            return forecast.TempHigh;
        }

        double windChill = 35.74 + (0.6215 * forecast.TempHigh) - (35.75 * Math.Pow(forecast.WindSpeed, 0.16)) + (0.4275 * forecast.TempHigh * Math.Pow(forecast.WindSpeed, 0.16));

        return windChill;
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
    /// <param name="currentDate">Today or the day to consider as today</param>
    /// <param name="targetDate">Assumed to always be later than today</param>
    /// <param name="report">A ConditionsReport to update the precipitation subtotals of</param>
    /// <returns>
    /// Likelyhood of being dry, between 0-1 with 1 being definitely dry and 0 being definitely wet
    /// </returns>
    public static double ChanceDry(
        List<WeatherHistory> recentWeather,
        List<Forecast> upcomingWeather,
        DateOnly currentDate,
        DateOnly targetDate,
        ref ConditionsReport report
    ) {
        if (targetDate < currentDate) {
            throw new ArgumentException("Target date is before current date");
        } else if (recentWeather.Count() < 90) {
            throw new ArgumentException(
                "Predicting crag dryness requires at least 90 days of weather history"
            );
        }

        double chanceDry = 1.0;
        double precipitation = PrecipitaionTotal(recentWeather, upcomingWeather, currentDate, targetDate);
        chanceDry -= precipitation * DryRate;

        double snowpack = EstimatedSnowpack(recentWeather);
        chanceDry -= snowpack * DryRate;
        report.EstimatedSnowpack = snowpack;
        
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
        chanceDry = chanceDry > 0.0 ? chanceDry : 0.0;
        report.PreciptationDayBefore = precipitationDayBefore;

        Forecast targetForecast = upcomingWeather.Where(
            f => DateOnly.FromDateTime(f.Date.Date) == targetDate
        ).First();
        // Amount of precipitation day of increases the significance of the chance of precipitation
        double precipDayOfSignificance = 1.0 + ((targetForecast.Rain + targetForecast.Snow) / 10.0);
        chanceDry -=  targetForecast.Pop * precipDayOfSignificance;
        report.PreciptationDayOf = targetForecast.Rain + targetForecast.Snow;

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
            if (history.Precipitation > 0.0) {
                precip += history.Precipitation;
            } else if (history.TempHigh > 32) {
                double dryFactor = 0.0;
                dryFactor = (history.TempHigh - 32) * 0.02;
                dryFactor *= 1 - (history.Clouds / 100); // more clouds, less drying
                dryFactor *= 1 - (history.Humidity / 100); // more humidity, less drying
                dryFactor *= 1 + (history.WindSpeed / 100); // more wind, more drying
                precip -= dryFactor;

                precip = precip > 0.0 ? precip : 0.0;
            }
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
            if (forecast.Rain > 0.0 || forecast.Snow > 0.0) {
                precip += forecast.Rain;
                precip += forecast.Snow;
            } else if (forecast.TempHigh > 32) {
                double dryFactor = 0.0;
                dryFactor = (forecast.TempHigh - 32) * 0.02;
                dryFactor *= 1 - (forecast.Clouds / 100); // more clouds, less drying
                dryFactor *= 1 - (forecast.Humidity / 100); // more humidity, less drying
                dryFactor *= 1 + (forecast.WindSpeed / 100); // more wind, more drying
                precip -= dryFactor;

                precip = precip > 0.0 ? precip : 0.0;
            }
        }

        return precip;
    }

    /// <summary>
    /// Make an estimated guess about how much snow is on the ground as of current date based on snow in the past 90
    /// days minus melting.
    /// 
    /// TODO: This is not doing a great job -- it's over estimating snow pack quite dramatically.
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
                // Base melt rate = 0.5mm melt per day for every degree above freezing
                meltFactor = (weather.TempHigh - 32) * 0.5;
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
}
