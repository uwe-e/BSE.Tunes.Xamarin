﻿using System;
using System.IO;

namespace BSE.Tunes.XApp.Extensions
{
    public static class UriBuilderExtension
    {
        internal static void AppendToPath(this UriBuilder builder, string pathToAdd)
        {
            var completePath = Path.Combine(builder.Path, pathToAdd);
            builder.Path = completePath;
        }
    }
}
