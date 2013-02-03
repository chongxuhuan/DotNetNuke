Feature: Site Wizard
	In order to re-create the site
	As n admin
	I want to use site wizard correctly

@MustBeDefaultAdminCredentials
Scenario: Site should apply settings from site wizard
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page Site Wizard
	And I click the Site Wizard Next button
	And I click the Site Wizard Next button
	And I select the skin or container 2-Column-Left-Mega-Menu
	And I click the Site Wizard Next button
	And I select the skin or container PageTitle_Blue
	And I click the Site Wizard Next button
	And I set site name to New Site
	And I click the Site Wizard Finish button
	And I navigate to the admin page Site Settings
	Then Site's default skin should be 2-Column-Left-Mega-Menu
	And Site's default container should be PageTitle_Blue
	And Site's name should be New Site
