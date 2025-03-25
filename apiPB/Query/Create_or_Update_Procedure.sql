IF OBJECT_ID('dbo.InsertWorkersFields', 'P') IS NOT NULL
    DROP PROCEDURE dbo.InsertWorkersFields;
GO

CREATE PROCEDURE InsertWorkersFields
	@WorkerID INT,
    @FieldValue VARCHAR(255)
AS
BEGIN
	declare @maxLine INT = 0;
	DECLARE @fieldName VARCHAR(255) = NULL;
	DECLARE @Notes VARCHAR(255) = NULL;
    DECLARE @HideOnLayout BIT = 0;
    DECLARE @TBCreated DATETIME = GETDATE();
    DECLARE @TBModified DATETIME = GETDATE();
    DECLARE @TBCreatedID INT = 18;
    DECLARE @TBModifiedID INT = 18;

	SELECT TOP 1 @maxLine = Line, @fieldName = FieldName
    FROM RM_WorkersFields wf
    WHERE wf.WorkerID = @WorkerID
    ORDER BY Line DESC;
	IF @fieldName = 'Last Login'
    BEGIN
        UPDATE RM_WorkersFields
        SET FieldValue = @FieldValue,
            TBModified = @TBModified,
            TBModifiedID = @TBModifiedID
        WHERE WorkerID = @WorkerID AND FieldName = @fieldName AND Line = @maxLine;
    END
    ELSE
    BEGIN
		INSERT INTO RM_WorkersFields(WorkerID, Line, FieldName, FieldValue, Notes, HideOnLayout, TBCreated, TBModified, TBCreatedID, TBModifiedID)
		VALUES (@WorkerID, @maxLine + 1, 'Last Login', @FieldValue, @Notes, @HideOnLayout, @TBCreated, @TBModified, @TBCreatedID, @TBModifiedID);
    END
END;