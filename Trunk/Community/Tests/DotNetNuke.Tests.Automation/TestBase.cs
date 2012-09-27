#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System.Configuration;
using System;
using System.IO;

namespace DotNetNuke.Tests.UI.WatiN.Common
{
    public class TestBase
    {
        #region Variables
        protected readonly bool AutoCloseIE = Boolean.Parse(ConfigurationManager.AppSettings["AutoCloseIE"]);
        protected readonly bool AutoMovePointer = Boolean.Parse(ConfigurationManager.AppSettings["AutoMovePointer"]);
        protected readonly int IETimeOut = Int32.Parse(ConfigurationManager.AppSettings["IETimeOut"]);
        protected readonly bool SilentMode = Boolean.Parse(ConfigurationManager.AppSettings["SilentMode"]);
        protected bool ScreenCaptureEnabled = Boolean.Parse(ConfigurationManager.AppSettings["ScreenCaptureEnabled"]);
        protected bool VideoEnabled = Boolean.Parse(ConfigurationManager.AppSettings["VideoEnabled"]);

        //Set application wide variables that cannot be changed by inheriting class.
        protected readonly double VideoZoom = Double.Parse(ConfigurationManager.AppSettings["VideoZoom"]);
        protected readonly int VideoFramesPerSec = Int32.Parse(ConfigurationManager.AppSettings["VideoFramesPerSec"]);
        protected readonly int CaptionFontSize = Int32.Parse(ConfigurationManager.AppSettings["CaptionFontSize"]);
        #endregion

        #region Properties
        protected virtual string SiteURL { get { return TestEnvironment.SiteUrl; } }
        //protected TestContext TestContext { get { return TestContext.CurrentContext; } }

        protected string LanguagePath
        {
            get
            {
                return TestEnvironment.LanguagePath;
            }
        }

        protected string LocaleName
        {
            get
            {
                return TestEnvironment.LocaleName;
            }
        }

        protected string EnglishLocaleName
        {
            get
            {
                return TestEnvironment.EnglishLocaleName;
            }
        }

        protected string LanguageCode
        {
            get
            {
                return TestEnvironment.LanguageCode;
            }
        }

        protected string TestEmailPath
        {
            get
            {
                return TestEnvironment.TestEmailPath;
            }
        }

        protected  string TestFilesPath
        {
            get { return TestEnvironment.TestFilesPath; }
        }

        protected string TestVersion
        {
            get { return ConfigurationManager.AppSettings["TestVersion"]; }
        }

        protected string VersionType
        {
            get { return ConfigurationManager.AppSettings["VersionType"]; }
        }

        protected string ExtLanguagePath
        {
            get
            {
                return TestEnvironment.ExtensionLanguagePath;
            }
        }

        protected string ImagePath
        {
            get
            {
                return Path.Combine(TestEnvironment.TestFilesPath, TestEnvironment.ImagePath);
            }
        }

        protected string ImageName
        {
            get
            {
                return TestEnvironment.ImagePath;
            }
        }

        protected string SkinPath
        {
            get
            {
                return Path.Combine(TestEnvironment.TestFilesPath, TestEnvironment.SkinPath);
            }
        }

        protected string SkinName
        {
            get
            {
                return TestEnvironment.SkinName;
            }
        }

        protected string SkinUsed
        {
            get
            {
                return TestEnvironment.SkinUsed;
            }
        }

        protected string ContainerUsed
        {
            get
            {
                return TestEnvironment.ContainerUsed;
            }
        }
        #endregion

        #region Helper Methods
        public static string GetDNNUrlWithPage(string page)
        {
            if (!TestEnvironment.SiteUrl.EndsWith("/") && !page.StartsWith("/")) { page = "/" + page; }
            if (TestEnvironment.SiteUrl.EndsWith("/") && page.StartsWith("/")) { page = page.Substring(1); }

            return String.Concat(TestEnvironment.SiteUrl, page);
        }
        #endregion

        #region Video / Screenshot methods
        //public void StartAutoVideoCapture(string videoName, string videoMessage, TriggerEvent triggerEvent)
        //{
        //    if (!SilentMode && VideoEnabled)
        //    {
        //        Capture.SetCaptionAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
        //        Capture.SetCaptionFontSize(CaptionFontSize);
        //        Capture.SetCaption(videoMessage);
        //        Capture.AutoEmbedRecording(triggerEvent, videoName, new CaptureParameters { Zoom = VideoZoom }, VideoFramesPerSec);
        //    }
        //}

        //public ScreenRecorder StartVideoCapture(string videoMessage)
        //{
        //    if (!SilentMode && VideoEnabled)
        //    {
        //        Capture.SetCaptionAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
        //        Capture.SetCaptionFontSize(CaptionFontSize);
        //        Capture.SetCaption(videoMessage);
        //        return Capture.StartRecording(new CaptureParameters { Zoom = VideoZoom }, VideoFramesPerSec);
        //    }

        //    return null;
        //}

        //public void StopVideoCapture(ScreenRecorder screenRecorder, string videoName)
        //{
        //    if (screenRecorder != null && (!SilentMode && VideoEnabled))
        //    {
        //        screenRecorder.Stop();
        //        TestLog.EmbedVideo(videoName, screenRecorder.Video);
        //    }
        //}

        //public void StartScreenCapture(string screenshotName, string screenshotMessage, TriggerEvent triggerEvent)
        //{
        //    if (!SilentMode && ScreenCaptureEnabled)
        //    {
        //        Capture.SetCaptionAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
        //        Capture.SetCaptionFontSize(CaptionFontSize);
        //        Capture.SetCaption(screenshotMessage);
        //        Capture.AutoEmbedScreenshot(triggerEvent, screenshotName, new CaptureParameters { Zoom = VideoZoom });
        //    }
        //}
        #endregion
    }
}