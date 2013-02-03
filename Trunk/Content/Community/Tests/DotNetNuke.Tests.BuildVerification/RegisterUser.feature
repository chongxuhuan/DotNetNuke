Feature: Register User
	In order to join the site
	As an anonymouse user
	I want to be able to register

Scenario: Register User
	Given I am on the site home page
	When I click the Register link
	And I fill in the Register User form
		| Control      | Value						|
		| User Name    | RegisterUserTest			|
		| Email        | RegisterUserTest@dnn.com	|
		| Password     | password					|
		| Display Name | RegisterUserTest DN		|
	And I click the Register button
	Then I am logged in as the RegisterUserTest DN user