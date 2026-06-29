namespace API.Models.InforErp
{
    public class InforIonSettings
    {
        public string TokenUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
        public string ApiBaseUrl { get; set; }
        public string IdoBasePath { get; set; }
        public string Scope { get; set; }
        public string GrantType { get; set; }
        public string X_Infor_MongooseConfig { get; set; }
        public string ApiVersion { get; set; }
    }
}
