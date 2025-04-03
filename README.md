# GunksAlert
GunksAlert is a web app that alerts climbers when conditions are looking promising in various climbing desitinations. It's purpose is to provide a heads up to climbers when conditions are looking good during the months of the year when climbing is generally "out of season".

For instance, from December through about April every year climbing outdoors in Vermont is typically not happening. During these months, a Vermont climber could subscribe to GunksAlert to receive a text message when there has been a long spell of dry, sunny weather and a warm forecast in New Paltz, NY for the coming weekend.

> **Heads up!** Currently in development. See the [design doc](./docs/design.md) for details on the plan.

## Features
Users can sign up for alerts at a crag of their choice. They can select days of the week and months of the year for which they would like to be notified of good climbing conditions.

Alerts will be sent via SMS messages.

## How are climbable conditions predicted?
GunksAlert fetches weather forecasts and history via [OpenWeatherMap](https://openweathermap.org/) to predict conditions. Generally speaking, it looks at weather data such as

- Estimated snowpack
- Recent precipitation events
- Temperature, wind, humidity, chance of precipitaion, etc. of each climbing day

The `ConditionsChecker` class handles most of the works of making predictions on climbing conditions.

## Project structure
This project is separated into

- **GunksAlert.Cli**: A CLI app that is used for automating maintenance of weather data.
- **GunksAlert.Api**: The web app. Contains API endpoints used by the CLI tool as well as routes for standard views.

## Command line usage
Usage for the GunksAlert.Cli app:

```
GunksAlert CLI - Maintenance Tool
=================================

Description:
Execute maintenance tasks via the GunksAlert API for keeping weather history 
and forecast data up to date.

Usage:
gunks [options]

Examples:
gunks --update=weather-history
gunks --update=weather-history --date 2025-01-11
gunks --clear=forecast

Options:
-h, --help              Show this help message and exit.

-r, --refresh-weather   Refresh all weather records. Get most recent forecasts and weather
                        history remove duplicate histories, ensure there are a minimum of
                        90 days of weather history.

-d, --date <DATE>       Specify the date for operations like updating weather history
                        or clearing weather history. The date must be in the format
                        `yyyy-MM-dd`.

-s, --start_date <DATE> Specify the start date for range-based tasks like updating
                        updating or clearing weather history. The date must be in the format
                        `yyyy-MM-dd`.

-e, --end_date <DATE>   Specify the end date for range-based tasks like updating
                        updating or clearing weather history. The date must be in the format
                        `yyyy-MM-dd`.

-u, --update <VALUE>    Update forecast or weather history. Required value:
                        - `forecast`
                        - `weather-history`
                        Values are case-insensitive.

-c, --clear <VALUE>     Clear weather history or forecast. Required value:
                        - `forecast`
                        - `weather-history`
                        Values are case-insensitive.
```
