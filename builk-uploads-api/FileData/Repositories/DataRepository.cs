
using builk_uploads_api.DataContext;
using builk_uploads_api.DataContext.Context;
using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.DataContext.Models;
using builk_uploads_api.FileData.Domain;
using builk_uploads_api.FileData.Domain.Factories;
using builk_uploads_api.Resources;
using builk_uploads_api.Settings;
using builk_uploads_api.Shared.Repositories;
using builk_uploads_api.Utils;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace builk_uploads_api.FileData.Repositories
{
    public class DataRepository : SPBaseRepository
    {
        private readonly IStringLocalizer<TranslateResources> _localizer;
        private readonly List<string> _ValidFileFormats = new List<string>();
        private readonly DataConfigContext _DbContext;

        public DataRepository(IOptions<AppSettings> appSettings, DataConfigContext dbContext, IStringLocalizer<TranslateResources> localizer) : base(appSettings)
        {
            this._localizer = localizer;
            this._ValidFileFormats = this._AppSettings.AllowedFileFormats.ToList();
            this._DbContext = dbContext;
     
        }

        public SaveDataResult SaveData(UploadRequest request)
        {

            try
            {

                if (this.ValidateFormat(request.file, this._ValidFileFormats) == false)
                {
                    return new SaveDataResult
                    {
                        success = false,
                        message = _localizer[ErrorEnum.InvalidFileExtension.ToString()],
                        errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.InvalidFileExtension,
                       _localizer[ErrorEnum.InvalidFileExtension.ToString()] + " " + Path.GetExtension(request.file.FileName)+ " " + _localizer[ErrorEnum.InvalidFileExtensionDescription.ToString()],0,0,Severity.Fatal) }
                    };
                }
                else
                {

                    if (request != null)
                    {
                        string[,] data = this.ReadFile(request.file);
               
                        SourceConfig initialConfiguration = new SourceConfig();
                        var configSorce = (from s in this._DbContext.tb_Source
                                           join c in this._DbContext.tb_SourceConfiguration on s.id equals c.Source.id
                                           where c.alias == request.alias
                                           select new Configurations
                                           {
                                               id = c.id,
                                               idSource = s.id,
                                               type = s.type,
                                               tableName = c.tableName,
                                               conectionString = c.conectionString,
                                               sharePointListName = c.sharePointListName,
                                               sharePointSiteUrl = c.sharePointSiteUrl
                                           }).FirstOrDefault();

                        if (configSorce != null)
                        {
                            var columns = (from cs in this._DbContext.tb_ColumnBySource
                                           join sc in this._DbContext.tb_SourceConfiguration on cs.SourceConfiguration.id equals sc.id
                                           join dt in this._DbContext.tb_DataType on cs.DataType.id equals dt.id
                                           join vl in this._DbContext.tb_Validation on cs.Validation.id equals vl.id into validateField
                                           from v in validateField.DefaultIfEmpty()
                                           where sc.alias == request.alias
                                           select new Columns
                                           {
                                               id = sc.id,
                                               filecolumnName = cs.filecolumnName,
                                               columnName = cs.columnName,
                                               type = dt.name,
                                               validation = v.validation,
                                               idValidation = v.id,
                                               validationErrorMsg= v.validationErrorMsg,
                                               isIdentifier=cs.isIdentifier

                                           }).ToList();
                            if (columns.Count() == 0)
                            {
                                return new SaveDataResult
                                {
                                    success = false,
                                    message = _localizer[ErrorEnum.ConfigurationNotFound.ToString()] ,
                                    errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.ColumnsNotFound,
                                     _localizer[ErrorEnum.ColumnsConfigurationNotFound.ToString()], 0, 0 , Severity.Fatal) }
                                };
                            }

                            initialConfiguration = new SourceConfig
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
                        else
                        {
                            return new SaveDataResult
                            {
                                success = false,
                                message = _localizer[ErrorEnum.ConfigurationNotFound.ToString()],
                                errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.ConfigurationNotFound,
                                    _localizer[ErrorEnum.ConfigurationNotFound.ToString()], 0, 0 , Severity.Fatal) }
                            };
                        }

                        var ValidColumns = ValidateColumns(data, initialConfiguration.Columns);
                        if (ValidColumns.Count() == 0)
                        {
                            if (initialConfiguration.idSource == (int)DataSource.SQL)
                            {
                                if (initialConfiguration.conectionString != null)
                                {
                                    var optionsBuilder = new DbContextOptionsBuilder<DataUploadContext>();
                                    optionsBuilder.UseSqlServer(initialConfiguration.conectionString);
                                    var _dbSource = new DataUploadContext(optionsBuilder.Options,this._localizer);

                                   
                                    UploadResult result = _dbSource.DataToUpload(data, initialConfiguration);


                                    if (result.RowsInserted > 0 || result.RowsUpdated > 0)
                                    {
                                        return new SaveDataResult
                                        {
                                            success = true,
                                            message = _localizer[ErrorEnum.Uploaded.ToString()] + " " + (result.RowsUpdated == 1 ? result.RowsUpdated + " " + _localizer["SingularUpdatedData"] : result.RowsUpdated > 1 ? result.RowsUpdated + " " + _localizer["PluralUpdatedData"] : "") +
                                            (result.RowsInserted == 1 ? (result.RowsUpdated <= 0 ? " ": ", ") + result.RowsInserted + " " + _localizer["SingularInsertedData"]  : result.RowsInserted > 1 ? (result.RowsUpdated <= 0 ? "" : ", ") + result.RowsInserted + " " + _localizer["PluralInsertedData"] : ""),
                                            errorDetails = { }
                                        };
                                    }
                                    else
                                    {
                                        return new SaveDataResult
                                        {
                                            success = false,
                                            message = _localizer[ErrorEnum.UploadError.ToString()],
                                            errorDetails = result.errorDetails
                                        };
                                    }
                                }
                                else
                                {
                                    return new SaveDataResult
                                    {
                                        success = false,
                                        message = _localizer[ErrorEnum.InvalidConection.ToString()],
                                        errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.NotFoundConectionString, _localizer[ErrorEnum.InvalidConection.ToString()], 0, 0 ,Severity.Fatal) }
                                    };
                                }

                            }
                            else if (initialConfiguration.idSource == (int)DataSource.SHAREPOINT)
                            {
                                UploadResult result = new UploadResult();
                                result.errorDetails = new List<ErrorDetails>();
                                bool error = false;
                                int rowsUploaded = 0;
                                int rowsUpdated = 0;
                                var _SpContext = SPBaseRepository.GetSPContext(initialConfiguration.sharePointSiteUrl, this._AppSettings.SharePointSettings.NetworkLogin, this._AppSettings.SharePointSettings.Password, this._AppSettings.SharePointSettings.Domain);
                                var SpInternalNames = initialConfiguration.Columns.Select(x => new { x.columnName }).ToList();
                                int count = 0;
                                var list = SPBaseRepository.GetListByTittleAsync(_SpContext, initialConfiguration.sharePointListName);
                                var ListInCloud = SPBaseRepository.GetListItemsAsync(_SpContext, initialConfiguration.sharePointListName);
                                var itemCreationInfromation = SPBaseRepository.listinformation();
                                bool toUpdate = false;
                                List<string> primarysKeyList = new List<string>();
                                var primaryColumn = initialConfiguration.Columns.Find(x => x.isIdentifier == true);
                                foreach (var item in ListInCloud)
                                {
                                    var sourceColumnData = item[primaryColumn.columnName]?.ToString();
                                    primarysKeyList.Add(sourceColumnData);
                                    //item[primaryColumn.columnName] = "Test";
                                    //item.Update();
                                    //_SpContext.ExecuteQueryAsync().Wait();

                                }





                                for (int i = 0; i < data.GetLongLength(0); i++)
                                {
                                    toUpdate = false;
                                    var newItem = list.AddItem(itemCreationInfromation);
                                    count = 0;
                                    for (int j = 0; j < data.GetLength(1); j++)
                                    {

                                        if (i != 0)
                                        {
                                            var documentHeader = data[0, j];
                                            var columnConfig = initialConfiguration.Columns.Find(x => x.filecolumnName.ToUpper() == documentHeader.ToUpper());
                                            var fieldTo = primarysKeyList.Find(x => x == data[i, j]);
                                            if (fieldTo != null)
                                            {
                                                toUpdate = true;

                                                foreach (var item in ListInCloud)
                                                {
                                                    var sourceColumnData = item[primaryColumn.columnName]?.ToString();
                                                    if (sourceColumnData == fieldTo)
                                                    {

                                                        for (int k = 0; k < data.GetLength(1); k++)
                                                        {
                                                            var d = data[i, k];
                                                            var internalName = SpInternalNames[k].columnName;
                                                            if (columnConfig != null && data[i, j] != null)
                                                            {
                                                                if (columnConfig.validation != null)
                                                                {
                                                                    bool fieldValidation = SPColumnValidation(columnConfig.validation, data[i, k]);
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
                                                                        bool IsInt = Int32.TryParse(data[i, j], out num);
                                                                        if (IsInt)
                                                                            item[internalName] = data[i, k];
                                                                        else
                                                                        {
                                                                            error = true;
                                                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                                                             _localizer["Thedata"] + " " + data[i, j] + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                                                            result.errorDetails.Add(ErrorValidation);
                                                                        }
                                                                        break;
                                                                    case variablesType.Boolean:
                                                                        bool IsBool = Boolean.TryParse(data[i, j], out IsBool);
                                                                        if (IsBool)
                                                                            item[internalName] = data[i, k];
                                                                        else
                                                                        {
                                                                            error = true;
                                                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                                                             _localizer["Thedata"] + " " + data[i, j] + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                                                            result.errorDetails.Add(ErrorValidation);
                                                                        }
                                                                        break;

                                                                    case variablesType.Datetime:
                                                                        DateTime date;
                                                                        bool IsDate = DateTime.TryParse(data[i, j], out date);
                                                                        if (IsDate)
                                                                            item[internalName] = data[i, k];
                                                                        else
                                                                        {
                                                                            error = true;
                                                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                                                             _localizer["Thedata"] + " " + data[i, j] + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                                                            result.errorDetails.Add(ErrorValidation);
                                                                        }
                                                                        break;
                                                                    default:
                                                                        item[internalName] = data[i, k];
                                                                        break;
                                                                };

                                                            }


                                                            //var internalName = SpInternalNames[k].columnName;
                                                            //item[internalName] = data[i, k];
                                                            //item.Update();
                                                            //_SpContext.ExecuteQueryAsync().Wait();

                                                        }
                                                        if (!error)
                                                        {
                                                            rowsUpdated++;
                                                            item.Update();
                                                            _SpContext.ExecuteQueryAsync().Wait();
                                                        }

                                                        break;
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                if (columnConfig != null && data[i, j] != null)
                                                {

                                                    if (columnConfig.validation != null)
                                                    {
                                                        bool fieldValidation = SPColumnValidation(columnConfig.validation, data[i, j]);
                                                        if (!fieldValidation)
                                                        {
                                                            error = true;
                                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.InvalidData,
                                                            columnConfig.validationErrorMsg, j + 1, i + 1, Severity.Fatal);
                                                            result.errorDetails.Add(ErrorValidation);
                                                        }
                                                    }

                                                    var internalName = SpInternalNames[count].columnName;
                                                    count++;

                                                    switch (columnConfig.type)
                                                    {
                                                        case variablesType.Int:
                                                            int num;
                                                            bool IsInt = Int32.TryParse(data[i, j], out num);
                                                            if (IsInt)
                                                                newItem[internalName] = data[i, j];
                                                            else
                                                            {
                                                                error = true;
                                                                ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                                                 _localizer["Thedata"] + " " + data[i, j] + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                                                result.errorDetails.Add(ErrorValidation);
                                                            }
                                                            break;
                                                        case variablesType.Boolean:
                                                            bool IsBool = Boolean.TryParse(data[i, j], out IsBool);
                                                            if (IsBool)
                                                                newItem[internalName] = data[i, j];
                                                            else
                                                            {
                                                                error = true;
                                                                ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                                                 _localizer["Thedata"] + " " + data[i, j] + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                                                result.errorDetails.Add(ErrorValidation);
                                                            }
                                                            break;

                                                        case variablesType.Datetime:
                                                            DateTime date;
                                                            bool IsDate = DateTime.TryParse(data[i, j], out date);
                                                            if (IsDate)
                                                                newItem[internalName] = data[i, j];
                                                            else
                                                            {
                                                                error = true;
                                                                ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.DataType,
                                                                 _localizer["Thedata"] + " " + data[i, j] + " " + _localizer["correspondsData"] + " " + documentHeader, j + 1, i + 1, Severity.Fatal);
                                                                result.errorDetails.Add(ErrorValidation);
                                                            }
                                                            break;
                                                        default:
                                                            newItem[internalName] = data[i, j];
                                                            break;
                                                    };
                                                }
                                            }

                                        }

                                    }
                                    if (i != 0 && !error && !toUpdate)
                                    {
                                        newItem.Update();
                                        _SpContext.ExecuteQueryAsync().Wait();
                                        rowsUploaded++;
                                    }

                                }

                                return new SaveDataResult
                                {
                                    success = error ? false : true,
                                    message = error ? _localizer[ErrorEnum.UploadError.ToString()] : _localizer[ErrorEnum.Uploaded.ToString()] + ", " + (rowsUpdated == 1 ? rowsUpdated + " " + _localizer["SingularUpdatedData"] + (rowsUploaded > 0 ? ", " : "") : rowsUpdated > 1 ? rowsUpdated + " " + _localizer["PluralUpdatedData"] + (rowsUploaded > 0 ? ", " : "") : "") +
                                            (rowsUploaded == 1 ? rowsUploaded + " " + _localizer["SingularInsertedData"] : (rowsUploaded >= 2 ? +rowsUploaded + " " + _localizer["PluralInsertedData"] : "")),

                                    errorDetails = result.errorDetails
                                };

                            }

                        }
                        else
                        {
                            return new SaveDataResult
                            {
                                success = false,
                                message = ValidColumns.Find(x => x.errorCode == 7) != null ? _localizer[ErrorEnum.InvalidCulumnsNumber.ToString()] :
                                (ValidColumns.Find(x => x.errorCode == 6) != null ?_localizer[ErrorEnum.InvalidColumn.ToString()] : _localizer[ErrorEnum.PrimaryKeyError.ToString()]) ,
                                errorDetails = ValidColumns
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, (JsonConvert.SerializeObject(request)));
                throw ex;
            }

            return new SaveDataResult();
        }

        private List<ErrorDetails> ValidateColumns(string[,] fileColumns, List<Columns> sourceColumns)
        {
            try
            {
                List<string> headers = new List<string>();
                List<ErrorDetails> errorList = new List<ErrorDetails>();
                for (int i = 0; i < fileColumns.GetLength(1); i++)
                {
                    string header = fileColumns[0, i];
                    headers.Add(header);
                }

                var validatePrimary = sourceColumns.Find(x => x.isIdentifier==true);
                var finderDocPrimaryKey = headers.Find(x => x.ToUpper() == validatePrimary.filecolumnName.ToUpper());


                if (finderDocPrimaryKey==null)
                {
                    var error = ErrorFactory.GetError(ErrorEnum.PrimaryKeyError, _localizer["PrimaryKeyErrorDescrip"], 0, 0, Severity.Fatal);
                    errorList.Add(error);
                    return errorList;
                }


                if (headers.Count() > 0)
                {
                    if (headers.Count() == sourceColumns.Count())
                    {
                        foreach (var item in headers)
                        {
                            var c = sourceColumns.Find(x => x.filecolumnName.ToUpper().Trim() == item.ToUpper().Trim());
                            if (c == null)
                            {
                                var error = ErrorFactory.GetError(ErrorEnum.InvalidColumns,
                                   _localizer["TheColumn"] + " " + item+ " " + _localizer["NotMatch"], 0,0, Severity.Fatal);
                                errorList.Add(error);
                            }

                        }
                    }
                    else
                    {
                        var error = ErrorFactory.GetError(ErrorEnum.InvalidCulumnsNumber, _localizer["The document has"] + " " +  headers.Count().ToString() + " "+
                        _localizer["ColumnComparation"] + " " +  sourceColumns.Count().ToString(), 0,0, Severity.Fatal);
                        errorList.Add(error);
                    }

                }

                return errorList;
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, ($"Request=> {JsonConvert.SerializeObject(fileColumns) + JsonConvert.SerializeObject(sourceColumns)}"));
                throw ex;
            }

        }
        private string[,] ReadFile(IFormFile file)
        {
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileLocation = $"File/uploads/{fileName}";
                if (!Directory.Exists("File/uploads"))
                {
                    Directory.CreateDirectory("File/uploads");
                }
                return (Path.GetExtension(file.FileName).ToLower()) switch
                {
                    ".xlsx" => this.ReadExcelFormat(file, fileLocation),
                    ".csv" => this.ReadCsvFormat(file, fileLocation),
                    _ => null,
                };
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, (JsonConvert.SerializeObject(file)));
                throw ex;
            }
        }

        private string[,] ReadExcelFormat(IFormFile file, string path)
        {
            try
            {
                using (FileStream fileStream = File.Create(path))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        string[,] data = new string[reader.RowCount, reader.FieldCount];
                        int row = 0;
                        while (reader.Read())
                        {
                            var numColumns = reader.FieldCount;
                            int column = 0;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.GetValue(column) != null)
                                    data[row, column] = reader.GetValue(column).ToString();
                                column++;
                            }
                            row++;
                        }

                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, ($"Request=> {JsonConvert.SerializeObject(file) + JsonConvert.SerializeObject(path)}"));

                throw ex;
            }
        }
        private string[,] ReadCsvFormat(IFormFile file, string path)
        {
            try
            {

                using (FileStream fileStream = File.Create(path))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var reader = new StreamReader(path))
                {
                    string delimiter = DetectDelimiter(reader);
                    var col = reader.ReadLine().Split(delimiter).Count();
                    var row = (File.ReadAllLines(path).Length);
                    string[,] data = new string[row, col];
                    string[] headers = reader.ReadLine().Split(',');
                    string[] Lineas = File.ReadAllLines(path);
                    int contRow = 0;

                    foreach (var item in Lineas)
                    {
                        var valores = item.Split(delimiter);
                        int column = 0;
                        for (int i = 0; i < valores.Length; i++)
                        {
                            data[contRow, column] = valores[i].ToString();
                            column++;
                        }
                        contRow++;

                    }

                    return data;
                }


            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, (JsonConvert.SerializeObject(file + path)));
                throw ex;
            }
        }

        private bool ValidateFormat(IFormFile file, List<string> extensions)
        {
            try
            {
                if (file != null)
                    return extensions.Contains(Path.GetExtension(file.FileName).ToLower());
                else
                    return false;
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog(ex.ToString(), ex.StackTrace, ($"Request=> {JsonConvert.SerializeObject(file) + JsonConvert.SerializeObject(extensions)}"));
                return false;
            }
        }

        private string DetectDelimiter(StreamReader reader)
        {
            var possibleDelimiters = new List<string> { ",", ";", "\t", "|" };
            var headerLine = reader.ReadLine();
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();

            foreach (var possibleDelimiter in possibleDelimiters)
            {
                if (headerLine.Contains(possibleDelimiter))
                {
                    return possibleDelimiter;
                }
            }
            return possibleDelimiters[0];
        }

    }
}


