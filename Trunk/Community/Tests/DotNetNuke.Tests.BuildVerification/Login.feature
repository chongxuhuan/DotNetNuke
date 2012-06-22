Feature: Site Login
	In order to login to the site
	As a user of the site
	I want to be able to enter my credentials and login

@MustBeDefaultAdminCredentialsForceUpdate
Scenario: Login With Default Admin Password Forces Update
	Given I am on the site home page
	And I have pressed Login
	And I have entered the default Admin Username and the default password
	When I press Login
	Then I should be forced to enter a new password to proceed


@MustBeDefaultAdminCredentialsForceUpdate
Scenario: Login With Default Admin Password Update Password
	Given I am on the site home page
	And I have pressed Login
	And I have entered the default Admin Username and the default password
	And I press Login
	When I enter and confirm my new password
	Then I should be logged in as the admin user

@MustBeHostDefaultCredentials
Scenario: Login With Default Host Credentials
	Given I am on the site home page
	And I have pressed Login
	When I enter the default host username
	And I enter the default host password
	And I press Login
	Then I should be logged in as the Host user

Scenario: Login Fails With Invalid Credentials
	Given I am on the site home page
	And I have pressed Login
	When I enter an Invalid username
	And I enter an Invalid Password
	And I press Login
	Then I should see a login error
	And I should not be logged in