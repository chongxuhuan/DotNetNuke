Feature: Add Super User
	In order to manage my site better
	As a Super User
	I want to be able to add Super Users


Scenario: Add Super User
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Superuser Accounts
	When I click Add User
	And I fill in the user form
		| Control      | Value						|
		| User Name    | SuperUserTest1				|
		| First Name   | Philip						|
		| Last name    | Beadle						|
		| Display Name | Philip Beadle				|
		| Email        | philip.beadle@dnncorp.com	|
		| Password     | password					|
	And I click Add User
	Then Super User Account SuperUserTest1 is created correctly



