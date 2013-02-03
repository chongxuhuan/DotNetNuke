Feature: Subscribing to a Member Service
	In order to subscribe to the website's services
	As a customer
	I want to be able to subscribe to services 

@MustHaveAUserWithFullProfile
@MustHaveSandboxedPaymentSettings
@MustHaveServiceWithFee
Scenario: Subscribing to a Member Service With A Fee
	Given I am on the site home page
	And I have logged in as the user MichaelWoods password1234
	And I have clicked on my name
	When I click Edit Profile
	And I click Manage Services
	And I Subscribe to a service with a fee
	Then I should be on the PayPal site with all the fields filled out
