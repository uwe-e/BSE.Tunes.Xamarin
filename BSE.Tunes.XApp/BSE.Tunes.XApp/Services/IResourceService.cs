using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp.Services
{
    public interface IResourceService
    {
        string GetString(string key);
        string GetString(string key, string defaultValue);
    }
}
