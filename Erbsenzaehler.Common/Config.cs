using System;
using System.Configuration;

namespace Erbsenzaehler
{
    public static class Config
    {
        private const string SettingPrefix = "Erbsenzaehler";
        
        public static string DatabaseConnectionString => Setting("DatabaseConnectionString", null) ?? ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
        
        public static string Setting(string key, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(SettingPrefix + "." + key);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            value = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            value = ConfigurationManager.AppSettings[SettingPrefix + "." + key];
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            value = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}