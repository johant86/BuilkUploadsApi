using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace builk_uploads_api.DataContext.Models
{
    public class Configurations
    {
        public int id { set; get; }
        public int idSource { set; get; }
        public string type { set; get; }
        public string tableName { set; get; }
        public string conectionString { set; get; }
        public string sharePointListName { set; get; }
        public string sharePointSiteUrl { set; get; }
    }

    public class Columns
    {
        public int id { set; get; }
        public string filecolumnName { set; get; }
        public string columnName { set; get; }
        public string type { set; get; }
        public int? idValidation { set; get; }
        public string validation { set; get; }
        public string validationErrorMsg { set; get; }
        public int order { set; get; }
    }
}
