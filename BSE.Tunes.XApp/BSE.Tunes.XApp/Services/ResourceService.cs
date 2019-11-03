using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace BSE.Tunes.XApp.Services
{
    public class ResourceService : IResourceService
    {
        public string GetString(string key)
        {
            return AppSettings.ResourceManager.Value.GetString(key, CultureInfo.CurrentCulture);
        }

        public string GetString(string key, string defaultValue)
        {
            var translation = AppSettings.ResourceManager.Value.GetString(key, CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(translation) == true)
            {
                translation = defaultValue;
            }
            return translation;
        }
    }
}
