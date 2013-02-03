Feature: Newsletter
	In order to inform my users about news and special offers
	As a super user
	I want to be able to send newsletters to users of my site

@MustHaveEmailSetUpForSiteDumpToFolder
@MustHaveUser1Added
@MustHaveEmptyEmailFolder
@SiteMustRunInFullTrust
Scenario: Send Newsletter To Registered Users
	Given I am on the site home page
	And I have logged in as the host
	And I am on the admin page Newsletters
	When I fill in the newsletter form with the following information
		| Label        | Value                  |
		| FromAddress  | test@dnncorp.com       |
		| ReplyTo      | reply@dnncorp.com      |
		| SubjectField | New Newsletter Subject |
		| ContentField | This is the Newsletter |
	And I check the role Registered Users on the newsletters page
	And I click Send
	Then The following newsletter should be sent to testuser1@dnn.com
		| Label      | Value                  |
		| From		 | test@dnncorp.com       |
		| ReplyTo    | reply@dnncorp.com      |
		| Subject	 | New Newsletter Subject |
		| Content	 | This is the Newsletter |
	And The following newsletter should be sent to admin@change.me
		| Label      | Value                  |
		| From		 | test@dnncorp.com       |
		| ReplyTo    | reply@dnncorp.com      |
		| Subject	 | New Newsletter Subject |
		| Content	 | This is the Newsletter |
	And The following bulk email report should be sent to test@dnncorp.com
		| Label      | Value                  |
		| From		 | test@dnncorp.com       |
		| Recipients | 2					  |
		| Messages	 | 2					  |
		| Subject	 | New Newsletter Subject |