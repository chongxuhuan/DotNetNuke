/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add ContentItemID Column to Tabs Table */
/******************************************/

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns WHERE TABLE_NAME='{objectQualifier}Tabs' AND COLUMN_NAME='ContentItemID')
	BEGIN
		-- Add new Column
		ALTER TABLE {databaseOwner}{objectQualifier}Tabs
			ADD ContentItemID int NOT NULL
	END

GO

/* Add ContentItemID Column to Modules Table */
/*********************************************/

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns WHERE TABLE_NAME='{objectQualifier}Modules' AND COLUMN_NAME='ContentItemID')
	BEGIN
		-- Add new Column
		ALTER TABLE {databaseOwner}{objectQualifier}Modules
			ADD ContentItemID int NOT NULL
	END

GO

ALTER TABLE {databaseOwner}[{objectQualifier}Modules]
	ADD  CONSTRAINT [FK_{objectQualifier}Modules_{objectQualifier}ContentItems] FOREIGN KEY([ContentItemID])
		REFERENCES {databaseOwner}[{objectQualifier}ContentItems] ([ContentItemID])
GO

ALTER TABLE {databaseOwner}[{objectQualifier}Tabs] 
	ADD  CONSTRAINT [FK_{objectQualifier}Tabs_{objectQualifier}ContentItems] FOREIGN KEY([ContentItemID])
		REFERENCES {databaseOwner}[{objectQualifier}ContentItems] ([ContentItemID])
GO
	
/* Update Tabs View to Include ContentItemID */
/*********************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}vw_Tabs]') AND OBJECTPROPERTY(id, N'IsVIEW') = 1)
  DROP VIEW {databaseOwner}{objectQualifier}vw_Tabs
GO
	
CREATE VIEW {databaseOwner}{objectQualifier}vw_Tabs
AS
	SELECT     
		T.TabID, 
		T.TabOrder, 
		T.PortalID, 
		T.TabName, 
		T.IsVisible, 
		T.ParentId, 
		T.[Level], 
        CASE WHEN LEFT(LOWER(T.IconFile), 7) = 'fileid=' THEN
                  (SELECT Folder + FileName
                    FROM  {databaseOwner}{objectQualifier}Files
                    WHERE fileid = CAST((RIGHT(LOWER(T.IconFile), Len(T.IconFile) - 7)) AS int)) ELSE T.IconFile END AS IconFile, 
        CASE WHEN LEFT(LOWER(T.IconFileLarge), 7) = 'fileid=' THEN
                  (SELECT Folder + FileName
                    FROM  {databaseOwner}{objectQualifier}Files
                    WHERE fileid = CAST((RIGHT(LOWER(T.IconFileLarge), Len(T.IconFileLarge) - 7)) AS int)) ELSE T.IconFileLarge END AS IconFileLarge, 
		T.DisableLink, 
		T.Title, 
		T.Description, 
		T.KeyWords, 
		T.IsDeleted, 
		T.SkinSrc, 
		T.ContainerSrc, 
		T.TabPath, 
		T.StartDate, 
		T.EndDate, 
		T.Url, 
		CASE WHEN EXISTS (SELECT  1 FROM {databaseOwner}{objectQualifier}Tabs T2 WHERE T2.ParentId = T.TabId) THEN 'true' ELSE 'false' END AS HasChildren, 
		T.RefreshInterval, 
		T.PageHeadText, 
		T.IsSecure, 
		T.PermanentRedirect, 
		T.SiteMapPriority,
		CI.ContentItemID,
		CI.Content,
		CI.ContentKey,
		CI.Indexed,
		T.CreatedByUserID, 
		T.CreatedOnDate, 
		T.LastModifiedByUserID, 
		T.LastModifiedOnDate
	FROM {databaseOwner}{objectQualifier}Tabs AS T
		INNER JOIN  {databaseOwner}{objectQualifier}ContentItems AS CI ON T.ContentItemID = CI.ContentItemID

GO

/* Update AddTab Stored Procedure */
/**********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddTab]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddTab
GO

CREATE PROCEDURE {databaseOwner}{objectQualifier}AddTab
	@ContentItemID		int,
	@PortalID           int,
	@TabName            nvarchar(50),
	@IsVisible          bit,
	@DisableLink        bit,
	@ParentId           int,
	@IconFile           nvarchar(100),
	@IconFileLarge      nvarchar(100),
	@Title              nvarchar(200),
	@Description        nvarchar(500),
	@KeyWords           nvarchar(500),
	@Url                nvarchar(255),
	@SkinSrc            nvarchar(200),
	@ContainerSrc       nvarchar(200),
	@TabPath            nvarchar(255),
	@StartDate          datetime,
	@EndDate            datetime,
	@RefreshInterval    int,
	@PageHeadText	    nvarchar(500),
	@IsSecure           bit,
	@PermanentRedirect	bit,
	@SiteMapPriority	float,
	@CreatedByUserID	int,
	@CultureCode		nvarchar(50)

AS

	INSERT INTO {databaseOwner}{objectQualifier}Tabs (
		ContentItemID,
		PortalID,
		TabName,
		IsVisible,
		DisableLink,
		ParentId,
		IconFile,
		IconFileLarge,
		Title,
		Description,
		KeyWords,
		IsDeleted,
		Url,
		SkinSrc,
		ContainerSrc,
		TabPath,
		StartDate,
		EndDate,
		RefreshInterval,
		PageHeadText,
		IsSecure,
		PermanentRedirect,
		SiteMapPriority,
		CreatedByUserID,
		CreatedOnDate,
		LastModifiedByUserID,
		LastModifiedOnDate,
		CultureCode
	)
	VALUES (
		@ContentItemID,
		@PortalID,
		@TabName,
		@IsVisible,
		@DisableLink,
		@ParentId,
		@IconFile,
		@IconFileLarge,
		@Title,
		@Description,
		@KeyWords,
		0,
		@Url,
		@SkinSrc,
		@ContainerSrc,
		@TabPath,
		@StartDate,
		@EndDate,
		@RefreshInterval,
		@PageHeadText,
		@IsSecure,
		@PermanentRedirect,
		@SiteMapPriority,
		@CreatedByUserID,
		getdate(),
		@CreatedByUserID,
		getdate(),
		@CultureCode
	)

	SELECT SCOPE_IDENTITY()
GO

/* Update Modules View */
/***********************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}vw_Modules]') AND OBJECTPROPERTY(id, N'IsVIEW') = 1)
  DROP VIEW {databaseOwner}{objectQualifier}vw_Modules
GO

CREATE VIEW {databaseOwner}{objectQualifier}vw_Modules
AS
	SELECT     
		M.PortalID, 
		TM.TabID, 
		TM.TabModuleID, 
		M.ModuleID, 
		M.ModuleDefID, 
		TM.ModuleOrder, 
		TM.PaneName, 
		M.ModuleTitle, 
		TM.CacheTime, 
		TM.CacheMethod, 
		TM.Alignment, 
		TM.Color, 
		TM.Border, 
		CASE WHEN LEFT(LOWER(TM.IconFile), 6) = 'fileid' THEN (SELECT Folder + FileName FROM {databaseOwner}{objectQualifier}Files WHERE 'fileid=' + CONVERT(varchar, {databaseOwner}{objectQualifier}Files.FileID) = TM.IconFile) ELSE TM.IconFile END AS IconFile, 
		M.AllTabs, 
		TM.Visibility, 
		TM.IsDeleted, 
		M.Header, 
		M.Footer, 
		M.StartDate, 
		M.EndDate, 
		TM.ContainerSrc, 
		TM.DisplayTitle, 
		TM.DisplayPrint, 
		TM.DisplaySyndicate, 
		TM.IsWebSlice, 
		TM.WebSliceTitle, 
		TM.WebSliceExpiryDate, 
		TM.WebSliceTTL, 
		M.InheritViewPermissions, 
		MD.DesktopModuleID, 
		MD.DefaultCacheTime, 
		MC.ModuleControlID, 
		DM.BusinessControllerClass, 
		DM.IsAdmin, 
		DM.SupportedFeatures,
		CI.ContentItemID,
		CI.Content,
		CI.ContentKey,
		CI.Indexed,
		M.CreatedByUserID, 
		M.CreatedOnDate, 
		M.LastModifiedByUserID, 
		M.LastModifiedOnDate
	FROM {databaseOwner}{objectQualifier}ModuleDefinitions AS MD 
		INNER JOIN {databaseOwner}{objectQualifier}Modules AS M ON MD.ModuleDefID = M.ModuleDefID 
		INNER JOIN {databaseOwner}{objectQualifier}ContentItems AS CI ON M.ContentItemID = CI.ContentItemID
		INNER JOIN {databaseOwner}{objectQualifier}ModuleControls AS MC ON MD.ModuleDefID = MC.ModuleDefID 
		INNER JOIN {databaseOwner}{objectQualifier}DesktopModules AS DM ON MD.DesktopModuleID = DM.DesktopModuleID 
		LEFT OUTER JOIN {databaseOwner}{objectQualifier}TabModules AS TM ON M.ModuleID = TM.ModuleID
	WHERE (MC.ControlKey IS NULL)

GO

/* Update AddModule Stored Procedure */
/*************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddModule]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddModule
GO

CREATE PROCEDURE {databaseOwner}{objectQualifier}AddModule
	@ContentItemID				int,
	@PortalID					int,
	@ModuleDefId				int,
	@ModuleTitle				nvarchar(256),
	@AllTabs					bit,
	@Header						ntext,
	@Footer						ntext,
	@StartDate					datetime,
	@EndDate					datetime,
	@InheritViewPermissions     bit,
	@IsDeleted					bit,
	@CreatedByUserID  			int
	
AS
	INSERT INTO {databaseOwner}{objectQualifier}Modules (
		ContentItemID, 
		PortalId,
		ModuleDefId,
		ModuleTitle,
		AllTabs,
		Header,
		Footer, 
		StartDate,
		EndDate,
		InheritViewPermissions,
		IsDeleted,
		CreatedByUserID,
		CreatedOnDate,
		LastModifiedByUserID,
		LastModifiedOnDate
	)
	values (
		@ContentItemID,
		@PortalID,
		@ModuleDefId,
		@ModuleTitle,
		@AllTabs,
		@Header,
		@Footer, 
		@StartDate,
		@EndDate,
		@InheritViewPermissions,
		@IsDeleted,
		@CreatedByUserID,
		getdate(),
		@CreatedByUserID,
		getdate()
	)

	SELECT SCOPE_IDENTITY()
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
