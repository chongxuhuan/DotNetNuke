Feature: Workflow
	In order to publish html content in workflow
	As an editor
	I want to use the workflow correctly

@MustBeDefaultAdminCredentials
@MustHaveEmailSetUpForSiteDumpToFolder
@MustHaveEmptyEmailFolder
@SiteMustRunInFullTrust
Scenario: The token in email subject should be replace
	Given I am on the site home page
	And I have logged in as the host
	And The email subject has been update to include tokens
	When I go to the Module 0 Settings page
	And I change workflow to Content Approval
	And I Submit content of Module 0
	Then Subject in notification email should replace all tokens

