USE [DBSidewall]
GO

/****** Object:  Table [dbo].[ParametriFoglioDiCalcolo]    Script Date: 15/10/2023 11:25:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ParametriFoglioDiCalcolo](
	[ID] [int] NOT NULL,
	[Descrizione] [varchar](max) NULL,
	[Valore] [float] NULL,
	[DataUltimoAggiornamento] [varchar](50) NULL,
	[Note] [varchar](max) NULL,
 CONSTRAINT [PK_ParametriFoglioDiCalcolo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

