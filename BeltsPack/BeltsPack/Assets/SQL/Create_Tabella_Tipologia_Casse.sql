USE [DBSidewall]
GO
/****** Object:  Table [dbo].[Tipologia_Casse]    Script Date: 25/07/2022 08:42:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tipologia_Casse](
	[Nome_Cassa] [varchar](50) NULL,
	[L_Min] [decimal](18, 0) NULL,
	[L_Max] [decimal](18, 0) NULL,
	[P_Min] [decimal](18, 0) NULL,
	[P_Max] [decimal](18, 0) NULL,
	[W_Longherone] [decimal](18, 0) NULL,
	[H_Longherone] [decimal](18, 0) NULL,
	[S_Longherone] [decimal](18, 0) NULL,
	[Presenza_Ganci] [varchar](50) NULL,
	[Rinforzo_Longherone] [varchar](50) NULL,
	[Solo_Ritti] [varchar](50) NULL,
	[Incroci_Spalle] [varchar](50) NULL,
	[L_Rinforzo_Longherone] [decimal](18, 0) NULL
) ON [PRIMARY]
GO
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 1', CAST(0 AS Decimal(18, 0)), CAST(3500 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), CAST(750 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'No', N'No', N'Si', N'No', CAST(0 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 2', CAST(3501 AS Decimal(18, 0)), CAST(5800 AS Decimal(18, 0)), CAST(751 AS Decimal(18, 0)), CAST(1500 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'No', N'No', N'No', N'No', CAST(0 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 3', CAST(3501 AS Decimal(18, 0)), CAST(5800 AS Decimal(18, 0)), CAST(1501 AS Decimal(18, 0)), CAST(3000 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'Si', N'No', N'No', N'No', CAST(0 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 4', CAST(3501 AS Decimal(18, 0)), CAST(5800 AS Decimal(18, 0)), CAST(3001 AS Decimal(18, 0)), CAST(5000 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'No', CAST(1 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 5', CAST(3501 AS Decimal(18, 0)), CAST(5800 AS Decimal(18, 0)), CAST(5001 AS Decimal(18, 0)), CAST(15000 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'Si', CAST(1 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 6', CAST(5801 AS Decimal(18, 0)), CAST(11800 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), CAST(5000 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'No', CAST(2 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 7', CAST(5801 AS Decimal(18, 0)), CAST(11800 AS Decimal(18, 0)), CAST(5001 AS Decimal(18, 0)), CAST(10000 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'Si', CAST(2 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 8', CAST(5801 AS Decimal(18, 0)), CAST(11800 AS Decimal(18, 0)), CAST(10001 AS Decimal(18, 0)), CAST(15000 AS Decimal(18, 0)), CAST(120 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(5 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'Si', CAST(2 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 9', CAST(11801 AS Decimal(18, 0)), CAST(13600 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), CAST(5000 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'No', CAST(3 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 10', CAST(11801 AS Decimal(18, 0)), CAST(13600 AS Decimal(18, 0)), CAST(5001 AS Decimal(18, 0)), CAST(10000 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(80 AS Decimal(18, 0)), CAST(4 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'Si', CAST(3 AS Decimal(18, 0)))
INSERT [dbo].[Tipologia_Casse] ([Nome_Cassa], [L_Min], [L_Max], [P_Min], [P_Max], [W_Longherone], [H_Longherone], [S_Longherone], [Presenza_Ganci], [Rinforzo_Longherone], [Solo_Ritti], [Incroci_Spalle], [L_Rinforzo_Longherone]) VALUES (N'Tipo 11', CAST(11801 AS Decimal(18, 0)), CAST(13600 AS Decimal(18, 0)), CAST(5001 AS Decimal(18, 0)), CAST(10000 AS Decimal(18, 0)), CAST(120 AS Decimal(18, 0)), CAST(50 AS Decimal(18, 0)), CAST(5 AS Decimal(18, 0)), N'Si', N'Si', N'No', N'Si', CAST(3 AS Decimal(18, 0)))
