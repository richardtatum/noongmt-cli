# NOONGMT CLI

This is a CLI tool to allow for adding, updating and listing posts on https://noongmt.com

## Run this locally

Publish the .NET console app for the appropriate architecture:

### Linux
```sh
dotnet publish -r linux-x64 -o /path/to/output/folder
```
### Windows
```sh
dotnet publish -r win-x64  -o /path/to/output/folder
```

Override the default values in the `/path/to/output/folder/appsettings.json` file:
```json
  "NoonGmtOptions": {
    "BaseUrl": "http://localhost",
    "ApiKey": "YOUR-API-KEY-HERE"
  },
  "SpotifyOptions": {
    "AuthenticationEndpoint": "https://accounts.spotify.com/api/token",
    "TrackInformationEndpoint": "https://api.spotify.com/v1/tracks",
    "ClientId": "YOUR-CLIENT-ID",
    "ClientSecret": "YOUR-CLIENT-SECRET"
  },
  "AuthenticationOptions": {
    "FilePath": "spotify-auth.json",
    "ExpiryThreshold": 300
  }
```

Run the application:
```
# ./NoonGMT.CLI
Usage: NoonGMT.CLI [command]

NoonGMT.CLI

Commands:
  list
  add
  update
  get
  count
  queue

Options:
  --completion    Generate a shell completion code
  -h, --help      Show help message
  --version       Show version
```

## Bonus
Set the output of the publish command to a folder be in your `$PATH`, or copy every file afterwards to allow calling the command from anywhere:
```
# noongmt list
[31/01/2024] Track: Otis Kane - Run, Description: The bass hits hard the moment the song starts and is quickly followed by Kanes' velvet vocals. I can't get enough of it.
[30/01/2024] Track: Lous and The Yakuza - Dilemme, Description: I first came across Lous and The Yakuza through NPRs' Tiny Desk Concert and I have been transfixed ever since.
[29/01/2024] Track: Pa Salieu - Betty, Description: Some upbeat hustle vibes to get you through a Monday. Pa isn't the only one who could do with a million cash.
[28/01/2024] Track: H.E.R. - Do To Me, Description: The vocals from H.E.R are like honey in this. The weekend is almost over but this makes it all the more palatable.
[27/01/2024] Track: Raheaven - 7AM, Description: The Saturday morning come-down.
```
