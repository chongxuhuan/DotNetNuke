Feature: NewsLetter
	In order to send newsletter
	As an Admin
	I want to user NewsLetter control correctly

@MustBeDefaultAdminCredentials
Scenario: Should able to cancel preview
	Given I am on the site home page
	And I have logged in as the admin
	When I navigate to the admin page Newsletters
	And I input subject and message body
	And I click Preview Email
	Then I should see Cancel Preview button
