Feature: Site Group Management
	In order to use site group
	As a host
	I want to be use site group management

@MustBeDefaultAdminCredentials
Scenario: Ensure I can't delete another portal's administrator in the site group
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Site Management
	When I create a new child portal
	And I navigate to the Host professional feature page SiteGroups
	And I create a new site group
	And I add the child portal to the group
	And I visit to the child portal
	Then I shouldn't see delete button on administrator
