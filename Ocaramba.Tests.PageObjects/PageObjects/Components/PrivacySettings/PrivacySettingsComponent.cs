namespace Ocaramba.Tests.PageObjects.PageObjects.Components.PrivacySettings
{
    public partial class PrivacySettingsComponent : ProjectPageBase
    {
        public PrivacySettingsComponent(DriverContext driverContext) : base(driverContext)
        {
        }

        public void AcceptAllCookies()
        {
            Driver.SwitchTo().Frame(PrivacySettingsFrame);
            AgreeToAllCookiesButton.Click();
            Driver.SwitchTo().ParentFrame();
        }
    }
}
