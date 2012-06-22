Feature: SuperUser management
	In order to Create/Update/Delete super users
	As a host user
	I want to manage superuser correctly

@MustBeDefaultAdminCredentials
Scenario: Superuser can be deleted after add folder permission
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page SuperUser Accounts
	When I click Add User
	And I fill in the user form
	  | Control      | Value   |
	  | User Name    | testuser   |
	  | First Name   | testuser   |
	  | Last name    | testuser   |
	  | Display Name | testuser  |
	  | Email        | testuser@dnn.com  |
	  | Password     | testuser  |
	And I click Add User
	And I navigate to the admin page File Manager
	And I Apply a folder permission on user testuser
	And I am on the Host Page SuperUser Accounts
	And I delete testuser
	Then testuser should delete successful without exceptions
