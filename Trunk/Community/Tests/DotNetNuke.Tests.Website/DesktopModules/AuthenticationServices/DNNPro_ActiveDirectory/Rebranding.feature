Feature: Rebranding
	In order to upgrade Customers from CE to PE AD Authentication
	As a Super User
	I want the existing CE AD Auth Provider removed when the new PE AD Auth Provider is installed.

Scenario Outline: Install Pro AD Provider
	Given The CE AD Auth Provider is installed
	And Login as UID=<Scenario User Name> PWD=<Scenario Password> Role=<Scenario Role>
	When I install the Pro AD Auth Provider from Available Extensions
	Then The CE AD AUth Provider will be uninstalled
Examples: 
	| Scenario User Name | Scenario Password | Scenario Role |
	| shaun              | password          | Super User    |