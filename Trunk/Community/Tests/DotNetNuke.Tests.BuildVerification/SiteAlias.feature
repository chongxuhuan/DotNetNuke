Feature: Site Alias
	In order to customize my site
	As a Super user
	I want to be able to add, edit and delete site aliases 


Scenario: Add site alias
	Given I am on the site home page 
	And I have logged in as the host
	And I am on the admin page Site Settings
	And I am on the Advanced Settings Tab
	And I am on the Site Aliases Section
	And I have created an ASP.NET v4.0 site in IIS with the alias testportalalias
	When I click Add New Alias
	And I enter localhost/testportalalias in the portal alias field
	And I click Save from the site alias table
	And I click Update
	And I browse to localhost/testportalalias
	Then I should see my site
