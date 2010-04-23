using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace BlueDot
{
    public static class SettingsExtensions
    {
        public static void Set<TSettings>(this ApplicationSettingsBase settings,
            Expression<Func<TSettings, object>> property, object value)
        {
            var propertyName = property.GetPropertyInfo(typeof(TSettings)).Name;
            settings[propertyName] = value;
        }
    }
}