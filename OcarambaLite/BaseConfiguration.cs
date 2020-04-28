// <copyright file="BaseConfiguration.cs" company="Objectivity Bespoke Software Specialists">
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
    using Microsoft.Extensions.Configuration;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;

    /// <summary>
    /// SeleniumConfiguration that consume settings file file <see href="https://github.com/ObjectivityLtd/Ocaramba/wiki/Description%20of%20settings file%20file">More details on wiki</see>.
    /// </summary>
    public static class BaseConfiguration
    {
        /// <summary>
        /// The logger.
        /// </summary>
        public static readonly string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        /// <summary>
        /// Getting appsettings.json file.
        /// </summary>
        public static readonly IConfigurationRoot Builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{Env}.json", true, true)
            .Build();

        private static readonly ILogger Logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// Gets the Driver.
        /// </summary>
        /// <example>How to use it: <code>
        /// if (BaseConfiguration.TestBrowser == BrowserType.Firefox)
        ///     {
        ///     this.Driver.GetElement(this.fileLink.Format(fileName), "Click on file").Click();
        ///     };
        /// </code></example>
        public static BrowserType TestBrowser
        {
            get
            {
                string setting = Builder["appSettings:browser"];

                Logger.Trace(CultureInfo.CurrentCulture, "Browser value from settings file '{0}'", setting);
                bool supportedBrowser = Enum.TryParse(setting, out BrowserType browserType);
                if (supportedBrowser)
                {
                    return browserType;
                }

                return BrowserType.None;
            }
        }

        /// <summary>
        /// Gets the path to firefox profile.
        /// </summary>
        public static string PathToFirefoxProfile
        {
            get
            {
                string setting = Builder["appSettings:PathToFirefoxProfile"];

                Logger.Trace(CultureInfo.CurrentCulture, "Gets the path to firefox profile from settings file '{0}'", setting);
                if (string.IsNullOrEmpty(setting))
                {
                    return string.Empty;
                }

                return setting;
            }
        }

        /// <summary>
        /// Gets the application protocol (http or https).
        /// </summary>
        public static string Protocol
        {
            get
            {
                string setting = Builder["appSettings:protocol"];

                Logger.Trace(CultureInfo.CurrentCulture, "Gets the protocol from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the application host name.
        /// </summary>
        public static string Host
        {
            get
            {
                string setting = Builder["appSettings:host"];

                Logger.Trace(CultureInfo.CurrentCulture, "Gets the protocol from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public static string Url
        {
            get
            {
                string setting = Builder["appSettings:url"];

                Logger.Trace(CultureInfo.CurrentCulture, "Gets the url from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the java script or ajax waiting time [seconds].
        /// </summary>
        /// <example>How to use it: <code>
        /// this.Driver.IsElementPresent(this.statusCodeHeader, BaseConfiguration.MediumTimeout);
        /// </code></example>
        public static double MediumTimeout
        {
            get
            {
                double setting = Convert.ToDouble(Builder["appSettings:mediumTimeout"]);

                Logger.Trace(CultureInfo.CurrentCulture, "Gets the mediumTimeout from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the page load waiting time [seconds].
        /// </summary>
        /// <example>How to use it: <code>
        /// element.GetElement(locator, BaseConfiguration.LongTimeout, e => e.Displayed, customMessage);
        /// </code></example>
        public static double LongTimeout
        {
            get
            {
                double setting = Convert.ToDouble(Builder["appSettings:longTimeout"]);

                Logger.Trace(CultureInfo.CurrentCulture, "Gets the longTimeout from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the assertion waiting time [seconds].
        /// </summary>
        /// <example>How to use it: <code>
        /// this.Driver.IsElementPresent(this.downloadPageHeader, BaseConfiguration.ShortTimeout);
        /// </code></example>
        public static double ShortTimeout
        {
            get
            {
                double setting = Convert.ToDouble(Builder["appSettings:shortTimeout"]);

                Logger.Trace(CultureInfo.CurrentCulture, "Gets the shortTimeout from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets a value indicating whether enable full desktop screen shot. True by default.
        /// </summary>
        public static bool SeleniumScreenShotEnabled
        {
            get
            {
                string setting = Builder["appSettings:SeleniumScreenShotEnabled"];

                Logger.Trace(CultureInfo.CurrentCulture, "Selenium Screen Shot Enabled value from settings file '{0}'", setting);
                if (string.IsNullOrEmpty(setting))
                {
                    return true;
                }

                if (setting.ToLower(CultureInfo.CurrentCulture).Equals("true"))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether enable EventFiringWebDriver.
        /// </summary>
        public static bool EnableEventFiringWebDriver
        {
            get
            {
                string setting = Builder["appSettings:EnableEventFiringWebDriver"];

                Logger.Trace(CultureInfo.CurrentCulture, "Enable EventFiringWebDriver from settings file '{0}'", setting);
                if (string.IsNullOrEmpty(setting))
                {
                    return false;
                }

                if (setting.ToLower(CultureInfo.CurrentCulture).Equals("true"))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether use CurrentDirectory for path where assembly files are located.
        /// </summary>
        public static bool UseCurrentDirectory
        {
            get
            {
                string setting = Builder["appSettings:UseCurrentDirectory"];

                Logger.Trace(CultureInfo.CurrentCulture, "Use Current Directory value from settings file '{0}'", setting);
                if (string.IsNullOrEmpty(setting))
                {
                    return false;
                }

                if (setting.ToLower(CultureInfo.CurrentCulture).Equals("true"))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [get page source enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [get page source enabled]; otherwise, <c>false</c>.
        /// </value>
        public static bool GetPageSourceEnabled
        {
            get
            {
                string setting = Builder["appSettings:GetPageSourceEnabled"];

                Logger.Trace(CultureInfo.CurrentCulture, "Get Page Source Enabled value from settings file '{0}'", setting);
                if (string.IsNullOrEmpty(setting))
                {
                    return true;
                }

                if (setting.ToLower(CultureInfo.CurrentCulture).Equals("true"))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the download folder key value.
        /// </summary>
        public static string DownloadFolder
        {
            get
            {
                string setting = Builder["appSettings:DownloadFolder"];

                Logger.Trace(CultureInfo.CurrentCulture, "Get DownloadFolder value from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the screen shot folder key value.
        /// </summary>
        public static string ScreenShotFolder
        {
            get
            {
                string setting = Builder["appSettings:ScreenShotFolder"];

                Logger.Trace(CultureInfo.CurrentCulture, "Get ScreenShotFolder value from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the page source folder key value.
        /// </summary>
        public static string PageSourceFolder
        {
            get
            {
                string setting = Builder["appSettings:PageSourceFolder"];

                Logger.Trace(CultureInfo.CurrentCulture, "Get PageSourceFolder value from settings file '{0}'", setting);
                return setting;
            }
        }

        /// <summary>
        /// Gets the URL value 'Protocol://HostURL'.
        /// </summary>
        /// <example>How to use it: <code>
        /// var url = BaseConfiguration.GetUrlValue;
        /// </code></example>
        public static string GetUrlValue
        {
            get
            {
                Logger.Trace(CultureInfo.CurrentCulture, "Get Url value from settings file '{0}://{1}{2}'", Protocol, Host, Url);
                return string.Format(CultureInfo.CurrentCulture, "{0}://{1}{2}", Protocol, Host, Url);
            }
        }

        /// <summary>
        /// Converting settings from appsettings.json into the NameValueCollection, key - value pairs.
        /// </summary>
        /// <param name="preferences">Section name in appsettings.json file.</param>
        /// <returns>Settings.</returns>
        public static NameValueCollection GetNameValueCollectionFromAppsettings(string preferences)
        {
            NameValueCollection preferencesCollection = new NameValueCollection();
            var jsnonSettings = Builder.GetSection(preferences).Get<Dictionary<string, string>>();
            if (jsnonSettings == null)
            {
                return preferencesCollection;
            }

            foreach (var kvp in jsnonSettings)
            {
                string value = null;
                if (kvp.Value != null)
                {
                    value = kvp.Value.ToString();
                }

                preferencesCollection.Add(kvp.Key.ToString(), value);
            }

            return preferencesCollection;
        }
    }
}