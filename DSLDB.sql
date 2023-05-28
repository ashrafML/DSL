
/****** Object:  Table [dbo].[DSL_Methods]    Script Date: 5/28/2023 7:23:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DSL_Methods](
	[Sys_Key] [int] IDENTITY(1,1) NOT NULL,
	[MethodName] [nvarchar](1000) NULL,
	[MethodDefinition] [nvarchar](1000) NULL,
	[User_Key] [int] NULL,
	[Flag] [int] NULL,
	[HospID] [int] NULL,
	[Date_Created] [datetime] NULL,
 CONSTRAINT [PK_DSL_Methods] PRIMARY KEY CLUSTERED 
(
	[Sys_Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


