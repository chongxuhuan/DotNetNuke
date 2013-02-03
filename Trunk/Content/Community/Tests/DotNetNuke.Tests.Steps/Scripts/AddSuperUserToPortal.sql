DECLARE @UserID int
DECLARE @PortalID int

SELECT @UserID = UserID
	FROM {databaseOwner}{objectQualifier}Users
	WHERE  Username = 'host'
SET @PortalID = 0

IF NOT EXISTS ( SELECT 1 FROM {databaseOwner}{objectQualifier}UserPortals WHERE UserID = @UserID AND PortalID = @PortalID )
	BEGIN
		INSERT INTO {databaseOwner}{objectQualifier}UserPortals (
			UserID,
			PortalID,
			Authorised,
			CreatedDate
		)
		VALUES (
			@UserID,
			@PortalID,
			1,
			getdate()
		)
	END