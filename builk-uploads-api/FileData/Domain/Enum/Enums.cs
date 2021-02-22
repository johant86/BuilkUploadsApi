public enum ErrorEnum
{
    InvalidFileExtension,
    Exception,
    InvalidAlias,
    NotFoundConectionString,
    InvalidColumns,
    DataType,
    InvalidCulumnsNumber,
    InvalidData,
    ColumnsNotFound,
    ConfigurationNotFound,
    InvalidFileExtensionDescription,
    ColumnsConfigurationNotFound,
    Uploaded,
    UploadError,
    InvalidConection,
    InvalidColumn,
    PrimaryKeyError
}


public enum ErrorCodesEnum
{
    Extension = 1,
    Alias = 2,
    InvalidConection = 3,
    Validation = 4,
    Datatype = 5,
    InvalidColumn = 6,
    InvalidCulumnsNumber = 7,
    InvalidData = 8,
    ColumnsNotFound=9,
    ConfigurationNotFound=10,
    PrimaryKeyError=11
}

public enum DataSource
{
    SQL = 1,
    SHAREPOINT = 2,
}
public enum ValidationsEnum
{
    Phone = 1,
    Identification = 2,
    Email = 3
}


public static class Severity
{
    public const string Fatal = "FATAL";
    public const string Low = "LOW";
}
public static class MessageDescription
{
    public const string Validation = "Invalid data";
    public const string Extension = "Invalid extension";
    public const string Alias = "Invalid alias";
    public const string InvalidConection = "Invalid conection";
    public const string Datatype = "Data type error";
    public const string InvalidColumn = "Column name Error";
    public const string Uploaded = "Uploaded successfully";
    public const string UploadError = "Upload failure";
    public const string InvalidCulumnsNumber = "Column difference between document and destination";
    public const string ErrorConfigurations = "Configuration not found.";
}

public static class variablesType
{
    public const string String = "string";
    public const string Int = "int";
    public const string Datetime = "datetime";
    public const string Boolean = "bool";
    public const string Decimal = "decimal";
}


