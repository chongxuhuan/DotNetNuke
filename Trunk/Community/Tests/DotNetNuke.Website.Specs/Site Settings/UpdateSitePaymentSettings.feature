Feature: UpdateSitePaymentSettings
	In order to have paid for subscription seervices on my site
	As a site administrator
	I want to be able to update the Payment Settings

@MustBeDefaultAdminCredentials
Scenario: Go To PayPal Website with Use Sandbox checked
	Given I am on the site home page 
	And I have logged in as the admin
	When I navigate to the admin page Site Settings
	And I click Advanced Settings Tab
	And I click Payment Settings Section
	And I check Use Sandbox
	And I click Go To Payment Processor WebSite
	Then I should go to the PayPal Sandbox site

@MustBeDefaultAdminCredentials
Scenario: Go To PayPal Website with Use Sandbox Unchecked
	Given I am on the site home page 
	And I have logged in as the admin
	When I navigate to the admin page Site Settings
	And I click Advanced Settings Tab
	And I click Payment Settings Section
	And I uncheck Use Sandbox
	And I click Go To Payment Processor WebSite
	Then I should go to the PayPal Live site
