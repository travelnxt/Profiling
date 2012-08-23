CREATE TABLE [dbo].[ProfilerDebug](
	[ProfilerId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
	[Method] [nvarchar](50) NULL,
	[RequestUrl] [nvarchar](512) NULL,
	[MachineName] [nvarchar](256) NULL,
	[UserName] [nvarchar](256) NULL,
	[StartedUtc] [datetime] NULL,
	[TransactionId] [uniqueidentifier] NULL,
	[Code] [nvarchar](10) NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProfileLevel] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProfilerDebug] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
