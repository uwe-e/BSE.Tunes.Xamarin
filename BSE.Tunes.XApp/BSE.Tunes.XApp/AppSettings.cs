using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Utils;
using System;
using System.Reflection;
using System.Resources;
using Xamarin.Essentials;

namespace BSE.Tunes.XApp
{
    public static class AppSettings
    {
        const string ResourceId = "BSE.Tunes.XApp.Resx.AppResources";

        public static readonly Lazy<ResourceManager> ResourceManager =
            new Lazy<ResourceManager>(() => new ResourceManager(
                ResourceId,
                IntrospectionExtensions.GetTypeInfo(typeof(AppSettings)).Assembly
                )
            );

        public static string ServiceEndPoint
        {
            get => Preferences.Get(nameof(ServiceEndPoint), null);
            set => Preferences.Set(nameof(ServiceEndPoint), value);
        }

        public static User User
        {
            get => PreferencesHelpers.Get(nameof(User), default(User));
            set => PreferencesHelpers.Set(nameof(User), value);
        }
    }
}
