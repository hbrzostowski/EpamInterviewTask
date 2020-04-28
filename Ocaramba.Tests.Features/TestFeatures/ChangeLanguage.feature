Feature: ChangeLanguage

@interviewTask
Scenario: Verify possibility to change global page language to Dutch
	Given General UBS page is opened
	When I click DE language button
	Then page is translated to Dutch
