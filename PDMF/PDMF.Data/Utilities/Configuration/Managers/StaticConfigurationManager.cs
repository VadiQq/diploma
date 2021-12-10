using System.Collections.Generic;
using PDMF.Data.Exceptions;
using PDMF.Data.Utilities.Configuration.Abstract;

namespace PDMF.Data.Utilities.Configuration.Managers
{
    public class StaticConfigurationManager : IConfigurationManager
    {
        private readonly IDictionary<string, string> _settings;
        
        public StaticConfigurationManager(IDictionary<string, string> settings)
        {
            _settings = settings;
        }
        
        public string GetConnectionString(string connectionStringName)
        {
            return GetSetting(connectionStringName);
        }

        public string GetSetting(string appSettingName, bool canBeNull = true)
        {
            if (_settings.TryGetValue(appSettingName, out var value))
            {
                return value;
            }
            
            if (!canBeNull)
            {
                throw new ConfigurationException($"AppSetting {appSettingName} not configured or configured as null");
            }

            return null;
        }
    }
}