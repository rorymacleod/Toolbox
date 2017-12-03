using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace BlueDot.Configuration
{
    /// <summary>
    /// Accesses application settings stored in a set of files, where the settings in a base file can be overridden by 
    /// increasingly context-specific files.
    /// </summary>
    public class MultiFileSettingsProvider : MultiSettingsProviderBase, IApplicationSettingsProvider
    {
        protected override void ReadSettingsGroup(string key, string value, string sectionName, 
            IDictionary<string, object> values, SettingsContext context)
        {
            var format = value == null ? "{0}{2}" : "{0}({1}){2}";
            var basePath = GetSettingsFilePath(context);
            var path = Path.ChangeExtension(basePath, string.Format(format, key, value, Path.GetExtension(basePath)));

            ReadConfigSection(null, null, OpenConfig(path), sectionName, values);
        }
    }
}
