using Ocaramba.Extensions;
using Ocaramba.Types;
using OpenQA.Selenium;

namespace Ocaramba.Tests.PageObjects.PageObjects.Components.DomicileSelection
{
    public partial class DomicileSelectionComponent
    {
        private IWebElement DomicileSelectionButton => Driver.GetElement(new ElementLocator(Locator.XPath, "//button[@class = 'domicileSelection__toggle js-header-domicile-toggle']"));

        private IWebElement DomicileMenu => Driver.GetElement(new ElementLocator(Locator.XPath, "//*[contains(@class, 'list--secondLevel') and contains(@class, 'list--domicile') and @role = 'menu']"));

        private IWebElement RegionMenuButton => DomicileMenu.GetElement(new ElementLocator(Locator.XPath, "//button[contains(@class, 'open-region-panel')]"));

        private IWebElement CountryMenuButton => DomicileMenu.GetElement(new ElementLocator(Locator.XPath, "//button[contains(@class, 'open-country-panel')]"));

        private IWebElement RegionMenuItem(string region) => DomicileMenu.GetElement(new ElementLocator(Locator.XPath, $"//button[normalize-space(text()) = '{region}' and contains(@class, 'regionButton')]"));

        private IWebElement CountryMenuItem(string country) => DomicileMenu.GetElement(new ElementLocator(Locator.XPath, $"//a[normalize-space(text()) = '{country}' and contains(@class, 'countryLink')]"));
    }
}
