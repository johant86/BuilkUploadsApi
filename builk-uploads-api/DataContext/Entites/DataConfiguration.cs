using builk_uploads_api.DataContext.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace builk_uploads_api.DataContext.Entites
{
    //public class DataConfiguration
    //{
    //    [Key]
    //    public int id { set; get; }
    //    public int idSource { set; get; }
    //    public string type { set; get; }
    //    public string tableName { set; get; }
    //    public string conectionString { set; get; }
    //    public string sharePointListName { set; get; }
    //    public string sharePointSiteUrl { set; get; }
    //}

    public class Source
    {
        [Key]
        public int id { set; get; }
        public string type { set; get; }
        public DateTime lastModificationDate { set; get; }
        public int lastModificationUser { set; get; }
    }

    public class SourceConfiguration
    {
        [Key]
        public int id { set; get; }
        public virtual Source Source { set; get; }
        public string alias { set; get; }

        public string tableName { set; get; }

        public string conectionString { set; get; }

        public string sharePointSiteUrl { set; get; }

        public string sharePointListName { set; get; }

        public DateTime lastModificationDate { set; get; }

        public int lastModificationUser { set; get; }
       
    }

    public class ColumnBySource
    {
        [Key]
        public int id { set; get; }
        public virtual SourceConfiguration SourceConfiguration { set; get; }
        public string filecolumnName { set; get; }
        public string columnName { set; get; }
        public virtual Validation Validation { set; get; }
        public virtual DataType DataType { set; get; }
        public DateTime lastModificationDate { set; get; }
        public int lastModificationUser { set; get; }
    }

    public class DataType
    {
        [Key]
        public int id { set; get; }
        public string name { set; get; }
        public string description { set; get; }
        public DateTime lastModificationDate { set; get; }
        public int lastModificationUser { set; get; }
    }

    public class Validation
    {
        [Key]
        public int id { set; get; }
        public string name { set; get; }
        public string validation { set; get; }
        public DateTime lastModificationDate { set; get; }
        public int lastModificationUser { set; get; }
    }

    public class SourceConfig
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
