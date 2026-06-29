using API.Models.InforErp;
using API.Models.Salesforce;

namespace API.Interfaces
{
    public interface IAppSettingsManager
    {
        SalesforceSettings Salesforce { get; }
        InforIonSettings InforErp { get; }
    }
}
