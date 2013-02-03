Feature: Add Language
	In order to create a localized site
	As an administrator or super user
	I want to be able to add a language to my site


Scenario: Add New Language For The Site
	Given I am on the site home page
	And I have logged in as the host
	And I am on the admin page Languages
	When I press Add New Language
	And I select the language Afrikaans (Suid Afrika)
	And I select the fallback language English (United States)
	And I click Update
	Then The language Afrikaans (Suid Afrika) is added to the language page

Scenario: Install Language Pack Via Host Extensions
	Given I am on the site home page
	And I have logged in as the host
	And I am on the Host Page Extensions
	When I press Install Extension Wizard
	And I upload the language pack ResourcePack.Full.04.09.00.fr-CA
	And I complete the install extension wizard
	Then The language français (Canada) is added to the site