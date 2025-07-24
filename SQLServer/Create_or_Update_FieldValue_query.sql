DECLARE @CurrentDateTime VARCHAR(255) = FORMAT(GETDATE(), 'yyyy-MM-dd HH:mm:ss');
DECLARE @WorkerID INT = 22;
EXEC InsertWorkersFields @WorkerID, @FieldValue = @CurrentDateTime;
SELECT * FROM RM_WorkersFields AS wf 
WHERE wf.WorkerID = @WorkerID
ORDER BY wf.WorkerID, wf.Line