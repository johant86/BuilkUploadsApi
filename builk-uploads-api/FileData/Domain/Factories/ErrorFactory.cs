using System;

namespace builk_uploads_api.FileData.Domain.Factories
{
    public class ErrorFactory
    {

        public static ErrorDetails GetError(ErrorEnum errorEnum, string fileExtension = "", int column = 0, string exception = "", string alias = "")
        {
            switch (errorEnum)
            {
                case ErrorEnum.InvalidFileExtension:
                    return new ErrorDetails
                    {
                        error = $"The file extension {fileExtension} is not valid, please check extension specifications and try again",
                        columnNumber = column,
                        errorCode = 1,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = "Fatal"
                    };
                case ErrorEnum.InvalidAlias:
                    return new ErrorDetails
                    {
                        error = $"The alias {alias} was not found, please try again and use a valid alias configiration name",
                        columnNumber = 0,
                        errorCode = 2,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = "Fatal"
                    };

                case ErrorEnum.NotFoundConectionString:
                    return new ErrorDetails
                    {
                        error = $"Not found conection string",
                        columnNumber = 0,
                        errorCode = 2,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = "Fatal"
                    };

                default:
                    return new ErrorDetails
                    {
                        columnNumber = 0,
                        error = exception,
                        errorCode = 10,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = "Fatal"
                    };
            }
        }
    }

}
