CREATE TABLE [dbo].[ProfilerSqlTiming](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ExecuteType] [nvarchar](50) NULL,
	[CommandString] [nvarchar](1024) NULL,
	[StartMilliseconds] [decimal](18, 4) NULL,
	[DurationMilliseconds] [decimal](18, 4) NULL,
	[FirstFetchDurationMilliseconds] [decimal](18, 4) NULL,
	[TimingId] [uniqueidentifier] NOT NULL,
	[TransactionId] [uniqueidentifier] NOT NULL,
	[ProfiledOrder] [int] NULL,
	[SqlTimingId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ProfilerSqlTiming] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]