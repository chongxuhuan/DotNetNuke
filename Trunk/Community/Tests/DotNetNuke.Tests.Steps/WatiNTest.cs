using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

using DotNetNuke.Tests.Instance.Utilities;
using DotNetNuke.Tests.UI.WatiN.Common;

using WatiN.Core;

namespace DotNetNuke.Tests.Steps
{
    public class WatiNTest : DnnUnitTest
    {
        #region Properties

        public const string DBName = "test";

        private static string _winHandler;

        private IE ieInstance;
        private string _siteUrl = ConfigurationManager.AppSettings["SiteURL"];
        private string _physicalPath = ConfigurationManager.AppSettings["DefaultPhysicalAppPath"];
        private string _screenShotPath = ConfigurationManager.AppSettings["ScreenShotPath"];
        private WatiNAssert _watiNAssert;
        private WatiNBase _homePage;

        public IE IEInstance
        {
            get
            {
                if (ieInstance == null)
                {
                    if(!string.IsNullOrEmpty(_winHandler))
                    {
                        ieInstance = IE.AttachTo<IE>(Find.By("hWnd", _winHandler));
                    }
                    else
                    {
                        ieInstance = IE.AttachTo<IE>(Find.ByUrl(new Regex(".*" + _siteUrl + ".*", RegexOptions.IgnoreCase)));
                    }
                }

                return ieInstance;
            }
            set
            {
                ieInstance = value;
                _winHandler = ieInstance.hWnd.ToString();
                _homePage = null;
            }
        }

        public WatiNBase HomePage
        {
            get
            {
                if (_homePage == null)
                {
                    _homePage = new WatiNBase(IEInstance, _siteUrl, DBName);
                }

                return _homePage;
            }
        }

        public string SiteUrl
        {
            get
            {
                return _siteUrl;
            }
        }

        public string PhysicalPath
        {
            get
            {
                return _physicalPath;
            }
        }

        public string ScreenShotPath
        {
            get
            {
                return _screenShotPath;
            }
        }

        public WatiNAssert WatiNAssert
        {
            get
            {
                if (_watiNAssert == null)
                {
                    _watiNAssert = new WatiNAssert(IEInstance, Path.Combine(_screenShotPath, "BaseTests"));
                }

                return _watiNAssert;
            }
        }

        #endregion

        #region Constructors

        public WatiNTest() : this(0)
        {
            
        }

        public WatiNTest(int portalId) : base(portalId)
        {
            
        }

        #endregion

        #region Public Methods

        private IDictionary<string, WatiNBase> _pageInstances; 
        public T GetPage<T>() where T : WatiNBase
        {
            var type = typeof (T);
            if (_pageInstances == null)
            {
                _pageInstances = new Dictionary<string, WatiNBase>();
            }

            if (_pageInstances.ContainsKey(type.Name))
            {
                return _pageInstances[type.Name] as T;
            }

            var instance = Activator.CreateInstance(type, IEInstance, SiteUrl, DBName) as T;
            _pageInstances.Add(type.Name, instance);

            return instance;
        }

        public void WaitAjaxRequestComplete()
        {
            HomePage.ContentPaneDiv.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        #endregion
    }
}
