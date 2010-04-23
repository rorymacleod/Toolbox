using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Config = System.Configuration.Configuration;

namespace BlueDot.Configuration
{
    /// <summary>
    /// Abstract base class for a <see cref="SettingsProvider"/> that combines multiple sets of setting values.
    /// </summary>
    public abstract class MultiSettingsProviderBase : SettingsProvider
    {
        /// <summary>
        /// The key used in <see cref="SettingsContext"/> to optionally supply the setting file's path.
        /// </summary>
        public const string SettingsFilePathKey = "__SettingsFilePathKey";

        protected static Regex ValidCriteriaTermPattern = new Regex(@"\A/w\z", RegexOptions.Compiled);
        protected static string[] ReservedSettingsContextKeys = new[] { "Machine", "User", "SectionName", "SettingsClassType",
            SettingsFilePathKey };



        /// <summary>
        /// Event that is raised when criteria for selecting settings files are being determined.
        /// </summary>
        public event EventHandler<EventArgs<SettingsContext>> AddingCriteria;

        /// <summary>
        /// Gets or sets the name of the currently running application.
        /// </summary>
        /// <returns>A <see cref="String"/> that contains the application's shortened name, which does not contain a full path or
        /// extension, for example, SimpleAppSettings.</returns>
        public override string ApplicationName { get; set; }



        /// <summary>
        /// Adds criteria for selecting settings files to the given context.
        /// </summary>
        /// <param name="context">A <see cref="SettingsContext"/> describing the current application use.</param>
        protected virtual void AddCriteria(SettingsContext context)
        {
            context["Machine"] = Environment.MachineName;
            context["User"] = Environment.UserName;

#if DEBUG
            context["Debug"] = null;
#endif

            OnAddingCriteria(this, new EventArgs<SettingsContext>(context));
        }

        /// <summary>
        /// Returns the collection of settings property values for the specified application instance and settings
        /// property group.
        /// </summary>
        /// <returns>
        /// A <see cref="SettingsPropertyValueCollection"/> containing the values for the specified settings property
        /// group.
        /// </returns>
        /// <param name="context">A <see cref="SettingsContext"/> describing the current application use.</param>
        /// <param name="properties">A <see cref="SettingsPropertyCollection"/> containing the settings property group
        /// whose values are to be retrieved.</param>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, 
            SettingsPropertyCollection properties)
        {
            var values = new SettingsPropertyValueCollection();
            var sectionName = GetSectionName(context);
            context["SectionName"] = sectionName;
            AddCriteria(context);

            var dict = new Dictionary<string, object>();
            foreach (var pair in ValidateAndSortCriteria(context))
            {
                ReadSettingsGroup(pair.Key, pair.Value, sectionName, dict, context);
            }
            ReadLocalSettings(sectionName, dict);

            foreach (SettingsProperty property in properties)
            {
                var value = new SettingsPropertyValue(property);
                if (dict.ContainsKey(value.Name))
                {
                    value.SerializedValue = dict[value.Name];
                }
                else if (property.DefaultValue != null)
                {
                    value.SerializedValue = property.DefaultValue;
                }
                else
                {
                    value.PropertyValue = null;
                }
                value.IsDirty = false;
                values.Add(value);
            }

            return values;
        }

        /// <summary>
        /// Gets the base path.
        /// </summary>
        /// <param name="context">A <see cref="SettingsContext"/> describing the current application use.</param>
        /// <returns>The fully-qualified name of the default settings file.</returns>
        protected virtual string GetSettingsFilePath(SettingsContext context)
        {
            if (context[SettingsFilePathKey] != null)
                return context[SettingsFilePathKey].ToString();

            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var type = context["SettingsClassType"] as Type;
            var file = type != null ? type.Module.Name : context["SectionName"] as string;
            if (string.IsNullOrEmpty(file))
                file = "App";

            return Path.Combine(dir, file + ".config");
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> ValidateAndSortCriteria(SettingsContext context)
        {
            var list = new List<KeyValuePair<string, string>>();
            if (context["Machine"] != null)
                list.Add(new KeyValuePair<string,string>("Machine", (context["Machine"] ?? string.Empty).ToString()));
            if (context["User"] != null)
                list.Add(new KeyValuePair<string,string>("User", (context["Machine"] ?? string.Empty).ToString()));

            foreach (var key in context.Keys.OfType<string>().Except(ReservedSettingsContextKeys))
            {
                var value = context[key] == null ? null : context[key].ToString();
                if (ValidCriteriaTermPattern.IsMatch(string.Format("{0}{1}", key, value)))
                {
                    list.Add(new KeyValuePair<string,string>(key, value));
                }
            }

            return list;
        }

        /// <summary>
        /// Reads the latest settings for the current user and adds the values to the given dictionary.
        /// </summary>
        /// <param name="sectionName">The name of the section in the settings file to read.</param>
        /// <param name="values">A dictionary to add the read settings values to.</param>
        protected virtual void ReadLocalSettings(string sectionName, IDictionary<string, object> values)
        {
            var appDir = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.DataDirectory :
                Application.CommonAppDataPath;
            var appPath = Path.Combine(appDir, Path.GetFileName(Assembly.GetEntryAssembly().Location) + ".config");
            ReadConfigSection(null, null, OpenConfig(appPath), sectionName, values);

            var userDir = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.DataDirectory :
                Application.CommonAppDataPath;
            var userPath = Path.Combine(userDir, Path.GetFileName(Assembly.GetEntryAssembly().Location) + ".config");
            if (userPath != appPath)
            {
                ReadConfigSection(null, null, OpenConfig(userPath), sectionName, values);
            }
        }

        protected Config OpenConfig(string path)
        {
            if (!File.Exists(path))
                return null;

            var map = new ExeConfigurationFileMap { ExeConfigFilename = path };
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            return config;
        }

        protected abstract void ReadSettingsGroup(string key, string value, string sectionName, 
            IDictionary<string, object> values, SettingsContext context);

        /// <summary>
        /// Reads settings from the specified file.
        /// </summary>
        protected virtual void ReadConfigSection(string key, string value, Config config, string sectionName, 
            IDictionary<string, object> values)
        {
            if (config == null)
                return;

            var prefix = string.Format("{0}/{1}", key, value).TrimEnd('/');
            var sections = new[] {
                config.GetSection(prefix + "applicationSettings/" + sectionName) as ClientSettingsSection,
                config.GetSection(prefix + "userSettings/" + sectionName) as ClientSettingsSection,
            };

            foreach (var section in sections)
            {
                if (section == null)
                    return;

                foreach (SettingElement element in section.Settings)
                {
                    if (element.SerializeAs == SettingsSerializeAs.String)
                    {
                        values[element.Name] = element.Value.ValueXml.InnerText;
                    }
                    else
                    {
                        values[element.Name] = element.Value.ValueXml;
                    }
                }
            }
        }


        /// <summary>
        /// Gets the name of the section in the source settings file get settings from.
        /// </summary>
        /// <param name="context">A <see cref="SettingsContext"/> describing the current application use.</param>
        /// <returns>The name of a section in the settings file.</returns>
        protected virtual string GetSectionName(SettingsContext context)
        {
            var group = context["GroupName"] as string;
            var key = context["SettingsKey"] as string;
            var format = string.IsNullOrEmpty(group) || string.IsNullOrEmpty(key) ? "{0}{1}" : "{0}.{1}";
            var name = string.Format(format, group, key);

            return name;
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes
        /// specified in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
                name = GetType().Name;

            base.Initialize(name, config);
        }

        /// <summary>
        /// Raises the <see cref="AddingCriteria"/> event.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The event's arguments.</param>
        protected virtual void OnAddingCriteria(object sender, EventArgs<SettingsContext> e)
        {
            if (AddingCriteria != null)
                AddingCriteria(sender, e);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            var sectionName = GetSectionName(context);

        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            throw new NotImplementedException();
        }

        public void Reset(SettingsContext context)
        {
            throw new NotImplementedException();
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            throw new NotImplementedException();
        }
    }
}