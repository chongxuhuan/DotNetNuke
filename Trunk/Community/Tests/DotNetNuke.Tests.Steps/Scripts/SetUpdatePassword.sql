DECLARE @UserName NVARCHAR(128)

SET @UserName = 'admin'

 UPDATE {databaseOwner}[{objectQualifier}Users]
 SET UpdatePassword = 'True'
 WHERE Username = @UserName;
GO

PRINT ''
PRINT ' * * * END OF SCRIPT * * *'
PRINT ''
GO