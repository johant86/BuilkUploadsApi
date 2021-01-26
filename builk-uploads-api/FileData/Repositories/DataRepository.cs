
using builk_uploads_api.DataContext;
using builk_uploads_api.DataContext.Context;
using builk_uploads_api.FileData.Domain;
using builk_uploads_api.FileData.Domain.Factories;
using builk_uploads_api.Settings;
using builk_uploads_api.Utils;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace builk_uploads_api.FileData.Repositories
{
    public class DataRepository
    {
        private readonly List<string> _ValidFileFormats = new List<string>();
        protected readonly AppSettings _AppSettings;
        private readonly DataConfigContext _DbContext;
        public SQLRepository _SQLRepo { get; private set; }
        public SPRepository _SharePointRepo { get; private set; }

        public DataRepository(IOptions<AppSettings> appSettings, DataConfigContext dbContext)
        {
            this._AppSettings = appSettings.Value;
            this._ValidFileFormats = this._AppSettings.AllowedFileFormats.ToList();
            this._DbContext = dbContext;
        }

        public SaveDataResult SaveData(UploadRequest request)
        {

            if (this.ValidateFormat(request.file, this._ValidFileFormats) == false)
            {
                return new SaveDataResult
                {
                    success = false,
                    message = "Invalid file extension",
                    errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.InvalidFileExtension, Path.GetExtension(request.file.FileName), 0) }
                };
            }
            else
            {

                if (request != null)
                {
                    string[,] data = this.ReadFile(request.file);

                    var initialConfiguration = this._DbContext.DataUploadConfiguration(request.alias);
                    if (initialConfiguration.idSource == 1)
                    {
                        if (initialConfiguration.conectionString != null)
                        {
                            //string s = "Server=(LocalDB)\\LocalDB;Database=Test_DB;User Id=sa;Password=123456;";
                            var optionsBuilder = new DbContextOptionsBuilder<DataUploadContext>();
                            optionsBuilder.UseSqlServer(initialConfiguration.conectionString);
                            var _dbSource = new DataUploadContext(optionsBuilder.Options);
                            int registers = _dbSource.DataToUpload(data, initialConfiguration);
                        

                            if (registers > 0)
                            {
                                return new SaveDataResult
                                {
                                    success = true,
                                    message = "the information was uploaded successfully.",
                                    errorDetails = { }
                                };
                            }
                        }
                        else
                        {
                            return new SaveDataResult
                            {
                                success = false,
                                message = "Not fount conection string",
                                errorDetails = new List<ErrorDetails> { ErrorFactory.GetError(ErrorEnum.NotFoundConectionString, Path.GetExtension(request.file.FileName), 0) }
                            };
                        }

                    }
                    else if (initialConfiguration.idSource == 2)
                    {

                    }

                }
                else
                    throw new Exception("Not able to read data from provided file");

            }
            return new SaveDataResult();
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
                new LogErrors().WriteLog("DataRepository", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, string.Empty);
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
                                if(reader.GetValue(column) != null)
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
                new LogErrors().WriteLog("DataRepository", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, string.Empty);
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
                    string  delimiter = DetectDelimiter(reader);
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
                new LogErrors().WriteLog("DataRepository", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, string.Empty);
                throw ex;
            }
        }

        private bool ValidateFormat(IFormFile file, List<string> extensions)
        {
            try
            {
                return extensions.Contains(System.IO.Path.GetExtension(file.FileName).ToLower());
            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog("DataRepository", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, string.Empty);
                return false;
            }
        }

        private string DetectDelimiter(StreamReader reader)
        {
            // assume one of following delimiters
            var possibleDelimiters = new List<string> { ",", ";", "\t", "|" };

            var headerLine = reader.ReadLine();

            // reset the reader to initial position for outside reuse
            // Eg. Csv helper won't find header line, because it has been read in the Reader
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

//int count = 0;
//var v = new { Amount = 108, Message = "Hello" };
//do
//{
//    while (reader.Read())
//    {
//        count++;
//        DocumentFields documentData = new DocumentFields();
//        for (int column = 0; column < reader.FieldCount; column++)
//        {
//            var p = reader.GetValue(column);
//            //var v = reader.GetValue(column).GetType();
//            if (count > 1)
//            {

//            }
//        }
//    }
//} while (reader.NextResult());

