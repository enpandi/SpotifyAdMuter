# SpotifyAdMuter
lightweight WinForms app that mutes the Spotify app on Windows when there are advertisements


## behavior
- SAM does NOT block ads, it only mutes them. therefore, there will occasionally be stretches of silence.
- when Spotify starts playing a song or gets paused, SAM will unmute Spotify.
- when Spotify starts playing an advertisement, SAM will mute Spotify.
- when you click the green button in the app, SAM will toggle Spotify's mute state. you can use this button to mute songs or listen to ads if you want.


## troubleshooting / todo list
- if the ads aren't being muted and the button doesn't do anything, restart SAM. this probably has something to do with whether you opened SAM before or after Spotify.
- if the ads aren't being muted but the button still works, contact me.
- sometimes, Spotify will play video ads. SAM does not detect these ads and is unable to mute them. i've noticed that video ads only happen when i'm on the Spotify app, so if you spend long periods of time browsing Spotify then SAM may not be as useful to you. i might fix this in the future.
- in the future, i want to make SAM minimize to tray so that it doesn't add window clutter.
