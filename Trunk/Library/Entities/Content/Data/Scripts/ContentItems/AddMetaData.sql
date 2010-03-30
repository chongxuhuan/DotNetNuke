/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add AddMetaData Procedure */
/******************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddMetaData]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddMetaData
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}AddMetaData] 
	@ContentItemID		int,
	@Name				nvarchar(100),
	@Value				nvarchar(MAX)
AS
	DECLARE @MetaDataID	int
	SET @MetaDataID = (SELECT MetaDataID FROM {objectQualifier}MetaData WHERE MetaDataName = @Name)
	
	IF @MetaDataID IS NULL
		BEGIN
			--Insert new item into MetaData table
			INSERT INTO {databaseOwner}{objectQualifier}MetaData ( MetaDataName ) VALUES ( @Name )

			SET @MetaDataID = (SELECT SCOPE_IDENTITY() )
		END
		
	INSERT INTO {databaseOwner}{objectQualifier}ContentItems_MetaData (
		ContentItemID,
		MetaDataID,
		MetaDataValue
	)
	VALUES (
		@ContentItemID,
		@MetaDataID,
		@Value
	)
	
		
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
