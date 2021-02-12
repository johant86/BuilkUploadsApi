USE [FilesExport_DB]
GO
CREATE TABLE [dbo].[tb_Source]
(
    [id] INT IDENTITY NOT NULL,
	[type] VARCHAR(50)   NOT NULL,
	[lastModificationDate] DATETIME  NOT NULL,
	[lastModificationUser] INT NOT NULL,
	CONSTRAINT [PK_ID_tb_Source] PRIMARY KEY CLUSTERED ([id] ASC)
)

GO
CREATE TABLE [dbo].[tb_SourceConfiguration]
(
    [id] INT IDENTITY NOT NULL,
	[idSource] INT   NOT NULL,
	[alias] VARCHAR(50) NOT NULL,
	[tableName] VARCHAR(50) NOT NULL,
	[conectionString] VARCHAR(200)  NULL,
	[sharePointSiteUrl] VARCHAR(50)  NULL,
	[sharePointListName] VARCHAR(50)  NULL,
	[lastModificationDate] DATETIME  NOT NULL,
	[lastModificationUser] INT  NOT NULL,
	CONSTRAINT [PK_ID_tb_SourceConfiguration] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT UK_builkUploads_tb_SourceConfiguration UNIQUE ([alias]),
	CONSTRAINT [FK_builkUploads_tb_SourceConfiguration_idSource_tb_Source]
	FOREIGN KEY ([idSource])
    REFERENCES [dbo].[tb_Source] ([id])
)

GO
CREATE TABLE [dbo].[tb_Validation]
(
    [id] INT IDENTITY NOT NULL,
	[name] VARCHAR(50)  NULL,
	[validation]  VARCHAR(50) NOT NULL,	
	[lastModificationDate] DATETIME  NOT  NULL,
	[lastModificationUser] INT  NOT NULL,
	CONSTRAINT [PK_ID_tb_Validation] PRIMARY KEY CLUSTERED ([id] ASC)
)

GO
CREATE TABLE [dbo].[tb_DataType]
(
    [id] INT IDENTITY NOT NULL,
	[name] VARCHAR(50)  NULL,
	[description]  VARCHAR(50) NOT NULL,	
	[lastModificationDate] DATETIME  NOT  NULL,
	[lastModificationUser] INT  NOT NULL,
	CONSTRAINT [PK_ID_tb_DataType] PRIMARY KEY CLUSTERED ([id] ASC)
)

GO
CREATE TABLE [dbo].[tb_ColumnBySource]
(
    [id] INT IDENTITY NOT NULL,
	[idSourceConfiguration] INT NOT NULL,
	[filecolumnName] VARCHAR(50)  NULL,
	[columnName] VARCHAR(50)  NOT NULL,
	[idValidation] INT NULL,
	[idDataType] INT  NOT NULL,	
	[lastModificationDate] DATETIME   NULL,
	[lastModificationUser] INT  NULL,
	CONSTRAINT [PK_ID_tb_ColumnBySource] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_builkUploads_tb_ColumnsBySource_idSourceConfiguration_tb_SourceConfiguration]
	FOREIGN KEY ([idSourceConfiguration])
    REFERENCES [dbo].[tb_SourceConfiguration] ([id]),
	CONSTRAINT [FK_builkUploads_tb_ColumnBySource_idValidation_tb_Validations]
	FOREIGN KEY ([idValidation])
    REFERENCES [dbo].[tb_DataType] ([id]),
	CONSTRAINT [FK_builkUploads_tb_ColumnsBySource_idDataType_tb_DataType]
	FOREIGN KEY ([idDatatype])
)

GO

---------------------SOURCE----------------------------------
INSERT INTO [dbo].[tb_Source] VALUES ('SQL SERVER',GETDATE(),1);
INSERT INTO [dbo].[tb_Source] VALUES ('SHARE POINT',GETDATE(),1);

---------------------SERVER--------------------------------------
INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (1,'RAF','[dbo].[tb_Document]','Server=CRSJODEV014;Database=Data_Upload_API;User Id=toji;Password=Sykes2021;',NULL,NULL,GETDATE(),1);
INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (1,'Example2','[tb_Example2]','Server=(LocalDB)\LocalDB;Database=Test_DB;User Id=sa;Password=123456;',NULL,NULL,GETDATE(),1);

-------------------LOCAL DB-------------------------------------
INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (1,'RAF','[dbo].[tb_Document]','Server=(LocalDB)\LocalDB;Database=Test_DB;User Id=sa;Password=123456;',NULL,NULL,GETDATE(),1);
INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (1,'Example2','[tb_Example2]','Server=(LocalDB)\LocalDB;Database=Test_DB;User Id=sa;Password=123456;',NULL,NULL,GETDATE(),1);
INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (2,'RAFTNotifier',NULL,NULL,'http://lamazdev005/tools/RAFT','RAFTNotifier',GETDATE(),1);

---------------------FIRST TABLE------------------------------------------------------------------
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'RAFTID','Raftid',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'TokenUsed','TokenUsed',NULL,3,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Token','Token',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'ResponseInfo','ResponseInfo',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Phone','Phone',1,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Email','Email',3,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Identification','Identification',2,1,GETDATE(),1)
---------------------SECUND TABLE---------------------------------------------
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'date','date',NULL,4,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'makeup','makeup',NULL,2,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'type','type',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'wager Type','wagerType',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'league','league',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'line Type','lineType',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'description','description',NULL,1,GETDATE(),1)

---------------------SHARE POINT LIST-------------------
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (3,'RAFTID','Title',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (3,'TokenUsed','TokenUsed',NULL,3,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (3,'Token','Token',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (3,'ResponseInfo','ResponseInfo',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (3,'Phone','Phone',1,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (3,'Email','Email',3,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (3,'Identification','Identification',2,1,GETDATE(),1)

---------------------VALIDATIONS-----------------------------------------------------
INSERT INTO [dbo].[tb_Validation] VALUES ('Phone validation','Phone',GETDATE(),1)
INSERT INTO [dbo].[tb_Validation] VALUES ('Identification validation','Identification',GETDATE(),1)
INSERT INTO [dbo].[tb_Validation] VALUES ('Email validation','Email',GETDATE(),1)


------------------------------------------------------------------------------------
-----------------------------TEST DB SOURCE-----------------------------------------
--------------------------------------------------------------------------------

USE [Test_DB]

GO
CREATE TABLE  [dbo].[tb_Document]
(
    [id] INT IDENTITY NOT NULL,
	[Raftid] VARCHAR(50)   NULL,
	[TokenUsed] BIT   NULL,
	[Token] VARCHAR(50)  NULL,
	[ResponseInfo] VARCHAR(50)  NULL,
	[Phone] VARCHAR(50) NULL,
    [Email] VARCHAR(50) NULL,
	[Identification] VARCHAR(50)  NULL,
)

CREATE TABLE  [dbo].[tb_Example2]
(
    [id] INT IDENTITY NOT NULL,			
	[date] DATETIME   NULL,
	[makeup] INT NOT NULL,
	[type] VARCHAR(50)   NULL,
	[wagerType] VARCHAR(50)   NULL,
	[league] VARCHAR(50)  NULL,
	[lineType] VARCHAR(50)  NULL,
    [description] VARCHAR(50)   NULL,
)
DELETE [dbo].[tb_Example2]
DELETE [dbo].[tb_Document]

SELECT  * FROM  [dbo].[tb_Example2]
SELECT  * FROM  [dbo].[tb_Document]

DBCC CHECKIDENT ('[dbo].[tb_Example2]', RESEED, 0)  
DBCC CHECKIDENT ('[dbo].[tb_Example2]')
DBCC CHECKIDENT ('[dbo].[tb_Document]', RESEED, 0) 
DBCC CHECKIDENT ('[dbo].[tb_Document]')

-------------------------------------------------------------------------------------
DBCC CHECKIDENT ('[dbo].[Source]', RESEED, 0)  
DBCC CHECKIDENT ('[dbo].[Source]')

INSERT INTO [dbo].[tb_Source] VALUES ('SQL SERVER',GETDATE(),1);
INSERT INTO [dbo].[tb_Source] VALUES ('SHARE POINT',GETDATE(),1);

INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (1,'RAF','[dbo].[tb_Document]','Server=(LocalDB)\LocalDB;Database=Test_DB;User Id=sa;Password=123456;',NULL,NULL,GETDATE(),1);
INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (1,'Example2','[tb_Example2]','Server=(LocalDB)\LocalDB;Database=Test_DB;User Id=sa;Password=123456;',NULL,NULL,GETDATE(),1);
INSERT INTO [dbo].[tb_SourceConfiguration] VALUES (2,'RAFTNotifier',NULL,NULL,'http://lamazdev005/tools/RAFT','RAFTNotifier',GETDATE(),1);

INSERT INTO [dbo].[tb_Validation] VALUES ('Phone validation','Phone',GETDATE(),1)
INSERT INTO [dbo].[tb_Validation] VALUES ('Identification validation','Identification',GETDATE(),1)
INSERT INTO [dbo].[tb_Validation] VALUES ('Email validation','Email',GETDATE(),1)

INSERT INTO [dbo].[tb_DataType] VALUES ('string','string',GETDATE(),1)
INSERT INTO [dbo].[tb_DataType] VALUES ('int','integer',GETDATE(),1)
INSERT INTO [dbo].[tb_DataType] VALUES ('bool','bolean',GETDATE(),1)
INSERT INTO [dbo].[tb_DataType] VALUES ('datetime','date',GETDATE(),1)

INSERT INTO [dbo].[tb_ColumnBySource] VALUES (1,'RAFTID','Raftid',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (1,'TokenUsed','TokenUsed',NULL,3,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (1,'Token','Token',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (1,'ResponseInfo','ResponseInfo',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (1,'Phone','Phone',1,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (1,'Email','Email',3,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (1,'Identification','Identification',2,1,GETDATE(),1)

INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'date','date',NULL,4,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'makeup','makeup',NULL,2,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'type','type',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'wager Type','wagerType',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'league','league',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'line Type','lineType',NULL,1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnBySource] VALUES (2,'description','description',NULL,1,GETDATE(),1)