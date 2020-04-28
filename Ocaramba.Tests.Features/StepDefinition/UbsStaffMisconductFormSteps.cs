using NLog;
using NUnit.Framework;
using Ocaramba.Tests.PageObjects.PageObjects.Global.UbsStaffMisconduct;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow;

namespace Ocaramba.Tests.Features.StepDefinition
{
    [Binding]
    public sealed class ChangeLanguageSteps
    {
#if net47 || net45
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
#endif
#if netcoreapp3_1
        private static readonly NLog.Logger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
#endif
        private readonly DriverContext driverContext;
        private readonly ScenarioContext scenarioContext;

        public ChangeLanguageSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext ?? throw new ArgumentNullException("scenarioContext");
            driverContext = this.scenarioContext["DriverContext"] as DriverContext;
        }

        [When(@"I click Submit button")]
        public void WhenIClickSubmitButton()
        {
            scenarioContext.Get<UbsStaffMisconductPage>("ReportUbsStaffMisconductPage").ClickSubmitButton();
        }

        [Then(@"mendatory fields will be highlighted")]
        public void ThenMendatoryFieldsWillBeHighlighted()
        {
            Logger.Info(CultureInfo.CurrentCulture, "Checking if mandatory fields are highlighted");
            var fieldsborderColor = scenarioContext.Get<UbsStaffMisconductPage>("ReportUbsStaffMisconductPage").GetMandatoryFieldsBorderColor();
            foreach (var (name, color) in fieldsborderColor)
            {
                Verify.That(driverContext, () => Assert.AreEqual("rgb(242, 187, 58)", color, $"Element with name: {name} has not been highlighted."), true, false);
            }
        }

        [Then(@"informative messages for mendatory fields will be displayed")]
        public void ThenInformativeMessagesForMendatoryFieldsWillBeDisplayed()
        {
            Logger.Info(CultureInfo.CurrentCulture, "Checking if informative labels are displayed for mandatory fields");
            var elementsNameWithoutErrorFormLabels = scenarioContext.Get<UbsStaffMisconductPage>("ReportUbsStaffMisconductPage").GetElementsNameWithoutInformativeLabel().ToList();
            var notDisplayedErrorFormNames = scenarioContext.Get<UbsStaffMisconductPage>("ReportUbsStaffMisconductPage").GetNotDisplayedErrorFormLabelNames().ToList();

            Verify.That(driverContext, true, false, () => Assert.IsEmpty(elementsNameWithoutErrorFormLabels,
                $"Error form label doesn't exist for given element(-s) name: {ConcatNames(elementsNameWithoutErrorFormLabels)}."), 
                () => Assert.IsEmpty(notDisplayedErrorFormNames, $"Error form label for given element(-s) name: {ConcatNames(notDisplayedErrorFormNames)} is not displayed."));
        }

        private string ConcatNames(IEnumerable<string> names)
        {
            string result = string.Empty;

            names.ToList().ForEach(name => result += $"{name},");
            result = result.TrimEnd(',');

            return result;
        }
    }
}
