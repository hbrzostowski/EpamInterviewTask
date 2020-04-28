using NLog;
using NUnit.Framework;
using Ocaramba.Tests.PageObjects.PageObjects.Global.Homepage;
using System;
using System.Globalization;
using TechTalk.SpecFlow;

namespace Ocaramba.Tests.Features.StepDefinitions
{
    [Binding]
    public class CommonSteps
    {
#if net47 || net45
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
#endif
#if netcoreapp3_1
        private static readonly NLog.Logger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
#endif

        private readonly DriverContext driverContext;
        private readonly ScenarioContext scenarioContext;      

        public CommonSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext ?? throw new ArgumentNullException("scenarioContext");
            this.driverContext = this.scenarioContext["DriverContext"] as DriverContext;
        }

        [Given(@"General UBS page is opened")]
        public void GivenGeneralUBSPageIsOpened()
        {
            var globalHomePage = new HomepagePage(driverContext).OpenGlobalHomepage();
            scenarioContext.Set(globalHomePage, "GlobalHomePage");
        }

        [Given(@"I navigate to Report misconduct of UBS staff page")]
        public void GivenINavigateToReportUsbStaffMisconductPage()
        {
            var reportUbsStaffMisconductPage = scenarioContext.Get<HomepagePage>("GlobalHomePage").NavigateToUbsStaffMisconductPage();
            scenarioContext.Set(reportUbsStaffMisconductPage, "ReportUbsStaffMisconductPage");
        }

        [When(@"I click DE language button")]
        public void WhenIClickDELanguageButton()
        {
            scenarioContext.Get<HomepagePage>("GlobalHomePage").ChangeLanguageToDutch();
        }

        [When(@"I change domicile to '(.*)','(.*)'")]
        public void WhenIChangeDomicileTo(string region, string country)
        {
            scenarioContext.Get<HomepagePage>("GlobalHomePage").DomicileSelection.SelectDomicile(region, country);
        }

        [Then(@"page is translated to Dutch")]
        public void ThenPageIsTranslatedToDutch()
        {
            Logger.Info(CultureInfo.CurrentCulture, "Checking if page is translated to Dutch");

            var expectedLanguage = "de";
            var expectedHeader = "Globale Themen";

            var globalHomePage = scenarioContext.Get<HomepagePage>("GlobalHomePage");
            var actualMetaDataLanguage = globalHomePage.GetMetaDataLanguage();
            var actualHeaderTitleText = globalHomePage.GetHeaderTitleText();

            Verify.That(driverContext, true, false, () => Assert.AreEqual(expectedLanguage, actualMetaDataLanguage, $"Language has been changed to {actualMetaDataLanguage} instead of {expectedLanguage}."),
                () => Assert.AreEqual(expectedHeader, actualHeaderTitleText, $"Translated header should be: {expectedHeader}, but is {actualHeaderTitleText}."));
        }

        [Then(@"I'm redirected to '(.*)' UBS page")]
        public void ThenIMRedirectedToSpecificUBSPage(string expectedHeaderTitleText)
        {
            Logger.Info(CultureInfo.CurrentCulture, $"Checking if user is redirected to {expectedHeaderTitleText} domectic page");
            var actualHeaderTitleText = scenarioContext.Get<HomepagePage>("GlobalHomePage").GetHeaderTitleText();
            Verify.That(driverContext, () => Assert.AreEqual(expectedHeaderTitleText, actualHeaderTitleText, $"Expected page header to be: {expectedHeaderTitleText}, but is: {actualHeaderTitleText}."), true, false);
        }
    }
}