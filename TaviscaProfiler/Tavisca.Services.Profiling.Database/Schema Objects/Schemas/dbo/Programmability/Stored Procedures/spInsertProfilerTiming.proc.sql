﻿CREATE PROCEDURE [dbo].[spInsertProfilerTiming]
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
