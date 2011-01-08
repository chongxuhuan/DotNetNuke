'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System
Imports System.IO
Imports System.Web.Configuration
Imports System.Xml
Imports DotNetNuke.Framework.Providers
Imports System.Xml.XPath

Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Config class provides access to the web.config file
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2005	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Config

#Region "Shared Methods"

        Public Shared Function AddAppSetting(ByVal xmlDoc As XmlDocument, ByVal Key As String, ByVal Value As String) As XmlDocument

            Dim xmlElement As XmlElement

            ' retrieve the appSettings node 
            Dim xmlAppSettings As XmlNode = xmlDoc.SelectSingleNode("//appSettings")

            If Not xmlAppSettings Is Nothing Then
                ' get the node based on key
                Dim xmlNode As XmlNode = xmlAppSettings.SelectSingleNode(("//add[@key='" + Key + "']"))

                If Not xmlNode Is Nothing Then
                    ' update the existing element
                    xmlElement = CType(xmlNode, XmlElement)
                    xmlElement.SetAttribute("value", Value)
                Else
                    ' create a new element
                    xmlElement = xmlDoc.CreateElement("add")
                    xmlElement.SetAttribute("key", Key)
                    xmlElement.SetAttribute("value", Value)
                    xmlAppSettings.AppendChild(xmlElement)
                End If
            End If

            ' return the xml doc
            Return xmlDoc

        End Function

        Public Shared Sub AddCodeSubDirectory(ByVal name As String)

            Dim xmlConfig As XmlDocument = Load()

            Dim xmlCompilation As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web/compilation")
            If xmlCompilation Is Nothing Then
                'Try location node
                xmlCompilation = xmlConfig.SelectSingleNode("configuration/location/system.web/compilation")
            End If

            'Get the CodeSubDirectories Node
            Dim xmlSubDirectories As XmlNode = xmlCompilation.SelectSingleNode("codeSubDirectories")
            If xmlSubDirectories Is Nothing Then
                'Add Node
                xmlSubDirectories = xmlConfig.CreateElement("codeSubDirectories")
                xmlCompilation.AppendChild(xmlSubDirectories)
            End If

            'Check if the node is already present
            Dim xmlSubDirectory As XmlNode = xmlSubDirectories.SelectSingleNode("add[@directoryName='" & name & "']")
            If xmlSubDirectory Is Nothing Then
                'Add Node
                xmlSubDirectory = xmlConfig.CreateElement("add")
                XmlUtils.CreateAttribute(xmlConfig, xmlSubDirectory, "directoryName", name)
                xmlSubDirectories.AppendChild(xmlSubDirectory)
            End If

            Save(xmlConfig)

        End Sub

        Public Shared Sub BackupConfig()
            Dim backupFolder As String = glbConfigFolder & "Backup_" & Now.Year.ToString & Now.Month.ToString & Now.Day.ToString & Now.Hour.ToString & Now.Minute.ToString & "\"

            'save the current config files
            Try
                If Not Directory.Exists(ApplicationMapPath & backupFolder) Then
                    Directory.CreateDirectory(ApplicationMapPath & backupFolder)
                End If

                If File.Exists(ApplicationMapPath & "\web.config") Then
                    File.Copy(ApplicationMapPath & "\web.config", ApplicationMapPath & backupFolder & "web_old.config", True)
                End If
            Catch ex As Exception
                'Error backing up old web.config 
                'This error is not critical, so can be ignored
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the default connection String as specified in the provider.
        ''' </summary>
        ''' <returns>The connection String</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''		[cnurse]	11/15/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetConnectionString() As String
            Return GetConnectionString(GetDefaultProvider("data").Attributes("connectionStringName"))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the specified connection String
        ''' </summary>
        ''' <param name="name">Name of Connection String to return</param>
        ''' <returns>The connection String</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''		[cnurse]	11/15/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetConnectionString(ByVal name As String) As String

            Dim connectionString As String = ""

            'First check if connection string is specified in <connectionstrings> (ASP.NET 2.0 / DNN v4.x)
            If name <> "" Then
                'ASP.NET 2 version connection string (in <connectionstrings>)
                'This will be for new v4.x installs or upgrades from v4.x
                connectionString = WebConfigurationManager.ConnectionStrings(name).ConnectionString
            End If

            If connectionString = "" Then
                'Next check if connection string is specified in <appsettings> (ASP.NET 1.1 / DNN v3.x)
                'This will accomodate upgrades from v3.x
                If name <> "" Then
                    connectionString = Config.GetSetting(name)
                End If
            End If

            Return connectionString
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the maximum file size allowed to be uploaded to the application
        ''' </summary>
        ''' <returns>Size in bytes</returns>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetMaxUploadSize() As Long
            Dim configNav = Load()
            Dim httpNode = configNav.SelectSingleNode("configuration//system.web//httpRuntime").CreateNavigator()
            ''httpNode.Attributes("maxRequestLength")
            ''Return 0

            Dim result = XmlUtils.GetAttributeValueAsLong(httpNode, "maxRequestLength", 0)

            Return result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the specified upgrade connection string
        ''' </summary>
        ''' <returns>The connection String</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''		[smehaffie]	07/13/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetUpgradeConnectionString() As String
            Dim _upgradeConnectionString As String = GetDefaultProvider("data").Attributes("upgradeConnectionString")

            Return _upgradeConnectionString
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the specified database owner
        ''' </summary>
        ''' <returns>The database owner</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''		[cnurse]	02/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDataBaseOwner() As String
            Dim _databaseOwner As String = GetDefaultProvider("data").Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

            Return _databaseOwner
        End Function

        Public Shared Function GetDefaultProvider(ByVal type As String) As Provider
            Dim _providerConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(type)

            ' Read the configuration specific information for this provider
            Return CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Provider)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the specified object qualifier
        ''' </summary>
        ''' <returns>The object qualifier</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''		[cnurse]	02/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetObjectQualifer() As String
            Dim objProvider As Provider = GetDefaultProvider("data")

            Dim _objectQualifier As String = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            Return _objectQualifier
        End Function

        ''' <summary>
        ''' Get's optional persistent cookie timeout value from web.config
        ''' </summary>
        ''' <returns>persistent cookie value</returns>
        ''' <remarks>allows users to override default asp.net values</remarks>
        Public Shared Function GetPersistentCookieTimeout() As Integer
            Dim configNav As XPathNavigator = Load().CreateNavigator()
            'Select the location node
            Dim locationNav As XPathNavigator = configNav.SelectSingleNode("configuration/location")
            Dim formsNav As XPathNavigator

            'Test for the existence of the location node if it exists then include that in the nodes of the XPath Query
            If locationNav Is Nothing Then
                formsNav = configNav.SelectSingleNode("configuration/system.web/authentication/forms")
            Else
                formsNav = configNav.SelectSingleNode("configuration/location/system.web/authentication/forms")
            End If

            Dim PersistentCookieTimeout As Integer = 0

            If Not Config.GetSetting("PersistentCookieTimeout") Is Nothing Then
                PersistentCookieTimeout = Integer.Parse(Config.GetSetting("PersistentCookieTimeout"))
            End If

            If PersistentCookieTimeout = 0 Then
                If Not formsNav Is Nothing Then
                    PersistentCookieTimeout = XmlUtils.GetAttributeValueAsInteger(formsNav, "timeout", 30)
                Else
                    PersistentCookieTimeout = 30
                End If
            End If

            Return PersistentCookieTimeout
        End Function

        Public Shared Function GetProvider(ByVal type As String, ByVal name As String) As Provider
            Dim _providerConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(type)

            ' Read the configuration specific information for this provider
            Return CType(_providerConfiguration.Providers(name), Provider)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the specified provider path
        ''' </summary>
        ''' <returns>The provider path</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''		[cnurse]	02/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetProviderPath(ByVal type As String) As String
            Dim objProvider As Provider = GetDefaultProvider(type)

            Dim _providerPath As String = objProvider.Attributes("providerPath")

            Return _providerPath
        End Function

        Public Shared Function GetSetting(ByVal setting As String) As String
            Return WebConfigurationManager.AppSettings(setting)
        End Function

        Public Shared Function GetSection(ByVal section As String) As Object
            Return WebConfigurationManager.GetWebApplicationSection(section)
        End Function

        Public Shared Function Load() As XmlDocument
            Return Load("web.config")
        End Function

        Public Shared Function Load(ByVal filename As String) As XmlDocument
            ' open the config file
            Dim xmlDoc As New XmlDocument
            xmlDoc.Load(Common.Globals.ApplicationMapPath & "\" & filename)
            ' test for namespace added by Web Admin Tool
            If xmlDoc.DocumentElement.GetAttribute("xmlns") <> "" Then
                ' remove namespace
                Dim strDoc As String = xmlDoc.InnerXml().Replace("xmlns=""http://schemas.microsoft.com/.NetConfiguration/v2.0""", "")
                xmlDoc.LoadXml(strDoc)
            End If
            Return xmlDoc
        End Function

        Public Shared Sub RemoveCodeSubDirectory(ByVal name As String)
            Dim xmlConfig As XmlDocument = Load()

            'Select the location node
            Dim xmlCompilation As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web/compilation")
            If xmlCompilation Is Nothing Then
                'Try location node
                xmlCompilation = xmlConfig.SelectSingleNode("configuration/location/system.web/compilation")
            End If

            'Get the CodeSubDirectories Node
            Dim xmlSubDirectories As XmlNode = xmlCompilation.SelectSingleNode("codeSubDirectories")
            If xmlSubDirectories Is Nothing Then
                'Parent doesn't exist so subDirectory node can't exist
                Exit Sub
            End If

            'Check if the node is present
            Dim xmlSubDirectory As XmlNode = xmlSubDirectories.SelectSingleNode("add[@directoryName='" & name & "']")
            If Not xmlSubDirectory Is Nothing Then
                'Remove Node
                xmlSubDirectories.RemoveChild(xmlSubDirectory)
            End If

            Save(xmlConfig)
        End Sub

        Public Shared Function Save(ByVal xmlDoc As XmlDocument) As String
            Return Save(xmlDoc, "web.config")
        End Function

        Public Shared Function Save(ByVal xmlDoc As XmlDocument, ByVal filename As String) As String
            Try
                Dim strFilePath As String = Common.Globals.ApplicationMapPath & "\" & filename
                Dim objFileAttributes As FileAttributes = FileAttributes.Normal
                If File.Exists(strFilePath) Then
                    ' save current file attributes
                    objFileAttributes = File.GetAttributes(strFilePath)
                    ' change to normal ( in case it is flagged as read-only )
                    File.SetAttributes(strFilePath, FileAttributes.Normal)
                End If
                ' save the config file
                Dim writer As New XmlTextWriter(strFilePath, Nothing)
                writer.Formatting = Formatting.Indented
                xmlDoc.WriteTo(writer)
                writer.Flush()
                writer.Close()
                ' reset file attributes
                File.SetAttributes(strFilePath, objFileAttributes)
                Return ""
            Catch exc As Exception
                ' the file permissions may not be set properly
                Return exc.Message
            End Try
        End Function

        Public Shared Function Touch() As Boolean
            File.SetLastWriteTime(Common.Globals.ApplicationMapPath & "\web.config", System.DateTime.Now)

            'Added to remove Syntax Warning - changing to a Sub would be better but would break binary compat
            Return True
        End Function

        Public Shared Sub UpdateConnectionString(ByVal conn As String)

            Dim xmlConfig As XmlDocument = Load()
            Dim name As String = GetDefaultProvider("data").Attributes("connectionStringName")

            'Update ConnectionStrings
            Dim xmlConnection As XmlNode = xmlConfig.SelectSingleNode("configuration/connectionStrings/add[@name='" + name + "']")
            XmlUtils.UpdateAttribute(xmlConnection, "connectionString", conn)

            'Update AppSetting
            Dim xmlAppSetting As XmlNode = xmlConfig.SelectSingleNode("configuration/appSettings/add[@key='" + name + "']")
            XmlUtils.UpdateAttribute(xmlAppSetting, "value", conn)

            'Save changes
            Save(xmlConfig)

        End Sub

        Public Shared Sub UpdateDataProvider(ByVal name As String, ByVal databaseOwner As String, ByVal objectQualifier As String)

            Dim xmlConfig As XmlDocument = Load()

            'Update provider
            Dim xmlProvider As XmlNode = xmlConfig.SelectSingleNode("configuration/dotnetnuke/data/providers/add[@name='" + name + "']")
            XmlUtils.UpdateAttribute(xmlProvider, "databaseOwner", databaseOwner)
            XmlUtils.UpdateAttribute(xmlProvider, "objectQualifier", objectQualifier)

            'Save changes
            Save(xmlConfig)

        End Sub

        Public Shared Function UpdateMachineKey() As String
            Dim backupFolder As String = glbConfigFolder & "Backup_" & Now.Year.ToString & Now.Month.ToString & Now.Day.ToString & Now.Hour.ToString & Now.Minute.ToString & "\"
            Dim xmlConfig As New XmlDocument
            Dim strError As String = ""

            'save the current config files
            BackupConfig()

            Try
                ' open the web.config
                xmlConfig = Load()

                ' create random keys for the Membership machine keys
                xmlConfig = UpdateMachineKey(xmlConfig)
            Catch ex As Exception
                strError += ex.Message
            End Try

            ' save a copy of the web.config
            strError += Save(xmlConfig, backupFolder & "web_.config")

            ' save the web.config
            strError += Save(xmlConfig)

            Return strError
        End Function

        Public Shared Function UpdateMachineKey(ByVal xmlConfig As XmlDocument) As XmlDocument

            Dim objSecurity As New PortalSecurity
            Dim validationKey As String = objSecurity.CreateKey(20)
            Dim decryptionKey As String = objSecurity.CreateKey(24)

            Dim xmlMachineKey As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web/machineKey")
            XmlUtils.UpdateAttribute(xmlMachineKey, "validationKey", validationKey)
            XmlUtils.UpdateAttribute(xmlMachineKey, "decryptionKey", decryptionKey)

            xmlConfig = AddAppSetting(xmlConfig, "InstallationDate", Date.Today.ToShortDateString)

            Return xmlConfig

        End Function

        Public Shared Function UpdateValidationKey() As String
            Dim backupFolder As String = glbConfigFolder & "Backup_" & Now.Year.ToString & Now.Month.ToString & Now.Day.ToString & Now.Hour.ToString & Now.Minute.ToString & "\"
            Dim xmlConfig As New XmlDocument
            Dim strError As String = ""

            'save the current config files
            BackupConfig()

            Try
                ' open the web.config
                xmlConfig = Load()

                ' create random keys for the Membership machine keys
                xmlConfig = UpdateValidationKey(xmlConfig)
            Catch ex As Exception
                strError += ex.Message
            End Try

            ' save a copy of the web.config
            strError += Save(xmlConfig, backupFolder & "web_.config")

            ' save the web.config
            strError += Save(xmlConfig)

            Return strError
        End Function

        Public Shared Function UpdateValidationkey(ByVal xmlConfig As XmlDocument) As XmlDocument
            Dim xmlMachineKey As XmlNode
            Dim strError As String = String.Empty

            xmlMachineKey = xmlConfig.SelectSingleNode("configuration/system.web/machineKey")

            If xmlMachineKey.Attributes("validationKey").Value = "F9D1A2D3E1D3E2F7B3D9F90FF3965ABDAC304902" Then
                Dim objSecurity As New PortalSecurity
                Dim validationKey As String = objSecurity.CreateKey(20)
                XmlUtils.UpdateAttribute(xmlMachineKey, "validationKey", validationKey)
            End If

            Return xmlConfig
        End Function

#End Region

    End Class

End Namespace
