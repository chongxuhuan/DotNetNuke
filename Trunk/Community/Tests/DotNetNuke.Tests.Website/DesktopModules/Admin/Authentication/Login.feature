Feature: How to login to a site with Popup Login Page
	In order to login to the site
	As a user of the site
	I want to be able to enter my credentials and login

Scenario Outline: Login as a User in the Registered Users role
	Given There is a user <Scenario User Name> <Scenario Password> with these roles
		| Role             |
		| Registered Users |
	And Click the Login link (typically located in the top right corner of each page) to open the User Login pop-up window 
	When I enter the Login username <Scenario User Name>
	And I enter the Login password <Scenario Password>
	And I click the Login button
	Then I should be logged in as the user

	Examples:
	| Scenario User Name | Scenario Password |
	| deadmau5           | password          |
	| philt3r            | password          |
