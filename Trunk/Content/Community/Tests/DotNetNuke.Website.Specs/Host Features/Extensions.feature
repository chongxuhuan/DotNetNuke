Feature: Extensions
	In order to manage extensions
	As a host
	I want to use extensions control correctly

@MustBeDefaultAdminCredentials
Scenario: Should not see next button when install an invalid package
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Extensions
	And I click Install Extension Wizard from action menu
	And I select the invalid package
	And I click Next Button
	Then I Should not see next button


@MustBeDefaultAdminCredentials
Scenario: Icon file should be available after call create package wizard
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Extensions
	And I click edit icon for module Journal
	And I click Create Package Button
	And I click Next Button
	And I click Next Button
	And I click Next Button
	And I click Next Button
	And I click Next Button
	And I click Return Button
	Then Icon file of module Journal should be available