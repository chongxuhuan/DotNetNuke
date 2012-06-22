Feature: User Accounts List
	In order to manage the Users on my site
	As an Administrator
	I want to view a list of Users

@MustBeDefaultAdminCredentials
@MustHaveUser1Added
Scenario: Should be able to edit the user delete the user and edit the Roles for a Regular User
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page User Accounts
	Then The user Testuser DN should have a link to edit the User
	And The user Testuser DN should have a link to delete the User 
	And The user Testuser DN should have a link to edit the Security Roles 

@MustBeDefaultAdminCredentials
Scenario: Should be able to edit the user and edit the Roles for the admin User
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page User Accounts
	Then The user Administrator Account should have a link to edit the User
	And The user Administrator Account should have a link to edit the Security Roles 

@MustBeDefaultAdminCredentials
Scenario: Should not be able to delete the admin User
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page User Accounts
	Then The user Administrator Account should have a link to edit the User
	And The user Administrator Account should not have a link to delete the User 

@HostUserMustBeMemberOfPortal
@MustBeDefaultAdminCredentials
Scenario: Should not be able to see Super User in account list
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page User Accounts
	Then The user SuperUser Account should not display

@MustBeDefaultAdminCredentials
Scenario: Should not be able to input invalid value in user name validation
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page Site Settings
	And I input [ in Security_UserNameValidation field
	And I Update Site Settings
	Then I should see error message

@MustBeDefaultAdminCredentials
Scenario: Super user's photo should be available in all sites
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page SiteManagement
	When I create a new child portal
	And I click my name to edit profile
	And I click Edit Profile button
	And I upload a picture for photo
	And I click Profile Update button
	And I visit child portal
	And I click my name to edit profile
	And I click Edit Profile button
	Then I should see the picture just upload
