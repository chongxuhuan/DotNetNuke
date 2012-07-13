Feature: Registration
	In order to register user
	As a regular user
	I want to use the registration feature correctly

@MustBeDefaultAdminCredentials
@MustHaveEmailSetUpForSiteDumpToFolder
@MustHaveEmptyEmailFolder
@SiteMustRunInFullTrust
Scenario: User Registration Notifcation mail should contains display name
	Given I am on the site home page
	When I click the Register link
	And I fill in the Register User form
		| Control      | Value						|
		| User Name    | RegisterUserTest			|
		| Email        | RegisterUserTest@dnn.com	|
		| Password     | password					|
		| Display Name | RegisterUserTest DN		|
	And I click the Register button
	Then The admin notification mail should contain RegisterUserTest DN

@MustBeDefaultAdminCredentials
@CustomRegistration
Scenario: Password should remain its value after page post back
	Given I am on the site home page
	And I have logged in as the host
	And I clean the cache
	When I log off
	And I click the Register link
	And I fill in the Register User form
		| Control      | Value			|
		| User Name    | Israel			|
		| Email        | israel@dnn.com	|
		| Password     | password		|
		| Display Name | Israel			|
	And I select country as Canada
	Then Password field's value should be password
