Feature: ManageProfileProperties
	In order to allow users to manage their Profile on my site
	As an Administrator
	I want to view a list of Users

@MustBeDefaultAdminCredentials
Scenario: View Profile Properties
	Given I am on the site home page
	And I have logged in as the admin
	And I am on the admin page Site Settings
	Then A list of Profile Properties is displayed

@ClearExtraProfileProperties
Scenario: Should be able to add a Profile Property
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	And I am on the admin page Site Settings
	When I click Add New Profile Property
	And I fill in the profile property form
		| Control				| Value				|
		| Property Name			| TestProperty		|
		| Data Type				| Text				|
		| Property Category		| Basic				|
		| Length				| 100				|
		| Required				| false				|
		| Visible				| true				|
		| ReadOnly				| false				|
	And I click Next
	And I click Return
	Then Profile Property TestProperty is created correctly

@MustBeDefaultAdminCredentials
Scenario: Should be able to edit a Profile Property
	Given I am on the site home page
	And I have logged in as the admin
	And I am on the admin page Site Settings
	When I click Edit LastName Link
	Then Edit Profile Page is displayed for LastName

@ClearExtraProfileProperties
@MustHaveAUserWithFullProfile
Scenario: A Profile Property with Visible set to false should be Visible to Admin User
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	And I am on the admin page Site Settings
	And I click Add New Profile Property
	And I fill in the profile property form
		| Control				| Value				|
		| Property Name			| VisibleFalse		|
		| Data Type				| Text				|
		| Property Category		| Basic				|
		| Length				| 100				|
		| Required				| false				|
		| Visible				| false				|
		| ReadOnly				| false				|
	And I click Next
	And I click Return
	And I am on the admin page User Accounts
	When I click the Edit User Link for Michael Woods
	And I click the Manage Profile Tab
	Then Profile Property VisibleFalse is visible

@ClearExtraProfileProperties
@MustHaveAUserWithFullProfile
Scenario: A Profile Property with Visible set to false should not be Visible to User
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	And I am on the admin page Site Settings
	And I click Add New Profile Property
	And I fill in the profile property form
		| Control				| Value				|
		| Property Name			| VisibleFalse		|
		| Data Type				| Text				|
		| Property Category		| Basic				|
		| Length				| 100				|
		| Required				| false				|
		| Visible				| false				|
		| ReadOnly				| false				|
	And I click Next
	And I click Return
	And I log off
	And I have logged in as the user MichaelWoods password1234
	When I have clicked on my name
	And I click Edit Profile
	And I click the Manage Profile Tab
	Then Profile Property VisibleFalse is not visible

@ClearExtraProfileProperties
@MustHaveAUserWithFullProfile
Scenario: A Profile Property with ReadOnly set to true should be Visible to Admin User
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	And I am on the admin page Site Settings
	And I click Add New Profile Property
	And I fill in the profile property form
		| Control				| Value				|
		| Property Name			| ReadOnly			|
		| Data Type				| Text				|
		| Property Category		| Basic				|
		| Length				| 100				|
		| Required				| false				|
		| Visible				| false				|
		| ReadOnly				| true				|
	And I click Next
	And I click Return
	And I am on the admin page User Accounts
	When I click the Edit User Link for Michael Woods
	And I click the Manage Profile Tab
	Then Profile Property ReadOnly is visible
	And Profile Property ReadOnly is editable

@ClearExtraProfileProperties
@MustHaveAUserWithFullProfile
Scenario: A Profile Property with numbers at the start should work
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	And I am on the admin page Site Settings
	And I click Add New Profile Property
	And I fill in the profile property form
		| Control				| Value				|
		| Property Name			| 1234  			|
		| Data Type				| Text				|
		| Property Category		| Name				|
		| Length				| 3  				|
		| Required				| false				|
		| Visible				| true				|
		| ReadOnly				| false				|
	And I click Next
	And I click Return
	And I log off
	And I have logged in as the user MichaelWoods password1234
	When I have clicked on my name
    Then the Edit Profile link should be visible
