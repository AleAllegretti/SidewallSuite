USE [DBSidewall]
GO

/****** Object:  Table [dbo].[OutputCalcoli]    Script Date: 15/10/2023 11:23:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OutputCalcoli](
	[Codice] [varchar](50) NOT NULL,
	[CapacitaTonOra] [float] NULL,
	[LarghezzaUtile] [float] NULL,
	[PesoNastro] [float] NULL,
	[MaxTensPulley] [float] NULL,
	[MaxTensLaterale] [float] NULL,
	[FattSicurezza] [float] NULL,
	[FattSicurezzaPiste] [float] NULL,
	[TailTakeUp] [float] NULL,
	[PotRichiesta] [float] NULL,
	[PotSuggerita] [decimal](18, 0) NULL,
	[MinPulleyDiameter] [float] NULL,
	[MinDeflectionWheel] [float] NULL,
	[MinWheelWidth] [float] NULL,
	[Versione] [int] NULL,
	[Data] [varchar](50) NULL,
 CONSTRAINT [PK_OutputCalcoli] PRIMARY KEY CLUSTERED 
(
	[Codice] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

