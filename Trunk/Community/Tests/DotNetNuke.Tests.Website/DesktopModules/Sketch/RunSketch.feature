Feature: Run Sketch
	In order to see how my sketch works
	As a Sketcher
	I want to be able to run my sketches


Scenario: Run a Sketch - All Steps Developed - No variables
	Given I am allowed to run a sketch
	And I can see a list of available Sketches to run
	When I run the sketch
	Then I should see my browser open and run the sketch

Scenario: Run a Sketch - All Steps Developed - Variables
	Given I am allowed to run a sketch
	And I can see a list of available Sketches to run
	When I run the sketch
	Then I should see a form to fill out with the required info to run the sketch
	And I should see my browser open and run the sketch 

Scenario: Run a Sketch - Not All Steps Developed - No variables
	Given I am allowed to run a sketch
	And I can see a list of available Sketches to run
	And I run the sketch
	And I see my browser open and run the sketch
	When the sketch gets to an undeveloped step
	Then a dummy step should show
	And the missing step's name and description should show
	And the missing step's mockup image should show
	And the sketch should be able to continue on to completion