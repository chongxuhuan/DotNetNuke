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
#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Web.Compilation;

using log4net.Config;

#endregion

namespace DotNetNuke.Instrumentation
{
    public static class DnnLog
    {
        private const string ConfigFile = "DotNetNuke.log4net.config";
        private static bool _configured;
        private static readonly object ConfigLock = new object();

        private static StackFrame CallingFrame
        {
            get
            {
                StackFrame frame = null;
                var stack = new StackTrace().GetFrames();

                int frameDepth = 0;
                if (stack != null)
                {
                    var reflectedType = stack[frameDepth].GetMethod().ReflectedType;
                    while (reflectedType == BuildManager.GetType("DotNetNuke.Services.Exceptions.Exceptions", false) ||
                           reflectedType == typeof(DnnLogger) || reflectedType == typeof(DnnLog))
                    {
                        frameDepth++;
                        reflectedType = stack[frameDepth].GetMethod().ReflectedType;
                    }
                    frame = stack[frameDepth];
                }
                return frame;
            }
        }

        private static Type CallingType
        {
            get
            {
                return CallingFrame.GetMethod().DeclaringType;
            }
        }

        private static void EnsureConfig()
        {
            lock (ConfigLock)
            {
                if (!_configured)
                {
                    var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFile);
                    if (File.Exists(configPath))
                    {
                        XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
                    }
                    _configured = true;
                }
            }
        }

        public static void SetupThreadContext()
        {
            //if (HttpContext.Current != null && HttpContext.Current.Request != null)
            //{
            //    ThreadContext.Properties["Request.URL"] = HttpContext.Current.Request.RawUrl;
            //    ThreadContext.Properties["Request.FilePath"] = HttpContext.Current.Request.FilePath;
            //    ThreadContext.Properties["Request.HttpMethod"] = HttpContext.Current.Request.HttpMethod;
            //    ThreadContext.Properties["Request.IsSecureConnection"] = HttpContext.Current.Request.IsSecureConnection;
            //    ThreadContext.Properties["Request.UserAgent"] = HttpContext.Current.Request.UserAgent;

            //    if (HttpContext.Current.User != null)
            //    {
            //        ThreadContext.Properties["UserIdentity"] = HttpContext.Current.User.Identity.Name;
            //    }

            //    //Entities.Portals.PortalSettings portal = Entities.Portals.PortalSettings.Current;
            //    //if (portal != null)
            //    //{
            //    //    ThreadContext.Properties["Portal.CultureCode"] = portal.CultureCode;
            //    //    ThreadContext.Properties["Portal.GUID"] = portal.GUID;
            //    //    ThreadContext.Properties["Portal.PortalID"] = portal.PortalId;
            //    //    ThreadContext.Properties["Portal.UesrID"] = portal.UserId;
            //    //    ThreadContext.Properties["Portal.Version"] = portal.Version;
            //    //}
            //}
        }

        /// <summary>
        ///   Standard method to use on method entry
        /// </summary>
        public static void MethodEntry()
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);

            if (log.Logger.IsEnabledFor(DnnLogger.LevelTrace))
            {
                SetupThreadContext();
                log.TraceFormat("Entering Method [{0}]", CallingFrame.GetMethod().Name);
            }
        }

        /// <summary>
        ///   Standard method to use on method exit
        /// </summary>
        public static void MethodExit(object returnObject)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelTrace))
            {
                if (returnObject == null)
                {
                    returnObject = "NULL";
                }

                SetupThreadContext();
                log.TraceFormat("Method [{0}] Returned [{1}]", CallingFrame.GetMethod().Name, returnObject);
            }
        }

        /// <summary>
        ///   Standard method to use on method exit
        /// </summary>
        public static void MethodExit()
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelTrace))
            {

                SetupThreadContext();
                log.TraceFormat("Method [{0}] Returned", CallingFrame.GetMethod().Name);
            }
        }

        #region Trace

        public static void Trace(string message)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelTrace))
            {

                SetupThreadContext();
                log.TraceFormat(message);
            }
        }

        public static void Trace(string format, params object[] args)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelTrace))
            {

                SetupThreadContext();
                log.TraceFormat(format, args);
            }
        }

        public static void Trace(IFormatProvider provider, string format, params object[] args)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelTrace))
            {

                SetupThreadContext();
                log.TraceFormat(provider, format, args);
            }
        }

        #endregion

        #region Debug

        public static void Debug(object message)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelDebug))
            {
                SetupThreadContext();
                log.Debug(message);
            }
        }

        public static void Debug(string format, params object[] args)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelDebug))
            {
                SetupThreadContext();
                log.DebugFormat(format, args);
            }
        }

        public static void Debug(IFormatProvider provider, string format, params object[] args)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelDebug))
            {
                SetupThreadContext();
                log.DebugFormat(provider, format, args);
            }
        }

        #endregion

        #region Info

        public static void Info(object message)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelInfo))
            {
                SetupThreadContext();
                log.Info(message);
            }
        }

        public static void Info(IFormatProvider provider, string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelInfo))
            {
                SetupThreadContext();
                log.InfoFormat(provider, format, args);
            }
        }

        public static void Info(string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelInfo))
            {
                SetupThreadContext();
                log.InfoFormat(format, args);
            }
        }

        #endregion

        #region Warn

        public static void Warn(string message, Exception exception)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelWarn))
            {
                SetupThreadContext();
                log.Warn(message, exception);
            }
        }

        public static void Warn(object message)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelWarn))
            {
                SetupThreadContext();
                log.Warn(message);
            }
        }

        public static void Warn(IFormatProvider provider, string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelWarn))
            {
                SetupThreadContext();
                log.WarnFormat(provider, format, args);
            }
        }


        public static void Warn(string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelWarn))
            {
                SetupThreadContext();
                log.WarnFormat(format, args);
            }
        }

        #endregion

        #region Error

        public static void Error(string message, Exception exception)
        {
            EnsureConfig();

            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelError))
            {
                SetupThreadContext();
                log.Error(message, exception);
            }
        }

        public static void Error(object message)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelError))
            {
                SetupThreadContext();
                log.Error(message);
            }
        }

        public static void Error(Exception exception)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelError))
            {
                SetupThreadContext();
                log.Error(exception.Message, exception);
            }
        }

        public static void Error(IFormatProvider provider, string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelError))
            {
                SetupThreadContext();
                log.ErrorFormat(provider, format, args);
            }
        }

        public static void Error(string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelError))
            {
                SetupThreadContext();
                log.ErrorFormat(format, args);
            }
        }

        #endregion

        #region Fatal

        public static void Fatal(string message, Exception exception)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelFatal))
            {
                SetupThreadContext();
                log.Fatal(message, exception);
            }
        }

        public static void Fatal(object message)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelFatal))
            {
                SetupThreadContext();
                log.Fatal(message);
            }
        }

        public static void Fatal(IFormatProvider provider, string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelFatal))
            {
                SetupThreadContext();
                log.FatalFormat(provider, format, args);
            }
        }

        public static void Fatal(string format, params object[] args)
        {
            EnsureConfig();
            var log = DnnLogger.GetClassLogger(CallingType);
            if (log.Logger.IsEnabledFor(DnnLogger.LevelFatal))
            {
                SetupThreadContext();
                log.FatalFormat(format, args);
            }
        }

        #endregion
    }
}