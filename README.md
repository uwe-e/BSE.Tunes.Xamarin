# BSEtunes Xamarin client
This is the Xamarin cross platform client of the BSEtunes system. With this client you can play singles, whole albums, the content of your self created playlists and randomized, the whole content of your selected BSEtunes.

![BSEtunes](Images/bsetunes_iphone_animation.gif)

All the views support dark and light mode on

### IOS

![IOS Home Page](Images/home_dark_ios.PNG) | ![IOS Home Page](Images/home_light_ios.PNG) 

and on
### Android

![Android Home Page](Images/home_dark_android.PNG) | ![Android Home Page](Images/home_light_android.PNG)

## Features
### Player

![Player View](Images/player_view_ios.png)

At the moment, the player is only available on ios. The app uses an adaption of the AudioToolbox. The reason is that all the audio files on the BSEtunes server are protected. Because of this, the request headers have to contain security information. The audiosfilestream allows a separate file download with such http requests.

#### Semi Important Information

Due to the copyrights of the streamed music files, the load of our server and perhaps because of the limited bandwidth during the operation of the server, a connection without a login is not possible. For this reason you will need a user account to connect to our server.

- You are not able to register yourself as a user via the app.
- A registration can only be done by the operator of the BSEtunes server.

### Globalization
The App supports globalization. Currently available languages are german and english.

### The playlist patchwork images

A playlist image is a composition of the cover images of the first four playlist entries. These cover images are stitched together via [Skia](https://github.com/mono/SkiaSharp).
The stitching process needs some performance. Because of that, the created images in different sizes are cached. If a playlist is changed, only the images for this playlist will be recreated.

### Settings

![Settings View](Images/settings_view_ios.png)

All settings can be changed in the settings view.

## Tools

The App was created using the following tools
- [Prism.DryIoc.Forms](https://prismlibrary.com/index.html): MVVM framework for building loosely coupled, modular, maintainable, and testable XAML applications.
- [SkiaSharp.Views.Form](https://github.com/mono/SkiaSharp): a cross-platform 2D graphics API for .NET platforms based on Google's Skia Graphics Library.
- [Xamarin.FFImageLoading](https://github.com/luberda-molinet/FFImageLoading): Image loading, caching & transforming library for Xamarin
- [CardsView](https://github.com/AndreiMisiukevich/CardView): CardsView | CarouselView | CoverflowView | CubeView framework for Xamarin.Forms

## BSE System

The whole BSE system constists of the following components

- BSEadmin: a Windows client for to adminster the system's music content.
	Its partly described at [code project](https://www.codeproject.com/Articles/43068/BSEtunes)
- BSEtunes UWP client at https://github.com/uwe-e/BSE.Tunes
- BSEtunes Web API at https://github.com/uwe-e/BSE.Tunes
  
