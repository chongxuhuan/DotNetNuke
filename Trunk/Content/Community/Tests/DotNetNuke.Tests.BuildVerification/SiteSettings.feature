Feature: Site Settings
	In order to customise the settings on my site
	As an administrator
	I want to able to view and update my site settings

@MustBeDefaultAdminCredentials
Scenario: Access Site Settings As Admin
	Given I am on the site home page 
	And I have logged in as the admin
	When I navigate to the admin page Site Settings
	Then I should be on the Site Settings Page
	And I should not see the SSL section on the page 
	And I should not see the Host section on the page


Scenario: Access Site Settings As Host
	Given I am on the site home page 
	And I have logged in as the host
	When I navigate to the admin page Site Settings
	Then I should be on the Site Settings Page
	And I should see the SSL section on the page 
	And I should see the Host section on the page
