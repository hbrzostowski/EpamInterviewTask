Feature: Domicile redirection

@interviewTask
Scenario Outline: Verify redirection to correct domectic page
	Given General UBS page is opened
	When I change domicile to '<region>','<country>'
	Then I'm redirected to '<country>' UBS page

	Examples: 
	| region			   | country		|
	| Europe			   | United Kingdom |
	| North America        | Canada         |
	| Middle East & Africa | Israel			|