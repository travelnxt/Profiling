/*
Deployment script for dProfilerLogs
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "dProfilerLogs"
:setvar DefaultDataPath "F:\SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "F:\SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
USE [master]
GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL
    AND DATABASEPROPERTYEX(N'$(DatabaseName)','Status') <> N'ONLINE')
BEGIN
    RAISERROR(N'The state of the target database, %s, is not set to ONLINE. To deploy to this database, its state must be set to ONLINE.', 16, 127,N'$(DatabaseName)') WITH NOWAIT
    RETURN
END

GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END

GO
PRINT N'Creating $(DatabaseName)...'
GO
CREATE DATABASE [$(DatabaseName)]
    ON 
    PRIMARY(NAME = [dProfilerLogs], FILENAME = N'$(DefaultDataPath)dProfilerLogs.mdf')
    LOG ON (NAME = [dProfilerLogs_log], FILENAME = N'$(DefaultLogPath)dProfilerLogs_log.ldf') COLLATE SQL_Latin1_General_CP1_CI_AS
GO
EXECUTE sp_dbcmptlevel [$(DatabaseName)], 90;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY NONE,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
USE [$(DatabaseName)]
GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';


GO
/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

GO
PRINT N'Creating [dbo].[ProfilerDebug]...';


GO
CREATE TABLE [dbo].[ProfilerDebug] (
    [ProfilerId]    UNIQUEIDENTIFIER NOT NULL,
    [Name]          NVARCHAR (512)   NOT NULL,
    [Method]        NVARCHAR (50)    NULL,
    [RequestUrl]    NVARCHAR (512)   NULL,
    [MachineName]   NVARCHAR (256)   NULL,
    [UserName]      NVARCHAR (256)   NULL,
    [StartedUtc]    DATETIME         NULL,
    [TransactionId] UNIQUEIDENTIFIER NULL,
    [Code]          NVARCHAR (10)    NULL,
    [Id]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProfileLevel]  NVARCHAR (50)    NULL,
    CONSTRAINT [PK_ProfilerDebug] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY]
) ON [PRIMARY];


GO
PRINT N'Creating [dbo].[ProfilerSqlParameter]...';


GO
CREATE TABLE [dbo].[ProfilerSqlParameter] (
    [Id]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [SqlTimingId] UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (256)   NULL,
    [Value]       NVARCHAR (1024)  NULL,
    [DbType]      NVARCHAR (50)    NULL,
    [Size]        INT              NULL,
    CONSTRAINT [PK_ProfiledSqlParameter] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY]
) ON [PRIMARY];


GO
PRINT N'Creating [dbo].[ProfilerSqlTiming]...';


GO
CREATE TABLE [dbo].[ProfilerSqlTiming] (
    [Id]                             BIGINT           IDENTITY (1, 1) NOT NULL,
    [ExecuteType]                    NVARCHAR (50)    NULL,
    [CommandString]                  NVARCHAR (1024)  NULL,
    [StartMilliseconds]              DECIMAL (18, 4)  NULL,
    [DurationMilliseconds]           DECIMAL (18, 4)  NULL,
    [FirstFetchDurationMilliseconds] DECIMAL (18, 4)  NULL,
    [TimingId]                       UNIQUEIDENTIFIER NOT NULL,
    [TransactionId]                  UNIQUEIDENTIFIER NOT NULL,
    [ProfiledOrder]                  INT              NULL,
    [SqlTimingId]                    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ProfilerSqlTiming] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY]
) ON [PRIMARY];


GO
PRINT N'Creating [dbo].[ProfilerTiming]...';


GO
CREATE TABLE [dbo].[ProfilerTiming] (
    [Id]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [TimingId]                UNIQUEIDENTIFIER NOT NULL,
    [TransactionId]           UNIQUEIDENTIFIER NOT NULL,
    [IsRoot]                  BIT              NOT NULL,
    [Name]                    NVARCHAR (512)   NULL,
    [KeyValues]               NVARCHAR (MAX)   NULL,
    [Duration]                DECIMAL (18, 4)  NULL,
    [DurationWithOutChildren] DECIMAL (18, 4)  NULL,
    [Start]                   DECIMAL (18, 4)  NULL,
    [ParentTimingId]          UNIQUEIDENTIFIER NULL,
    [ProfiledOrder]           INT              NULL,
    [SqlTimingDuration]       DECIMAL (18, 4)  NULL,
    [ExecutedScalers]         INT              NULL,
    [ExecutedNonQueries]      INT              NULL,
    [ExecutedReaders]         INT              NULL,
    [ManagedThreadId]         INT              NULL,
    CONSTRAINT [PK_ProfilerTiming] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY]
) ON [PRIMARY];


GO
PRINT N'Creating [dbo].[spInsertProfilerDebug]...';


GO
CREATE PROCEDURE [dbo].[spInsertProfilerDebug]
	@ProfilerId uniqueidentifier,
	@Name nvarchar(512),
	@Method nvarchar(50),
	@RequestUrl nvarchar(512),
	@MachineName nvarchar(256),
	@UserName	nvarchar(256),
	@StartedUtc datetime,
	@TransactionId uniqueidentifier,
	@Code nvarchar(10),
	@ProfileLevel nvarchar(50)
AS
INSERT INTO [dbo].[ProfilerDebug](
	ProfilerId, 
	Name, 
	Method, 
	RequestUrl, 
	MachineName, 
	UserName, 
	StartedUtc, 
	TransactionId, 
	Code, 
	ProfileLevel)
VALUES(
	@ProfilerId, 
	@Name, 
	@Method, 
	@RequestUrl, 
	@MachineName, 
	@UserName, 
	@StartedUtc, 
	@TransactionId, 
	@Code, 
	@ProfileLevel)
GO
PRINT N'Creating [dbo].[spInsertProfilerSqlParameter]...';


GO
CREATE PROCEDURE [dbo].[spInsertProfilerSqlParameter]
	@SqlTimingId uniqueidentifier,
	@Name nvarchar(245),
	@Value nvarchar(1024),
	@DbType nvarchar(50),
	@Size int
AS
INSERT INTO [dbo].[ProfilerSqlParameter](
	SqlTimingId, 
	Name, 
	Value, 
	DbType, 
	Size) 
VALUES(
	@SqlTimingId, 
	@Name, 
	@Value, 
	@DbType, 
	@Size)
GO
PRINT N'Creating [dbo].[spInsertProfilerSqlTiming]...';


GO
CREATE PROCEDURE [dbo].[spInsertProfilerSqlTiming]
	@ExecuteType nvarchar(50),
	@CommandString nvarchar(1024),
	@StartMilliseconds decimal(18, 4),
	@DurationMilliseconds decimal(18, 4),
	@FirstFetchDurationMilliseconds decimal(18,4),
	@TimingId uniqueidentifier,
	@TransactionId uniqueidentifier,
	@ProfiledOrder int,
	@SqlTimingId uniqueidentifier
AS
INSERT INTO [dbo].[ProfilerSqlTiming](
	ExecuteType, 
	CommandString, 
	StartMilliseconds, 
	DurationMilliseconds, 
	FirstFetchDurationMilliseconds, 
	TimingId, 
	TransactionId, 
	ProfiledOrder, 
	SqlTimingId)
VALUES(
	@ExecuteType, 
	@CommandString, 
	@StartMilliseconds, 
	@DurationMilliseconds, 
	@FirstFetchDurationMilliseconds, 
	@TimingId, 
	@TransactionId, 
	@ProfiledOrder, 
	@SqlTimingId)
GO
PRINT N'Creating [dbo].[spInsertProfilerTiming]...';


GO
CREATE PROCEDURE [dbo].[spInsertProfilerTiming]
	@TimingId uniqueidentifier,
	@TransactionId uniqueidentifier,
	@IsRoot bit,
	@Name nvarchar(512),
	@KeyValues nvarchar(MAX),
	@Duration decimal(18,4),
	@DurationWithOutChildren decimal(18, 4),
	@Start decimal(18,4),
	@ParentTimingId uniqueidentifier,
	@ProfiledOrder int,
	@SqlTimingDuration decimal(18, 4),
	@ExecutedScalers int ,
	@ExecutedNonQueries int,
	@ExecutedReaders int,
	@ManagedThreadId int
AS
INSERT INTO [dbo].[ProfilerTiming](
	TimingId, 
	TransactionId, 
	IsRoot, 
	Name, 
	KeyValues, 
	Duration, 
	DurationWithOutChildren, 
	Start, 
	ParentTimingId, 
	ProfiledOrder,
	SqlTimingDuration,
	ExecutedScalers,
	ExecutedNonQueries,
	ExecutedReaders,
	ManagedThreadId
	)
VALUES(
	@TimingId, 
	@TransactionId, 
	@IsRoot, 
	@Name, 
	@KeyValues, 
	@Duration, 
	@DurationWithOutChildren, 
	@Start, 
	@ParentTimingId, 
	@ProfiledOrder,
	@SqlTimingDuration,
	@ExecutedScalers,
	@ExecutedNonQueries,
	@ExecutedReaders,
	@ManagedThreadId
	)
GO
-- Refactoring step to update target server with deployed transaction logs
CREATE TABLE  [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)
GO
sp_addextendedproperty N'microsoft_database_tools_support', N'refactoring log', N'schema', N'dbo', N'table', N'__RefactorLog'
GO

GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

GO
