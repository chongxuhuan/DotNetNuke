Feature: Read Sketches
	In order to understand how a DNN feature works
	As a user with the correct permissions
	I want to be able to find the Sketch easily and read it

Scenario Outline: Reading a Sketch via the Online Help feature in DotNetNuke 
	Given There is a Page called Sketches with these permissions
		| Role             | Permission   | Value   |
		| Registered Users | Full Control | Allowed |
	And There is a Sketch View module on the page with these permissions
		| Role             | Permission   | Value   |
		| Registered Users | Full Control | Allowed |
	And Login as <Scenario User Name> <Scenario Password> <Scenario Role>
	When Once you click the Online Help you will be redirected to Default.aspx?tabname=sketches&helpculture=en-us&helpmodule=Sketch&helpversion=<Version>
	Then When you get to the Online Help page you will see the About The Sketches Module for version <Version>

	Examples: 
	| Scenario User Name | Scenario Password | Scenario Role   | Version  |
	| deadmau5           | password          | Anonymous       | 06.01.05 |
	| philt3r            | password          | Registered User | 06.02.03 |
