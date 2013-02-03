Feature: Security Roles
	In order to use security roles
	As an admin
	I want to be use security roles UI correctly

@MustBeDefaultAdminCredentials
Scenario: ManageUsers button shouldn't visible when the role is not approved
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page Security Roles
	And I click edit button of role Unverified Users
	And I set role's status to Disabled
	And I click Update button
	And I click edit button of role Unverified Users
	Then I shouldn't see "Manage Users in this Role" button
