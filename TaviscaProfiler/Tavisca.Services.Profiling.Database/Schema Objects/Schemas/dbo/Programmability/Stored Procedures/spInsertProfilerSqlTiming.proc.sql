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
