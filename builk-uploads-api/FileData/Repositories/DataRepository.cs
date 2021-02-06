
using builk_uploads_api.DataContext;
using builk_uploads_api.DataContext.Context;
using builk_uploads_api.DataContext.Entites;
using builk_uploads_api.FileData.Domain;
using builk_uploads_api.FileData.Domain.Factories;
using builk_uploads_api.Settings;
using builk_uploads_api.Shared.Repositories;
using builk_uploads_api.Utils;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace builk_uploads_api.FileData.Repositories
{
    public class DataRepository : SPBaseRepository
    {
        private readonly List<string> _ValidFileFormats = new List<string>();
        private readonly DataConfigContext _DbContext;


        public DataRepository(IOptions<AppSettings> appSettings, DataConfigContext dbContext): base(appSettings)
        {
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
                        message = MessageDescription.Extension,
                        errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.InvalidFileExtension,
                    Path.GetExtension(request.file.FileName), 0 , Severity.Fatal) }
                    };
                }
                else
                {

                    if (request != null)
                    {
                        string[,] data = this.ReadFile(request.file);

                        var initialConfiguration = this._DbContext.DataUploadConfiguration(request.alias);
                        if (initialConfiguration != null)
                        {
                            var ValidColumns = ValidateColumns(data, initialConfiguration.Columns);
                            if (ValidColumns.Count() == 0)
                            {
                                if (initialConfiguration.idSource == (int)Source.SQL)
                                {
                                    if (initialConfiguration.conectionString != null)
                                    {
                                        var optionsBuilder = new DbContextOptionsBuilder<DataUploadContext>();
                                        optionsBuilder.UseSqlServer(initialConfiguration.conectionString);
                                        var _dbSource = new DataUploadContext(optionsBuilder.Options);
                                        UploadResult result = _dbSource.DataToUpload(data, initialConfiguration);


                                        if (result.RowsInserted > 0)
                                        {
                                            return new SaveDataResult
                                            {
                                                success = true,
                                                message = MessageDescription.Uploaded,
                                                errorDetails = { }
                                            };
                                        }
                                        else
                                        {
                                            return new SaveDataResult
                                            {
                                                success = false,
                                                message = MessageDescription.UploadError,
                                                errorDetails = result.errorDetails
                                            };
                                        }
                                    }
                                    else
                                    {
                                        return new SaveDataResult
                                        {
                                            success = false,
                                            message = MessageDescription.InvalidConection,
                                            errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.NotFoundConectionString, Path.GetExtension(request.file.FileName), 0, Severity.Fatal) }
                                        };
                                    }

                                }
                                else if (initialConfiguration.idSource == (int)Source.SHAREPOINT)
                                {
                                    UploadResult result = new UploadResult();
                                    result.errorDetails = new List<ErrorDetails>();
                                    bool error = false;
                                    int rowsUploaded = 0;
                                    var _SpContext = SPBaseRepository.GetSPContext(initialConfiguration.sharePointSiteUrl, this._AppSettings.SharePointSettings.NetworkLogin, this._AppSettings.SharePointSettings.Password, this._AppSettings.SharePointSettings.Domain);
                                    var SpInternalNames = initialConfiguration.Columns.Select(x => new { x.columnName }).ToList();
                                    int count = 0;
                                    var list = SPBaseRepository.GetListByTittleAsync(_SpContext, initialConfiguration.sharePointListName);
                                    var itemCreationInfromation = SPBaseRepository.listinformation();
                                   

                                    for (int i = 0; i < data.GetLongLength(0); i++)
                                    {
                                        var newItem = list.AddItem(itemCreationInfromation);
                                        count = 0;
                                        for (int j = 0; j < data.GetLength(1); j++)
                                        {

                                            if (i != 0)
                                            {
                                                var documentHeader = data[0, j];
                                                var columnConfig = initialConfiguration.Columns.Find(x => x.filecolumnName.ToUpper() == documentHeader.ToUpper());
                                                if (columnConfig != null && data[i, j] != null)
                                                {
                                                    if (columnConfig.validation != null)
                                                    {
                                                        bool fieldValidation = SPColumnValidation((int)columnConfig.idValidation, data[i, j]);
                                                        if (!fieldValidation)
                                                        {
                                                            error = true;
                                                            ErrorDetails ErrorValidation = ErrorFactory.GetError(ErrorEnum.InvalidData,
                                                             $"The data {data[i, j]} does not comply with the validation of the { documentHeader} field ", j + 1, Severity.Fatal);
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
                                                                $"The data {data[i, j]} is not corresponds to the type of data valid for the {documentHeader}", j + 1, Severity.Fatal);
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
                                                                $"The data {data[i, j]} is not corresponds to the type of data valid for the {documentHeader}", j + 1, Severity.Fatal);
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
                                                                $"The data {data[i, j]} is not corresponds to the type of data valid for the {documentHeader}", j + 1, Severity.Fatal);
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
                                        if (i != 0 && !error)
                                        {
                                            newItem.Update();
                                            _SpContext.ExecuteQueryAsync().Wait();
                                            rowsUploaded++;
                                        }

                                    }

                                    return new SaveDataResult
                                    {
                                        success = error ? false : true,
                                        message = error ? MessageDescription.UploadError : rowsUploaded + " rows " + MessageDescription.Uploaded,
                                        errorDetails = result.errorDetails
                                    };

                                }
                          
                            }
                            else
                            {
                                return new SaveDataResult
                                {
                                    success = false,
                                    message = ValidColumns.Find(x => x.errorCode == 7) != null ? MessageDescription.InvalidCulumnsNumber : MessageDescription.InvalidColumn,
                                    errorDetails = ValidColumns
                                };
                            }
                        }
                        else
                        {
                            return new SaveDataResult
                            {
                                success = false,
                                message = MessageDescription.Alias,
                                errorDetails = new List<ErrorDetails> {
                            ErrorFactory.GetError(ErrorEnum.InvalidAlias, request.alias, 0,Severity.Fatal) }
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

        //public SaveRaftMasterResponse SaveRaft(SaveRaftMasterRequest saveRaftMasterRequest)
        //{
        //    try
        //    {

        //        var list = SPBaseRepository.GetListByTittleAsync(this._RaftContext, this._sharePointSettings.lists.Master);

        //        var itemCreationInfromation = new ListItemCreationInformation();
        //        var newItem = list.AddItem(itemCreationInfromation);

        //        //newItem["Title"] = saveRaftMasterRequest.firstName;
        //        //newItem["PrimerApellido"] = saveRaftMasterRequest?.lastName ?? string.Empty;
        //        //newItem["SegundoNombre"] = saveRaftMasterRequest?.secondName ?? string.Empty;
        //        //newItem["SegundoApellido"] = saveRaftMasterRequest?.secondLastName ?? string.Empty;
        //        //newItem["Badge"] = saveRaftMasterRequest?.badge ?? string.Empty;
        //        //newItem["Cedula"] = saveRaftMasterRequest?.identificationNumber ?? string.Empty;
        //        //newItem["Telefono"] = saveRaftMasterRequest?.phone ?? string.Empty;
        //        //newItem["TelefonoPersonal"] = saveRaftMasterRequest?.personalPhone ?? string.Empty;
        //        //newItem["CorreoCandidato"] = saveRaftMasterRequest?.candidateEmail ?? string.Empty;
        //        //newItem["CorreoPersonal"] = saveRaftMasterRequest?.personalEmail ?? string.Empty;
        //        //newItem["PerfilCandidato"] = saveRaftMasterRequest?.candidateProfile ?? string.Empty;
        //        //newItem["NiveldeIngles"] = saveRaftMasterRequest?.englishLevel ?? string.Empty;
        //        //newItem["ReferenciaExterna"] = saveRaftMasterRequest?.isExternalreference ?? true;
        //        //newItem["GradoAcademico"] = saveRaftMasterRequest?.academicGrade ?? string.Empty;
        //        //newItem["Otros"] = saveRaftMasterRequest?.othersDetails ?? string.Empty;
        //        //newItem["MetodoDePago"] = saveRaftMasterRequest?.paymentMethod ?? string.Empty;
        //        //newItem["IsResumeActive"] = saveRaftMasterRequest.isResumeActive;
        //        //newItem["IsResumeRequired"] = saveRaftMasterRequest?.isResumeRequired;
        //        //newItem["ModoTrabajo"] = saveRaftMasterRequest?.workType ?? string.Empty;
        //        //newItem["ResumeCandidato"] = saveRaftMasterRequest?.resumeUrl ?? string.Empty;
        //        //newItem["FechaReferencia"] = DateTime.Now.ToString();

        //        newItem.Update();
        //        this._RaftContext.ExecuteQueryAsync().Wait();

        //        return new SaveRaftMasterResponse { success = true, message = _localizer["Saved successfully"], errorDetails = string.Empty };
        //    }
        //    catch (Exception ex)
        //    {

        //        new LogError().WriteLog("RaftRepository", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return new SaveRaftMasterResponse { success = false, message = _localizer["Something went wrong please contact IT"], errorDetails = ex.Message };
        //    }
        //}

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
                                    item, headers.IndexOf(item) + 1, Severity.Fatal);
                                errorList.Add(error);
                            }

                        }
                    }
                    else
                    {
                        var error = ErrorFactory.GetError(ErrorEnum.InvalidCulumnsNumber, $"The document has {headers.Count()} columns while the destination table has {sourceColumns.Count()}.", 0, Severity.Fatal);
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


