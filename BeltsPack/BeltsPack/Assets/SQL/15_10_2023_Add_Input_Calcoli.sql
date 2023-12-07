USE [DBSidewall]
GO

/****** Object:  Table [dbo].[InputCalcoli]    Script Date: 15/10/2023 11:21:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InputCalcoli](
	[Codice] [varchar](50) NOT NULL,
	[Capacity] [float] NULL,
	[FillingFactor] [float] NULL,
	[VelocitaNastro] [float] NULL,
	[PendenzaMax] [float] NULL,
	[Elevazione] [float] NULL,
	[DistDalCentro] [float] NULL,
	[NomeMateriale] [varchar](50) NULL,
	[DensitaMateriale] [float] NULL,
	[AngoloCarico] [float] NULL,
	[DimensioneSingolo] [float] NULL,
	[Versione] [int] NULL,
	[Data] [varchar](50) NULL,
	[Forma] [varchar](50) NULL,
	[EdgeType] [int] NULL,
 CONSTRAINT [PK_InputCalcoli] PRIMARY KEY CLUSTERED 
(
	[Codice] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

