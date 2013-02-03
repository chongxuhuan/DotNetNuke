Feature: Languages
	In order to manage multiple languages
	As an admin
	I want to be using features in language correctly

@MustBeDefaultAdminCredentials
Scenario: Language selector should show the correct value of current language
	Given I am on the site home page
	And I have logged in as the host
	When I am on the Host Page Host Settings
	And I enable content localization setting
	And I navigate to the admin page Languages
	And I add new language es-ES
	And I enable content localization on the portal
	And I modify skin Home-Mega-Menu.ascx to set showMenu of Language control
	And I navigate to home page
	And I click language icon of es-ES
	Then The language drop down should also select language es-ES

@MustBeDefaultAdminCredentials
Scenario: RadEditorProvider language pack should create successful
	Given I am on the site home page
	And I have logged in as the host
	When I navigate to the admin page Languages
	And I try to create Provider language pack of RadEditor
	Then RadEditor language pack should create successful
	