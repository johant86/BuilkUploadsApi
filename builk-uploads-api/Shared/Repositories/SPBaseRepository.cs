using builk_uploads_api.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace builk_uploads_api.Shared.Repositories
{
    public class SPBaseRepository
    {
        protected readonly AppSettings _AppSettings;

        public SPBaseRepository(IOptions<AppSettings> appSettings)
        {
            this._AppSettings = appSettings.Value;
        }

        public static ClientContext GetSPContext(string Url, string User, string Password, string Domain)
        {
            ClientContext context = new ClientContext(Url);
            context.Credentials = new NetworkCredential(User, Password, Domain);
            return context;
        }

        public static ListItemCreationInformation ListInfo(ClientContext Context, string ListTitle)
        {
            return new ListItemCreationInformation();
        }
        public static List GetListByTittleAsync(ClientContext Context, string ListTitle)
        {
            try
            {
                var list = Context.Web.Lists.GetByTitle(ListTitle);
                Context.Load(list);
                Context.ExecuteQueryAsync().Wait();


                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static ListItemCreationInformation listinformation()
        {
            return new ListItemCreationInformation();
        }

        public static bool SPColumnValidation(string Validation, string value)
        {
            if (Regex.IsMatch(value, Validation))
                return true;
            return false;
        }
    }
}
