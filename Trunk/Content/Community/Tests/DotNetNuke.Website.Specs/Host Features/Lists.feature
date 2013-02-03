Feature: List
	In order to manage list items
	As a host user
	I want to be manage list items correctly

@MustBeDefaultAdminCredentials
Scenario: Delete items created by host user
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Lists
	And I add a new list TestList
	And I add new item TestItem to TestList
	Then I should see delete button of item TestItem in the list
