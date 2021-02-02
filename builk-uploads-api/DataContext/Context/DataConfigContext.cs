using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.Utils;
using Microsoft.Data.SqlClient;
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

        private DbSet<DataConfiguration> Configurations { get; set; }

        private DbSet<Columns> Columns { get; set; }

        public SourceConfiguration DataUploadConfiguration(string alias)
        {
            try
            {

                SourceConfiguration Configurations = new SourceConfiguration();

                var configSorce = this.Configurations.
                    FromSqlRaw($"SELECT SC.id, S.id AS 'idSource', S.[type], SC.tableName,SC.conectionString,SC.sharePointListName,SC.sharePointSiteUrl FROM  [tb_Source] S WITH(NOLOCK) INNER JOIN [tb_SourceConfiguration] SC WITH(NOLOCK) ON S.id = SC.idSource WHERE SC.alias = '{alias}'").AsEnumerable<DataConfiguration>().FirstOrDefault();

                if (configSorce != null)
                {
                    var columns = this.Columns.FromSqlRaw($"SELECT CS.id, CS.filecolumnName, CS.columnName, CS.[type], VL.[validation], VL.[id] AS 'idValidation', CS.[order] FROM [dbo].[tb_ColumnsBySource] CS WITH(NOLOCK) INNER JOIN [dbo].[tb_SourceConfiguration] SC WITH(NOLOCK) ON CS.idSourceConfiguration = SC.id LEFT JOIN [dbo].[tb_Validations] VL WITH(NOLOCK) ON VL.id = CS.idValidation WHERE SC.alias = '{alias}'")
                                  .AsEnumerable<Columns>().ToList();
                    return Configurations = new SourceConfiguration
                    {
                        type = configSorce.type,
                        idSource = configSorce.idSource,
                        tableName = configSorce.tableName,
                        conectionString = configSorce.conectionString,
                        sharePointListName = configSorce.sharePointListName,
                        sharePointSiteUrl = configSorce.sharePointSiteUrl,
                        Columns = columns
                    };
                }
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, (JsonConvert.SerializeObject($"Request=> {alias}")));

            }
            return null;
        }
    }
}
