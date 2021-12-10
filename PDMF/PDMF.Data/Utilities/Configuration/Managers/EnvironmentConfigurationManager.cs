using System;
using PDMF.Data.Exceptions;
using PDMF.Data.Utilities.Configuration.Abstract;

namespace PDMF.Data.Utilities.Configuration.Managers
{
    public class EnvironmentConfigurationManager : IConfigurationManager
    {
        public string GetConnectionString(string connectionStringName)
        {
            return GetSetting(connectionStringName, false);
        }

        public string GetSetting(string envSettingName, bool canBeNull = true)
        {
            var result = Environment.GetEnvironmentVariable(envSettingName);
            
            if (!canBeNull && result == null)
            {
                throw new ConfigurationException($"AppSetting {envSettingName} not configured or configured as null");
            }

            return result; 
        }
    }
}