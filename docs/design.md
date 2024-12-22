# GunksAlert Design Doc

- [Overview](#overview)
- [Scope, key features and MVP](#features)
- [Forecasting and alerts](#alert-forecasting)
- [Architecture](#architecture)
- [Components and services](#components)
- [User interface](#ui)
- [Entities](#entities)
    - [Crag](#crag)
    - [Forecast](#forecast)
    - [WeatherHistory](#weather-history)
    - [AlertCriteria](#alert-criteria)
    - [ClimbableConditions](#climbable-conditions)
    - [AlertPeriod](#alert-period)
    - [User](#user)
- [Challenges and open questions](#questions)
- [Gathering local knowledge on what makes for "good" conditions](#local-knowledge)


## <a name="overview"></a>Overview
This app alerts climbers when conditions are looking promising in various climbing desitinations. It's purpose is to provide a heads up to climbers when conditions are looking good during the months of the year when climbing is generally "out of season". For instance, from December through about April every year climbing outdoors in Vermont is typically not happening. During these months, a Vermont climber could subscribe to GunksAlert to receive a text message when there has been a long spell of dry, sunny weather and a warm forecast in New Paltz, NY for the coming weekend.

This doc covers the key features and overall design of the app.

## <a name="features"></a>Scope, key features and MVP
The initial version of this app should have the following features:

- 1 climbing destination to monitor (The Gunks, duh!)
- Static criteria set for what triggers an alert (e.g. 5 days of no precipitation, 45 degrees F, sunny)
- Send alerts via email
- Allow users to subscribe to alerts with an email address
- Monitor condition status for weekends
- Reasonable bot/spam protection to prevent phony sign-ups

Features that would be worth trying to implement soon afterwards:

- Multiple climbing destinations (Farley, MA would be nice)
- Monitor condition status for any day of the week
- Allow users to select which days of the week they're interested in getting alerts for
- Dynamic criteria editable by the user with basic templates available to simplify the UI
- Variable alert timing (how many days in advance do you want to know, how to handle repeat notifications)
- Cancel/follow-up alerts if forecast changes for the worse
- Send alerts via SMS
- Mobile friendly\*

> \*Maybe this should be a priority from the start?

Way down the road:

- Native mobile app
- Real time monitoring with "condition score"
- Advanced control over criteria of what makes for good conditions
- Long range forecasting with specialized external services
- Allow users to add  their own crags

## <a name="alert-forecasting"></a>Forecasting and alerts
Measuring the quality of conditions for determining if an upcoming period will be climbable should be based on:

- The weather history (5-10 days)
    - Precipition amount
    - Frequency/most recent precipitation event
    - Number of days sunny/dry/windy
    - Temperatures
    - Snow pack
- The forecast
    - Temperature
    - Precipitation chance/amount
    - Sky cover
    - Wind speed and direction
- The criteria selected by the user/determined by locals for what constitutes "good"

## <a name="architecture"></a>Architecture
This section covers

- Important compenents and services
- External services and third party libraries

### <a name="components"></a>Components and services
Here's a rough list of the key services this app will likely make use of.

- **ForecastManager**: This service will be responsible for fetching forecast data and storing it in the database
- **WeatherHistoryManager**: This service will be responsible for fetching weather history data and storing it in the database
- **ConditionsChecker**: This service will take in "go conditions", recent weather history, and forecast for an upcomming period and output whether or not the forecast matches the go conditions -- i.e. should we dispatch an alert.
- **AlertSender**: This should likely be an interface for describing classes that dispatch alerts. Alerts could be sent via text, email or some other method.

### External services and third party libraries
This app will make use of the following services and libraries

- **[OpenWeatherMap API](https://openweathermap.org/api)**: this service provides weather forecast and history for localized areas anywhere in the world. It allows for up to 1000 API calls per month for free.
- **React**: for front end components
- **A CSS framework**: To speed up development, I'll likely want to use some kind of CSS framework/design system. I don't think Bootstrap is the right fit for this project based on the style I'm looking for, so still shopping around on this one.
- **An SMS platform**: if this app is going to send text messages, I'll need to look for a reasonable priced (free?!) platform for sending those.

## <a name="ui"></a>User interface
The user interface for this app should

- Prioritize simplicity in design and navigation
- Be designed as mobile first
- Emphasize easy signup
- Allow for progressive customization for the user (e.g. editing climbable conditions)
- Have a look representative of other respected outdoor focused sites (Access Fund, Mountain Project, etc.)

When a user first opens the app they should see a simple tag line that quickly communicates the purpose of the app. Something like:

> Get notified when it's time to climb. [Sign up for condition alerts](#).

The sign up form should take in the users contact info, the crag they want to watch (just the Gunks to start), the days of the week they want to be notified of climbable conditions, and the date range to monitor conditions for.

One other required page would be for allowing a user to update/remove their contact preferences.

To start with, the UI could be as simple as these two pages. Providing a more detailed about page though would also be a good idea. Eventually, we'll want to show some other information such as

- **Current and forecasted conditions**: Show how things are looking at monitored crags. Can make use of weather icons to give the feel of a weather app. Could provide a "condition rating" using a bad, ok, good type of system.
- **Manage personalized climable conditions**: Provide a profile panel for a user to select what they consider climbable conditions.

## <a name="entities"></a>Entities
This app will need to store some data. This section covers the primary models used by the app.

### <a name="crag"></a>Crag
A `Crag` is  the location of a climbing area. When fetching forecast data and weather history, this entity is stores the data needed to construct the forecast/history query.

| Field             | Data type | Description                     |
| ----------------- | --------- | --------------------------------|
| Id                | int       | Primary key                  |
| Name              | string    | The crag's name, e.g. The Gunks |
| Longitude         | double    | Longitude of forecast location  |
| Latitude          | double    | Latitude of forecast location   |
| City*             | string    | Town/City of the crag           |
| State/Province*   | string    | State/Province of the crag      |
| Country*          | string    | Country of the crag             |

> \*Can probably leave of initial version

### <a name="forecast"></a>Forecast
The `Forecast` entity represents the weather forecast for a period of time.

| Field             | Data type      | Description                  |
| ----------------- | -------------- | ---------------------------- |
| Id                | int            | Primary key                  |
| Date              | DateTimeOffset | The forecast/history date    | 
| Summary           | string         | Human readable summary       |
| TempLow           | int            | Morning temperature          |
| TempHigh          | int            | Day temperature              |
| TempFeelsLike     | int            | Day temperature perception   |
| WindSpeed         | int            | Wind speed, mp/h             |
| WindGust          | int            | Wind gust, mp/h              |
| WindDegree        | int            | Wind direction               |
| Clouds            | double         | Percent of cloud cover       |
| Humidity          | double         | Precent humidity             |
| Pop               | double         | Probability of precipitation |
| Rain              | double         | Volume of rain               |
| Snow              | double         | Volume of snow               |
| DailyWeatherId    | int            | Standard weather summary ID  |

### <a name="weather-history"></a>WeatherHistory
The `WeatherHistory` entity represents the weather of a previous day. In structure, it's a simpler version of a `Forecast`.

| Field             | Data type      | Description                  |
| ----------------- | -------------- | ---------------------------- |
| Id                | int            | Primary key                  |
| Date              | DateTimeOffset | The forecast/history date    |
| Temp              | int            | Day temperature              |
| WindSpeed         | int            | Wind speed, mp/h             |
| WindDegree        | int            | Wind direction               |
| Clouds            | double         | Percent of cloud cover       |
| Humidity          | double         | Precent humidity             |
| Rain              | double         | Volume of rain               |
| Snow              | double         | Volume of snow               |

### <a name="alert-criteria"></a>AlertCriteria
The `AlertCriteria` entity sets the criteria for when an alert should be sent. It ties the `ClimbableConditions` and `AlertPeriod` entities to a user.

| Field                 | Data type      | Description            |
| --------------------- | -------------- | -----------------------|
| Id                    | int            | Primary key            |
| ClimbableConditionsId | int            | ID of the conditions   |
| CragId                | int            | ID of the crag         |
| AlertPeriod           | int            | ID of the alert period |
| UserId                | int            | ID of the user         |

### <a name="climbable-conditions"></a>ClimbableConditions
The `ClimbableConditions` entity represents the conditions that would be make for a good enough day to go climbing at a given crag.

| Field             | Data type | Description                  |
| ----------------- | --------- | ---------------------------- |
| Id                | int       | Primary key                  |
| CragId            | int       | ID of the crag               |
| TempMin           | int       | Min daytime temperature      |
| TempMax           | int       | Max daytime temperature      |
| TempIdeal         | int       | Ideal temperature            |
| WindSpeed         | int       | Wind speed, mp/h             |
| WindGust          | int       | Wind gust, mp/h              |
| WindDegree        | int       | Wind direction               |
| Clouds            | double    | Percent of cloud cover       |
| Humidity          | double    | Precent humidity             |
| Pop               | double    | Probability of precipitation |
| Rain              | double    | Volume of rain               |
| Snow              | double    | Volume of snow               |

### <a name="alert-period"></a>AlertPeriod

The `AlertPeriod` entity is the date range for which a user would like to receive alerts of climbable conditions.

| Field                 | Data type      | Description              |
| --------------------- | -------------- | ------------------------ |
| Id                    | int            | Primary key              |
| StartDate             | DateTimeOffset | Start date of the period | 
| EndDate               | DateTimeOffset | End date of the period   | 

### <a name="user"></a>User
The `User` entity stores information about climbers who want to receive alerts.

| Field             | Data type      | Description                           |
| ----------------- | -------------- | ------------------------------------- |
| Id                | int            | Primary key                           |
| Name              | string         | The username                          |
| Email             | string         | Email address                         |
| Phone             | string         | Phone number                          |
| WatchedCrags\*    | Collection     | List of crags they want alerts for    |
| Conditions\*      | Collection     | List of conditions for watched crags  |

> \* Probably not needed for version 1. Eventually though we want to allow users to determine which crags they want to be subscribed for and set up personalized "good conditions" for those crags.

## <a name="questions"></a>Challenges and open questions
Some things to think about

- Can I get an MVP out the door before it's too late in the winter for this to be interesting?
- How can I ensure that this app doesn't run wild on the API calls and go over the 1000 calls/mo limit for the free plan on OpenWeatherMap?
- How do I be a sure as possible that alerts are sent when the conditions actually will be good for climbing? I'm not a meteorolgist after all and I don't have specialized local knowledge of some of these crags.
- How to protect against bot signups? This will be especially important if this app sends SMS messages and needs to stay below a certain threshold to remain free.

## <a name="local-knowledge"></a>Gathering local knowledge on what makes for "good" conditions
To make sure that alerts are sent when climbing conditions will be actually good, it might be useful to get some local info about what would be reliable indicators that conditions will be good. Likely, I could probably get tips on this by posting on Mountain Project and explaining the goal.
