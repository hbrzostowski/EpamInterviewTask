// <copyright file="DriverContextHelper.cs" company="Objectivity Bespoke Software Specialists">
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
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Ocaramba.Helpers;
    using Ocaramba.Types;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;

    /// <summary>
    /// Contains handle to driver and methods for web browser.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Driver is disposed on test end")]
    public partial class DriverContext
    {
        /// <summary>
        /// Saves the screenshot.
        /// </summary>
        /// <param name="errorDetail">The error detail.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="title">The title.</param>
        /// <returns>Path to the screenshot.</returns>
        public string SaveScreenshot(ErrorDetail errorDetail, string folder, string title)
        {
            var fileName = string.Format(CultureInfo.CurrentCulture, "{0}_{1}_{2}.png", title, errorDetail.DateTime.ToString("yyyy-MM-dd HH-mm-ss-fff", CultureInfo.CurrentCulture), "browser");
            var correctFileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(CultureInfo.CurrentCulture), string.Empty));
            correctFileName = Regex.Replace(correctFileName, "[^0-9a-zA-Z._]+", "_");
            correctFileName = NameHelper.ShortenFileName(folder, correctFileName, "_", 255);

            var filePath = Path.Combine(folder, correctFileName);

            try
            {
                errorDetail.Screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
                FilesHelper.WaitForFileOfGivenName(BaseConfiguration.ShortTimeout, correctFileName, folder);
                Logger.Error(CultureInfo.CurrentCulture, "Test failed: screenshot saved to {0}.", filePath);
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "##teamcity[publishArtifacts '{0}']", filePath));
                return filePath;
            }
            catch (NullReferenceException)
            {
                Logger.Error("Test failed but was unable to get webdriver screenshot.");
            }

            return null;
        }

        /// <summary>
        /// Saves the page source.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The saved source file.</returns>
        public string SavePageSource(string fileName)
        {
            if (BaseConfiguration.GetPageSourceEnabled)
            {
                var fileNameShort = Regex.Replace(fileName, "[^0-9a-zA-Z._]+", "_");
                fileNameShort = NameHelper.ShortenFileName(this.PageSourceFolder, fileNameShort, "_", 255);
                var fileNameWithExtension = string.Format(CultureInfo.CurrentCulture, "{0}{1}", fileNameShort, ".html");
                var path = Path.Combine(this.PageSourceFolder, fileNameWithExtension);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                var pageSource = this.driver.PageSource;
                pageSource = pageSource.Replace("<head>", string.Format(CultureInfo.CurrentCulture, "<head><base href=\"http://{0}\" target=\"_blank\">", BaseConfiguration.Host));
                File.WriteAllText(path, pageSource);
                FilesHelper.WaitForFileOfGivenName(BaseConfiguration.LongTimeout, fileNameWithExtension, this.PageSourceFolder);
                Logger.Error(CultureInfo.CurrentCulture, "Test failed: page Source saved to {0}.", path);
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "##teamcity[publishArtifacts '{0}']", path));
                return path;
            }

            return null;
        }

        /// <summary>
        /// Takes and saves screen shot.
        /// </summary>
        /// <returns>Array of filepaths.</returns>
        public string[] TakeAndSaveScreenshot()
        {
            List<string> filePaths = new List<string>();

            if (BaseConfiguration.SeleniumScreenShotEnabled)
            {
                filePaths.Add(this.SaveScreenshot(new ErrorDetail(this.TakeScreenshot(), DateTime.Now, null), this.ScreenShotFolder, this.TestTitle));
            }

            return filePaths.ToArray();
        }

        /// <summary>
        /// Sets the driver options.
        /// </summary>
        /// <typeparam name="T">The type of DriverOptions for the specific Browser.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The Driver Options.
        /// </returns>
        private T SetDriverOptions<T>(T options)
            where T : DriverOptions
        {
            this.DriverOptionsSet?.Invoke(this, new DriverOptionsSetEventArgs(options));
            return options;
        }

        private FirefoxOptions AddFirefoxArguments(FirefoxOptions option)
        {
            NameValueCollection firefoxArguments = new NameValueCollection();
#if net47 || net45
            firefoxArguments = ConfigurationManager.GetSection("FirefoxArguments") as NameValueCollection;
#endif
#if netcoreapp3_1
            firefoxArguments = BaseConfiguration.GetNameValueCollectionFromAppsettings("FirefoxArguments");
#endif

            // if there are any arguments
            if (firefoxArguments != null)
            {
                // loop through all of them
                for (var i = 0; i < firefoxArguments.Count; i++)
                {
                    Logger.Trace(CultureInfo.CurrentCulture, "Setting FireFox Arguments {0}", firefoxArguments.GetKey(i));
                    option.AddArgument(firefoxArguments.GetKey(i));
                }
            }

            return option;
        }

        private string GetBrowserDriversFolder(string folder)
        {
#if netcoreapp3_1
            if (string.IsNullOrEmpty(folder))
            {
                folder = this.CurrentDirectory;
                Logger.Trace(CultureInfo.CurrentCulture, "Path to the directory containing driver {0}", folder);
            }
#endif
            return folder;
        }
    }
}
