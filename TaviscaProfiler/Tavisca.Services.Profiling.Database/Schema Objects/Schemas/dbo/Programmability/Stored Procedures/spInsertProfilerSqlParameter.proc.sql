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
