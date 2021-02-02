using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace builk_uploads_api.DataContext.Entites
{
    public class DataConfiguration
    {
        [Key]
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
        [Key]
        public int id { set; get; }
        public string filecolumnName { set; get; }
        public string columnName { set; get; }
        public string type { set; get; }
        public int? idValidation { set; get; }
        public string  validation { set; get; }
        public int order { set; get; }
    }

    public class SourceConfiguration
    {
        public int idSource { set; get; }
        public string type { set; get; }
        public string tableName { set; get; }
        public string conectionString { set; get; }
        public string sharePointListName { set; get; }
        public string sharePointSiteUrl { set; get; }
        public List<Columns> Columns { set; get; }   
    }





}
