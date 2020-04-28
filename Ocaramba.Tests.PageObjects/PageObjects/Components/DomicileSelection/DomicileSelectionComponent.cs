using NLog;
using Ocaramba.Extensions;
using Ocaramba.Tests.PageObjects.Helpers;
using Ocaramba.Types;
using OpenQA.Selenium.Support.UI;
using System;
using System.Globalization;

namespace Ocaramba.Tests.PageObjects.PageObjects.Components.DomicileSelection
{
    public partial class DomicileSelectionComponent : ProjectPageBase
    {
        private static readonly ILogger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public DomicileSelectionComponent(DriverContext driverContext) : base(driverContext)
        {
        }

        public void SelectDomicile(string region, string country)
        {
            CheckIfDomicilesAreSupported(region, country);

            Logger.Info(CultureInfo.CurrentCulture, "Clicking 'Select domicile button");
            DomicileSelectionButton.Click();

            Logger.Info(CultureInfo.CurrentCulture, $"Selecting domicile region: {region}");
            RegionMenuButton.Click();
            RegionMenuItem(region).Click();

            Logger.Info(CultureInfo.CurrentCulture, $"Selecting domicile country: {region}");
            CountryMenuButton.Click();
            WaitForDomicileCountriesDropdownToOpen();
            CountryMenuItem(country).Click();
        }

        private void CheckIfDomicilesAreSupported(string region, string country)
        {
            bool unsupportedRegion, unsupportedCountry;
            unsupportedRegion = !SupportedDomicile.Regions.Contains(region);
            unsupportedCountry = !SupportedDomicile.Countries.Contains(country);

            if (unsupportedRegion && unsupportedCountry)
            {
                throw new ArgumentException($"Provided region: {region} and country: {country} are not supported");
            }
            else if (unsupportedRegion)
            {
                throw new ArgumentException($"Provided region: {region} is not supported");
            }
            else if (unsupportedCountry)
            {
                throw new ArgumentException($"Provided country: {country} is not supported");
            }
        }

        private void WaitForDomicileCountriesDropdownToOpen()
        {
            new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(
                drv => drv.GetElement(new ElementLocator(Locator.XPath,
                "//*[contains(@class, 'list--thirdLevel') and contains(@class, 'list--country')]")).GetAttribute("style").Contains("block"));
        }
    }
}
