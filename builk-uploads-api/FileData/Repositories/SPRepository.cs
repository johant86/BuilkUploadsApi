using builk_uploads_api.Settings;
using builk_uploads_api.Utils;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
                new LogErrors().WriteLog("UploadDataController", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, $"Request=> {Url + User + Password + Domain}");
            }

        }
    }
}
