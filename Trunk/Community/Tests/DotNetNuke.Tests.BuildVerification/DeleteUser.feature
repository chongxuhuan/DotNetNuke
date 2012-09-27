Feature: Delete User
	In order to manage users on the site
	As an adminstrator
	I want to be able to delete users

@MustBeDefaultAdminCredentials
@MustHaveAUserWithFullProfile
Scenario: Delete User
	Given I am on the site home page
	And I have logged in as the admin
	And I am on the admin page User Accounts
	When I click Delete in the user table for user Michael Woods and confirm the deletion
	Then The User Account MichaelWoods with password password1234 and display name Michael Woods is deleted from the site
