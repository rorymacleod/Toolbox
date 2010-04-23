using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BlueDot.Configuration
{
    public class LocalFileMultiSettingsProvider : MultiSettingsProviderBase, IApplicationSettingsProvider
    {
        protected override void ReadSettingsGroup(string key, string value, string sectionName, 
            IDictionary<string, object> values, SettingsContext context)
        {
            ReadConfigSection(key, value, OpenConfig(GetSettingsFilePath(context)), sectionName, values);
        }
    }
}