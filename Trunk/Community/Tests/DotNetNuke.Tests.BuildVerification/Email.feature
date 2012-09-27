Feature: Email
	In order to communicate with my users
	As a host
	I want to be able to send emails from my site

@MustHaveEmailSetUpForSiteDumpToFolder
@MustHaveEmptyEmailFolder
@SiteMustRunInFullTrust
Scenario: Configure Email
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Host Settings
	And I have updated the Host Email on the Settings page
	And I am on the Advanced Settings Tab
	When I enter localhost into the SMTP server field
	And I select Annonymous SMTP Authentication
	And I click the SMTP Test link
	Then A test email should be sent to the host
