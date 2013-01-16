Feature: html rich editor
	In order to edit html content in rich editor
	As an Admin
	I want to work correctly in rich html editor

@MustBeDefaultAdminCredentials
@MustHaveKnownFileInFileManager
Scenario: Insert a hyper link to file which file name contains space
	Given I am on the site home page
	And I have logged in as the admin
	When I edit one of the html module content
	And I enter Hello and click hyper link manager button in rad text editor
	And I click the Telerik Editor HyperLink button
	And I insert a document which file name contains space
	And Insert link with the file
	Then I should see the hyper link insert in rad text editor

@MustBeDefaultAdminCredentials
Scenario: Save Template Command must work correctly
	Given I am on the site home page
	And I have logged in as the admin
	When I edit one of the html module content
	And I click SaveTemplate toolbar button
	Then SaveTemplate Dialog must open

