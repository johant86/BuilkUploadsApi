using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.DataContext.Models;
using builk_uploads_api.FileData.Domain;
using builk_uploads_api.FileData.Domain.Factories;
using builk_uploads_api.Resources;
using builk_uploads_api.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;


namespace builk_uploads_api.DataContext.Context
{
    public class DataUploadContext : DbContext
    {
        private readonly IStringLocalizer<TranslateResources> _localizer;
        public DataUploadContext(DbContextOptions options, IStringLocalizer<TranslateResources> localizer) : base(options)
        {
            this._localizer = localizer;
        }
        private DbSet<DataUploaded> Data { get; set; }

      

        public UploadResult DataToUpload(string[,] documentData, SourceConfig configuration)
        {

            try
            {
                var primaryColumn = configuration.Columns.Find(x => x.isIdentifier == true);
                SqlConnection sqlCnn = new SqlConnection(configuration.conectionString);
                List<string> primariesColumns = new List<string> ();

                sqlCnn.Open();
                SqlCommand sqlCmd = new SqlCommand($"SELECT  *  FROM  {configuration.tableName}", sqlCnn);
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    primariesColumns.Add(sqlReader.GetValue(primaryColumn.columnName).ToString());
                }
                sqlReader.Close();
                sqlCmd.Dispose();
                //sqlCnn.Close();

                Columns columnConfig = new Columns();
                UploadResult result = new UploadResult();
                result.errorDetails = new List<ErrorDetails>();
                List<string> querys = new List<string>();
                bool error = false;
                int dataUpload = 0;
                int RowsUpdated = 0;
                bool toUpdate = false;


                for (int i = 0; i < documentData.GetLength(0); i++)
                {
                    toUpdate = false;
                    var query = string.Empty;
                    var updateQuery = string.Empty;
                    var fieldToUpdate = string.Empty;
                    updateQuery = $"UPDATE  {configuration.tableName} SET ";
                    query += $"INSERT INTO  {configuration.tableName}  VALUES (";
                    for (int j = 0; j < documentData.GetLength(1); j++)
                    {
                        if (i != 0)
                        {
                            var documentHeader = documentData[0, j];
                            columnConfig = configuration.Columns.Find(x => x.filecolumnName.ToUpper() == documentHeader.ToUpper());
                            var exelData = documentData[i, j];
                            var fieldTo = primariesColumns.Find(x=>x==exelData);

                            if (fieldTo != null)
                            {
                                fieldToUpdate = ValidateType(columnConfig.type,fieldTo);
                                toUpdate = true;
                            }

                            if (columnConfig != null && exelData != null)
                            {

                                if (columnConfig.validation != null)
                                {
                                    bool fieldValidation = ValidateColumn(columnConfig.validation, exelData);

                                    if (!fieldValidation)
                                    {
                                        error = true;
                                        ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.InvalidData,
                                        columnConfig.validationErrorMsg, j + 1, i + 1, Severity.Fatal);
                                        result.errorDetails.Add(ErrorValidation);
                                    }
                                }
                                switch (columnConfig.type)
                                {
                                    case variablesType.Int:
                                        int num;
                                        bool IsInt = Int32.TryParse(exelData, out num);
                                        if (IsInt)
                                        {
                                            if (!toUpdate)
                                                query += $"{Int32.Parse(exelData)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                            else
                                                 updateQuery += $"{columnConfig.columnName}={Int32.Parse(exelData)}" + (documentData.GetLength(1) - 1 == j ? "" : ",") + "";                           
                                        }
                                        else
                                        {
                                            error = true;
                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                             _localizer["Thedata"] + " " + exelData + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                            result.errorDetails.Add(ErrorValidation);
                                        }
                                        break;
                                    case variablesType.Boolean:
                                        bool Bool;
                                        bool IsBool = Boolean.TryParse(exelData, out Bool);
                                        if (IsBool)
                                        {
                                            if (!toUpdate)
                                                query += $"{(Convert.ToBoolean(exelData) == true ? 1 : 0)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                            else
                                                updateQuery += $"{columnConfig.columnName}={(Convert.ToBoolean(exelData) == true ? 1 : 0)}" + (documentData.GetLength(1) - 1 == j ? "" : ",") + "";
                                        }
                                        else
                                        {
                                            error = true;
                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                             _localizer["Thedata"] + " " + exelData + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                            result.errorDetails.Add(ErrorValidation);
                                        }
                                        break;
                                    case variablesType.Decimal:
                                        if (!toUpdate)
                                            query += $"{Convert.ToDecimal(exelData)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                        else
                                            updateQuery += $"{columnConfig.columnName}={Convert.ToDecimal(exelData)}" + (documentData.GetLength(1) - 1 == j ? "" : ",") + "";
                                           
                                        break;
                                    case variablesType.Datetime:
                                        DateTime date;
                                        bool IsDate = DateTime.TryParse(exelData, out date);
                                        if (IsDate)
                                        {
                                            if (!toUpdate)
                                                query += $"CONVERT (DATETIME, '{Convert.ToDateTime(exelData)}', 103)" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                            else
                                                updateQuery += $"{columnConfig.columnName}= CONVERT (DATETIME, '{Convert.ToDateTime(exelData)}', 103)" + (documentData.GetLength(1) - 1 == j ? "" : ",") + "";
                                        }
                                        else
                                        {
                                            error = true;
                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                             _localizer["Thedata"] + " " + exelData + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                            result.errorDetails.Add(ErrorValidation);
                                        }
                                        break;
                                    default:
                                        if (!toUpdate)
                                            query += $"'{exelData}'" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                        else                                     
                                            updateQuery += $"{columnConfig.columnName}='{exelData}'" + (documentData.GetLength(1) - 1 == j ? "" : ",") + "";                                      

                                        break;
                                };
                            }

                        }
                    }
                    if (i > 0)
                    {
                        if(query.Contains(");"))
                        querys.Add(query);
                        if (updateQuery.Contains(",") && !error)
                        {
                            updateQuery += $" WHERE {primaryColumn.columnName}={fieldToUpdate}";
                            //var idRegister = this.Data.FromSqlRaw($"{updateQuery} SELECT MAX(id) AS id FROM {configuration.tableName}").AsEnumerable<DataUploaded>().FirstOrDefault();
                            sqlCmd = new SqlCommand($"{updateQuery} SELECT *  FROM {configuration.tableName} WHERE {primaryColumn.columnName}={fieldToUpdate}", sqlCnn);
                            sqlReader = sqlCmd.ExecuteReader();
                            while (sqlReader.Read())
                            {
                                RowsUpdated++;
                                var identiferUpdated = sqlReader.GetValue(primaryColumn.columnName);
                            }
                            sqlReader.Close();
                            sqlCmd.Dispose();
                        }
                    }

                }

                if (querys.Count > 0 && !error)
                {
                    foreach (var item in querys)
                    {
                        var idRegister = this.Data.FromSqlRaw($"{item} SELECT MAX(id) AS id FROM {configuration.tableName}").AsEnumerable<DataUploaded>().FirstOrDefault();
                        if (idRegister.id > 0)
                            dataUpload++;
                    }

                }
                sqlCnn.Close();
                result.RowsInserted = dataUpload;
                result.RowsUpdated = RowsUpdated;
                return result;
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, (JsonConvert.SerializeObject(configuration)));
                return null;
            }

        }

        public string ValidateType(string type, string value)
        {

            switch (type)
            {
                case variablesType.Boolean:
                    bool IsBool = Boolean.TryParse(value, out IsBool);
                    if (IsBool)
                        return $"{(Convert.ToBoolean(value) == true ? 1 : 0)}";
                    break;
                case variablesType.Int:
                    int num;
                    bool IsInt = Int32.TryParse(value, out num);
                    if (IsInt)
                        return $"{Int32.Parse(value)}";
                    break;
                case variablesType.Datetime:
                    DateTime date;
                    bool IsDate = DateTime.TryParse(value, out date);
                    if (IsDate)
                        return $"CONVERT (DATETIME, '{Convert.ToDateTime(value)}', 103)";
                    break;
            }

            return $"'{value}'";
        }


        public bool ValidateColumn(string validation, string value)
        {  
                    if (Regex.IsMatch(value, validation))
                        return true;
                  
            return false;
        }

        
    }
}
