/****** TABELLE DATABASE ******/

/****** Object:  Table [dbo].[A3_app_inventario] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[A3_app_inventario](
	[InvId] [int] IDENTITY(1,1) NOT NULL,
	[WorkerId] [int] NOT NULL,
	[SavedDate] [datetime] NULL,
	[Item] [varchar](max) NOT NULL,
	[Description] [varchar](max) NULL,
	[BarCode] [varchar](50) NULL,
	[FiscalYear] [smallint] NOT NULL,
	[Storage] [varchar](50) NOT NULL,
	[UoM] [varchar](50) NULL,
	[BookInv] [float] NULL,
	[PrevBookInv] [float] NULL,
	[BookInvDiff] [float] NULL,
	[InvRsn] [bit] NULL,
	[Imported] [bit] NOT NULL,
	[UserImp] [varchar](50) NULL,
	[DataImp] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[InvId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[A3_app_inventario] ADD  DEFAULT ((0)) FOR [Imported]
GO

/****** Object:  Table [dbo].[A3_app_prel_mat] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[A3_app_prel_mat](
	[PrelMatId] [int] IDENTITY(1,1) NOT NULL,
	[WorkerId] [int] NOT NULL,
	[SavedDate] [datetime] NULL,
	[Job] [varchar](10) NULL,
	[RtgStep] [smallint] NULL,
	[Alternate] [varchar](8) NULL,
	[AltRtgStep] [smallint] NULL,
	[Operation] [varchar](50) NOT NULL,
	[OperDesc] [varchar](max) NOT NULL,
	[Position] [smallint] NULL,
	[Component] [varchar](max) NULL,
	[BOM] [varchar](max) NOT NULL,
	[Variant] [varchar](50) NOT NULL,
	[ItemDesc] [nvarchar](max) NULL,
	[MOId] [int] NULL,
	[MONo] [varchar](50) NULL,
	[CreationDate] [datetime] NULL,
	[UoM] [varchar](50) NULL,
	[ProductionQty] [float] NOT NULL,
	[ProducedQty] [float] NOT NULL,
	[ResQty] [float] NOT NULL,
	[Storage] [varchar](50) NULL,
	[BarCode] [varchar](50) NOT NULL,
	[WC] [varchar](50) NULL,
	[PrelQty] [float] NULL,
	[Imported] [bit] NOT NULL,
	[UserImp] [varchar](10) NULL,
	[DataImp] [datetime] NULL,
	[Deleted] [bit] NULL,
	[NeededQty] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[PrelMatId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[A3_app_prel_mat] ADD  DEFAULT ((0)) FOR [Imported]
GO

ALTER TABLE [dbo].[A3_app_prel_mat] ADD  DEFAULT ((0)) FOR [NeededQty]
GO

/****** Object:  Table [dbo].[A3_app_reg_ore] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[A3_app_reg_ore](
	[RegOreId] [int] IDENTITY(1,1) NOT NULL,
	[WorkerId] [int] NOT NULL,
	[SavedDate] [datetime] NULL,
	[Job] [varchar](10) NULL,
	[RtgStep] [smallint] NULL,
	[Alternate] [varchar](8) NULL,
	[AltRtgStep] [smallint] NULL,
	[Operation] [varchar](50) NOT NULL,
	[OperDesc] [varchar](max) NOT NULL,
	[BOM] [varchar](max) NOT NULL,
	[Variant] [varchar](50) NOT NULL,
	[ItemDesc] [nvarchar](max) NULL,
	[MOId] [int] NULL,
	[MONo] [varchar](50) NULL,
	[CreationDate] [datetime] NULL,
	[uom] [varchar](50) NULL,
	[ProductionQty] [float] NOT NULL,
	[ProducedQty] [float] NOT NULL,
	[ResQty] [float] NOT NULL,
	[Storage] [varchar](50) NULL,
	[WC] [varchar](50) NULL,
	[WorkingTime] [bigint] NULL,
	[Imported] [bit] NOT NULL,
	[UserImp] [varchar](10) NULL,
	[DataImp] [datetime] NULL,
	[Closed] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[RegOreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[A3_app_reg_ore] ADD  DEFAULT ((0)) FOR [Imported]
GO

/****** Object:  Table [dbo].[A3_app_Settings] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[A3_app_Settings](
	[SettingsId] [int] NOT NULL,
	[MagoUrl] [varchar](max) NULL,
	[Username] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[Company] [varchar](50) NULL,
	[SpecificatorType] [int] NULL,
	[TerminaLavorazioniUtente] [bit] NULL,
	[RectificationReasonPositive] [varchar](50) NULL,
	[RectificationReasonNegative] [varchar](50) NULL,
	[Storage] [varchar](50) NULL,
	[SyncGlobalActive] [bit] NULL,
	[ExternalProgram] [nvarchar](255) NULL,
	[ExternalReferences] [int] NULL,
	[ControlloUoM] [bit] NULL,
	[AbilitaLog] [bit] NULL,
 CONSTRAINT [PK_A3_app_Settings] PRIMARY KEY CLUSTERED 
(
	[SettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[A3_app_Settings] ADD  CONSTRAINT [DF_SettingsId_DefaultValue]  DEFAULT ((1)) FOR [SettingsId]
GO

ALTER TABLE [dbo].[A3_app_Settings] ADD  DEFAULT ('PietribiasiApp') FOR [ExternalProgram]
GO

ALTER TABLE [dbo].[A3_app_Settings]  WITH CHECK ADD  CONSTRAINT [CHK_SettingsId_OneOnly] CHECK  (([SettingsId]=(1)))
GO

ALTER TABLE [dbo].[A3_app_Settings] CHECK CONSTRAINT [CHK_SettingsId_OneOnly]
GO

/****** VISTE DATABASE ******/

/****** Object:  View [dbo].[vw_api_giacenze] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_api_giacenze]
AS
SELECT        dbo.MA_Items.Item, dbo.MA_Items.Description, dbo.MA_ItemsPurchaseBarCode.BarCode, dbo.MA_ItemsBalances.FiscalYear, dbo.MA_ItemsBalances.Storage, dbo.MA_ItemsBalances.BookInv, isnull(dbo.MA_Items.BaseUoM,'NR') UoM
FROM            dbo.MA_ItemsBalances INNER JOIN
                         dbo.MA_Items ON dbo.MA_ItemsBalances.Item = dbo.MA_Items.Item LEFT OUTER JOIN
                         dbo.MA_ItemsPurchaseBarCode ON dbo.MA_Items.Item = dbo.MA_ItemsPurchaseBarCode.Item
WHERE        (dbo.MA_ItemsBalances.FiscalYear = (SELECT MAX(FiscalYear) FROM MA_ItemsBalances WHERE Item = MA_Items.Item)/*YEAR(GETDATE())*/) AND (dbo.MA_ItemsBalances.Storage = 'SEDE')
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "MA_ItemsPurchaseBarCode"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 210
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MA_Items"
            Begin Extent = 
               Top = 6
               Left = 248
               Bottom = 136
               Right = 474
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MA_ItemsBalances"
            Begin Extent = 
               Top = 6
               Left = 512
               Bottom = 136
               Right = 755
            End
            DisplayFlags = 280
            TopColumn = 13
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_giacenze'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_giacenze'
GO

/****** Object:  View [dbo].[vw_api_job] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  CREATE view [dbo].[vw_api_job]
  as
  -- 1 no parametri

  select distinct j.job, j.Description
  from [dbo].[MA_MO]  OP with (nolock)
  inner join [dbo].[MA_Jobs] J with (nolock) on op.Job = j.Job
  where op.MOStatus in ('20578304','20578305')
GO

/****** Object:  View [dbo].[vw_api_mosteps] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  CREATE view [dbo].[vw_api_mosteps]
  as
  -- 2 parametro Job

  --select j.Job, l.[RtgStep],l.[Alternate],l.[AltRtgStep], l.WC, l.Operation, 'SEDE' Storage
  --from [dbo].[MA_MO]  OP with (nolock)
  --inner join [dbo].[MA_Jobs] J with (nolock) on op.Job = j.Job
  --inner join [dbo].[MA_MOSteps] L with (nolock) on op.MOId = l.MOId
  --where op.MOStatus in ('20578304','20578305', '20578307') and l.MOStatus in ('20578304','20578305', '20578307')
  --and l.ProcessingTime > l.ActualProcessingTime


  select j.Job, 
	  l.[RtgStep],
	  l.[Alternate],
	  l.[AltRtgStep], 
	  l.Operation, 
	  o.Description OperDesc, /*c.Position, c.Component,*/ 
	  op.[BOM], op.Variant, 
	  i.Description ItemDesc, 
	  op.[MOId], op.[MONo], 
	  op.CreationDate, 
	  op.uom, 
	  op.ProductionQty, 
	  op.ProducedQty, 
	  op.ProductionQty - op.ProducedQty as ResQty, 
	  'SEDE' Storage, 
	  l.WC,
	  c.PickingSpecificator, 
	  c.PickingSpecificatorType
  from [dbo].[MA_MO]  OP with (nolock)
  inner join [dbo].[MA_Jobs] J with (nolock) on op.Job = j.Job
  inner join [dbo].[MA_MOSteps] L with (nolock) on op.MOId = l.MOId
  inner join [dbo].[MA_Operations] O with (nolock) on l.Operation = o.Operation
  inner join [dbo].[MA_Items] I with (nolock) on OP.BOM = I.Item
  inner join [dbo].[MA_MOComponents] C with (nolock) on op.MOId = c.MOId --aggiunto per le ultime modifiche di Mago
  where op.MOStatus in ('20578304','20578305') and l.MOStatus in ('20578304','20578305')
  -- and l.ProcessingTime > l.ActualProcessingTime
  --and c.ReferredPosition = -1 and c.Closed = 0
GO

/****** Object:  View [dbo].[vw_api_mosteps_mocomponents] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/* parametro Job,RtgStep,Alternate,AltRtgStep*/
CREATE VIEW [dbo].[vw_api_mosteps_mocomponents]
AS
SELECT        J.Job, L.RtgStep, L.Alternate, L.AltRtgStep, L.Operation, O.Description AS OperDesc, C.Position,C.UoM PrelUoM,  C.Component,C.NeededQty PrelNeededQty, c.PickedQuantity, C.NeededQty - c.PickedQuantity PrelResQty, OP.BOM, OP.Variant, I.Description AS ItemDesc, OP.MOId, OP.MONo, OP.CreationDate, OP.UoM, OP.ProductionQty, 
                         OP.ProducedQty, OP.ProductionQty - OP.ProducedQty AS ResQty, 'SEDE' AS Storage, P.BarCode, l.WC
FROM            dbo.MA_MO AS OP WITH (nolock) INNER JOIN
                         dbo.MA_Jobs AS J WITH (nolock) ON OP.Job = J.Job INNER JOIN
                         dbo.MA_MOSteps AS L WITH (nolock) ON OP.MOId = L.MOId INNER JOIN
                         dbo.MA_Operations AS O WITH (nolock) ON L.Operation = O.Operation INNER JOIN
                         dbo.MA_Items AS I WITH (nolock) ON OP.BOM = I.Item INNER JOIN
                         dbo.MA_MOComponents AS C WITH (nolock) ON OP.MOId = C.MOId LEFT OUTER JOIN
                         dbo.MA_ItemsPurchaseBarCode AS P WITH (nolock) ON C.Component = P.Item
WHERE        (OP.MOStatus IN ('20578304', '20578305')) AND (L.MOStatus IN ('20578304', '20578305')) /* AND (L.ProcessingTime > L.ActualProcessingTime) */ AND (C.ReferredPosition = - 1) AND (C.Closed = 0)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -480
         Left = 0
      End
      Begin Tables = 
         Begin Table = "OP"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 286
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "J"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 241
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "L"
            Begin Extent = 
               Top = 270
               Left = 38
               Bottom = 400
               Right = 293
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "O"
            Begin Extent = 
               Top = 402
               Left = 38
               Bottom = 532
               Right = 268
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "I"
            Begin Extent = 
               Top = 534
               Left = 38
               Bottom = 664
               Right = 264
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "C"
            Begin Extent = 
               Top = 666
               Left = 38
               Bottom = 796
               Right = 276
            End
            DisplayFlags = 280
            TopColumn = 4
         End
         Begin Table = "MA_ItemsPurchaseBarCode"
            Begin Extent = 
               Top = 502
               Left = 331
               Bottom = 632
               Right = 503
            End
            DisplayFlags = 280
            TopC' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_mosteps_mocomponents'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'olumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_mosteps_mocomponents'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_mosteps_mocomponents'
GO

/****** Object:  View [dbo].[vw_api_workers] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_api_workers]
AS
SELECT W.WorkerID, W.Name, W.LastName, W.PIN, WFP.FieldValue AS Password, WFT.FieldValue AS TipoUtente, ISNULL(SSV.Storage, 'SEDE') AS StorageVersamenti, ISNULL(SSP.Storage, 'SEDE') AS Storage, ISNULL(WFSPQ.FieldValue, '') 
                  AS LastLogin
FROM     dbo.RM_Workers AS W INNER JOIN
                  dbo.RM_WorkersFields AS WFP ON W.WorkerID = WFP.WorkerID AND WFP.FieldName = 'Password Versamenti' INNER JOIN
                  dbo.RM_WorkersFields AS WFT ON W.WorkerID = WFT.WorkerID AND WFT.FieldName = 'Tipo Utente' LEFT OUTER JOIN
                  dbo.RM_WorkersFields AS WFSV ON W.WorkerID = WFSV.WorkerID AND WFSV.FieldName = 'Divisione' LEFT OUTER JOIN
                  dbo.MA_Storages AS SSV ON WFSV.FieldValue = SSV.Storage LEFT OUTER JOIN
                  dbo.RM_WorkersFields AS WFSP ON W.WorkerID = WFSP.WorkerID AND WFSP.FieldName = 'Deposito Prelievo' LEFT OUTER JOIN
                  dbo.MA_Storages AS SSP ON WFSP.FieldValue = SSP.Storage LEFT OUTER JOIN
                  dbo.RM_WorkersFields AS WFSPQ ON W.WorkerID = WFSPQ.WorkerID AND WFSPQ.FieldName = 'Last Login'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[44] 4[17] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "W"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 320
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "WFP"
            Begin Extent = 
               Top = 7
               Left = 368
               Bottom = 170
               Right = 590
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "WFT"
            Begin Extent = 
               Top = 7
               Left = 638
               Bottom = 170
               Right = 860
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "WFSV"
            Begin Extent = 
               Top = 7
               Left = 908
               Bottom = 170
               Right = 1130
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SSV"
            Begin Extent = 
               Top = 7
               Left = 1178
               Bottom = 170
               Right = 1482
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "WFSP"
            Begin Extent = 
               Top = 175
               Left = 48
               Bottom = 338
               Right = 270
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SSP"
            Begin Extent = 
               Top = 175
               Left = 318
               Bottom = 338
               Right = 622
            End
            DisplayFlags = 280
            TopColumn = 0
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_workers'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       End
         Begin Table = "WFSPQ"
            Begin Extent = 
               Top = 175
               Left = 670
               Bottom = 338
               Right = 892
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_workers'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_api_workers'
GO

/****** Object:  View [dbo].[vw_api_workersfield] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_api_workersfield]
AS
SELECT [WorkerID]
      ,[Line]
      ,[FieldName]
      ,[FieldValue]
      ,[Notes]
      ,[HideOnLayout]
      ,[TBCreated]
      ,[TBModified]
      ,[TBCreatedID]
      ,[TBModifiedID]
  FROM [PIETRIBIASISRLM4].[dbo].[RM_WorkersFields]
GO

/****** Object:  View [dbo].[vw_om_action_messages] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[vw_om_action_messages]
as

/*

E_OPENMES_ACTION_STATUS , 31305 , Da Fare , 0 , 2051604480
E_OPENMES_ACTION_STATUS , 31305 , In Lavorazione , 1 , 2051604481
E_OPENMES_ACTION_STATUS , 31305 , Eseguita , 2 , 2051604482
E_OPENMES_ACTION_STATUS , 31305 , Errore , 3 , 2051604483
E_OPENMES_ACTION_STATUS , 31305 , Tutte , 4 , 2051604484
E_OPENMES_ACTION_STATUS , 31305 , WIP , 5 , 2051604485
E_OPENMES_ACTION_STATUS , 31305 , In attesa , 6 , 2051604486

E_OPENMES_ACTION_TYPE , 31304 , Lancio in Produzione , 0 , 2051538944
E_OPENMES_ACTION_TYPE , 31304 , Consuntivazione OdP , 1 , 2051538945
E_OPENMES_ACTION_TYPE , 31304 , Prelievo Materiale , 2 , 2051538946
E_OPENMES_ACTION_TYPE , 31304 , Tutte , 3 , 2051538947
E_OPENMES_ACTION_TYPE , 31304 , Creazione OdP , 4 , 2051538948
E_OPENMES_ACTION_TYPE , 31304 , Movimentazione Distinta Base , 5 , 2051538949


MOStatus
Stato produzione , 314 , Lanciato , 0 , 20578304
Stato produzione , 314 , In Lavorazione , 1 , 20578305
Stato produzione , 314 , Terminata , 2 , 20578306
Stato produzione , 314 , Creato , 3 , 20578307
Stato produzione , 314 , Proposto da MRP , 4 , 20578308
Stato produzione , 314 , Pianificato da CRP , 5 , 20578309
Stato produzione , 314 , Proposto da MRP-CRP , 6 , 20578310

E_WORKER_MESSAGE_TYPE , 31301 , Suggerimento , 0 , 2051342336
E_WORKER_MESSAGE_TYPE , 31301 , Attenzione , 1 , 2051342337
E_WORKER_MESSAGE_TYPE , 31301 , Errore , 2 , 2051342338

*/
SELECT A.[ActionId]
      ,A.[MOId]
      ,A.[RtgStep]
      ,A.[Alternate]
      ,A.[AltRtgStep]
      ,A.[MONo]
      ,A.[BOM]
      ,A.[Variant]
      ,A.[WC]
      ,A.[Operation]
      ,A.[Job]
      ,A.[WorkerId]
      ,A.[ActionType]
      ,A.[ActionStatus]
      ,A.[ActionMessage]
      ,A.[Closed]
      ,A.[WorkerProcessingTime]
      ,A.[WorkerSetupTime]
      ,A.[ActualProcessingTime]
      ,A.[ActualSetupTime]
      ,A.[Storage]
      ,A.[SpecificatorType]
      ,A.[Specificator]
      ,A.[ReturnMaterialQtyLower]
      ,A.[PickMaterialQtyGreater]
      ,A.[ProductionLotNumber]
      ,A.[ProductionQty]
	  ,A.[TBCreated]
	  ,A.[TBCreatedID]
      ,A.[DeliveryDate]
      ,A.[ConfirmChildMOs]
      ,A.[MOStatus]
	  ,M.[MessageId]
      ,M.[MessageType]
      ,M.[MessageDate]
      ,M.[MessageText]
  FROM [dbo].[MA_OMActions] A
  LEFT OUTER JOIN [dbo].[MA_OMMessages] M on A.MOId = M.MOId

GO