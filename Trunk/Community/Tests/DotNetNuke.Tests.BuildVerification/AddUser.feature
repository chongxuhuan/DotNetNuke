Feature: Add User
	In order to have users on the site
	As an adminstrator
	I want to be able to add new users

@MustBeDefaultAdminCredentials
Scenario: Add User
	Given I am on the site home page
	And I have logged in as the admin
	And I am on the admin page User Accounts
	When I click Add User
	And I fill in the user form
		| Control      | Value				|
		| User Name    | adduser			|
		| First Name   | user1FN			|
		| Last name    | user1LN			|
		| Display Name | tuser DN			|
		| Email        | adduser@dnn.com	|
		| Password     | password			|
	And I click Add User
	Then User Account adduser is created correctly
