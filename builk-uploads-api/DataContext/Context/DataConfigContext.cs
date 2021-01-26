using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

               

                var configSorce = this.Configurations.FromSqlRaw("sp_GetBuilkUploadsConfigurationsByAlias @prmAlias",
                     new SqlParameter("@prmAlias", alias.Trim()))
                    .AsEnumerable<DataConfiguration>().FirstOrDefault();

                //var columns = this.Columns.FromSqlRaw("sp_GetColumnsBySource @prmAlias",
                //    new SqlParameter("@prmAlias", alias))
                //   .AsEnumerable<Columns>().ToList();

                var columns = this.Columns.FromSqlRaw($"SELECT CS.id, CS.filecolumnName, CS.columnName, CS.[type], VL.[validation], CS.[order] FROM [dbo].[tb_ColumnsBySource] CS WITH(NOLOCK) INNER JOIN [dbo].[tb_SourceConfiguration] SC WITH(NOLOCK) ON CS.idSourceConfiguration = SC.id LEFT JOIN [dbo].[tb_Validations] VL WITH(NOLOCK) ON VL.id = CS.idValidation WHERE SC.alias = '{alias}'")
                   .AsEnumerable<Columns>().ToList();

                SourceConfiguration Configurations = new SourceConfiguration
                {
                    type = configSorce.type,
                    idSource = configSorce.idSource,
                    tableName = configSorce.tableName,
                    conectionString =  configSorce.conectionString,
                    sharePointListName = configSorce.sharePointListName,
                    sharePointSiteUrl = configSorce.sharePointSiteUrl,
                    Columns = columns
                };

                return Configurations;

            }
            catch (Exception ex)
            {
                throw ex;
                new LogErrors().WriteLog("DataConfigContext", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, string.Empty);
            }
        }


        //public int UploadDocumentFields(DocumentFields fields)
        //{
        //    try
        //    {
        //      SqlParameter outPutVal = new SqlParameter("@synchronization_version", SqlDbType.Int);
        //        //    var parameters = new[] {
        //        //    new SqlParameter("@prmRaftid", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = fields.Raftid },
        //        //    new SqlParameter("@prmTokenUsed", SqlDbType.Bit) { Direction = ParameterDirection.Input, Value = fields.TokenUsed },
        //        //    new SqlParameter("@prmToken", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = fields.Token },
        //        //    new SqlParameter("@prmResponseInfo", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = fields.ResponseInfo },
        //        //    new SqlParameter("@synchronization_version", SqlDbType.BigInt) { Direction = ParameterDirection.InputOutput, Value = 0 }
        //        //};
        //        var result = this.Document.FromSqlRaw("sp_InsetDocumentFields @prmRaftid,@prmTokenUsed,@prmToken,@prmResponseInfo,@P_Success OUTPUT", //parameters).FirstOrDefault();
        //             new SqlParameter("@prmRaftid", fields.Raftid.Trim()),
        //             new SqlParameter("@prmTokenUsed", fields.TokenUsed),
        //             new SqlParameter("@prmToken", fields.Token.Trim()),
        //             new SqlParameter("@prmResponseInfo", fields.ResponseInfo.Trim()),
        //             outPutVal).FirstOrDefault();
        //             outPutVal.Direction = ParameterDirection.Output;
        //              int value = Convert.ToInt32(outPutVal.Direction);
        //        return value;
        //    }
        //    catch (Exception ex)    
        //    {
        //        throw ex;
        //    }
        //}

    }
}
