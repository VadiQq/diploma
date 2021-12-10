using System.Collections.Generic;

namespace PDMF.GMDHBatchProgram.Configuration
{
    public static class StaticSettings
    {
        public static Dictionary<string, string> Settings = new()
        {
            { "AzureStorageConnection", "DefaultEndpointsProtocol=https;AccountName=pmdf;AccountKey=spPdqLQ0hN7JdmjFAzJNQnlPPJ0Ea1bj6m9c3wQec/KTETY7W+i1zIEk+5fJ0VmI4p4FcikobMRlIDNdubP3Zg=="},
            { "DefaultConnection", "Server=tcp:pmdf.database.windows.net,1433;Database=pdmfdatabase;Persist Security Info=False;User ID=pmdfAdmin;Password=Kyoceramita_1;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;MultipleActiveResultSets=true;"},
        };
    }
}