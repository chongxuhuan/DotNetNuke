Feature: File Manager
	In order to Manage files
	As a administrator
	I want to use file manager correctly

@MustBeDefaultAdminCredentials
Scenario: Synchornize files correctly when there are files without protect extensions exist in secure folder
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page File Manager
	And I add a Secure folder called Secure Folder
	And I copy a simple zip file to the secure folder called Secure Folder
	And I try to synchorize folder recursive
	And I select the secure folder called Secure Folder
	Then I should see the file exist in file manager

@MustBeDefaultAdminCredentials
Scenario: Moving a file to a folder that already contains a file with the same name
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page File Manager
	And I add a Standard folder called folder1
	And I add a Standard folder called folder2
	And I add the file Do Change.doc to the folder folder1
	And I add the file Do Change.doc to the folder folder2
	And Moving the file from folder1 to folder2
	Then The file should moved to folder2 without error

@MustBeDefaultAdminCredentials
Scenario: Login Page should appear when accessing a restricted file in a secure folder when Logged out
	Given I am on the site home page 300
	And I have logged in as the admin
	When I navigate to the admin page File Manager
	And I add a Secure folder called SecurePermissionsFolder
	And I click the folder SecurePermissionsFolder 
	And I Uncheck the Browse Folder permission for the role All Users
	And I Uncheck the View permission for the role All Users
	And I click Update File Manager
	And I add the file Do Change.doc to the folder SecurePermissionsFolder
	And I have created the default page called Secure File from the Ribbon Bar
	And The page Secure File has View permission set to Grant for the role All Users
	And I am viewing the page called Secure File
	And I edit one of the html module content
	And I enter Secure File Content and click hyper link manager button in rad text editor
	And I click the Telerik Editor HyperLink button
	And I select the folder SecurePermissionsFolder
	And I link to the document Do Change.doc
	And I click Save on the Html Module
	And I log off
	And I am viewing the page called Secure File
	And I click the link Secure File Content
	Then I should see the Login screen

@MustBeDefaultAdminCredentials
@MustHaveAUserWithFullProfile
Scenario: Login Page and Access Denied message should appear when accessing a restricted file in a secure folder when Logged in
	Given I am on the site home page 300
	And I have logged in as the admin
	When I navigate to the admin page File Manager
	And I add a Secure folder called SecurePermissionsFolderScenario2
	And I click the folder SecurePermissionsFolderScenario2 
	And I Uncheck the Browse Folder permission for the role All Users
	And I Uncheck the View permission for the role All Users
	And I click Update File Manager
	And I add the file Do Change.doc to the folder SecurePermissionsFolderScenario2
	And I have created the default page called Secure File Scenario 2 from the Ribbon Bar
	And The page Secure File Scenario 2 has View permission set to Grant for the role All Users
	And I am viewing the page called Secure File Scenario 2
	And I edit one of the html module content
	And I enter Secure File Content Scenario 2 and click hyper link manager button in rad text editor
	And I click the Telerik Editor HyperLink button
	And I select the folder SecurePermissionsFolderScenario2
	And I link to the document Do Change.doc
	And I click Save on the Html Module
	And I log off
	And I have logged in as the user MichaelWoods password1234
	And I am viewing the page called Secure File Scenario 2
	And I click the link Secure File Content
	Then I should see the File Access Error message

@MustBeDefaultAdminCredentials
@SiteMustRunInMediumTrust
Scenario: UNC Folder Provider shouldn't be able to create under medium trust
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page File Manager
	And I clik Manage Folder Types button from action menu
	And I Click Add New Type Button
	And I input in folder type name field
	And I select Folder type as UNCFolderProvider
	Then the UNC settings should be disabled
