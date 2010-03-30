/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add DeleteMetaData Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}DeleteMetaData]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}DeleteMetaData
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}DeleteMetaData] 
	@ContentItemId		int,
	@Name				nvarchar(100),
	@Value				nvarchar(MAX)
	
AS
	DELETE FROM {databaseOwner}{objectQualifier}ContentItems_MetaData
	FROM {databaseOwner}{objectQualifier}ContentItems_MetaData AS cm
		INNER JOIN {databaseOwner}{objectQualifier}MetaData AS m ON cm.MetaDataID = m.MetaDataID
	WHERE cm.ContentItemId = @ContentItemId AND m.MetaDataName = @Name AND cm.MetaDataValue = @Value
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
