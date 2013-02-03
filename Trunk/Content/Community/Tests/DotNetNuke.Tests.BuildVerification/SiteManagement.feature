Feature: Sites
	In order to customise my installation
	As a host user
	I want to be able to create and maintain separate sites

@ClearSmtpSettings
Scenario: Add Child Site
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Site Management
	When I click the Add New Site Link
	And I fill in the portal form for a child portal with the alias testportal1 the title test portal 
	And I select the current user for the site admin
	And I click the Create Site Link
	Then The child portal with the alias testportal1 and the title test portal should be created correctly
