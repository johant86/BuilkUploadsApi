using builk_uploads_api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using System.Net;

namespace builk_uploads_api.Shared.Repositories
{
    public class SPBaseRepository
    {
        protected readonly AppSettings _AppSettings;

        //public SPBaseRepository(IOptions<SharePointSettings> sharePointSettings)
        //{
        //    this._sharePointSettings = sharePointSettings.Value;
        //}
        public SPBaseRepository(IOptions<AppSettings> appSettings)
        {
            this._AppSettings = appSettings.Value;
        }

        public static ClientContext GetSPContext(System.String Url, System.String User, System.String Password, System.String Domain)
        {
            ClientContext context = new ClientContext(Url);
            context.Credentials = new NetworkCredential(User, Password, Domain);
            return context;
        }
    }
}
