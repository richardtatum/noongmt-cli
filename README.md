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
[24/01/2024] Track: 52pGXXhSsBPx3jOSxlU35N, Description: A day doesn't really go by without me muttering 'Shoulda pulled up' under my breath
[23/01/2024] Track: 5OaTl9fKnzumyRLFrTKhOp, Description: Listening to me trying to remember the lyrics must be hilarious.
[22/01/2024] Track: 0hquQWY3xvYqN4qtiquniF, Description: My only real take away from the (decidedly average) Elvis film.
[21/01/2024] Track: 17phhZDn6oGtzMe56NuWvj, Description: Definitely go watch his cover of Tennessee Whiskey on YouTube
[20/01/2024] Track: 0dIgcqNnTDWT3q8DT5KyX3, Description: Good Friday vibes delivered on a Saturday
```
