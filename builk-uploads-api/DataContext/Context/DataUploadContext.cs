using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.DataContext.Models;
using builk_uploads_api.FileData.Domain;
using builk_uploads_api.FileData.Domain.Factories;
using builk_uploads_api.Utils;
using Microsoft.EntityFrameworkCore;
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
        public DataUploadContext(DbContextOptions options) : base(options)
        { }
        private DbSet<DataUploaded> Data { get; set; }

        public UploadResult DataToUpload(string[,] documentData, SourceConfig configuration)
        {
            try
            {
                Columns columnConfig = new Columns();
                UploadResult result = new UploadResult();
                result.errorDetails = new List<ErrorDetails>();
                List<string> querys = new List<string>();
                bool error = false;
                int dataUpload = 0;

                for (int i = 0; i < documentData.GetLength(0); i++)
                {
                    var query = string.Empty;
                    query += $"INSERT INTO  {configuration.tableName}  VALUES (";
                    for (int j = 0; j < documentData.GetLength(1); j++)
                    {
                        if (i != 0)
                        {
                            var documentHeader = documentData[0, j];
                            columnConfig = configuration.Columns.Find(x => x.filecolumnName.ToUpper() == documentHeader.ToUpper());
                            var exelData = documentData[i, j];

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
                                            query += $"{Int32.Parse(exelData)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                        else
                                        {
                                            error = true;
                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                            $"The data {exelData} is not corresponds to the type of data valid for the {documentHeader}", j + 1, i + 1, Severity.Fatal);
                                            result.errorDetails.Add(ErrorValidation);
                                        }
                                        break;
                                    case variablesType.Boolean:
                                        bool IsBool = Boolean.TryParse(exelData, out IsBool);
                                        if (IsBool)
                                            query += $"{(Convert.ToBoolean(exelData) == true ? 1 : 0)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                        else
                                        {
                                            error = true;
                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                            $"The data {exelData} is not corresponds to the type of data valid for the {documentHeader}", j + 1, i + 1, Severity.Fatal);
                                            result.errorDetails.Add(ErrorValidation);
                                        }
                                        break;
                                    case variablesType.Decimal:
                                        query += $"{Convert.ToDecimal(exelData)}" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                        break;
                                    case variablesType.Datetime:
                                        DateTime date;
                                        bool IsDate = DateTime.TryParse(exelData, out date);
                                        if (IsDate)
                                            query += $"CONVERT (DATETIME, '{Convert.ToDateTime(exelData)}', 103)" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                        else
                                        {
                                            error = true;
                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                            $"The data {exelData} is not corresponds to the type of data valid for the {documentHeader}", j + 1, i + 1, Severity.Fatal);
                                            result.errorDetails.Add(ErrorValidation);
                                        }
                                        break;
                                    default:
                                        query += $"'{exelData}'" + (documentData.GetLength(1) - 1 == j ? ");" : ",") + "";
                                        break;
                                };
                            }

                        }
                    }
                    if (i > 0 && query.Contains(");"))
                    {
                        querys.Add(query);
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

                result.RowsInserted = dataUpload;
                return result;
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, (JsonConvert.SerializeObject(configuration)));
                return null;
            }

        }

        public bool ValidateType(string type, string value)
        {

            switch (type)
            {
                case variablesType.Boolean:
                    bool IsBool = Boolean.TryParse(value, out IsBool);
                    if (IsBool)
                        return IsBool;
                    break;
                case variablesType.Int:
                    int num;
                    bool IsInt = Int32.TryParse(value, out num);
                    if (IsInt)
                        return true;
                    break;
                case variablesType.Datetime:
                    DateTime date;
                    bool IsDate = DateTime.TryParse(value, out date);
                    if (IsDate)
                        return true;
                    break;
            }

            return false;
        }


        public bool ValidateColumn(string validation, string value)
        {  
                   // var expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
                    if (Regex.IsMatch(value, validation))
                        return true;
                  
            return false;
        }

        public bool ValidateColumnById(int idValidation, string value)
        {
            switch (idValidation)
            {
                case (int)ValidationsEnum.Phone:
                    if (value.Trim().Length == 8)
                        return true;
                    break;
                case (int)ValidationsEnum.Identification:
                    if (value.Trim().Length >= 9 && value.Trim().Length <= 21)
                        return true;
                    break;
                case (int)ValidationsEnum.Email:
                    var expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
                    if (Regex.IsMatch(value, expresion))
                        return true;
                    break;
            }

            return false;
        }

    }
}
