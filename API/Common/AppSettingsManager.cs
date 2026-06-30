using API.Interfaces;
using API.Models.InforErp;
using API.Models.Salesforce;

namespace API.Common
{
    public class AppSettingsManager : IAppSettingsManager
    {
        public SalesforceSettings Salesforce { get; private set; }
        public InforIonSettings InforErp { get; private set; }

        public AppSettingsManager(IConfiguration configuration)
        {
            Salesforce = configuration.GetSection("Salesforce").Get<SalesforceSettings>()
                         ?? throw new InvalidOperationException("Salesforce settings missing in appsettings.json");
            InforErp = configuration.GetSection("InforErp").Get<InforIonSettings>()
                       ?? throw new InvalidOperationException("Infor Erp settings missing in appsettings.json");
        }
    }
}
