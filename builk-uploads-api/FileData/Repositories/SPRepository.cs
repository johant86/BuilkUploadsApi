using builk_uploads_api.Settings;
using builk_uploads_api.Utils;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Net;

namespace builk_uploads_api.FileData.Repositories
{
    public class SPRepository
    {
        protected readonly AppSettings _AppSettings;

        public SPRepository(IOptions<AppSettings> appSettings)
        {
            this._AppSettings = appSettings.Value;
        }

        public static ClientContext GetSPContext(string Url, string User, string Password, string Domain)
        {
            try
            {
                ClientContext context = new ClientContext(Url);
                context.Credentials = new NetworkCredential(User, Password, Domain);
                return context;
            }
            catch (Exception ex)

            {
                throw ex;
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, (JsonConvert.SerializeObject($"Request=> {Url + User + Password + Domain}")));
            }

        }
    }
}
