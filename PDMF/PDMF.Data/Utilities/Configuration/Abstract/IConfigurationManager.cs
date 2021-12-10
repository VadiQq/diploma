namespace PDMF.Data.Utilities.Configuration.Abstract
{
    public interface IConfigurationManager
    {
        string GetConnectionString(string connectionStringName);
        string GetSetting(string appSettingName, bool canBeNull = true);
    }
}