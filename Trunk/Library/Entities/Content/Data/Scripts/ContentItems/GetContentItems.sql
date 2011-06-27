/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetContentItems Procedure */
/********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetContentItems]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetContentItems
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetContentItems] 
	@ContentTypeId	int,
	@TabId			int,
	@ModuleId		int
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}ContentItems
	WHERE (ContentTypeId = @ContentTypeId OR @ContentTypeId IS NULL)
		AND (TabId = @TabId OR @TabId IS NULL)
		AND (ModuleId = @ModuleId OR @ModuleId IS NULL)
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
