using System;
using PDMF.Data.Utilities.Configuration.Abstract;

namespace PDMF.Data.Utilities.Configuration
{
    public static partial class Configurations
    {
        private static bool _configure = true;
        
        public static void InitializeConfigurationManager(IConfigurationManager configurationManagerRealization)
        {
            if ( _configurationManager != null)
            {
                throw new InvalidOperationException("Configuration manager is already initialized.");
            }
            
            _configurationManager = configurationManagerRealization;
            
            if (!_configure)
            {
                return;
            }

            _configure = false;
        }

        private static IConfigurationManager _configurationManager;

        public static string GetConnectionString(string connectionStringName)
        {
            return _configurationManager.GetConnectionString(connectionStringName);
        }

        public static string GetSetting(string settingName, bool canBeNull = true)
        {
            return _configurationManager.GetSetting(settingName, canBeNull);
        }
        
        public static Func<string, string> VirtualPathMapper { get; set; } = s => "";
        
        public static int GetSettingOrDefault(string settingName, int defaultValue)
        {
            string appSettingValue = _configurationManager.GetSetting(settingName);
            return !int.TryParse(appSettingValue, out var result) ? defaultValue : result;
        }
    }

    public class ConfigurationFailedException : Exception
    {
        public ConfigurationFailedException(string settingName, string message)
            : base(message)
        {
            SettingName = settingName;
        }

        public string SettingName { get; }
    }
}