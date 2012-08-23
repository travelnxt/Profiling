CREATE PROCEDURE [dbo].[spGetSqlTimings]
	@TimingId uniqueidentifier,
	@TransactionId uniqueidentifier
as 
select 
	TimingId, 
	TransactionId,
	PST.SqlTimingId ,
	ExecuteType, 
	CommandString, 
	StartMilliseconds, 
	DurationMilliseconds,  
	FirstFetchDurationMilliseconds, 
	ProfiledOrder,
	PSP.Name, 
	PSP.Value, 
	PSP.Size,
	PSP.DbType
from [dbo].[ProfilerSqlTiming] PST
left outer join [dbo].[ProfilerSqlParameter] PSP on PST.SqlTimingId = PSP.SqlTimingId
where PST.TransactionId = @TransactionId and PST.TimingId = @TimingId 
order by PST.StartMilliseconds
