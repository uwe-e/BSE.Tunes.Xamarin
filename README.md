# BSE.Tunes.Xamarin
The Xamarin version of BSEtunes can in ios play single songs, whole albums and randomized, the whole content of this BSEtunes.

It has a home page,

![IOS Home Page](Images/IMG_1108_1.PNG)

an album detail page,

![IOS Detail Page](Images/IMG_1112_1.PNG)

the albums page,

![IOS Albums Page](Images/IMG_1109_1.PNG)

a search page,

![IOS Search Page](Images/IMG_1110_1.PNG)

and a settings page

![IOS Settings Page](Images/IMG_1111_1.PNG)

As player, the app uses an adaption of the AudioToolbox. The reason therefore is, all the audio files on the BSEtunes server are protected. Because of that, the request headers have to contain security information. The audiosfilestream allows a separate file download with such http requests.
