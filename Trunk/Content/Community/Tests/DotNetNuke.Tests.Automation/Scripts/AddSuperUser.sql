DECLARE @knownUserName NVARCHAR(128)
DECLARE @knownPassword NVARCHAR(128)
DECLARE @knownSalt NVARCHAR(128)
DECLARE @ApplicationName nvarchar(256)
DECLARE @UserName nvarchar(256) 
DECLARE @Email nvarchar(256)
DECLARE @PasswordQuestion nvarchar(256)
DECLARE @PasswordAnswer nvarchar(128)
DECLARE @IsApproved bit
DECLARE @CurrentTimeUtc datetime 
DECLARE @CreateDate datetime
DECLARE @UniqueEmail int
DECLARE @PasswordFormat int
Declare @UserId uniqueidentifier
SET @knownUserName = 'host'

SELECT @knownPassword = Password, @knownSalt = PasswordSalt
 FROM aspnet_Membership
 INNER JOIN aspnet_users
 ON aspnet_Membership.UserId = aspnet_users.UserId
 WHERE UserName = @knownUserName;


SET @ApplicationName = 'DotNetNuke'
SET @UserName = 'TestSuperUser' --The new user
SET @Email = 'TestSuperUser@dnn.com' --You can set this to whatever you want
SET @PasswordQuestion = ''
SET @PasswordAnswer = ''
SET @IsApproved = 1
SET @CurrentTimeUtc = GETDATE()
SET @CreateDate = @CurrentTimeUtc
SET @UniqueEmail = 0
SET @PasswordFormat = 2 --NOTE: Value from existing user!

--Make the stored procedure call
EXEC dbo.aspnet_Membership_CreateUser @ApplicationName, @Username, @knownPassword,
                 @knownSalt, @email, @passwordquestion, @PasswordAnswer, 
                 @IsApproved, @CurrentTimeUtc, @CreateDate, @UniqueEmail,
                 @PasswordFormat, @UserId

--Insert the record into the DotNetNuke users table
INSERT INTO {databaseOwner}[{objectQualifier}Users] (Username, FirstName, LastName, IsSuperUser, Email,
                     DisplayName, UpdatePassword)
     VALUES(@Username, 'TestSuperUserFN', 'TestSuperUserLN', 1, @Email, 'TestSuperUser DN', 0)

GO

