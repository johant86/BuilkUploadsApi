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

        public static ListItemCollection GetListItemsAsync(ClientContext Context, string ListTitle, string ViewName = "All Items", int RowLimit = 5000)
        {
            try
            {
                List list = Context.Web.Lists.GetByTitle(ListTitle);
                Context.Load(list);
                Context.ExecuteQueryAsync().Wait();
                View view = list.Views.GetByTitle(ViewName);
                Context.Load(view);
                Context.ExecuteQueryAsync().Wait();

                CamlQuery query = new CamlQuery();
                query.ViewXml = view.ViewQuery;
                query.DatesInUtc = false;

                ListItemCollection items = list.GetItems(query);
                Context.Load(items);
                Context.ExecuteQueryAsync().Wait();


                return items;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public static bool SPColumnValidation(string Validation, string value)
        {
            if (Regex.IsMatch(value, Validation))
                return true;
            return false;
        }
    }
}
