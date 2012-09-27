Feature: pages management
	In order to let user can manage pages easily
	As an administrator
	I want to be use page management correctly

@MustBeDefaultAdminCredentials
Scenario: Click help icon
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page Pages
	And I click on a page in the page treeview
	And I click help icon
	Then I should see help text

@MustBeDefaultAdminCredentials
@MustNotHaveTheTestPages
Scenario: Page create with template should apply permisson from template
	Given I am on the site home page
	And I have logged in as the host
	And I clean the cache
	And I have created the default page called Test A from the Ribbon Bar
	And The page Test A has View permission set to Grant for the role Registered Users
	And I export template of the page Test A
	And I have created page called Test B with template TestA from the Ribbon Bar
	Then The page Test B should have View permisson on role Registered Users

@MustBeDefaultAdminCredentials
@ResetPageQuota
Scenario: Should not allow to add page when page quota is exceed
	Given I am on the site home page
	And I have logged in as the host
	When I navigate to the admin page Site Settings
	And I update page quota to 5
	And I log off
	And I have logged in as the admin
	Then the add page button should disabled in Ribbon bar