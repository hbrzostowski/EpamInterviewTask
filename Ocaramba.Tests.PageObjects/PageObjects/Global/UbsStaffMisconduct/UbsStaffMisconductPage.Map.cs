using Ocaramba.Extensions;
using Ocaramba.Types;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Ocaramba.Tests.PageObjects.PageObjects.Global.UbsStaffMisconduct
{
    public partial class UbsStaffMisconductPage
    {
        private IWebElement UbsStaffMisconductFrame => Driver.GetElement(new ElementLocator(Locator.TagName, "iframe"));

        private IWebElement SubmitButton => Driver.GetElement(new ElementLocator(Locator.Id, "senddata"));

        private IList<IWebElement> MandatoryFields => Driver.FindElements(By.XPath("//*[@aria-required = 'true']"));

        private IList<IWebElement> GetFormErrorLabels() => Driver.FindElements(By.XPath("//label[@class = 'form__error']"));
    }
}
