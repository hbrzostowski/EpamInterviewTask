using Ocaramba.Extensions;
using Ocaramba.Types;
using OpenQA.Selenium;

namespace Ocaramba.Tests.PageObjects.PageObjects.Components.PrivacySettings
{
    public partial class PrivacySettingsComponent
    {
        private IWebElement PrivacySettingsFrame => Driver.GetElement(new ElementLocator(Locator.TagName, "iframe"));

        private IWebElement PrivacySettingsModelWindow => Driver.GetElement(new ElementLocator(Locator.XPath, "//*[@id = 'doc' and contains(@class, 'popup')]"));

        private IWebElement AgreeToAllCookiesButton => PrivacySettingsModelWindow.GetElement(new ElementLocator(Locator.XPath, ".//button[contains(@class, 'allowAllCookies')]/span"));
    }
}
