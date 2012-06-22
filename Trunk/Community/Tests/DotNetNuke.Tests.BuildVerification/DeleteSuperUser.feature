Feature: Delete Super User
	In order to manage my site better
	As a Super User
	I want to be able to delete Super Users

@TestSuperUserMustExist
Scenario: Delete Super User
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Superuser Accounts
	When I click Delete in the Super User table for TestSuperUser DN and confirm the deletion
	Then The User Account TestSuperUser with password password and display name TestSuperUser DN is deleted from the site
