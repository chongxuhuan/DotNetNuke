/*
-- Database Utility ---------------------------------------------------------------------------
Description : Reset a Password in a DotNetNuke database
Author : Tony Tullemans
Date Created : 18.04.2007
Note/s : Before you run this script you must know the UserName and Password of another
 registered DNN user in the database you wish to affect.
----------------------------------------------------------------------------------------------- 
*/

DECLARE @databaseName VARCHAR(128)
SELECT @databaseName = DB_NAME()

PRINT 'RESET PASSWORD IN DATABASE : ' + @databaseName 
PRINT '-----------------------------' + REPLICATE('-', DATALENGTH(@databaseName ));

DECLARE @knownUserName NVARCHAR(128)
DECLARE @lostUserName NVARCHAR(128)
DECLARE @lostUserId NVARCHAR(128)
DECLARE @knownPassword NVARCHAR(128)
DECLARE @knownSalt NVARCHAR(128)

SET @knownUserName = 'host'
SET @lostUserName = 'admin'

SELECT @knownPassword = Password, @knownSalt = PasswordSalt
 FROM aspnet_Membership
 INNER JOIN aspnet_users
 ON aspnet_Membership.UserId = aspnet_users.UserId
 where UserName = @knownUserName;

PRINT ''
PRINT 'Known Password for "' + @knownUserName + '" is : ' + @knownPassword
PRINT 'Known Password Salt for "' + @knownUserName + '" is : ' + @knownSalt

SELECT @lostUserId = aspnet_Membership.UserId
 FROM aspnet_Membership
 INNER JOIN aspnet_users
 ON aspnet_Membership.UserId = aspnet_users.UserId
 WHERE UserName = @lostUserName;

PRINT ''
PRINT 'UserID for "' + @lostUserName + '" is : ' + @lostUserId
PRINT ''

IF (DATALENGTH(@lostUserName) <= 0 OR @lostUserName IS NULL) 
PRINT 'Invalid Lost User Name ' + @lostUserName
ELSE BEGIN
 IF (DATALENGTH(@knownUserName) <= 0 OR @knownUserName IS NULL) 
PRINT 'Invalid Lost User Name ' + @lostUserName
 ELSE BEGIN
 IF (DATALENGTH(@knownPassword) <= 0 OR @knownPassword IS NULL) 
PRINT 'Invalid Known Password ' + @knownPassword
 ELSE BEGIN
 IF (DATALENGTH(@knownSalt) <= 0 OR @knownSalt IS NULL) 
PRINT 'Invalid Known Salt ' + @knownSalt
 ELSE BEGIN
 PRINT ''
 PRINT 'BEFORE'
 SELECT left(UserName, 12) as UserName, aspnet_Membership.UserId, left(Email, 20) as Email, Password, PasswordSalt
 FROM aspnet_Membership INNER JOIN aspnet_users ON aspnet_Membership.UserId = aspnet_users.UserId
 WHERE UserName IN ( @knownUserName, @lostUserName );
 PRINT ''
 PRINT 'Changing Password for User Id : "' + @lostUserId + '" to "' + @knownPassword + '"'
 PRINT ''
 UPDATE aspnet_Membership
 SET Password = @knownPassword,
 PasswordSalt = @knownSalt
 WHERE UserId = @lostUserId;
 
 UPDATE {databaseOwner}[{objectQualifier}Users]
 SET UpdatePassword = 'False'
 WHERE Username = @lostUserName;
 PRINT ''
 PRINT 'AFTER'
 SELECT left(UserName, 12) as UserName, aspnet_Membership.UserId, left(Email, 20) as Email, Password, PasswordSalt
 FROM aspnet_Membership INNER JOIN aspnet_users ON aspnet_Membership.UserId = aspnet_users.UserId
 WHERE UserName IN ( @knownUserName, @lostUserName );
 END
 END
 END
END
GO

PRINT ''
PRINT ' * * * END OF SCRIPT * * *'
PRINT ''
GO