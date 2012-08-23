CREATE PROCEDURE [dbo].[spGetTraceAndTimings]
	@TransactionId uniqueidentifier
as
select 
	ProfilerId, 
	PD.Name as DebugName, 
	Method, 
	RequestUrl, 
	MachineName, 
	UserName, 
	StartedUtc, 
	PD.TransactionId, 
	Code, 
	ProfileLevel ,
	TimingId,  
	IsRoot, 
	PT.Name as TimingName, 
	KeyValues, 
	Duration, 
	DurationWithOutChildren, 
	Start, 
	ParentTimingId, 
	ProfiledOrder, 
	SqlTimingDuration, 
	ExecutedNonQueries, 
	ExecutedScalers, 
	ExecutedReaders, 
	ManagedThreadId 
from ProfilerDebug PD
left outer join ProfilerTiming PT
on PD.TransactionId = PT.TransactionId
where PD.TransactionId = @TransactionId 
order by Start, ManagedThreadId
