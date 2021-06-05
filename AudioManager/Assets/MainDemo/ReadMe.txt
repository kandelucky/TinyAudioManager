Warning!
For correctly working:
You need to add in the project new tags - "music" and "fx". (Edit > Project Settings > Tags and Layers)
Add all demo scenes to Build Settings window (File > Build Settings..)
// cenes folders (MainDemo > DemoScenes & >> AllOptions/MinimumOptions & ZombiSoft > TinyAudioManager > Demo)
This project was create first off all for main mobile game. For correctly visualise you need to change game windows resolutions to any mobile portrait sizes. (for example 1280x720)

Added:

. Music/Ambient/Fx On/Off (if OFF - sounds will not played)

. If Music/Ambient(M/A) is OFF and the game will try play any M/A - AudioManager save name this M/A and when M/A will ON - will started last M/A

. If Music/Ambient was stopped and switch them on On/Off - Music/Ambient will not played

. Sounds DontDestroyOnLoad

. Sliders volume settings

. Toggles/Switchers On/Off settings

. Music/ambient custom or default fade settings (default settings You can sets in AudioManager)

. Music/Ambient custom fade play, now is with delay and stops old music/ with default fade

. New fade options (old AudioSource will deleted after fade, at the same time new music/ambient will playing in new AudioSource with fade)

. Sliders options

If master slider volume = 0 - all sliders change colors and all toggles/switchers becomes OFF positions
If master slider volume = 0 and You will change any Other slider value - all sliders change colors and all toggles/switchers becomes ON positions. master volume slider value become on 0.5 (min 0 = OFF, max 1)
If volume sliders value = 0 - Toggles/Switchers becomes on OFF positions
If volume sliders value = 0 and become more than 0, Toggles/Switchers becomes on ON positions
If Toggles/Switchers becomes on ON position - the volume slider value become on 0.5 (min 0 = OFF, max 1)

. Scene starts with Music/Ambient (new Music/Ambient if next scene) or not

. Continues from previous scene Music/Ambient or not

. If new scene have new music, stop old music(default fade) and start new(default fade)

. Random Ambient

. Now every Fx sounds play as new AudioSource file and destroyed when playing ends (for playing 2 or more sound fx at the same time)

. Now You can play Fx sounds as many times as you want (like loop)

. Saves/Loads sliders/switches/toggles values

Fixed

. Ambient is always fade and it have new delay funqcion

. Simple play music funqcion

. Simple play ambient funqcion

. Simple funqcion for to call play fxSound funqcion (with name)

. Looping now is always true (for not looping music You can play sound fx)


// Also
// Main demo contains the font and sounds/ambients/music files from TinyAudioManager
// Unity assest size (7,6 MB.) contains ~ 6 MB. music and grafick files