// <copyright file="DriverContext.cs" company="Objectivity Bespoke Software Specialists">
// Copyright (c) Objectivity Bespoke Software Specialists. All rights reserved.
// </copyright>
// <license>
//     The MIT License (MIT)
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// </license>

namespace Ocaramba
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using NLog;
    using Ocaramba.Helpers;
    using Ocaramba.Logger;
    using Ocaramba.Types;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Edge;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Safari;

    /// <summary>
    /// Contains handle to driver and methods for web browser.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Driver is disposed on test end")]
    public partial class DriverContext
    {
        private static readonly ILogger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        private readonly Collection<ErrorDetail> verifyMessages = new Collection<ErrorDetail>();

        /// <summary>
        /// Gets or sets the handle to current driver.
        /// </summary>
        /// <value>
        /// The handle to driver.
        /// </value>
        private IWebDriver driver;

        private ChromeDriverService serviceChrome;

        private TestLogger logTest;

        /// <summary>
        /// Occurs when [driver options set].
        /// </summary>
        public event EventHandler<DriverOptionsSetEventArgs> DriverOptionsSet;

        /// <summary>
        /// Gets or sets the test title.
        /// </summary>
        /// <value>
        /// The test title.
        /// </value>
        public string TestTitle { get; set; }

        /// <summary>
        /// Gets Sets Folder name for ScreenShot.
        /// </summary>
        public string ScreenShotFolder
        {
            get
            {
                return FilesHelper.GetFolder(BaseConfiguration.ScreenShotFolder, this.CurrentDirectory);
            }
        }

        /// <summary>
        /// Gets Sets Folder name for Download.
        /// </summary>
        public string DownloadFolder
        {
            get
            {
                return FilesHelper.GetFolder(BaseConfiguration.DownloadFolder, this.CurrentDirectory);
            }
        }

        /// <summary>
        /// Gets Sets Folder name for PageSource.
        /// </summary>
        public string PageSourceFolder
        {
            get
            {
                return FilesHelper.GetFolder(BaseConfiguration.PageSourceFolder, this.CurrentDirectory);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [test failed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [test failed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsTestFailed { get; set; }

        /// <summary>
        /// Gets or sets test logger.
        /// </summary>
        public TestLogger LogTest
        {
            get
            {
                return this.logTest ?? (this.logTest = new TestLogger());
            }

            set
            {
                this.logTest = value;
            }
        }

        /// <summary>
        /// Gets driver Handle.
        /// </summary>
        public IWebDriver Driver
        {
            get
            {
                return this.driver;
            }
        }

        /// <summary>
        /// Gets all verify messages.
        /// </summary>
        public Collection<ErrorDetail> VerifyMessages
        {
            get
            {
                return this.verifyMessages;
            }
        }

        /// <summary>
        /// Gets or sets directory where assembly files are located.
        /// </summary>
        public string CurrentDirectory { get; set; }

        private FirefoxOptions FirefoxOptions
        {
            get
            {
                FirefoxOptions options = new FirefoxOptions();

                if (!string.IsNullOrEmpty(BaseConfiguration.PathToFirefoxProfile))
                {
                    try
                    {
                        var pathToCurrentUserProfiles = BaseConfiguration.PathToFirefoxProfile; // Path to profile
                        var pathsToProfiles = Directory.GetDirectories(pathToCurrentUserProfiles, "*.default", SearchOption.TopDirectoryOnly);

                        options.Profile = new FirefoxProfile(pathsToProfiles[0]);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Logger.Error(CultureInfo.CurrentCulture, "problem with loading firefox profile {0}", e.Message);
                    }
                }

                options.SetPreference("toolkit.startup.max_resumed_crashes", "999999");
                options.SetPreference("network.automatic-ntlm-auth.trusted-uris", BaseConfiguration.Host ?? string.Empty);

                // retrieving settings from config file
                NameValueCollection firefoxPreferences = new NameValueCollection();
                NameValueCollection firefoxExtensions = new NameValueCollection();

                firefoxPreferences = BaseConfiguration.GetNameValueCollectionFromAppsettings("FirefoxPreferences");
                firefoxExtensions = BaseConfiguration.GetNameValueCollectionFromAppsettings("FirefoxExtensions");

                // preference for downloading files
                options.SetPreference("browser.download.dir", this.DownloadFolder);
                options.SetPreference("browser.download.folderList", 2);
                options.SetPreference("browser.download.managershowWhenStarting", false);
                options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/vnd.ms-excel, application/x-msexcel, application/pdf, text/csv, text/html, application/octet-stream");

                // disable Firefox's built-in PDF viewer
                options.SetPreference("pdfjs.disabled", true);

                // disable Adobe Acrobat PDF preview plugin
                options.SetPreference("plugin.scan.Acrobat", "99.0");
                options.SetPreference("plugin.scan.plid.all", false);

                // if there are any extensions
                if (firefoxExtensions != null)
                {
                    // loop through all of them
                    for (var i = 0; i < firefoxExtensions.Count; i++)
                    {
                        Logger.Trace(CultureInfo.CurrentCulture, "Installing extension {0}", firefoxExtensions.GetKey(i));
                        try
                        {
                            options.Profile.AddExtension(firefoxExtensions.GetKey(i));
                        }
                        catch (FileNotFoundException)
                        {
                            Logger.Trace(CultureInfo.CurrentCulture, "Installing extension {0}", this.CurrentDirectory + FilesHelper.Separator + firefoxExtensions.GetKey(i));
                            options.Profile.AddExtension(this.CurrentDirectory + FilesHelper.Separator + firefoxExtensions.GetKey(i));
                        }
                    }
                }

                options = this.AddFirefoxArguments(options);

                // custom preferences
                // if there are any settings
                if (firefoxPreferences == null)
                {
                    return options;
                }

                // loop through all of them
                for (var i = 0; i < firefoxPreferences.Count; i++)
                {
                    Logger.Trace(CultureInfo.CurrentCulture, "Set custom preference '{0},{1}'", firefoxPreferences.GetKey(i), firefoxPreferences[i]);

                    // and verify all of them
                    switch (firefoxPreferences[i])
                    {
                        // if current settings value is "true"
                        case "true":
                            options.SetPreference(firefoxPreferences.GetKey(i), true);
                            break;

                        // if "false"
                        case "false":
                            options.SetPreference(firefoxPreferences.GetKey(i), false);
                            break;

                        // otherwise
                        default:
                            int temp;

                            // an attempt to parse current settings value to an integer. Method TryParse returns True if the attempt is successful (the string is integer) or return False (if the string is just a string and cannot be cast to a number)
                            if (int.TryParse(firefoxPreferences.Get(i), out temp))
                            {
                                options.SetPreference(firefoxPreferences.GetKey(i), temp);
                            }
                            else
                            {
                                options.SetPreference(firefoxPreferences.GetKey(i), firefoxPreferences[i]);
                            }

                            break;
                    }
                }

                return options;
            }
        }

        private ChromeOptions ChromeOptions
        {
            get
            {
                ChromeOptions options = new ChromeOptions();

                // retrieving settings from config file
                NameValueCollection chromePreferences = BaseConfiguration.GetNameValueCollectionFromAppsettings("ChromePreferences");
                NameValueCollection chromeExtensions = BaseConfiguration.GetNameValueCollectionFromAppsettings("ChromeExtensions");
                NameValueCollection chromeArguments = BaseConfiguration.GetNameValueCollectionFromAppsettings("chromeArguments");

                options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
                options.AddUserProfilePreference("download.default_directory", this.DownloadFolder);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddExcludedArgument("enable-automation");
                options.AddAdditionalCapability("useAutomationExtension", false);

                // if there are any extensions
                if (chromeExtensions != null)
                {
                    // loop through all of them
                    for (var i = 0; i < chromeExtensions.Count; i++)
                    {
                        Logger.Trace(CultureInfo.CurrentCulture, "Installing extension {0}", chromeExtensions.GetKey(i));
                        try
                        {
                            options.AddExtension(chromeExtensions.GetKey(i));
                        }
                        catch (FileNotFoundException)
                        {
                            Logger.Trace(CultureInfo.CurrentCulture, "Installing extension {0}", this.CurrentDirectory + FilesHelper.Separator + chromeExtensions.GetKey(i));
                            options.AddExtension(this.CurrentDirectory + FilesHelper.Separator + chromeExtensions.GetKey(i));
                        }
                    }
                }

                // if there are any arguments
                if (chromeArguments != null)
                {
                    // loop through all of them
                    for (var i = 0; i < chromeArguments.Count; i++)
                    {
                        Logger.Trace(CultureInfo.CurrentCulture, "Setting Chrome Arguments {0}", chromeArguments.GetKey(i));
                        options.AddArgument(chromeArguments.GetKey(i));
                    }
                }

                // custom preferences
                // if there are any settings
                if (chromePreferences == null)
                {
                    return options;
                }

                // loop through all of them
                for (var i = 0; i < chromePreferences.Count; i++)
                {
                    Logger.Trace(CultureInfo.CurrentCulture, "Set custom preference '{0},{1}'", chromePreferences.GetKey(i), chromePreferences[i]);

                    // and verify all of them
                    switch (chromePreferences[i])
                    {
                        // if current settings value is "true"
                        case "true":
                            options.AddUserProfilePreference(chromePreferences.GetKey(i), true);
                            break;

                        // if "false"
                        case "false":
                            options.AddUserProfilePreference(chromePreferences.GetKey(i), false);
                            break;

                        // otherwise
                        default:
                            int temp;

                            // an attempt to parse current settings value to an integer. Method TryParse returns True if the attempt is successful (the string is integer) or return False (if the string is just a string and cannot be cast to a number)
                            if (int.TryParse(chromePreferences.Get(i), out temp))
                            {
                                options.AddUserProfilePreference(chromePreferences.GetKey(i), temp);
                            }
                            else
                            {
                                options.AddUserProfilePreference(chromePreferences.GetKey(i), chromePreferences[i]);
                            }

                            break;
                    }
                }

                return options;
            }
        }

        /// <summary>
        /// Takes the screenshot.
        /// </summary>
        /// <returns>An image of the page currently loaded in the browser.</returns>
        public Screenshot TakeScreenshot()
        {
            try
            {
                var screenshotDriver = (ITakesScreenshot)this.driver;
                var screenshot = screenshotDriver.GetScreenshot();
                return screenshot;
            }
            catch (NullReferenceException)
            {
                Logger.Error("Test failed but was unable to get webdriver screenshot.");
            }
            catch (UnhandledAlertException)
            {
                Logger.Error("Test failed but was unable to get webdriver screenshot.");
            }

            return null;
        }

        /// <summary>
        /// Starts the specified Driver.
        /// </summary>
        /// <exception cref="NotSupportedException">When driver not supported.</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Driver disposed later in stop method")]

        public void Start()
        {
            switch (BaseConfiguration.TestBrowser)
            {
                case BrowserType.Firefox:

                    this.driver = new FirefoxDriver(this.SetDriverOptions(this.FirefoxOptions));
                    break;
                case BrowserType.Chrome:
                    this.serviceChrome = ChromeDriverService.CreateDefaultService();
                    this.driver = new ChromeDriver(this.serviceChrome, this.SetDriverOptions(this.ChromeOptions));
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format(CultureInfo.CurrentCulture, "Driver {0} is not supported", BaseConfiguration.TestBrowser));
            }

            if (BaseConfiguration.EnableEventFiringWebDriver)
            {
                this.driver = new MyEventFiringWebDriver(this.driver);
            }
        }

        /// <summary>
        /// Maximizes the current window if it is not already maximized.
        /// </summary>
        public void WindowMaximize()
        {
            this.driver.Manage().Window.Maximize();
        }

        /// <summary>
        /// Deletes all cookies from the page.
        /// </summary>
        public void DeleteAllCookies()
        {
            this.driver.Manage().Cookies.DeleteAllCookies();
        }

        /// <summary>
        /// Stop browser instance.
        /// </summary>
        public void Stop()
        {
            if (this.serviceChrome != null)
            {
                this.serviceChrome.Dispose();
            }

            if (this.driver != null)
            {
                this.driver.Quit();
            }
        }
    }
}
