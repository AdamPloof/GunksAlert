using System;
using System.Collections.Generic;
using Xunit;

using GunksAlert.Api.Services;
using GunksAlert.Api.Models;

namespace GunksAlert.Tests.Services;

public class WindChillTests {
    [Fact]
    public void ColdDaysNoWindNoEffect() {
        Forecast f = MakeForecast(20.0, 1.0);
        double windChill = ConditionsChecker.WindChill(f);

        Assert.Equal(20.0, windChill);
    }

    [Fact]
    public void WarmDaysHighWindNoEffect() {
        Forecast f = MakeForecast(65.0, 15.0);
        double windChill = ConditionsChecker.WindChill(f);

        Assert.Equal(65.0, windChill);
    }

    [Fact]
    public void WindChillMatchesNoaaCalculations() {
        Forecast f = MakeForecast(45.0, 10.0);
        double windChill = Math.Round(ConditionsChecker.WindChill(f), 1);

        Assert.Equal(39.8, windChill);
    }

    private Forecast MakeForecast(double tempHigh, double windSpeed) {
        Forecast forecast = new Forecast() {
            Date = new DateTimeOffset(),
            Summary = "Test",
            TempHigh = tempHigh,
            TempLow = tempHigh - 3,
            WindSpeed = windSpeed,
            WindGust = windSpeed + 3
        };

        return forecast;
    }
}
