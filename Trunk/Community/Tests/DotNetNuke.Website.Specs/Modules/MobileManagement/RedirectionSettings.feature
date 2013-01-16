Feature: Redirection Rule settings
	In order to manage mobile redirection rules
	As an admin
	I want to be use redirection settings UI correctly

@MustBeDefaultAdminCredentials
Scenario: User Profile page should not appear in target drop down list
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page Site Redirection Management
	And I click Create button
	Then User Profile page should not appear in target drop down list
