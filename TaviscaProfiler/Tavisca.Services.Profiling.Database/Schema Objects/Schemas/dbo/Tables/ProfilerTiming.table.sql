CREATE TABLE [dbo].[ProfilerTiming](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TimingId] [uniqueidentifier] NOT NULL,
	[TransactionId] [uniqueidentifier] NOT NULL,
	[IsRoot] [bit] NOT NULL,
	[Name] [nvarchar](512) NULL,
	[KeyValues] [nvarchar](Max) NULL,
	[Duration] [decimal](18, 4) NULL,
	[DurationWithOutChildren] [decimal](18, 4) NULL,
	[Start] [decimal](18, 4) NULL,
	[ParentTimingId] [uniqueidentifier] NULL,
	[ProfiledOrder] [int] NULL,
	[SqlTimingDuration] [decimal](18, 4) NULL,
	[ExecutedScalers] [int] NULL,
	[ExecutedNonQueries] [int] NULL,
	[ExecutedReaders] [int] NULL,
	[ManagedThreadId] [int] NULL,
 CONSTRAINT [PK_ProfilerTiming] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
