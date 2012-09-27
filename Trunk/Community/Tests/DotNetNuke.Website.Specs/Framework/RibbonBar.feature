Feature: Ribbon Bar
	In order to manage site quickly
	As an admin
	I want the ribbon bar work correctly

@MustBeDefaultAdminCredentials
Scenario: Modules is shown in correct catagory
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Extensions
	And I update module HTML Pro to catagory Enterprise
	Then module HTML Pro should display after change module catagory to Enterprise

@MustBeDefaultAdminCredentials
Scenario: Should not create page which name conflict with site alias
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Site Management
	When I create a new child portal
	And I navigate to home page
	And I try to create page named Child
	Then The page should not create
