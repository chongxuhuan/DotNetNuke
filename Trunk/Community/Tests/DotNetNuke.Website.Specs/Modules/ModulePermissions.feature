Feature: Module Permissions
	In order to manage the access to features of a module
	As an administrator
	I want to be able to set the permissions for each role

@MustBeDefaultAdminCredentials
@MustHaveAUserWithFullProfile
Scenario: User With Manage Settings permission can change settings
	Given I am on the site home page
	And I have logged in as the admin
	And I have created the default page called Manage Settings from the Ribbon Bar
	And The page Manage Settings has View permission set to Grant for the role Registered Users
	And The page Manage Settings has Add permission set to Grant for the role Registered Users
	And The page Manage Settings has Add Content permission set to Grant for the role Registered Users
	And The page Manage Settings has Delete permission set to Grant for the role Registered Users
	And I am viewing the page called Manage Settings
	And I edit one of the html module content
	And I enter the text Manage This Module in rad text editor
	And I click Save on the Html Module
	When I go to the Module 0 Permissions tab
	And I Uncheck Inherit View Permissions
	And The View permission is set to Grant for the role Registered Users
	And I log off
	And I have logged in as the user MichaelWoods password1234
	And I am viewing the page called Manage Settings


	 