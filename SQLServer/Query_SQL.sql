select w.workerid, w.name, w.lastname, w.pin, WFP.FieldValue Password, wft.FieldValue TipoUtente, isnull(SSV.Storage,'SEDE') StorageVersamenti, isnull(SSP.Storage,'SEDE') Storage, isnull (WFSPQ.FieldValue, '') LastLogin 
from RM_Workers W
inner join [dbo].[RM_WorkersFields] WFP on W.WorkerID = WFP.WorkerID and WFP.FieldName = 'Password Versamenti'
inner join [dbo].[RM_WorkersFields] WFT on W.WorkerID = WFT.WorkerID and WFT.FieldName = 'Tipo Utente'
left outer join [dbo].[RM_WorkersFields] WFSV on W.WorkerID = WFSV.WorkerID and WFSV.FieldName = 'Divisione'
left outer join dbo.MA_Storages SSV on WFSV.FieldValue = SSV.Storage
left outer join [dbo].[RM_WorkersFields] WFSP on W.WorkerID = WFSP.WorkerID and WFSP.FieldName = 'Deposito Prelievo'
left outer join dbo.MA_Storages SSP on WFSP.FieldValue = SSP.Storage
left outer join [dbo].[RM_WorkersFields] WFSPQ on W.WorkerID = WFSPQ.WorkerID and WFSPQ.FieldName = 'Last Login'