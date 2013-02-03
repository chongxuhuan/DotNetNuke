Feature: Render correct version of labelled content to a person reading the Help
	In order to learn the correct procedure to complete a task
	As a customer of DNN
	I want to be shown the task steps for the exact version I am using

Scenario: Visitor Views Help page with helpVersion=06.02.02
	Given I go to http://localhelp.dotnetnuke.com/Default.aspx?tabid=2045&helpVersion=06.02.02
	Then the page will contain 'DotNetNuke Community Edition 6.2.2'

Scenario: Visitor Views Help page with helpVersion=06.02.03
	Given I go to http://localhelp.dotnetnuke.com/Default.aspx?tabid=2045&helpVersion=06.02.01
	Then the page will contain 'DotNetNuke Community Edition 6.2.1'