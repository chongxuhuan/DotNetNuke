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
using System.Globalization;
using System.IO;
using System.Web.Configuration;
using System.Xml;
using System.Xml.XPath;

using DotNetNuke.Framework.Providers;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

#endregion

namespace DotNetNuke.Common.Utilities
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Config class provides access to the web.config file
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2005	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class Config
    {
        #region ConfigFileType enum

        public enum ConfigFileType
        {
            DotNetNuke,
            //compatible with glbDotNetNukeConfig
            SiteAnalytics,
            Compression,
            SiteUrls,
            SolutionsExplorer
        }

        #endregion

        public static XmlDocument AddAppSetting(XmlDocument xmlDoc, string key, string value)
        {
            XmlElement xmlElement;
            //retrieve the appSettings node 
            XmlNode xmlAppSettings = xmlDoc.SelectSingleNode("//appSettings");
            if (xmlAppSettings != null)
            {
                //get the node based on key
                XmlNode xmlNode = xmlAppSettings.SelectSingleNode(("//add[@key='" + key + "']"));
                if (xmlNode != null)
                {
                    //update the existing element
                    xmlElement = (XmlElement)xmlNode;
                    xmlElement.SetAttribute("value", value);
                }
                else
                {
					//create a new element
                    xmlElement = xmlDoc.CreateElement("add");
                    xmlElement.SetAttribute("key", key);
                    xmlElement.SetAttribute("value", value);
                    xmlAppSettings.AppendChild(xmlElement);
                }
            }
			//return the xml doc
            return xmlDoc;
        }

        public static void AddCodeSubDirectory(string name)
        {
            XmlDocument xmlConfig = Load();
            XmlNode xmlCompilation = xmlConfig.SelectSingleNode("configuration/system.web/compilation");
            if (xmlCompilation == null)
            {
				//Try location node
                xmlCompilation = xmlConfig.SelectSingleNode("configuration/location/system.web/compilation");
            }
            //Get the CodeSubDirectories Node
            XmlNode xmlSubDirectories = xmlCompilation.SelectSingleNode("codeSubDirectories");
            if (xmlSubDirectories == null)
            {
				//Add Node
                xmlSubDirectories = xmlConfig.CreateElement("codeSubDirectories");
                xmlCompilation.AppendChild(xmlSubDirectories);
            }
            //Check if the node is already present
            XmlNode xmlSubDirectory = xmlSubDirectories.SelectSingleNode("add[@directoryName='" + name + "']");
            if (xmlSubDirectory == null)
            {
				//Add Node
                xmlSubDirectory = xmlConfig.CreateElement("add");
                XmlUtils.CreateAttribute(xmlConfig, xmlSubDirectory, "directoryName", name);
                xmlSubDirectories.AppendChild(xmlSubDirectory);
            }
            Save(xmlConfig);
        }

        public static void BackupConfig()
        {
            string backupFolder = Globals.glbConfigFolder + "Backup_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + "\\";
            //save the current config files
			try
            {
                if (!Directory.Exists(Globals.ApplicationMapPath + backupFolder))
                {
                    Directory.CreateDirectory(Globals.ApplicationMapPath + backupFolder);
                }
                if (File.Exists(Globals.ApplicationMapPath + "\\web.config"))
                {
                    File.Copy(Globals.ApplicationMapPath + "\\web.config", Globals.ApplicationMapPath + backupFolder + "web_old.config", true);
                }
            }
            catch (Exception e)
            {
                Exceptions.LogException(e);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the default connection String as specified in the provider.
        /// </summary>
        /// <returns>The connection String</returns>
        /// <remarks></remarks>
        /// <history>
        ///		[cnurse]	11/15/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetConnectionString()
        {
            return GetConnectionString(GetDefaultProvider("data").Attributes["connectionStringName"]);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the specified connection String
        /// </summary>
        /// <param name="name">Name of Connection String to return</param>
        /// <returns>The connection String</returns>
        /// <remarks></remarks>
        /// <history>
        ///		[cnurse]	11/15/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetConnectionString(string name)
        {
            string connectionString = "";
			//First check if connection string is specified in <connectionstrings> (ASP.NET 2.0 / DNN v4.x)
            if (!String.IsNullOrEmpty(name))
            {
                //ASP.NET 2 version connection string (in <connectionstrings>)
                //This will be for new v4.x installs or upgrades from v4.x
                connectionString = WebConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            if (String.IsNullOrEmpty(connectionString))
            {
                if (!String.IsNullOrEmpty(name))
                {
					//Next check if connection string is specified in <appsettings> (ASP.NET 1.1 / DNN v3.x)
                	//This will accomodate upgrades from v3.x
                    connectionString = GetSetting(name);
                }
            }
            return connectionString;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Returns the decryptionkey from webconfig machinekey
        /// </summary>
        /// <returns>decryption key</returns>
        /// -----------------------------------------------------------------------------
        public static string GetDecryptionkey()
        {
            var configNav = Load();
            var httpNode = configNav.SelectSingleNode("configuration//system.web//machineKey").CreateNavigator();

            var result = XmlUtils.GetAttributeValue(httpNode, "decryptionKey");

            return result;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Returns the maximum file size allowed to be uploaded to the application in bytes
        /// </summary>
        /// <returns>Size in bytes</returns>
        /// -----------------------------------------------------------------------------
        public static long GetMaxUploadSize()
        {
            var configNav = Load();
            var httpNode = configNav.SelectSingleNode("configuration//system.web//httpRuntime").CreateNavigator();
            //'httpNode.Attributes("maxRequestLength")
            //'Return 0

            var result = XmlUtils.GetAttributeValueAsLong(httpNode, "maxRequestLength", 0);

            return result * 1024;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the specified upgrade connection string
        /// </summary>
        /// <returns>The connection String</returns>
        /// <remarks></remarks>
        /// <history>
        ///		[smehaffie]	07/13/2008	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetUpgradeConnectionString()
        {
            return GetDefaultProvider("data").Attributes["upgradeConnectionString"];
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the specified database owner
        /// </summary>
        /// <returns>The database owner</returns>
        /// <remarks></remarks>
        /// <history>
        ///		[cnurse]	02/13/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetDataBaseOwner()
        {
            string databaseOwner = GetDefaultProvider("data").Attributes["databaseOwner"];
            if (!String.IsNullOrEmpty(databaseOwner) && databaseOwner.EndsWith(".") == false)
            {
                databaseOwner += ".";
            }
            return databaseOwner;
        }

        public static Provider GetDefaultProvider(string type)
        {
            ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration(type);
            //Read the configuration specific information for this provider
            return (Provider)providerConfiguration.Providers[providerConfiguration.DefaultProvider];
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the specified object qualifier
        /// </summary>
        /// <returns>The object qualifier</returns>
        /// <remarks></remarks>
        /// <history>
        ///		[cnurse]	02/13/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetObjectQualifer()
        {
            Provider provider = GetDefaultProvider("data");
            string objectQualifier = provider.Attributes["objectQualifier"];
            if (!String.IsNullOrEmpty(objectQualifier) && objectQualifier.EndsWith("_") == false)
            {
                objectQualifier += "_";
            }
            return objectQualifier;
        }

        public static int GetAuthCookieTimeout()
        {
            XPathNavigator configNav = Load().CreateNavigator();
            //Select the location node
            XPathNavigator locationNav = configNav.SelectSingleNode("configuration/location");
            XPathNavigator formsNav;
            //Test for the existence of the location node if it exists then include that in the nodes of the XPath Query
            if (locationNav == null)
            {
                formsNav = configNav.SelectSingleNode("configuration/system.web/authentication/forms");
            }
            else
            {
                formsNav = configNav.SelectSingleNode("configuration/location/system.web/authentication/forms");
            }
            return (formsNav != null) ? XmlUtils.GetAttributeValueAsInteger(formsNav, "timeout", 30) : 30; ;
            
        }

        /// <summary>
        ///   Get's optional persistent cookie timeout value from web.config
        /// </summary>
        /// <returns>persistent cookie value</returns>
        /// <remarks>
        ///   allows users to override default asp.net values
        /// </remarks>
        public static int GetPersistentCookieTimeout()
        {
            int persistentCookieTimeout = 0;
            if (!String.IsNullOrEmpty(GetSetting("PersistentCookieTimeout")))
            {
                persistentCookieTimeout = int.Parse(GetSetting("PersistentCookieTimeout"));
            }

            return (persistentCookieTimeout == 0) ? GetAuthCookieTimeout() : persistentCookieTimeout;
        }

        public static Provider GetProvider(string type, string name)
        {
            ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration(type);
            //Read the configuration specific information for this provider
            return (Provider)providerConfiguration.Providers[name];
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the specified provider path
        /// </summary>
        /// <returns>The provider path</returns>
        /// <remarks></remarks>
        /// <history>
        ///		[cnurse]	02/13/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetProviderPath(string type)
        {
            Provider objProvider = GetDefaultProvider(type);
            string providerPath = objProvider.Attributes["providerPath"];
            return providerPath;
        }

        public static string GetSetting(string setting)
        {
            return WebConfigurationManager.AppSettings[setting];
        }

        public static object GetSection(string section)
        {
            return WebConfigurationManager.GetWebApplicationSection(section);
        }

        public static XmlDocument Load()
        {
            return Load("web.config");
        }

        public static XmlDocument Load(string filename)
        {
			//open the config file
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Globals.ApplicationMapPath + "\\" + filename);
			//test for namespace added by Web Admin Tool
            if (!String.IsNullOrEmpty(xmlDoc.DocumentElement.GetAttribute("xmlns")))
            {
				//remove namespace
                string strDoc = xmlDoc.InnerXml.Replace("xmlns=\"http://schemas.microsoft.com/.NetConfiguration/v2.0\"", "");
                xmlDoc.LoadXml(strDoc);
            }
            return xmlDoc;
        }

        public static void RemoveCodeSubDirectory(string name)
        {
            XmlDocument xmlConfig = Load();
            //Select the location node
            XmlNode xmlCompilation = xmlConfig.SelectSingleNode("configuration/system.web/compilation");
            if (xmlCompilation == null)
            {
				//Try location node
                xmlCompilation = xmlConfig.SelectSingleNode("configuration/location/system.web/compilation");
            }
            //Get the CodeSubDirectories Node
            XmlNode xmlSubDirectories = xmlCompilation.SelectSingleNode("codeSubDirectories");
            if (xmlSubDirectories == null)
            {
				//Parent doesn't exist so subDirectory node can't exist
                return;
            }
            //Check if the node is present
            XmlNode xmlSubDirectory = xmlSubDirectories.SelectSingleNode("add[@directoryName='" + name + "']");
            if (xmlSubDirectory != null)
            {
				//Remove Node
                xmlSubDirectories.RemoveChild(xmlSubDirectory);

                Save(xmlConfig);
            }
        }

        public static string Save(XmlDocument xmlDoc)
        {
            return Save(xmlDoc, "web.config");
        }

        public static string Save(XmlDocument xmlDoc, string filename)
        {
            try
            {
                string strFilePath = Globals.ApplicationMapPath + "\\" + filename;
                FileAttributes objFileAttributes = FileAttributes.Normal;
                if (File.Exists(strFilePath))
                {
					//save current file attributes
                    objFileAttributes = File.GetAttributes(strFilePath);
                    //change to normal ( in case it is flagged as read-only )
                    File.SetAttributes(strFilePath, FileAttributes.Normal);
                }
                //save the config file
                var writer = new XmlTextWriter(strFilePath, null) { Formatting = Formatting.Indented };
                xmlDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                //reset file attributes
                File.SetAttributes(strFilePath, objFileAttributes);
                return "";
            }
            catch (Exception exc)
            {
				//the file permissions may not be set properly
                DnnLog.Error(exc);
                return exc.Message;
            }
        }

        public static bool Touch()
        {
            try
            {
                File.SetLastWriteTime(Globals.ApplicationMapPath + "\\web.config", DateTime.Now);
                return true;
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return false;
            }
        }

        public static void UpdateConnectionString(string conn)
        {
            XmlDocument xmlConfig = Load();
            string name = GetDefaultProvider("data").Attributes["connectionStringName"];
			
            //Update ConnectionStrings
            XmlNode xmlConnection = xmlConfig.SelectSingleNode("configuration/connectionStrings/add[@name='" + name + "']");
            XmlUtils.UpdateAttribute(xmlConnection, "connectionString", conn);
			
            //Update AppSetting
            XmlNode xmlAppSetting = xmlConfig.SelectSingleNode("configuration/appSettings/add[@key='" + name + "']");
            XmlUtils.UpdateAttribute(xmlAppSetting, "value", conn);
			
            //Save changes
            Save(xmlConfig);
        }

        public static void UpdateDataProvider(string name, string databaseOwner, string objectQualifier)
        {
            XmlDocument xmlConfig = Load();

            //Update provider
            XmlNode xmlProvider = xmlConfig.SelectSingleNode("configuration/dotnetnuke/data/providers/add[@name='" + name + "']");
            XmlUtils.UpdateAttribute(xmlProvider, "databaseOwner", databaseOwner);
            XmlUtils.UpdateAttribute(xmlProvider, "objectQualifier", objectQualifier);

            //Save changes
            Save(xmlConfig);
        }

        public static string UpdateMachineKey()
        {
            string backupFolder = Globals.glbConfigFolder + "Backup_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + "\\";
            var xmlConfig = new XmlDocument();
            string strError = "";

            //save the current config files
            BackupConfig();
            try
            {
                //open the web.config
                xmlConfig = Load();

                //create random keys for the Membership machine keys
                xmlConfig = UpdateMachineKey(xmlConfig);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                strError += ex.Message;
            }
			
			//save a copy of the web.config
            strError += Save(xmlConfig, backupFolder + "web_.config");

            //save the web.config
            strError += Save(xmlConfig);

            return strError;
        }

        public static XmlDocument UpdateMachineKey(XmlDocument xmlConfig)
        {
            var portalSecurity = new PortalSecurity();
            string validationKey = portalSecurity.CreateKey(20);
            string decryptionKey = portalSecurity.CreateKey(24);

            XmlNode xmlMachineKey = xmlConfig.SelectSingleNode("configuration/system.web/machineKey");
            XmlUtils.UpdateAttribute(xmlMachineKey, "validationKey", validationKey);
            XmlUtils.UpdateAttribute(xmlMachineKey, "decryptionKey", decryptionKey);
            
            xmlConfig = AddAppSetting(xmlConfig, "InstallationDate", DateTime.Today.ToString("d", new CultureInfo("en-US")));
            return xmlConfig;
        }

        public static string UpdateValidationKey()
        {
            string backupFolder = Globals.glbConfigFolder + "Backup_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + "\\";
            var xmlConfig = new XmlDocument();
            string strError = "";

            //save the current config files
            BackupConfig();
            try
            {
                //open the web.config
                xmlConfig = Load();

                //create random keys for the Membership machine keys
                xmlConfig = UpdateValidationKey(xmlConfig);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                strError += ex.Message;
            }
			
			//save a copy of the web.config
            strError += Save(xmlConfig, backupFolder + "web_.config");

            //save the web.config
            strError += Save(xmlConfig);
            return strError;
        }

        public static XmlDocument UpdateValidationKey(XmlDocument xmlConfig)
        {
            XmlNode xmlMachineKey = xmlConfig.SelectSingleNode("configuration/system.web/machineKey");
            if (xmlMachineKey.Attributes["validationKey"].Value == "F9D1A2D3E1D3E2F7B3D9F90FF3965ABDAC304902")
            {
                var objSecurity = new PortalSecurity();
                string validationKey = objSecurity.CreateKey(20);
                XmlUtils.UpdateAttribute(xmlMachineKey, "validationKey", validationKey);
            }
            return xmlConfig;
        }

        /// <summary>
        ///   Gets the path for the specificed Config file
        /// </summary>
        /// <param name = "file">The config.file to get the path for</param>
        /// <returns>fully qualified path to the file</returns>
        /// <remarks>
        ///   Will copy the file from the template directory as requried
        /// </remarks>
        public static string GetPathToFile(ConfigFileType file)
        {
            return GetPathToFile(file, false);
        }

        /// <summary>
        ///   Gets the path for the specificed Config file
        /// </summary>
        /// <param name = "file">The config.file to get the path for</param>
        /// <param name = "overwrite">force an overwrite of the config file</param>
        /// <returns>fully qualified path to the file</returns>
        /// <remarks>
        ///   Will copy the file from the template directory as requried
        /// </remarks>
        public static string GetPathToFile(ConfigFileType file, bool overwrite)
        {
            string fileName = EnumToFileName(file);
            string path = Path.Combine(Globals.ApplicationMapPath, fileName);

            if (!File.Exists(path) || overwrite)
            {
                //Copy from \Config
                string pathToDefault = Path.Combine(Globals.ApplicationMapPath + Globals.glbConfigFolder, fileName);
                if ((File.Exists(pathToDefault)))
                {
                    File.Copy(pathToDefault, path, true);
                }
            }

            return path;
        }

        private static string EnumToFileName(ConfigFileType file)
        {
            switch (file)
            {
                case ConfigFileType.SolutionsExplorer:
                    return "SolutionsExplorer.opml.config";
                default:
                    return file + ".config";
            }
        }
    }
}
