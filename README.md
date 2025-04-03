# GunksAlert
GunksAlert is a web app that alerts climbers when conditions are looking promising in various climbing desitinations. It's purpose is to provide a heads up to climbers when conditions are looking good during the months of the year when climbing is generally "out of season".

For instance, from December through about April every year climbing outdoors in Vermont is typically not happening. During these months, a Vermont climber could subscribe to GunksAlert to receive a text message when there has been a long spell of dry, sunny weather and a warm forecast in New Paltz, NY for the coming weekend.

> **Heads up!** Currently in development. See the [design doc](./docs/design.md) for details on the plan.

## Features
Users can sign up for alerts at a crag of their choice. They can select days of the week and months of the year for days that they would like to be notified of good climbing conditions.

Alerts will be sent via SMS messages.

## How are climbable conditions predicted
GunksAlert fetches weather forecasts and history via [OpenWeatherMap](https://openweathermap.org/) to predict conditions. Generally speaking, it looks at weather data such as

- Estimated snowpack
- Recent precipitation events
- Temperature, wind, humidity, chance of precipitaion, etc. of each climbing day

The `ConditionsChecker` class handles most of the works of making predictions on climbing conditions.
