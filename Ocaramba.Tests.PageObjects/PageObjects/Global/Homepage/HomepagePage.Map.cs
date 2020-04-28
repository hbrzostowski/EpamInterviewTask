using Ocaramba.Extensions;
using Ocaramba.Types;
using OpenQA.Selenium;

namespace Ocaramba.Tests.PageObjects.PageObjects.Global.Homepage
{
    public partial class HomepagePage
    {
        private IWebElement HeaderTitle => Driver.GetElement(new ElementLocator(Locator.XPath, "//*[@class = 'header__title']/span/span"));

        private IWebElement DutchLanguageButton => Driver.GetElement(new ElementLocator(Locator.XPath, "//*[contains(@class, 'language-label') and normalize-space(text()) = 'DE']"));

        private IWebElement MetaDataElement => Driver.FindElement(By.XPath("//meta[@name = 'datalayer-metadata']"));

        private IWebElement ContactButton => Driver.GetElement(new ElementLocator(Locator.XPath, "//*[contains(@class, 'contactIcon')]"));

        private IWebElement ReportUbsStaffMisconductLink => Driver.GetElement(new ElementLocator(Locator.LinkText, "Report misconduct of UBS staff"));
    }
}
