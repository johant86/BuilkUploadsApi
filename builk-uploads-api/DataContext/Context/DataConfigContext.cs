
using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;

namespace builk_uploads_api.DataContext
{
    public class DataConfigContext : DbContext
    {
        public DataConfigContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Source> tb_Source { get; set; }
        public DbSet<SourceConfiguration> tb_SourceConfiguration { get; set; }
        public DbSet<ColumnBySource> tb_ColumnBySource { get; set; }
        public DbSet<Validation> tb_Validation { get; set; }
        public DbSet<DataType> tb_DataType { get; set; }

    }
}
