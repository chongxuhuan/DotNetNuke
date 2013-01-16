Feature: Add Module 
	In order to add content to my site
	As an administrator
	I want to be able to add modules to my site

@MustBeDefaultAdminCredentials
@MustUseRibbonBar
@MustHaveAUserWithFullProfile
@MustNotHaveViewPage
Scenario: Add HTML Module From Ribbon Bar Same As Page Vis
	Given I am on the site home page
	And I have logged in as the admin
	And I have created the blank page called View Page from the Ribbon Bar
	And The page View Page has View permission set to Grant for the role All Users
	And I am viewing the page called View Page
	When I create the module View with Same As Page visibility and the content This is a test 
	Then As an admin on the page View Page I can see the module View and its content This is a test
	And As MichaelWoods with the password password1234 on the page View Page I can see the module View and its content This is a test
	And As an anonymous user on the page View Page I can see the module View and its content This is a test

@MustBeDefaultAdminCredentials
@MustUseRibbonBar
@MustHaveAUserWithFullProfile
@MustNotHaveEditPage
Scenario: Add HTML Module From Ribbon Bar Page Editors Only Vis
	Given I am on the site home page
	And I have logged in as the admin
	And I have created the blank page called Edit Page from the Ribbon Bar
	And The page Edit Page has View permission set to Grant for the role All Users
	And The page Edit Page has edit permissions set for the user MichaelWoods with display name Michael Woods
	And I am viewing the page called Edit Page
	When I create the module Edit with Page Editors Only visibility and the content This is for page editors only
	Then As an admin on the page Edit Page I can see the module Edit and its content This is for page editors only
	And As MichaelWoods with the password password1234 on the page Edit Page I can see the module Edit and its content This is for page editors only
	And As an anonymous user on the page Edit Page I can not see the module Edit and its content This is for page editors only

