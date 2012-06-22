Feature: Search Crawler Search Result
	In order to display search crawler result
	I want the search result control work correctly

@MustBeDefaultAdminCredentials
Scenario: Should only display current site's result
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page SiteManagement
	When I create a new child portal
	And I am on the Host Page Schedule
	And I run scheduler SearchCrawler
	And I visit child portal
	And I input search text who in search box and click search button
	Then I should not see result from parent portal