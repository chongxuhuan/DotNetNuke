Feature: How to add a new user to a DotNetNuke site using the Add New User module
	In order to have users on the site
	As an adminstrator
	I want to be able to add new users

Scenario Outline: Add User from the Users Accounts Logged In As a User in the Administrators Role
	Given There is a Page called User Accounts with these permissions
		| Role      | Permission | Value   |
		| Administrators | View       | Allowed |
	And There is a User Accounts module on the page with these permissions
		| Role                       | Permission | Value   |
		| Administrators | View       | Allowed |
	And There is no user <User Name>
	And Login as UID=<Scenario User Name> PWD=<Scenario Password> Role=<Scenario Role>
	And Click the Add New User link. This opens the Add New User interface
	When In the User Name text box, enter <User Name>
	And In the First Name text box, enter <First Name>
	And In the Last Name text box, enter <Last Name>
	And In the Display Name text box, enter <User Name>
	And In the Email Address text box, enter <Email>
	And At Authorize, select <Authorize>
	And At Notify, select <Notify>
	And I set Add User Random Password to <Random Password>
	And In the Password text box, enter <Password>
	And In the Confirm Password text box, re-enter the same password <Confirm Password>
	And Click the Add New User link.
	Then The newly added user account can now be viewed and modified using the User Accounts module
	And If Authorize is checked the new user will automatically gain access to the Registered User role and any roles set for Auto Assignment
	And If Authorize is unchecked the new user will be created but will not be able to access the restricted areas of the site
	And If Notify is checked the new user will be sent a notification email 
	And If Notify is unchecked the new user will not be sent a notification email 

	Examples:
	| Scenario User Name | Scenario Password | Scenario Role  | User Name | First Name | Last Name | Display Name | Email              | Authorize | Notify | Random Password | Password | Confirm Password |
	| deadmau5           | password          | Administrators | wilson    | Jon        | Wilson    | Wilson       | wilson@dnncorp.com | True      | True   | False           | password | password         |
	| philt3r            | password          | Administrators | tiesto    | The        | Dutchman  | Tiesto       | tiesto@dnncorp.com | True      | True   | False           | password | password         |
