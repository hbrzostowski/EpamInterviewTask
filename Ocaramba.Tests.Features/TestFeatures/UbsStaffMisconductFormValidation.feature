Feature: UbsStaffMisconductFormValidation

@interviewTask
Scenario: Verify Report misconduct of UBS staff form validation
	Given General UBS page is opened
	And I navigate to Report misconduct of UBS staff page
	When I click Submit button
	Then mendatory fields will be highlighted
	And informative messages for mendatory fields will be displayed