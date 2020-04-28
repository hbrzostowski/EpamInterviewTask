using NLog;
using Ocaramba.Extensions;
using Ocaramba.Tests.PageObjects.PageObjects.Components.DomicileSelection;
using Ocaramba.Tests.PageObjects.PageObjects.Components.PrivacySettings;
using Ocaramba.Tests.PageObjects.PageObjects.Global.UbsStaffMisconduct;
using System;
using System.Globalization;

namespace Ocaramba.Tests.PageObjects.PageObjects.Global.Homepage
{
    public partial class HomepagePage : ProjectPageBase
    {
#if net47 || net45
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
#endif
#if netcoreapp3_1
        private static readonly NLog.Logger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
#endif

        public HomepagePage(DriverContext driverContext) : base(driverContext)
        {
            DomicileSelection = new DomicileSelectionComponent(driverContext);
        }

        public DomicileSelectionComponent DomicileSelection { get; }

        public HomepagePage OpenGlobalHomepage()
        {
            var url = BaseConfiguration.GetUrlValue;
            Driver.NavigateTo(new Uri(url));
            Logger.Info(CultureInfo.CurrentCulture, "Opening page {0}", url);
            new PrivacySettingsComponent(DriverContext).AcceptAllCookies();

            return this;
        }

        public string GetHeaderTitleText() => HeaderTitle.Text;

        public void ChangeLanguageToDutch()
        {
            Logger.Info(CultureInfo.CurrentCulture, "Changing language to Dutch");
            DutchLanguageButton.Click();
        }

        public string GetMetaDataLanguage() => MetaDataElement.GetAttribute("data-meta-language");

        public UbsStaffMisconductPage NavigateToUbsStaffMisconductPage()
        {
            Logger.Info(CultureInfo.CurrentCulture, "Navigating to Report misconduct of UBS staff page");
            ContactButton.Click();
            ReportUbsStaffMisconductLink.Click();

            return new UbsStaffMisconductPage(DriverContext);
        }
    }
}
