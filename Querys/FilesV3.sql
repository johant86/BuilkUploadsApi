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
CREATE TABLE [dbo].[tb_Validations]
(
    [id] INT IDENTITY NOT NULL,
	[name] VARCHAR(50)  NULL,
	[validation]  VARCHAR(50) NOT NULL,	
	[lastModificationDate] DATETIME  NOT  NULL,
	[lastModificationUser] INT  NOT NULL,
	CONSTRAINT [PK_ID_tb_Validations] PRIMARY KEY CLUSTERED ([id] ASC)
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
CREATE TABLE [dbo].[tb_ColumnsBySource]
(
    [id] INT IDENTITY NOT NULL,
	[idSourceConfiguration] INT NOT NULL,
	[filecolumnName] VARCHAR(50)  NULL,
	[columnName] VARCHAR(50)  NOT NULL,
	[idValidation] INT NULL,
	[type] VARCHAR(50)  NOT NULL,	
	[lastModificationDate] DATETIME   NULL,
	[lastModificationUser] INT  NULL,
	CONSTRAINT [PK_ID_tb_ColumnsBySource] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_builkUploads_tb_ColumnsBySource_idSourceConfiguration_tb_SourceConfiguration]
	FOREIGN KEY ([idSourceConfiguration])
    REFERENCES [dbo].[tb_SourceConfiguration] ([id]),
	CONSTRAINT [FK_builkUploads_tb_ColumnsBySource_idValidation_tb_Validations]
	FOREIGN KEY ([idValidation])
    REFERENCES [dbo].[tb_Validations] ([id])
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
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'RAFTID','Raftid',NULL,'string',1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'TokenUsed','TokenUsed',NULL,'bool',2,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Token','Token',NULL,'string',3,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'ResponseInfo','ResponseInfo',NULL,'string',4,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Phone','Phone',1,'string',5,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Email','Email',3,'string',6,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (1,'Identification','Identification',2,'string',7,GETDATE(),1)
---------------------SECUND TABLE---------------------------------------------
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (2,'date','date',NULL,'datetime',1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (2,'makeup','makeup',NULL,'int',2,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (2,'type','type',NULL,'string',2,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (2,'wager Type','wagerType',NULL,'string',2,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (2,'league','league',NULL,'string',3,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (2,'line Type','lineType',NULL,'string',4,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (2,'description','description',NULL,'string',5,GETDATE(),1)

---------------------SHARE POINT LIST-------------------
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (3,'RAFTID','Title',NULL,'string',1,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (3,'TokenUsed','TokenUsed',NULL,'bool',2,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (3,'Token','Token',NULL,'string',3,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (3,'ResponseInfo','ResponseInfo',NULL,'string',4,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (3,'Phone','Phone',1,'string',5,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (3,'Email','Email',3,'string',6,GETDATE(),1)
INSERT INTO [dbo].[tb_ColumnsBySource] VALUES (3,'Identification','Identification',2,'string',7,GETDATE(),1)

---------------------VALIDATIONS-----------------------------------------------------
INSERT INTO [dbo].[tb_Validations] VALUES ('Phone validation','Phone',GETDATE(),1)
INSERT INTO [dbo].[tb_Validations] VALUES ('Identification validation','Identification',GETDATE(),1)
INSERT INTO [dbo].[tb_Validations] VALUES ('Email validation','Email',GETDATE(),1)


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

select * from [dbo].[tb_Document]
