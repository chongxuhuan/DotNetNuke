#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

#region Usings

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;
using System.Xml.Serialization;

using DotNetNuke.Application;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.Providers;

#endregion

namespace DotNetNuke.Services.Exceptions
{
	/// <summary>
	/// Base Portal Exception.
	/// </summary>
    [Serializable]
    public class BasePortalException : Exception
    {
        private string m_AbsoluteURL;
        private string m_AbsoluteURLReferrer;
        private int m_ActiveTabID;
        private string m_ActiveTabName;
        private string m_AssemblyVersion;
        private string m_DefaultDataProvider;
        private string m_ExceptionGUID;
        private int m_FileColumnNumber;
        private int m_FileLineNumber;
        private string m_FileName;
        private string m_InnerExceptionString;
        private string m_Message;
        private string m_Method;
        private int m_PortalID;
        private string m_PortalName;
        private string m_RawURL;
        private string m_Source;
        private string m_StackTrace;
        private string m_UserAgent;
        private int m_UserID;
        private string m_UserName;

        //default constructor
		public BasePortalException()
        {
        }

        //constructor with exception message
		public BasePortalException(string message) : base(message)
        {
            InitializePrivateVariables();
        }

        //constructor with message and inner exception
        public BasePortalException(string message, Exception inner) : base(message, inner)
        {
            InitializePrivateVariables();
        }

        protected BasePortalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            InitializePrivateVariables();
            m_AssemblyVersion = info.GetString("m_AssemblyVersion");
            m_PortalID = info.GetInt32("m_PortalID");
            m_PortalName = info.GetString("m_PortalName");
            m_UserID = info.GetInt32("m_UserID");
            m_UserName = info.GetString("m_Username");
            m_ActiveTabID = info.GetInt32("m_ActiveTabID");
            m_ActiveTabName = info.GetString("m_ActiveTabName");
            m_RawURL = info.GetString("m_RawURL");
            m_AbsoluteURL = info.GetString("m_AbsoluteURL");
            m_AbsoluteURLReferrer = info.GetString("m_AbsoluteURLReferrer");
            m_UserAgent = info.GetString("m_UserAgent");
            m_DefaultDataProvider = info.GetString("m_DefaultDataProvider");
            m_ExceptionGUID = info.GetString("m_ExceptionGUID");
            m_InnerExceptionString = info.GetString("m_InnerExceptionString");
            m_FileName = info.GetString("m_FileName");
            m_FileLineNumber = info.GetInt32("m_FileLineNumber");
            m_FileColumnNumber = info.GetInt32("m_FileColumnNumber");
            m_Method = info.GetString("m_Method");
            m_StackTrace = info.GetString("m_StackTrace");
            m_Message = info.GetString("m_Message");
            m_Source = info.GetString("m_Source");
        }

        public string AssemblyVersion
        {
            get
            {
                return m_AssemblyVersion;
            }
        }

        public int PortalID
        {
            get
            {
                return m_PortalID;
            }
        }

        public string PortalName
        {
            get
            {
                return m_PortalName;
            }
        }

        public int UserID
        {
            get
            {
                return m_UserID;
            }
        }

        public string UserName
        {
            get
            {
                return m_UserName;
            }
        }

        public int ActiveTabID
        {
            get
            {
                return m_ActiveTabID;
            }
        }

        public string ActiveTabName
        {
            get
            {
                return m_ActiveTabName;
            }
        }

        public string RawURL
        {
            get
            {
                return m_RawURL;
            }
        }

        public string AbsoluteURL
        {
            get
            {
                return m_AbsoluteURL;
            }
        }

        public string AbsoluteURLReferrer
        {
            get
            {
                return m_AbsoluteURLReferrer;
            }
        }

        public string UserAgent
        {
            get
            {
                return m_UserAgent;
            }
        }

        public string DefaultDataProvider
        {
            get
            {
                return m_DefaultDataProvider;
            }
        }

        public string ExceptionGUID
        {
            get
            {
                return m_ExceptionGUID;
            }
        }

        public string FileName
        {
            get
            {
                return m_FileName;
            }
        }

        public int FileLineNumber
        {
            get
            {
                return m_FileLineNumber;
            }
        }

        public int FileColumnNumber
        {
            get
            {
                return m_FileColumnNumber;
            }
        }

        public string Method
        {
            get
            {
                return m_Method;
            }
        }

        [XmlIgnore]
        public new MethodBase TargetSite
        {
            get
            {
                return base.TargetSite;
            }
        }

        private void InitializePrivateVariables()
        {
			//Try and get the Portal settings from context
            //If an error occurs getting the context then set the variables to -1
            try
            {
                var context = HttpContext.Current;
                var portalSettings = PortalController.GetCurrentPortalSettings();
                var innerException = new Exception(Message, this);
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }
                var exceptionInfo = Exceptions.GetExceptionInfo(innerException);

                m_AssemblyVersion = DotNetNukeContext.Current.Application.Version.ToString(3);
                if (portalSettings != null)
                {
                    m_PortalID = portalSettings.PortalId;
                    m_PortalName = portalSettings.PortalName;
                    m_ActiveTabID = portalSettings.ActiveTab.TabID;
                    m_ActiveTabName = portalSettings.ActiveTab.TabName;
                }
                else
                {
                    m_PortalID = -1;
                    m_PortalName = "";
                    m_ActiveTabID = -1;
                    m_ActiveTabName = "";
                }

                var currentUserInfo = UserController.GetCurrentUserInfo();
                m_UserID = (currentUserInfo != null) ? currentUserInfo.UserID : -1;

                if (m_UserID != -1)
                {
                    currentUserInfo = UserController.GetUserById(m_PortalID, m_UserID);
                    m_UserName = currentUserInfo != null ? currentUserInfo.Username : "";
                }
                else
                {
                    m_UserName = "";
                }

                if (context != null)
                {
                    m_RawURL = context.Request.RawUrl;
                    m_AbsoluteURL = context.Request.Url.AbsolutePath;
                    if (context.Request.UrlReferrer != null)
                    {
                        m_AbsoluteURLReferrer = context.Request.UrlReferrer.AbsoluteUri;
                    }
                    m_UserAgent = context.Request.UserAgent;
                }
                else
                {
                    m_RawURL = "";
                    m_AbsoluteURL = "";
                    m_AbsoluteURLReferrer = "";
                    m_UserAgent = "";
                }
                try
                {
                    ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration("data");
                    string strTypeName = ((Provider)objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider]).Type;
                    m_DefaultDataProvider = strTypeName;
                    
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);

                    m_DefaultDataProvider = "";
                }

                m_ExceptionGUID = Guid.NewGuid().ToString();

                if (exceptionInfo != null)
                {
                    m_FileName = exceptionInfo.FileName;
                    m_FileLineNumber = exceptionInfo.FileLineNumber;
                    m_FileColumnNumber = exceptionInfo.FileColumnNumber;
                    m_Method = exceptionInfo.Method;
                }
                else
                {
                    m_FileName = "";
                    m_FileLineNumber = -1;
                    m_FileColumnNumber = -1;
                    m_Method = "";
                }

                try
                {
                    m_StackTrace = StackTrace;
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);

                    m_StackTrace = "";
                }
                try
                {
                    m_Message = Message;
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);

                    m_Message = "";
                }
                try
                {
                    m_Source = Source;
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);

                    m_Source = "";
                }
            }
            catch (Exception exc)
            {
                m_PortalID = -1;
                m_UserID = -1;
                m_AssemblyVersion = "-1";
                m_ActiveTabID = -1;
                m_ActiveTabName = "";
                m_RawURL = "";
                m_AbsoluteURL = "";
                m_AbsoluteURLReferrer = "";
                m_UserAgent = "";
                m_DefaultDataProvider = "";
                m_ExceptionGUID = "";
                m_FileName = "";
                m_FileLineNumber = -1;
                m_FileColumnNumber = -1;
                m_Method = "";
                m_StackTrace = "";
                m_Message = "";
                m_Source = "";
                Instrumentation.DnnLog.Error(exc);

            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
			//Serialize this class' state and then call the base class GetObjectData
            info.AddValue("m_AssemblyVersion", m_AssemblyVersion, typeof (string));
            info.AddValue("m_PortalID", m_PortalID, typeof (Int32));
            info.AddValue("m_PortalName", m_PortalName, typeof (string));
            info.AddValue("m_UserID", m_UserID, typeof (Int32));
            info.AddValue("m_UserName", m_UserName, typeof (string));
            info.AddValue("m_ActiveTabID", m_ActiveTabID, typeof (Int32));
            info.AddValue("m_ActiveTabName", m_ActiveTabName, typeof (string));
            info.AddValue("m_RawURL", m_RawURL, typeof (string));
            info.AddValue("m_AbsoluteURL", m_AbsoluteURL, typeof (string));
            info.AddValue("m_AbsoluteURLReferrer", m_AbsoluteURLReferrer, typeof (string));
            info.AddValue("m_UserAgent", m_UserAgent, typeof (string));
            info.AddValue("m_DefaultDataProvider", m_DefaultDataProvider, typeof (string));
            info.AddValue("m_ExceptionGUID", m_ExceptionGUID, typeof (string));
            info.AddValue("m_FileName", m_FileName, typeof (string));
            info.AddValue("m_FileLineNumber", m_FileLineNumber, typeof (Int32));
            info.AddValue("m_FileColumnNumber", m_FileColumnNumber, typeof (Int32));
            info.AddValue("m_Method", m_Method, typeof (string));
            info.AddValue("m_StackTrace", m_StackTrace, typeof (string));
            info.AddValue("m_Message", m_Message, typeof (string));
            info.AddValue("m_Source", m_Source, typeof (string));
            base.GetObjectData(info, context);
        }
    }
}