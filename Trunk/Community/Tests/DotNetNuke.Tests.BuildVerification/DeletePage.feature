Feature: Delete Page
	In order to manage the pages on my site
	As an administrator
	I want to be able to delete pages

@MustUseRibbonBar
@MustHaveTestPageAdded
Scenario: Delete Page From Ribbon Bar
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	And I am viewing the page called Test Page
	When I click Delete Page from the Ribbon Bar
	And I click confirm in the pop up
	Then The page called Test Page is deleted
	