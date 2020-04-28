using NLog;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ocaramba.Tests.PageObjects.PageObjects.Global.UbsStaffMisconduct
{
    public partial class UbsStaffMisconductPage : ProjectPageBase
    {
        private static readonly ILogger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public UbsStaffMisconductPage(DriverContext driverContext) : base(driverContext)
        {
            Driver.SwitchTo().Frame(UbsStaffMisconductFrame);
        }

        public void ClickSubmitButton()
        {
            Logger.Info(CultureInfo.CurrentCulture, "Clicking Submit button");
            SubmitButton.Click();
        }

        public IEnumerable<(string name, string color)> GetMandatoryFieldsBorderColor()
        {
            //skipping radio buttons which doesn't have border
            return MandatoryFields.Where(field => field.GetProperty("type") != "radio").Select(field => (name: field.GetProperty("name"), color: field.GetCssValue("border-color")));
        }

        public IEnumerable<string> GetElementsNameWithoutInformativeLabel()
        {
            var uniqueMandatoryFieldsName = MandatoryFields.GroupBy(x => x.GetProperty("name")).Select(x => x.Key);

            return uniqueMandatoryFieldsName.Except(GetFormErrorLabels().Select(label => label.GetProperty("htmlFor")));
        }

        public IEnumerable<string> GetNotDisplayedErrorFormLabelNames()
        {
            return GetFormErrorLabels().Where(label => label.GetCssValue("display").Equals("none")).Select(label => label.GetProperty("htmlFor"));
        }
    }
}
