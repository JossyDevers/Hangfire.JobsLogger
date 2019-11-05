﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;

namespace Hangfire.JobsLogger.Helper
{
    internal static class Utility
    {
        public static string ReadStringResource(string resourceName)
        {
            var assembly = typeof(Utility).Assembly;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new MissingManifestResourceException($"Cannot find resource {resourceName}");

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
