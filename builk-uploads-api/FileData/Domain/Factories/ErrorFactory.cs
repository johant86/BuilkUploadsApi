using System;

namespace builk_uploads_api.FileData.Domain.Factories
{
    public class ErrorFactory
    {

        public static ErrorDetails GetError(ErrorEnum errorEnum, string error = "", int col=0, int row=0, string severity = "")
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            switch (errorEnum)
            {
                case ErrorEnum.InvalidFileExtension:
                    return new ErrorDetails
                    {
                        error = $"The file extension {error} is not valid, please check extension specifications and try again",
                        //columnNumber = column,
                        position = col!=0 ? alpha[col-1].ToString()+ row : "-",
                        errorCode = (int)ErrorCodesEnum.Extension,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };
                case ErrorEnum.InvalidAlias:
                    return new ErrorDetails
                    {
                        error = $"The alias {error} was not found, please try again and use a valid alias configiration name",
                        //columnNumber = 0,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.Alias,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                case ErrorEnum.NotFoundConectionString:
                    return new ErrorDetails
                    {
                        error = $"Not found conection string",
                        //columnNumber = 0,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.InvalidConection,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                case ErrorEnum.InvalidColumns:
                    return new ErrorDetails
                    {
                        error = $"the column {error} do not match the document settings, please check the column name.",
                        //columnNumber = column,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.InvalidColumn,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                case ErrorEnum.DataType:
                    return new ErrorDetails
                    {
                        error = error,
                        //columnNumber = column,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.Datatype,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                case ErrorEnum.InvalidCulumnsNumber:
                    return new ErrorDetails
                    {
                        error = error,
                        //columnNumber = column,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.InvalidCulumnsNumber,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                case ErrorEnum.InvalidData:
                    return new ErrorDetails
                    {
                        error = error,
                        //columnNumber = column,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.InvalidData,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                case ErrorEnum.ColumnsNotFound:
                    return new ErrorDetails
                    {
                        error = error,
                        //columnNumber = column,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.ColumnsNotFound,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                case ErrorEnum.ConfigurationNotFound:
                    return new ErrorDetails
                    {
                        error = error,
                        //columnNumber = column,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        errorCode = (int)ErrorCodesEnum.ConfigurationNotFound,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };

                default:
                    return new ErrorDetails
                    {
                        //columnNumber = 0,
                        position = col != 0 ? alpha[col - 1].ToString() + row : "-",
                        error = "",
                        errorCode = 10,
                        errorDate = DateTime.Now.ToShortDateString(),
                        severity = severity
                    };
            }
        }
    }

}
