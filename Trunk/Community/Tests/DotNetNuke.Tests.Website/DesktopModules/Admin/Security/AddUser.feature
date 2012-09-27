Feature: How to add a new user to a DotNetNuke site using the Add New User module
	In order to have users on the site
	As an adminstrator
	I want to be able to add new users

Scenario Outline: Add User from the Users Accounts Logged In As a User in the Administrators Role
	Given There is a Page called User Accounts with these permissions
		| Role      | Permission | Value   |
		| All Users | View       | Allowed |
	And There is a User Accounts module on the page with these permissions
		| Role                       | Permission | Value   |
		| User Account Administrator | View       | Allowed |
	Given Login as UID=<Scenario User Name> PWD=<Scenario Password> Role=<Scenario Role>
	And I click Add User
	When I set Add User User Name to <User Name>
	And I set Add User First Name to <First Name>
	And I set Add User Last Name to <Last Name>
	And I set Add User Display Name to <User Name>
	And I set Add User Email to <Email>
	And I set Add User Password to <Password>
	And I set Add User Authorize to <Authorize>
	And I set Add User Notify to <Notify>
	And I set Add User Random Password to <Randon Password>
	And I set Add User Password to <Password>
	And I set Add User Confirm Password to <Confirm Password>
	And I click Add New User
	Then The newly added user account can now be viewed and modified using the User Accounts module
	And If Authorize is checked the new user will automatically gain access to the Registered User role and any roles set for Auto Assignment
	And If Authorize is unchecked the new user will be created but will not be able to access the restricted areas of the site
	And If Notify is checked the new user will be sent a notification email 
	And If Notify is unchecked the new user will not be sent a notification email 

	Examples:
	| Scenario User Name | Scenario Password | Scenario Role  | User Name | First Name | Last Name | Display Name | Email              | Authorize | Notify | Randon Password | Password | Confirm Password |
	| deadmau5           | password          | Administrators | wilson    | Jon        | Wilson    | Wilson       | wilson@dnncorp.com | True      | True   | False           | password | password         |
	| philt3r            | password          | Administrators | tiesto    | The        | Dutchman  | Tiesto       | tiesto@dnncorp.com | True      | True   | False           | password | password         |
