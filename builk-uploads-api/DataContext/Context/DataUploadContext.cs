using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;


namespace builk_uploads_api.DataContext.Context
{
    public class DataUploadContext : DbContext
    {
        public DataUploadContext(DbContextOptions options) : base(options)
        { }
        private DbSet<DataUploaded> Data { get; set; }

        public int DataToUpload(string[,] documentData, SourceConfiguration configuration)
        {
            try
            {
                Columns columnConfig = new Columns();
                int dataUpload = 0;

                for (int i = 0; i < documentData.GetLength(0); i++)
                {
                    var query = string.Empty;
                    query  += $"INSERT INTO  {configuration.tableName}  VALUES (";
                    for (int j = 0; j < documentData.GetLength(1); j++)
                    {
                        if (i != 0)
                        {
                            var documentHeader = documentData[0, j];
                            columnConfig = configuration.Columns.Find(x => x.filecolumnName == documentHeader);
                            var exelData = documentData[i, j];
                            
                            //if(exelData!=null)
                            switch (columnConfig.type)
                            {
                                case "int":
                                    query += $"'{Int32.Parse(exelData)}'"+(documentData.GetLength(1)-1 == j ? ");" : ",") +"";
                                    break;
                                case "bool":
                                    query += $"{(Convert.ToBoolean(exelData) == true ? 1 : 0)}" + (documentData.GetLength(1)-1 == j ? ");" : ",") + "";
                                    break;
                                case "decimal":
                                    query += $"{Convert.ToDecimal(exelData)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                    break;
                                case "datetime":
                                    query += $"{Convert.ToDateTime(exelData)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                    break;
                                default:
                                    query += $"'{exelData}'" + (documentData.GetLength(1)-1 == j ? ");" : ",") + "";
                                    break;
                            };
                        }
                    }
                    if (i > 0)
                    {
                        var idRegister = this.Data.FromSqlRaw($"{query} SELECT MAX(id) AS id FROM {configuration.tableName}").AsEnumerable<DataUploaded>().FirstOrDefault();
                        if (idRegister.id > 0)
                            dataUpload++;

                    }
                }
                return dataUpload;
            }
            catch (Exception ex)
            {
                throw;
                new LogErrors().WriteLog("DataUploadContext", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, string.Empty);
            }
        }

    }
}
