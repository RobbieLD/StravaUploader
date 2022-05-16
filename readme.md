# Strava Uploader
This is an app which enables automatic uploading of activities to strava with out the need to use the proprietary apps which conme with most devices.

![Release](https://github.com/RobbieLD/StravaUploader/actions/workflows/publish/badge.svg)


## Installation
1. Simply download te `StravaUploader.exe` from the releases page along with the `appsettings.json` file. This is the master config for the app and needs to be in the same directory as the app.
2. Update `appsettings.json` with the values as required based on the table below in the configuration section.
3. [Optional] Add a shortcut to the app in the windows start up directory as described [here](https://support.microsoft.com/en-us/windows/add-an-app-to-run-automatically-at-startup-in-windows-10-150da165-dcd9-7230-517b-cf3c295d89dd).

## Usage
This all will run in the system tray and show a small orange Strava Icon while it's running. It will communicate with you via windows notifications. Right clicking on the system tray icon allows you to view log files, the `appsettings.json` and see the current version as well exiting the app.

## Configuration (appsettings.json)

| Property            | Description                                                                                                                                                                                                                                                                                                         | Default                           | Type     |
|---------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------|----------|
| Auth.CallbackUrl    | This is the URL the app uses to listen on for the auth call back. You only need to adjust this if something else is listening on that port                                                                                                                                                                          | http://localhost:8080/auth/       | string   |
| Auth.Scopes         | These are the required scopes for uploading activities to Strava                                                                                                                                                                                                                                                    | activity:read_all, activity:write | string[] |
| Auth.ResponseType   | This is the type of response Strava will send. You don't need to change this.                                                                                                                                                                                                                                       | code                              | string   |
| Auth.ClientId       | The client id from the [API page](https://developers.strava.com/) of Strava (you'll need to sign up to the Strava API for this).                                                                                                                                                                                                                      |                                   | string   |
| Auth.ClientSecret   | The client secret from the [API page](https://developers.strava.com/) of (you'll need to sign up to the Strava API for this).                                                                                                                                                                                                                         |                                   | string   |
| Device.Name         | This is the name of the device the app will watch for being plugged in to search for activities. To find the name of your device, connect it and run the Powershell commandlet `Get-Volume`. The `Friendly Name` column is what you'll need to use, case is important. For Germin devices this is just `GARMIN`.        |                                   | string   |
| Device.Path         | The path in your device (relative to the root) where the activities are stored. For example a Garmin device which has a folder in the root called GARMIN and a sub folder called `ACTIVITIES` would set the value like `GARMIN\\ACTIVITY`                                                                             |                                   | string   |
| Device.FileType     | The type of file to tell Strava you are uploading. This is probably the extension of your activity files but it it's worth checking [here](https://developers.strava.com/docs/uploads/#:~:text=Supported%20File%20Types,compatibility%20with%20other%20fitness%20applications.) to see if your device is supported. |                                   | string   |
| Device.CheckOnStart | Should the app check for the specified device when the app is started.                                                                                                                                                                                                                                              | true                              | boolean  |
| CheckForUpdates     | Should the app check for updates when it starts.                                                                                                                                                                                                                                                                    | true                              | boolean  |

## Compatibility
As this app uses a couple of windows only APIs to send toast notifications and detects devices being connected it's a Windows 10+ only.

## Trouble Shooting
Most errors which the app might encounter as well as diagnostic logs for HTTP calls are logged to the app log. This is located in the same directory as the app is running and can be located by right clicking on the system tray icon and selecting `View Logs`

## Contributing 
PRs or information for adding support for more devices is apreciated.
