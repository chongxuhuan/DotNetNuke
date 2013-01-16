Feature: Create a Sketch of a Feature
	In order to explain in a step by step nature a feature of DNN
	As a Product Manager, Engineer, Test Writer or Technical Writer
	I want to be able to quickly communicate how the feature works

Scenario Outline: Creating a Sketch with existing Steps and Outcomes
	Given There is a Page called Sketches with these permissions
		| Role      | Permission | Value   |
		| All Users | View       | Allowed |
	And There is a Sketch module on the page with these permissions
		| Role                       | Permission | Value   |
		| User Account Administrator | View       | Allowed |
	Given Login as UID=<Scenario User Name> PWD=<Scenario Password> Role=<Scenario Role>
	When Right click on the Feature in the Sketches tree and select Add Sketch
	And In the Title text box, enter the name of the Sketch <Title>
	And In the Page Permission grid set the permissions for the Page for this Sketch
	And In the Module Permission grid set the permissions for the Module for this Sketch
	And In the Description field describe the main function of the Sketch and its expected outcome <Description>
	And In the Pre-requsites field start typing the Pre-requsite and select the Pre-requsite you wish to use from the autocomplete list <Pre-requsites>
	And In the Steps field start typing the step and select the Step you wish to use from the autocomplete list <Steps>
	And In the Examples grid fill out a value for each field
	And Click the Save button
	Then A new Sketch node will appear in the Sketches tree
	And The Sketch editing form will switch to View mode
Examples: 
| Scenario User Name | Scenario Password | Scenario Role    | Title                                              | Description             | Steps                                                                       | Pre-requsites       |
| philt3r            | password          | Registered Users | Creating a Sketch with existing Steps and Outcomes | Here is the description | [Login as a user in the Administrator role][Go to /Admin/UserAccounts.aspx] | SMTP must be set up |
| deadmau5           | password          | Administrator    | Creating a Sketch with existing Steps and Outcomes | Here is the description | [Login as a user in the Administrator role][Go to /Admin/UserAccounts.aspx] | SMTP must be set up |