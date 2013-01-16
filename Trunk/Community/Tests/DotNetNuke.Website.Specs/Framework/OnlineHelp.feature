Feature: Online Help
	In order to display a link to the Online Help
	As an Administrator
	I want to be shown the correct URL in the module menu

@MustBeDefaultAdminCredentials
@MustHaveHelpUrlFilledOutForHtmlModule
Scenario: Link to Url Online Help URL Field when Help URL Field is populated
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	When I click the Module 0 Online Help
	Then I should go to the site hive.dotnetnuke.com

@MustBeDefaultAdminCredentials
@MustHaveEmptyHelpUrlForHtmlModule
Scenario: Link to Url Online Help URL Field when Help URL Field is empty
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	When I click the Module 0 Online Help
	Then I should go to the site hive.dotnetnuke.com

