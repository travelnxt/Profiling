CREATE TABLE [dbo].[ProfilerSqlParameter](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SqlTimingId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NULL,
	[Value] [nvarchar](1024) NULL,
	[DbType] [nvarchar](50) NULL,
	[Size] [int] NULL,
 CONSTRAINT [PK_ProfiledSqlParameter] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
