USE [FilesExport_DB]
GO
CREATE TABLE [dbo].[tb_Source]
(
    [id] INT IDENTITY NOT NULL,
	[type] VARCHAR(50)   NOT NULL,
	[lastModificationDate] DATETIME  NOT NULL,
	[lastModificationUser] INT NOT NULL
)
INSERT INTO [dbo].[tb_Source] VALUES ('SQL SERVER',GETDATE(),1);
INSERT INTO [dbo].[tb_Source] VALUES ('SHARE POINT',GETDATE(),1);

GO
CREATE TABLE [dbo].[tb_SourceConfiguration]
(
    [id] INT IDENTITY NOT NULL,
	[idSource] INT   NOT NULL,
	[alias] VARCHAR(50) NOT NULL,
	--[storeProcedpName] VARCHAR(50) NOT NULL,
	[tableName] VARCHAR(50) NOT NULL,
	[conectionString] VARCHAR(200)  NULL,
	[sharePointSiteUrl] VARCHAR(50)  NULL,
	[sharePointListName] VARCHAR(50)  NULL,
	[lastModificationDate] DATETIME   NULL,
	[lastModificationUser] INT  NULL
)

INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (1,'RAF','[dbo].tb_Document','Server=(LocalDB)\LocalDB;Database=Test_DB;User Id=sa;Password=123456;',NULL,NULL,GETDATE(),1);

GO
CREATE TABLE [dbo].[tb_ColumnsBySource]
(
    [id] INT IDENTITY NOT NULL,
	[idSourceConfiguration] INT NOT NULL,
	[filecolumnName] VARCHAR(50)  NULL,
	--[StoreProcParameter] VARCHAR(50)  NOT NULL,
	[columnName] VARCHAR(50)  NOT NULL,
	[idValidation] INT NULL,
	[type] VARCHAR(50)  NOT NULL,	
	[lastModificationDate] DATETIME   NULL,
	[lastModificationUser] INT  NULL
)

INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'RAFTID','Raftid',NULL,'string',GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'TokenUsed','TokenUsed',NULL,'bool',GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Token','Token',NULL,'string',GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'ResponseInfo','ResponseInfo',NULL,'string',GETDATE(),1)

GO
CREATE PROCEDURE [dbo].[sp_GetColumnsBySource]
@prmAlias VARCHAR(50)
AS
BEGIN
    SELECT 
	CS.id,
	CS.filecolumnName,
	CS.StoreProcParameter,
	CS.[type],
	VL.[validation]
	FROM  [dbo].[tb_ColumnsBySource] CS WITH(NOLOCK) 
	INNER JOIN [dbo].[tb_SourceConfiguration] SC WITH(NOLOCK) ON CS.idSourceConfiguration = SC.id
	LEFT JOIN [dbo].[tb_Validations] VL WITH(NOLOCK) ON VL.id = CS.idValidation
	WHERE SC.alias = @prmAlias

END

GO
CREATE TABLE [dbo].[tb_Validations]
(
    [id] INT IDENTITY NOT NULL,
	[name] VARCHAR(50) NOT NULL,
	[validation]  VARCHAR(50) NOT NULL,	
	[lastModificationDate] DATETIME   NULL,
	[lastModificationUser] INT  NULL
)

GO

GO
CREATE PROCEDURE [dbo].[sp_GetBuilkUploadsConfigurationsByAlias]
@prmAlias VARCHAR(50)
AS
BEGIN
    SELECT SC.id, S.id AS 'idSource', S.[type], SC.tableName,SC.conectionString,SC.sharePointListName,SC.sharePointSiteUrl FROM  [tb_Source] S WITH(NOLOCK) INNER JOIN [tb_SourceConfiguration] SC WITH(NOLOCK) ON S.id = SC.idSource WHERE SC.alias =@prmAlias

END

GO

CREATE TABLE [dbo].[storageProceduresError](
	[storageProcedureErrorID] INT IDENTITY(1,1) NOT NULL,
	[ErrorCategoryId] BIGINT NOT NULL,
	[ErrorNumber] BIGINT NOT NULL,
	[ErrorProcedure] VARCHAR(100) NOT NULL,
	[ErrorLine] BIGINT NOT NULL,
	[ErrorDescription] VARCHAR(500) NOT NULL,
	[ErrorDate] DATETIME NOT NULL,
)
GO
CREATE PROCEDURE [dbo].[storageProceduresErrorInsert](
	  @P_ErrorNumber		BIGINT
	, @P_ErrorCategoryId	INT = 0
	, @P_ErrorProcedure		NVARCHAR (100)
	, @P_ErrorLine			BIGINT
	, @P_ErrorDescription	NVARCHAR (500)
)
AS
BEGIN
	SET NOCOUNT ON;
		INSERT INTO [dbo].[storageProceduresError](
				  [errorNumber]
				, [ErrorCategoryId]
				, [errorProcedure]
				, [errorLine]
				, [errorDescription]
				, [errorDate]
			)
			VALUES 
			(	
				  @P_ErrorNumber		
				, @P_ErrorCategoryId	
				, @P_ErrorProcedure		
				, @P_ErrorLine			
				, @P_ErrorDescription
				, GETDATE()
			)
END	


GO


--------------------------------------------------------------------------------
-----------------------------TEST DB SOURCE-------------------------------------
--------------------------------------------------------------------------------

USE [Test_DB]
GO
CREATE TABLE [dbo].[storageProceduresError](
	[storageProcedureErrorID] INT IDENTITY(1,1) NOT NULL,
	[ErrorCategoryId] BIGINT NOT NULL,
	[ErrorNumber] BIGINT NOT NULL,
	[ErrorProcedure] VARCHAR(100) NOT NULL,
	[ErrorLine] BIGINT NOT NULL,
	[ErrorDescription] VARCHAR(500) NOT NULL,
	[ErrorDate] DATETIME NOT NULL,
)
GO
CREATE PROCEDURE [dbo].[storageProceduresErrorInsert](
	  @P_ErrorNumber		BIGINT
	, @P_ErrorCategoryId	INT = 0
	, @P_ErrorProcedure		NVARCHAR (100)
	, @P_ErrorLine			BIGINT
	, @P_ErrorDescription	NVARCHAR (500)
)
AS
BEGIN
	SET NOCOUNT ON;
		INSERT INTO [dbo].[storageProceduresError](
				  [errorNumber]
				, [ErrorCategoryId]
				, [errorProcedure]
				, [errorLine]
				, [errorDescription]
				, [errorDate]
			)
			VALUES 
			(	
				  @P_ErrorNumber		
				, @P_ErrorCategoryId	
				, @P_ErrorProcedure		
				, @P_ErrorLine			
				, @P_ErrorDescription
				, GETDATE()
			)
END	


GO
CREATE TABLE  [dbo].[tb_Document]
(
    [id] INT IDENTITY NOT NULL,
	[Raftid] varchar(50)   NULL,
	[TokenUsed] bit   NULL,
	[Token] varchar(50)  NULL,
	[ResponseInfo] varchar(50)  NULL,
)

INSERT INTO [dbo].[tb_Document] VALUES ('RAFID219886',1,'301198861003722020','192.168.1.1')

CREATE PROCEDURE [dbo].[sp_GetDocument]

AS
BEGIN
    SELECT * FROM  [dbo].[tb_Document] T WITH(NOLOCK)
END

GO
CREATE PROCEDURE [dbo].[sp_InsetDocumentFields]
@prmRaftid VARCHAR(50),
@prmTokenUsed BIT,
@prmToken VARCHAR(50),
@prmResponseInfo VARCHAR(50)

AS
BEGIN
DECLARE @P_Success INT

BEGIN TRY
	BEGIN TRAN localTransaction
    INSERT INTO [dbo].[tb_Document] VALUES (@prmRaftid,@prmTokenUsed,@prmToken,@prmResponseInfo);
	SET @P_Success = (SELECT MAX(id) AS id FROM [dbo].[tb_Document]);
	SELECT @P_Success AS 'id';

	COMMIT TRAN localTransaction			
END TRY
BEGIN CATCH
	SET @P_Success = 0;
	ROLLBACK TRAN localTransaction
	DECLARE @localErrorNumber BIGINT;
	DECLARE @localErrorProcedure VARCHAR (100);
	DECLARE @localErrorLine BIGINT;
	DECLARE @localErrorDescription VARCHAR (500);

	SELECT	  @localErrorNumber			= ERROR_NUMBER()
			, @localErrorProcedure		= ERROR_PROCEDURE() 
			, @localErrorLine			= ERROR_LINE()
			, @localErrorDescription	= ERROR_MESSAGE() 
	;  
	
	EXECUTE	[dbo].[storageProceduresErrorInsert]
			  @P_ErrorNumber		= @localErrorNumber
			, @P_ErrorCategoryId	= 1 
			, @P_ErrorProcedure		= @localErrorProcedure
			, @P_ErrorLine			= @localErrorLine
			, @P_ErrorDescription	= @localErrorDescription
END CATCH
END

GO