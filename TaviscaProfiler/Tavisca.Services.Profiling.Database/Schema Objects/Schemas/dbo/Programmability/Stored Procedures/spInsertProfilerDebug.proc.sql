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
