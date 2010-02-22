'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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
Imports System.IO
Imports System.Web.Caching
Imports System.Threading
Imports System.Collections.Specialized
Imports System.Xml
Imports System.Xml.XPath

Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Entities.Host
Imports System.Collections.Generic
Imports DotNetNuke.Services.Cache


Namespace DotNetNuke.Services.Localization

    ''' <summary>
    ''' <para>CultureDropDownTypes allows the user to specify which culture name is displayed in the drop down list that is filled 
    ''' by using one of the helper methods.</para>
    ''' </summary>
    <Serializable()> _
    Public Enum CultureDropDownTypes
        'Displays the culture name in the format "&lt;languagefull&gt; (&lt;country/regionfull&gt;) in the .NET Framework language
        DisplayName
        'Displays the culture name in the format "&lt;languagefull&gt; (&lt;country/regionfull&gt;) in English
        EnglishName
        'Displays the culture identifier
        Lcid
        'Displays the culture name in the format "&lt;languagecode2&gt; (&lt;country/regioncode2&gt;)
        Name
        'Displays the culture name in the format "&lt;languagefull&gt; (&lt;country/regionfull&gt;) in the language that the culture is set to display
        NativeName
        'Displays the IS0 639-1 two letter code
        TwoLetterIsoCode
        'Displays the ISO 629-2 three letter code "&lt;languagefull&gt; (&lt;country/regionfull&gt;)
        ThreeLetterIsoCode
    End Enum

    Public Class Localization

#Region "Private Members"

        'Object used to lock SyncLock block
        Private Shared objLock As New Object

        Private Shared _defaultKeyName As String = "resourcekey"
        Private Shared _timeZoneListItems() As ListItem
        Private Shared strShowMissingKeys As String = ""
        Private Shared strUseBrowserLanguageDefault As String = ""
        Private Shared strUseLanguageInUrlDefault As String = ""

#End Region

#Region "Public Shared Properties"

        Public Shared ReadOnly Property ApplicationResourceDirectory() As String
            Get
                Return "~/App_GlobalResources"
            End Get
        End Property

        Public Shared ReadOnly Property ExceptionsResourceFile() As String
            Get
                Return ApplicationResourceDirectory + "/Exceptions.resx"
            End Get
        End Property

        Public Shared ReadOnly Property GlobalResourceFile() As String
            Get
                Return ApplicationResourceDirectory + "/GlobalResources.resx"
            End Get
        End Property

        Public Shared ReadOnly Property LocalResourceDirectory() As String
            Get
                Return "App_LocalResources"
            End Get
        End Property

        Public Shared ReadOnly Property LocalSharedResourceFile() As String
            Get
                Return "SharedResources.resx"
            End Get
        End Property

        Public Shared ReadOnly Property SharedResourceFile() As String
            Get
                Return ApplicationResourceDirectory + "/SharedResources.resx"
            End Get
        End Property

        Public Shared ReadOnly Property SupportedLocalesFile() As String
            Get
                Return ApplicationResourceDirectory + "/Locales.xml"
            End Get
        End Property

        Public Shared ReadOnly Property SystemLocale() As String
            Get
                Return "en-US"
            End Get
        End Property

        Public Shared ReadOnly Property SystemTimeZoneOffset() As Integer
            Get
                Return -480
            End Get
        End Property

        Public Shared ReadOnly Property TimezonesFile() As String
            Get
                Return ApplicationResourceDirectory + "/TimeZones.xml"
            End Get
        End Property


#End Region

#Region "Private Enums"

        Private Enum CustomizedLocale
            None = 0
            Portal = 1
            Host = 2
        End Enum

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CurrentCulture returns the current Culture being used
        ''' is 'key'.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property CurrentCulture() As String
            Get
                Return System.Threading.Thread.CurrentThread.CurrentCulture.ToString    ' _CurrentCulture
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The KeyName property returns and caches the name of the key attribute used to lookup resources.
        ''' This can be configured by setting ResourceManagerKey property in the web.config file. The default value for this property
        ''' is 'key'.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Property KeyName() As String
            Get
                Return _defaultKeyName
            End Get
            Set(ByVal Value As String)
                _defaultKeyName = Value
                If _defaultKeyName Is Nothing Or _defaultKeyName = String.Empty Then
                    _defaultKeyName = "resourcekey"
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ShowMissingKeys property returns the web.config setting that determines 
        ''' whether to render a visual indicator that a key is missing
        ''' is 'key'.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/20/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ShowMissingKeys() As Boolean
            Get
                If String.IsNullOrEmpty(strShowMissingKeys) Then
                    If Config.GetSetting("ShowMissingKeys") Is Nothing Then
                        strShowMissingKeys = "false"
                    Else
                        strShowMissingKeys = Config.GetSetting("ShowMissingKeys").ToLower()
                    End If
                End If
                Return Boolean.Parse(strShowMissingKeys)
            End Get
        End Property

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetLocalesCallBack gets a Dictionary of Locales by 
        ''' Portal from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetLocalesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Dim locales As Dictionary(Of String, Locale)
            If portalID > Null.NullInteger Then
                locales = CBO.FillDictionary(Of String, Locale)("CultureCode", DataProvider.Instance().GetLanguagesByPortal(portalID), New Dictionary(Of String, Locale))
            Else
                locales = CBO.FillDictionary(Of String, Locale)("CultureCode", DataProvider.Instance().GetLanguages(), New Dictionary(Of String, Locale))
            End If
            Return locales
        End Function

        Private Shared Function GetResourceFileLookupDictionary(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return New Dictionary(Of String, Boolean)
        End Function

        Private Shared Function GetResourceFileLookupDictionary() As Dictionary(Of String, Boolean)
            Return CBO.GetCachedObject(Of Dictionary(Of String, Boolean))(New CacheItemArgs(DataCache.ResourceFileLookupDictionaryCacheKey, _
                                                                    DataCache.ResourceFileLookupDictionaryTimeOut, _
                                                                    DataCache.ResourceFileLookupDictionaryCachePriority), AddressOf GetResourceFileLookupDictionary)
        End Function

        Private Shared Function GetResourceFileCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim cacheKey As String = cacheItemArgs.CacheKey
            Dim resources As Dictionary(Of String, String) = Nothing

            'Get resource file lookup to determine if the resource file even exists
            Dim resourceFileExistsLookup As Dictionary(Of String, Boolean) = GetResourceFileLookupDictionary()

            If (Not resourceFileExistsLookup.ContainsKey(cacheKey)) OrElse resourceFileExistsLookup(cacheKey) Then
                Dim filePath As String = Nothing
                'check if an absolute reference for the resource file was used
                If cacheKey.Contains(":\") AndAlso Path.IsPathRooted(cacheKey) Then
                    'if an absolute reference, check that the file exists
                    If File.Exists(cacheKey) Then
                        filePath = cacheKey
                    End If
                End If

                'no filepath found from an absolute reference, try and map the path to get the file path
                If filePath Is Nothing Then
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath(ApplicationPath + cacheKey)
                End If

                'The file is not in the lookup, or we know it exists as we have found it before
                If File.Exists(filePath) Then
                    Dim doc As XPathDocument = Nothing
                    doc = New XPathDocument(filePath)
                    resources = New Dictionary(Of String, String)
                    For Each nav As XPathNavigator In doc.CreateNavigator.Select("root/data")
                        If nav.NodeType <> XmlNodeType.Comment Then
                            resources(nav.GetAttribute("name", String.Empty)) = nav.SelectSingleNode("value").Value
                        End If
                    Next

                    cacheItemArgs.CacheDependency = New DNNCacheDependency(filePath)

                    'File exists so add it to lookup with value true, so we are safe to try again
                    resourceFileExistsLookup(cacheKey) = True
                Else
                    'File does not exist so add it to lookup with value false, so we don't try again
                    resourceFileExistsLookup(cacheKey) = False
                End If
            End If

            Return resources
        End Function

        Private Shared Function GetResourceFile(ByVal resourceFile As String) As Dictionary(Of String, String)
            Return CBO.GetCachedObject(Of Dictionary(Of String, String))(New CacheItemArgs(resourceFile, DataCache.ResourceFilesCacheTimeOut, _
                                                                    DataCache.ResourceFilesCachePriority), AddressOf GetResourceFileCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetResourceFileName is used to build the resource file name according to the
        ''' language
        ''' </summary>
        ''' <param name="language">The language</param>
        ''' <param name="ResourceFileRoot">The resource file root</param>
        ''' <returns>The language specific resource file</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetResourceFileName(ByVal ResourceFileRoot As String, ByVal language As String) As String
            Dim ResourceFile As String

            language = language.ToLower()

            If Not ResourceFileRoot Is Nothing Then
                If language = SystemLocale.ToLower Or language = "" Then
                    Select Case Right(ResourceFileRoot, 5).ToLower
                        Case ".resx"
                            ResourceFile = ResourceFileRoot
                        Case ".ascx"
                            ResourceFile = ResourceFileRoot & ".resx"
                        Case ".aspx"
                            ResourceFile = ResourceFileRoot & ".resx"
                        Case Else
                            ResourceFile = ResourceFileRoot + ".ascx.resx"       ' a portal module
                    End Select
                Else
                    Select Case Right(ResourceFileRoot, 5).ToLower
                        Case ".resx"
                            ResourceFile = ResourceFileRoot.Replace(".resx", "." + language + ".resx")
                        Case ".ascx"
                            ResourceFile = ResourceFileRoot.Replace(".ascx", ".ascx." + language + ".resx")
                        Case ".aspx"
                            ResourceFile = ResourceFileRoot.Replace(".aspx", ".aspx." + language + ".resx")
                        Case Else
                            ResourceFile = ResourceFileRoot + ".ascx." + language + ".resx"       ' a portal module
                    End Select
                End If
            Else
                If language = SystemLocale.ToLower Or language = "" Then
                    ResourceFile = SharedResourceFile
                Else
                    ResourceFile = SharedResourceFile.Replace(".resx", "." + language + ".resx")
                End If
            End If

            Return ResourceFile
        End Function

        Private Shared Function GetStringInternal(ByVal key As String, ByVal userLanguage As String, ByVal resourceFileRoot As String, ByVal objPortalSettings As PortalSettings, ByVal disableShowMissngKeys As Boolean) As String
            'make the default translation property ".Text"
            If key.IndexOf(".") < 1 Then
                key += ".Text"
            End If

            Dim resourceValue As String = Null.NullString
            Dim bFound As Boolean = TryGetStringInternal(key, userLanguage, resourceFileRoot, objPortalSettings, resourceValue)

            'If the key can't be found then it doesn't exist in the Localization Resources
            If ShowMissingKeys And Not disableShowMissngKeys Then
                If bFound Then
                    resourceValue = "[L]" & resourceValue
                Else
                    resourceValue = "RESX:" & key
                End If
            End If

            Return resourceValue
        End Function

        Private Shared Function LoadResourceFileCallback(ByVal cacheItemArgs As CacheItemArgs) As Dictionary(Of String, String)
            Dim fileName As String = DirectCast(cacheItemArgs.ParamList(0), String)
            Dim filePath As String = HttpContext.Current.Server.MapPath(fileName)
            Dim dicResources As New Dictionary(Of String, String)

            If File.Exists(filePath) Then
                Dim doc As XPathDocument = Nothing
                Try
                    doc = New XPathDocument(filePath)
                    For Each nav As XPathNavigator In doc.CreateNavigator.Select("root/data")
                        If nav.NodeType <> XmlNodeType.Comment Then
                            dicResources(nav.GetAttribute("name", String.Empty)) = nav.SelectSingleNode("value").Value
                        End If
                    Next
                Catch    'exc As Exception
                End Try
            End If

            Return dicResources
        End Function

        ''' <summary>
        ''' Provides localization support for DataControlFields used in DetailsView and GridView controls
        ''' </summary>
        ''' <param name="controlField">The field control to localize</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <remarks>
        ''' The header of the DataControlField is localized.
        ''' It also localizes text for following controls: ButtonField, CheckBoxField, CommandField, HyperLinkField, ImageField
        ''' </remarks>
        Private Shared Sub LocalizeDataControlField(ByVal controlField As DataControlField, ByVal resourceFile As String)

            Dim localizedText As String

            'Localize Header Text
            If Not String.IsNullOrEmpty(controlField.HeaderText) Then
                localizedText = GetString((controlField.HeaderText + ".Header"), resourceFile)
                If Not String.IsNullOrEmpty(localizedText) Then
                    controlField.HeaderText = localizedText
                    controlField.AccessibleHeaderText = controlField.HeaderText
                End If
            End If

            Select Case True
                Case TypeOf controlField Is TemplateField
                    'do nothing

                Case TypeOf controlField Is ButtonField
                    Dim button As ButtonField = DirectCast(controlField, ButtonField)
                    localizedText = GetString(button.Text, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then button.Text = localizedText

                Case TypeOf controlField Is CheckBoxField
                    Dim checkbox As CheckBoxField = DirectCast(controlField, CheckBoxField)
                    localizedText = GetString(checkbox.Text, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then checkbox.Text = localizedText

                Case TypeOf controlField Is CommandField
                    Dim commands As CommandField = DirectCast(controlField, CommandField)

                    localizedText = GetString(commands.CancelText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then commands.CancelText = localizedText

                    localizedText = GetString(commands.DeleteText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then commands.DeleteText = localizedText

                    localizedText = GetString(commands.EditText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then commands.EditText = localizedText

                    localizedText = GetString(commands.InsertText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then commands.InsertText = localizedText

                    localizedText = GetString(commands.NewText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then commands.NewText = localizedText

                    localizedText = GetString(commands.SelectText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then commands.SelectText = localizedText

                    localizedText = GetString(commands.UpdateText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then commands.UpdateText = localizedText

                Case TypeOf controlField Is HyperLinkField
                    Dim link As HyperLinkField = DirectCast(controlField, HyperLinkField)
                    localizedText = GetString(link.Text, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then link.Text = localizedText

                Case TypeOf controlField Is ImageField
                    Dim image As ImageField = DirectCast(controlField, ImageField)
                    localizedText = GetString(image.AlternateText, resourceFile)
                    If Not String.IsNullOrEmpty(localizedText) Then image.AlternateText = localizedText

            End Select

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the TimeZone file name for a given resource and language
        ''' </summary>
        ''' <param name="filename">Resource File</param>
        ''' <param name="language">Language</param>
        ''' <returns>Localized File Name</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function TimeZoneFile(ByVal filename As String, ByVal language As String) As String
            If language = Services.Localization.Localization.SystemLocale Then
                Return filename
            Else
                Return filename.Substring(0, filename.Length - 4) + "." + language + ".xml"
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TryGetFromResourceFile is used to get the string from a resource file
        ''' </summary>
        ''' <remarks>This method searches a resource file for the key.  It first checks the 
        ''' user's language, then the fallback language and finally the default language.
        ''' </remarks>
        ''' <param name="key">The resource key</param>
        ''' <param name="resourceFile">The resource file to search</param>
        ''' <param name="userLanguage">The user's language</param>
        ''' <param name="fallbackLanguage">The fallback language for the user's language</param>
        ''' <param name="defaultLanguage">The portal's default language</param>
        ''' <param name="portalID">The id of the portal</param>
        ''' <param name="resourceValue">The resulting resource value - returned by reference</param>
        ''' <returns>True if successful, false if not found</returns>
        ''' <history>
        ''' 	[cnurse]	01/30/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function TryGetFromResourceFile(ByVal key As String, ByVal resourceFile As String, ByVal userLanguage As String, ByVal fallbackLanguage As String, ByVal defaultLanguage As String, ByVal portalID As Integer, ByRef resourceValue As String) As Boolean
            'Try the user's language first
            Dim bFound As Boolean = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, userLanguage), portalID, resourceValue)

            If Not bFound AndAlso Not (fallbackLanguage = userLanguage) Then
                'Try fallback language next
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, fallbackLanguage), portalID, resourceValue)
            End If

            If Not bFound AndAlso Not (defaultLanguage = userLanguage OrElse defaultLanguage = fallbackLanguage) Then
                'Try default Language last
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, defaultLanguage), portalID, resourceValue)
            End If

            Return bFound
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TryGetFromResourceFile is used to get the string from a resource file
        ''' </summary>
        ''' <remarks>This method searches a specific language version of the resource file for 
        ''' the key.  It first checks the Portal version, then the Host version and finally
        ''' the Application version</remarks>
        ''' <param name="key">The resource key</param>
        ''' <param name="resourceFile">The resource file to search</param>
        ''' <param name="portalID">The id of the portal</param>
        ''' <param name="resourceValue">The resulting resource value - returned by reference</param>
        ''' <returns>True if successful, false if not found</returns>
        ''' <history>
        ''' 	[cnurse]	01/30/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function TryGetFromResourceFile(ByVal key As String, ByVal resourceFile As String, ByVal portalID As Integer, ByRef resourceValue As String) As Boolean
            'Try Portal Resource File
            Dim bFound As Boolean = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Portal, resourceValue)

            If Not bFound Then
                'Try Host Resource File
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Host, resourceValue)
            End If

            If Not bFound Then
                'Try Portal Resource File
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.None, resourceValue)
            End If

            Return bFound
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TryGetFromResourceFile is used to get the string from a specific resource file
        ''' </summary>
        ''' <remarks>This method searches a specific resource file for  the key.</remarks>
        ''' <param name="key">The resource key</param>
        ''' <param name="resourceFile">The resource file to search</param>
        ''' <param name="portalID">The id of the portal</param>
        ''' <param name="resourceValue">The resulting resource value - returned by reference</param>
        ''' <param name="resourceType">An enumerated CustomizedLocale - Application - 0, 
        ''' Host - 1, Portal - 2 - that identifies the file to search</param>
        ''' <returns>True if successful, false if not found</returns>
        ''' <history>
        ''' 	[cnurse]	01/30/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function TryGetFromResourceFile(ByVal key As String, ByVal resourceFile As String, ByVal portalID As Integer, ByVal resourceType As CustomizedLocale, ByRef resourceValue As String) As Boolean
            Dim dicResources As Dictionary(Of String, String) = Nothing
            Dim bFound As Boolean = Null.NullBoolean

            Dim resourceFileName As String = resourceFile
            Select Case resourceType
                Case CustomizedLocale.Host
                    resourceFileName = resourceFile.Replace(".resx", ".Host.resx")
                Case CustomizedLocale.Portal
                    resourceFileName = resourceFile.Replace(".resx", ".Portal-" + portalID.ToString + ".resx")
            End Select

            If resourceFileName.ToLowerInvariant().StartsWith("desktopmodules") OrElse resourceFileName.ToLowerInvariant().StartsWith("admin") Then
                resourceFileName = "~/" + resourceFileName
            End If

            'Local resource files are either named ~/... or <ApplicationPath>/...
            'The following logic creates a cachekey of /....
            Dim cacheKey As String = resourceFileName.Replace("~/", "/").ToLowerInvariant()
            If Not String.IsNullOrEmpty(ApplicationPath) Then
                If ApplicationPath.ToLowerInvariant <> "/portals" Then
                    If cacheKey.StartsWith(ApplicationPath.ToLowerInvariant()) Then
                        cacheKey = cacheKey.Substring(ApplicationPath.Length)
                    End If
                Else
                    cacheKey = "~" & cacheKey
                    If cacheKey.StartsWith("~" & ApplicationPath.ToLowerInvariant()) Then
                        cacheKey = cacheKey.Substring(ApplicationPath.Length + 1)
                    End If
                End If
            End If

            'Get resource file lookup to determine if the resource file even exists
            Dim resourceFileExistsLookup As Dictionary(Of String, Boolean) = GetResourceFileLookupDictionary()

            If (Not resourceFileExistsLookup.ContainsKey(cacheKey)) OrElse resourceFileExistsLookup(cacheKey) Then
                'File is not in lookup or its value is true so we know it exists
                dicResources = GetResourceFile(cacheKey)

                If dicResources IsNot Nothing Then
                    bFound = dicResources.TryGetValue(key, resourceValue)
                End If
            End If

            Return bFound
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TryGetStringInternal is used to get the string from a resource file
        ''' </summary>
        ''' <remarks>This method searches a resource file for the key.  It first checks the 
        ''' user's language, then the fallback language and finally the default language.
        ''' </remarks>
        ''' <param name="key">The resource key</param>
        ''' <param name="userLanguage">The user's language</param>
        ''' <param name="resourceFile">The resource file to search</param>
        ''' <param name="objPortalSettings">The portal settings</param>
        ''' <param name="resourceValue">The resulting resource value - returned by reference</param>
        ''' <returns>True if successful, false if not found</returns>
        ''' <history>
        ''' 	[cnurse]	01/30/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function TryGetStringInternal(ByVal key As String, ByVal userLanguage As String, ByVal resourceFile As String, ByVal objPortalSettings As PortalSettings, ByRef resourceValue As String) As Boolean
            Dim defaultLanguage As String = Null.NullString
            Dim fallbackLanguage As String = SystemLocale
            Dim portalId As Integer = Null.NullInteger

            'Get the default language
            If Not objPortalSettings Is Nothing Then
                defaultLanguage = objPortalSettings.DefaultLanguage
                portalId = objPortalSettings.PortalId
            End If

            'Set the userLanguage if not passed in
            If String.IsNullOrEmpty(userLanguage) Then
                userLanguage = Thread.CurrentThread.CurrentCulture.ToString()
            End If

            'Default the userLanguage to the defaultLanguage if not set
            If String.IsNullOrEmpty(userLanguage) Then
                userLanguage = defaultLanguage
            End If

            'Get Fallback language
            Dim userLocale As Locale = GetLocale(userLanguage)
            If userLocale IsNot Nothing AndAlso Not String.IsNullOrEmpty(userLocale.Fallback) Then
                fallbackLanguage = userLocale.Fallback
            End If

            If String.IsNullOrEmpty(resourceFile) Then
                resourceFile = SharedResourceFile
            End If

            'Try the resource file for the key
            Dim bFound As Boolean = TryGetFromResourceFile(key, resourceFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, resourceValue)

            If Not bFound Then
                If Not (SharedResourceFile.ToLowerInvariant() = resourceFile.ToLowerInvariant()) Then
                    'try to use a module specific shared resource file
                    Dim localSharedFile As String = resourceFile.Substring(0, resourceFile.LastIndexOf("/") + 1) & Localization.LocalSharedResourceFile

                    If Not (localSharedFile.ToLowerInvariant() = resourceFile.ToLowerInvariant()) Then
                        bFound = TryGetFromResourceFile(key, localSharedFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, resourceValue)
                    End If
                End If
            End If

            If Not bFound Then
                'try to use a global shared resource file
                If Not (SharedResourceFile.ToLowerInvariant() = resourceFile.ToLowerInvariant()) Then
                    bFound = TryGetFromResourceFile(key, SharedResourceFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, resourceValue)
                End If
            End If

            Return bFound
        End Function

#End Region

#Region "Public Methods"

        Public Function GetFixedCurrency(ByVal Expression As Decimal, ByVal Culture As String, Optional ByVal NumDigitsAfterDecimal As Integer = -1, Optional ByVal IncludeLeadingDigit As Microsoft.VisualBasic.TriState = TriState.UseDefault, Optional ByVal UseParensForNegativeNumbers As Microsoft.VisualBasic.TriState = TriState.UseDefault, Optional ByVal GroupDigits As Microsoft.VisualBasic.TriState = TriState.UseDefault) As String
            Dim oldCurrentCulture As String = CurrentCulture
            Dim newCulture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo(Culture)
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture
            Dim currencyStr As String = FormatCurrency(Expression, NumDigitsAfterDecimal, IncludeLeadingDigit, UseParensForNegativeNumbers, GroupDigits)
            Dim oldCulture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo(oldCurrentCulture)
            System.Threading.Thread.CurrentThread.CurrentUICulture = oldCulture
            System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture

            Return currencyStr
        End Function

        Public Function GetFixedDate(ByVal Expression As Date, ByVal Culture As String, Optional ByVal NamedFormat As Microsoft.VisualBasic.DateFormat = DateFormat.GeneralDate, Optional ByVal IncludeLeadingDigit As Microsoft.VisualBasic.TriState = TriState.UseDefault, Optional ByVal UseParensForNegativeNumbers As Microsoft.VisualBasic.TriState = TriState.UseDefault, Optional ByVal GroupDigits As Microsoft.VisualBasic.TriState = TriState.UseDefault) As String
            Dim oldCurrentCulture As String = CurrentCulture
            Dim newCulture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo(Culture)
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture
            Dim dateStr As String = FormatDateTime(Expression, NamedFormat)
            Dim oldCulture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo(oldCurrentCulture)
            System.Threading.Thread.CurrentThread.CurrentUICulture = oldCulture
            System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture

            Return dateStr
        End Function

#End Region

#Region "Public Shared Methods"

        Public Shared Sub AddLanguageToPortal(ByVal portalID As Integer, ByVal languageID As Integer, ByVal clearCache As Boolean)
            DataProvider.Instance().AddPortalLanguage(portalID, languageID, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("portalID/languageID", portalID.ToString & "/" & languageID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_CREATED)

            If clearCache Then
                DataCache.ClearPortalCache(portalID, False)
            End If
        End Sub

        Public Shared Sub AddLanguagesToPortal(ByVal portalID As Integer)
            For Each language As Locale In GetLocales(Null.NullInteger).Values
                'Add Portal/Language to PortalLanguages
                AddLanguageToPortal(portalID, language.LanguageID, False)
            Next

            DataCache.ClearPortalCache(portalID, True)
        End Sub

        Public Shared Sub AddLanguageToPortals(ByVal languageID As Integer)
            Dim controller As New PortalController
            For Each portal As PortalInfo In controller.GetPortals()
                'Add Portal/Language to PortalLanguages
                AddLanguageToPortal(portal.PortalID, languageID, False)
            Next

            DataCache.ClearHostCache(True)
        End Sub

        Public Shared Sub DeleteLanguage(ByVal language As Locale)
            DataProvider.Instance().DeleteLanguage(language.LanguageID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(language, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGE_DELETED)
            DataCache.ClearHostCache(True)
        End Sub

        Public Shared Function GetExceptionMessage(ByVal key As String, ByVal defaultValue As String) As String
            If HttpContext.Current Is Nothing Then
                Return defaultValue
            Else
                Return GetString(key, ExceptionsResourceFile)
            End If
        End Function

        Public Shared Function GetExceptionMessage(ByVal key As String, ByVal defaultValue As String, ByVal ParamArray params() As Object) As String
            If HttpContext.Current Is Nothing Then
                Return String.Format(defaultValue, params)
            Else
                Return String.Format(GetString(key, ExceptionsResourceFile), params)
            End If
            Return String.Format(GetString(key, ExceptionsResourceFile), params)
        End Function

        Public Shared Function GetLocale(ByVal code As String) As Locale
            Dim dicLocales As Dictionary(Of String, Locale) = GetLocales(Null.NullInteger)
            Dim language As Locale = Nothing

            If dicLocales IsNot Nothing Then
                dicLocales.TryGetValue(code, language)
            End If

            Return language
        End Function

        Public Shared Function GetLocaleByID(ByVal languageID As Integer) As Locale
            Dim dicLocales As Dictionary(Of String, Locale) = GetLocales(Null.NullInteger)
            Dim language As Locale = Nothing

            For Each kvp As KeyValuePair(Of String, Locale) In dicLocales
                If kvp.Value.LanguageID = languageID Then
                    language = kvp.Value
                    Exit For
                End If
            Next

            Return language
        End Function

        Public Shared Function GetLocales(ByVal portalID As Integer) As Dictionary(Of String, Locale)
            Dim cacheKey As String = String.Format(DataCache.LocalesCacheKey, portalID.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of String, Locale))(New CacheItemArgs(cacheKey, DataCache.LocalesCacheTimeOut, DataCache.LocalesCachePriority, portalID), AddressOf GetLocalesCallBack)
        End Function

        Public Shared Function GetResourceFileName(ByVal resourceFileName As String, ByVal language As String, ByVal mode As String, ByVal portalId As Integer) As String
            If Not resourceFileName.EndsWith(".resx") Then
                resourceFileName &= ".resx"
            End If

            If language <> Localization.SystemLocale Then
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + language + ".resx"
            End If

            If mode = "Host" Then
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + "Host.resx"
            ElseIf mode = "Portal" Then
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + "Portal-" + portalId.ToString + ".resx"
            End If

            Return resourceFileName
        End Function

        Public Shared Function GetResourceFile(ByVal Ctrl As Control, ByVal FileName As String) As String
            Return Ctrl.TemplateSourceDirectory + "/" + Services.Localization.Localization.LocalResourceDirectory + "/" + FileName
        End Function

        Public Shared Function GetPageLocale(ByVal portalSettings As PortalSettings) As CultureInfo
            Dim pageCulture As CultureInfo = Nothing
            Dim enabledLocales As Dictionary(Of String, Locale) = Nothing

            If Not portalSettings Is Nothing Then
                enabledLocales = Localization.GetLocales(portalSettings.PortalId)
            End If

            'used as temporary variable to get info about the preferred locale
            Dim preferredLocale As String = ""
            'used as temporary variable where the language part of the preferred locale will be saved
            Dim preferredLanguage As String = ""

            'first try if a specific language is requested by cookie, querystring, or form
            If Not (HttpContext.Current Is Nothing) Then
                Try
                    preferredLocale = HttpContext.Current.Request("language")
                    If preferredLocale <> "" Then
                        If Services.Localization.Localization.LocaleIsEnabled(preferredLocale) Then
                            pageCulture = New CultureInfo(preferredLocale)
                        Else
                            preferredLanguage = preferredLocale.Split("-"c)(0)
                        End If
                    End If
                Catch
                End Try
            End If

            If pageCulture Is Nothing Then
                ' next try to get the preferred language of the logged on user
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                If objUserInfo.UserID <> -1 Then
                    If objUserInfo.Profile.PreferredLocale <> "" Then
                        If Localization.LocaleIsEnabled(preferredLocale) Then
                            pageCulture = New CultureInfo(objUserInfo.Profile.PreferredLocale)
                        Else
                            If preferredLanguage = "" Then
                                preferredLanguage = objUserInfo.Profile.PreferredLocale.Split("-"c)(0)
                            End If
                        End If
                    End If
                End If
            End If

            If pageCulture Is Nothing AndAlso portalSettings.EnableBrowserLanguage Then
                ' use Request.UserLanguages to get the preferred language
                If Not (HttpContext.Current Is Nothing) Then
                    If Not (HttpContext.Current.Request.UserLanguages Is Nothing) Then
                        Try
                            For Each userLang As String In HttpContext.Current.Request.UserLanguages
                                'split userlanguage by ";"... all but the first language will contain a preferrence index eg. ;q=.5
                                Dim userlanguage As String = userLang.Split(";"c)(0)
                                If Localization.LocaleIsEnabled(userlanguage) Then
                                    pageCulture = New CultureInfo(userlanguage)
                                ElseIf userLang.Split(";"c)(0).IndexOf("-") <> -1 Then
                                    'if userLang is neutral we don't need to do this part since
                                    'it has already been done in LocaleIsEnabled( )
                                    Dim templang As String = userLang.Split(";"c)(0)
                                    For Each _localeCode As String In enabledLocales.Keys
                                        If _localeCode.Split("-"c)(0) = templang.Split("-"c)(0) Then
                                            'the preferredLanguage was found in the enabled locales collection, so we are going to use this one
                                            'eg, requested locale is en-GB, requested language is en, enabled locale is en-US, so en is a match for en-US
                                            pageCulture = New CultureInfo(_localeCode)
                                            Exit For
                                        End If
                                    Next
                                End If
                                If Not pageCulture Is Nothing Then
                                    Exit For
                                End If
                            Next
                        Catch
                        End Try
                    End If
                End If
            End If

            If pageCulture Is Nothing And preferredLanguage <> "" Then
                'we still don't have a good culture, so we are going to try to get a culture with the preferredlanguage instead
                For Each _localeCode As String In enabledLocales.Keys
                    If _localeCode.Split("-"c)(0) = preferredLanguage Then
                        'the preferredLanguage was found in the enabled locales collection, so we are going to use this one
                        'eg, requested locale is en-GB, requested language is en, enabled locale is en-US, so en is a match for en-US
                        pageCulture = New CultureInfo(_localeCode)
                        Exit For
                    End If
                Next
            End If

            'we still have no culture info set, so we are going to use the fallback method
            If pageCulture Is Nothing Then
                If portalSettings.DefaultLanguage = "" Then
                    ' this is a last resort, as the portal default language should always be set
                    ' however if its not set, return the first enabled locale
                    ' if there are no enabled locales, return the systemlocale
                    If enabledLocales.Count > 0 Then
                        For Each _localeCode As String In enabledLocales.Keys
                            pageCulture = New CultureInfo(_localeCode)
                            Exit For
                        Next
                    Else
                        pageCulture = New CultureInfo(Services.Localization.Localization.SystemLocale)
                    End If
                Else
                    ' as the portal default language can never be disabled, we know this language is available and enabled
                    pageCulture = New CultureInfo(portalSettings.DefaultLanguage)
                End If
            End If

            If pageCulture Is Nothing Then
                'just a safeguard, to make sure we return something
                pageCulture = New CultureInfo(Services.Localization.Localization.SystemLocale)
            End If

            'finally set the cookie
            DotNetNuke.Services.Localization.Localization.SetLanguage(pageCulture.Name)

            Return pageCulture
        End Function

#Region "GetString"

        Public Shared Function GetString(ByVal key As String, ByVal ctrl As Control) As String
            'We need to find the parent module
            Dim parentControl As Control = ctrl.Parent
            Dim localizedText As String
            Dim moduleControl As IModuleControl = TryCast(parentControl, IModuleControl)

            If moduleControl Is Nothing Then
                Dim pi As System.Reflection.PropertyInfo = parentControl.GetType.GetProperty("LocalResourceFile")
                If Not pi Is Nothing Then
                    'If control has a LocalResourceFile property use this
                    localizedText = GetString(key, pi.GetValue(parentControl, Nothing).ToString())
                Else
                    'Drill up to the next level 
                    localizedText = GetString(key, parentControl)
                End If
            Else
                'We are at the Module Level so return key
                'Get Resource File Root from Parents LocalResourceFile Property
                localizedText = GetString(key, moduleControl.LocalResourceFile)
            End If

            Return localizedText
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <overloads>One of six overloads</overloads>
        ''' <summary>
        ''' GetString gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="name">The resourcekey to find</param>
        ''' <returns>The localized Text</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetString(ByVal name As String) As String
            Return GetString(name, Nothing, PortalController.GetCurrentPortalSettings(), Nothing, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <overloads>One of six overloads</overloads>
        ''' <summary>
        ''' GetString gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="name">The resourcekey to find</param>
        ''' <param name="objPortalSettings">The current portals Portal Settings</param>
        ''' <returns>The localized Text</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetString(ByVal name As String, ByVal objPortalSettings As PortalSettings) As String
            Return GetString(name, Nothing, objPortalSettings, Nothing, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <overloads>One of six overloads</overloads>
        ''' <summary>
        ''' GetString gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="name">The resourcekey to find</param>
        ''' <param name="ResourceFileRoot">The Local Resource root</param>
        ''' <returns>The localized Text</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetString(ByVal name As String, ByVal ResourceFileRoot As String, ByVal disableShowMissingKeys As Boolean) As String
            Return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), Nothing, disableShowMissingKeys)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <overloads>One of six overloads</overloads>
        ''' <summary>
        ''' GetString gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="name">The resourcekey to find</param>
        ''' <param name="ResourceFileRoot">The Local Resource root</param>
        ''' <returns>The localized Text</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetString(ByVal name As String, ByVal ResourceFileRoot As String) As String
            Return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), Nothing, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <overloads>One of six overloads</overloads>
        ''' <summary>
        ''' GetString gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="name">The resourcekey to find</param>
        ''' <param name="ResourceFileRoot">The Local Resource root</param>
        ''' <param name="strLanguage">A specific language to lookup the string</param>
        ''' <returns>The localized Text</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetString(ByVal name As String, ByVal ResourceFileRoot As String, ByVal strlanguage As String) As String
            Return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), strlanguage, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <overloads>One of six overloads</overloads>
        ''' <summary>
        ''' GetString gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="name">The resourcekey to find</param>
        ''' <param name="ResourceFileRoot">The Local Resource root</param>
        ''' <param name="objPortalSettings">The current portals Portal Settings</param>
        ''' <param name="strLanguage">A specific language to lookup the string</param>
        ''' <returns>The localized Text</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetString(ByVal name As String, ByVal ResourceFileRoot As String, ByVal objPortalSettings As PortalSettings, ByVal strLanguage As String) As String
            Return GetString(name, ResourceFileRoot, objPortalSettings, strLanguage, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <overloads>One of six overloads</overloads>
        ''' <summary>
        ''' GetString gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="key">The resourcekey to find</param>
        ''' <param name="resourceFileRoot">The Local Resource root</param>
        ''' <param name="objPortalSettings">The current portals Portal Settings</param>
        ''' <param name="userLanguage">A specific language to lookup the string</param>
        ''' <param name="disableShowMissingKeys">Disables the show missing keys flag</param>
        ''' <returns>The localized Text</returns>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Documented
        '''     [cnurse]    01/30/2008  Refactored to use Dictionaries and to cahe the portal and host 
        '''                             customisations and language versions separately
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetString(ByVal key As String, ByVal resourceFileRoot As String, ByVal objPortalSettings As PortalSettings, ByVal userLanguage As String, ByVal disableShowMissingKeys As Boolean) As String
            Return GetStringInternal(key, userLanguage, resourceFileRoot, objPortalSettings, disableShowMissingKeys)
        End Function

#End Region

#Region "GetStringUrl"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetStringUrl gets the localized string corresponding to the resourcekey
        ''' </summary>
        ''' <param name="name">The resourcekey to find</param>
        ''' <param name="ResourceFileRoot">The Local Resource root</param>
        ''' <returns>The localized Text</returns>
        ''' <remarks>
        ''' This function should be used to retrieve strings to be used on URLs.
        ''' It is the same as <see cref="GetString">GetString(name,ResourceFileRoot</see> method
        ''' but it disables the ShowMissingKey flag, so even it testing scenarios, the correct string
        ''' is returned
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	11/21/2006	Added
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetStringUrl(ByVal name As String, ByVal ResourceFileRoot As String) As String
            Return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), Nothing, True)
        End Function

#End Region

#Region "Get System Message"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage.
        ''' </summary>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Supported tags:
        ''' - All fields from HostSettings table in the form of: [Host:<b>field</b>]
        ''' - All properties defined in <see cref="T:DotNetNuke.PortalInfo" /> in the form of: [Portal:<b>property</b>]
        ''' - [Portal:URL]: The base URL for the portal
        ''' - All properties defined in <see cref="T:DotNetNuke.UserInfo" /> in the form of: [User:<b>property</b>]
        ''' - All values stored in the user profile in the form of: [Profile:<b>key</b>]
        ''' - [User:VerificationCode]: User verification code for verified registrations
        ''' - [Date:Current]: Current date
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal objPortal As PortalSettings, ByVal MessageName As String) As String
            Return GetSystemMessage(Nothing, objPortal, MessageName, Nothing, GlobalResourceFile, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage.
        ''' </summary>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="objUser">Reference to the user used to personalize the message.</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Supported tags:
        ''' - All fields from HostSettings table in the form of: [Host:<b>field</b>]
        ''' - All properties defined in <see cref="T:DotNetNuke.PortalInfo" /> in the form of: [Portal:<b>property</b>]
        ''' - [Portal:URL]: The base URL for the portal
        ''' - All properties defined in <see cref="T:DotNetNuke.UserInfo" /> in the form of: [User:<b>property</b>]
        ''' - All values stored in the user profile in the form of: [Profile:<b>key</b>]
        ''' - [User:VerificationCode]: User verification code for verified registrations
        ''' - [Date:Current]: Current date
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal objUser As UserInfo) As String
            Return GetSystemMessage(Nothing, objPortal, MessageName, objUser, GlobalResourceFile, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''         ''' Gets a SystemMessage.
        ''' </summary>
        ''' <param name="strLanguage">A specific language to get the SystemMessage for.</param>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="objUser">Reference to the user used to personalize the message.</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Supported tags:
        ''' - All fields from HostSettings table in the form of: [Host:<b>field</b>]
        ''' - All properties defined in <see cref="T:DotNetNuke.PortalInfo" /> in the form of: [Portal:<b>property</b>]
        ''' - [Portal:URL]: The base URL for the portal
        ''' - All properties defined in <see cref="T:DotNetNuke.UserInfo" /> in the form of: [User:<b>property</b>]
        ''' - All values stored in the user profile in the form of: [Profile:<b>key</b>]
        ''' - [User:VerificationCode]: User verification code for verified registrations
        ''' - [Date:Current]: Current date
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal strLanguage As String, ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal objUser As UserInfo) As String
            Return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, GlobalResourceFile, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage.
        ''' </summary>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Supported tags:
        ''' - All fields from HostSettings table in the form of: [Host:<b>field</b>]
        ''' - All properties defined in <see cref="T:DotNetNuke.PortalInfo" /> in the form of: [Portal:<b>property</b>]
        ''' - [Portal:URL]: The base URL for the portal
        ''' - All properties defined in <see cref="T:DotNetNuke.UserInfo" /> in the form of: [User:<b>property</b>]
        ''' - All values stored in the user profile in the form of: [Profile:<b>key</b>]
        ''' - [User:VerificationCode]: User verification code for verified registrations
        ''' - [Date:Current]: Current date
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal ResourceFile As String) As String
            Return GetSystemMessage(Nothing, objPortal, MessageName, Nothing, ResourceFile, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage.
        ''' </summary>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="objUser">Reference to the user used to personalize the message.</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Supported tags:
        ''' - All fields from HostSettings table in the form of: [Host:<b>field</b>]
        ''' - All properties defined in <see cref="T:DotNetNuke.PortalInfo" /> in the form of: [Portal:<b>property</b>]
        ''' - [Portal:URL]: The base URL for the portal
        ''' - All properties defined in <see cref="T:DotNetNuke.UserInfo" /> in the form of: [User:<b>property</b>]
        ''' - All values stored in the user profile in the form of: [Profile:<b>key</b>]
        ''' - [User:VerificationCode]: User verification code for verified registrations
        ''' - [Date:Current]: Current date
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal objUser As UserInfo, ByVal ResourceFile As String) As String
            Return GetSystemMessage(Nothing, objPortal, MessageName, objUser, ResourceFile, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage passing extra custom parameters to personalize.
        ''' </summary>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <param name="Custom">An ArrayList with replacements for custom tags.</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Custom tags are of the form <b>[Custom:n]</b>, where <b>n</b> is the zero based index which 
        ''' will be used to find the replacement value in <b>Custom</b> parameter.
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        '''     [cnurse]    10/06/2004  Moved from SystemMessages to Localization
        '''     [DanCaron]  10/27/2004  Simplified Profile replacement, added Membership replacement
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal ResourceFile As String, ByVal Custom As ArrayList) As String
            Return GetSystemMessage(Nothing, objPortal, MessageName, Nothing, ResourceFile, Custom)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage passing extra custom parameters to personalize.
        ''' </summary>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="objUser">Reference to the user used to personalize the message.</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <param name="Custom">An ArrayList with replacements for custom tags.</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Custom tags are of the form <b>[Custom:n]</b>, where <b>n</b> is the zero based index which 
        ''' will be used to find the replacement value in <b>Custom</b> parameter.
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        '''     [cnurse]    10/06/2004  Moved from SystemMessages to Localization
        '''     [DanCaron]  10/27/2004  Simplified Profile replacement, added Membership replacement
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal objUser As UserInfo, ByVal ResourceFile As String, ByVal Custom As ArrayList) As String
            Return GetSystemMessage(Nothing, objPortal, MessageName, objUser, ResourceFile, Custom)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage passing extra custom parameters to personalize.
        ''' </summary>
        ''' <param name="strLanguage">A specific language to get the SystemMessage for.</param>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="objUser">Reference to the user used to personalize the message.</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <param name="Custom">An ArrayList with replacements for custom tags.</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Custom tags are of the form <b>[Custom:n]</b>, where <b>n</b> is the zero based index which 
        ''' will be used to find the replacement value in <b>Custom</b> parameter.
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        '''     [cnurse]    10/06/2004  Moved from SystemMessages to Localization
        '''     [DanCaron]  10/27/2004  Simplified Profile replacement, added Membership replacement
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal strLanguage As String, ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal objUser As UserInfo, ByVal ResourceFile As String, ByVal Custom As ArrayList) As String
            Return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, ResourceFile, Custom, Nothing, "", -1)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage passing extra custom parameters to personalize.
        ''' </summary>
        ''' <param name="strLanguage">A specific language to get the SystemMessage for.</param>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="objUser">Reference to the user used to personalize the message.</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <param name="Custom">An ArrayList with replacements for custom tags.</param>
        ''' <param name="CustomCaption">prefix for custom tags</param>
        ''' <param name="AccessingUserID">UserID of the user accessing the system message</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Custom tags are of the form <b>[Custom:n]</b>, where <b>n</b> is the zero based index which 
        ''' will be used to find the replacement value in <b>Custom</b> parameter.
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/07/2004	Documented
        '''     [cnurse]    10/06/2004  Moved from SystemMessages to Localization
        '''     [DanCaron]  10/27/2004  Simplified Profile replacement, added Membership replacement
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal strLanguage As String, ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal objUser As UserInfo, ByVal ResourceFile As String, ByVal Custom As ArrayList, ByVal CustomCaption As String, ByVal AccessingUserID As Integer) As String
            Return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, ResourceFile, Custom, Nothing, CustomCaption, AccessingUserID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SystemMessage passing extra custom parameters to personalize.
        ''' </summary>
        ''' <param name="strLanguage">A specific language to get the SystemMessage for.</param>
        ''' <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        ''' <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        ''' <param name="objUser">Reference to the user used to personalize the message.</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <param name="CustomArray">An ArrayList with replacements for custom tags.</param>
        ''' <param name="CustomDictionary">An IDictionary with replacements for custom tags.</param>
        ''' <param name="CustomCaption">prefix for custom tags</param>
        ''' <param name="AccessingUserID">UserID of the user accessing the system message</param>
        ''' <returns>The message body with all tags replaced.</returns>
        ''' <remarks>
        ''' Custom tags are of the form <b>[Custom:n]</b>, where <b>n</b> is the zero based index which 
        ''' will be used to find the replacement value in <b>Custom</b> parameter.
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    09/09/2009  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSystemMessage(ByVal strLanguage As String, ByVal objPortal As PortalSettings, ByVal MessageName As String, ByVal objUser As UserInfo, ByVal ResourceFile As String, ByVal CustomArray As ArrayList, ByVal CustomDictionary As IDictionary, ByVal CustomCaption As String, ByVal AccessingUserID As Integer) As String
            Dim strMessageValue As String
            strMessageValue = GetString(MessageName, ResourceFile, objPortal, strLanguage)

            If strMessageValue <> "" Then
                If CustomCaption = "" Then
                    CustomCaption = "Custom"
                End If
                Dim objTokenReplace As New Services.Tokens.TokenReplace(Scope.SystemMessages, strLanguage, objPortal, objUser)
                If (AccessingUserID <> -1) And (Not objUser Is Nothing) Then
                    If objUser.UserID <> AccessingUserID Then
                        objTokenReplace.AccessingUser = New UserController().GetUser(objPortal.PortalId, AccessingUserID)
                    End If
                End If
                If CustomArray IsNot Nothing Then
                    strMessageValue = objTokenReplace.ReplaceEnvironmentTokens(strMessageValue, CustomArray, CustomCaption)
                Else
                    strMessageValue = objTokenReplace.ReplaceEnvironmentTokens(strMessageValue, CustomDictionary, CustomCaption)
                End If
            End If

            Return strMessageValue
        End Function

#End Region

        '' -----------------------------------------------------------------------------
        '' <summary>
        '' GetTimeZones gets a collection of Tme Zones in the relevant language
        '' </summary>
        '' <param name="language">Language</param>
        ''	<returns>The TimeZones as a Name/Value Collection</returns>
        '' <history>
        '' 	[cnurse]	10/29/2004	Modified to exit gracefully if no relevant file
        '' </history>
        '' -----------------------------------------------------------------------------
        Public Shared Function GetTimeZones(ByVal language As String) As NameValueCollection
            language = language.ToLower
            Dim cacheKey As String = "dotnetnuke-" + language + "-timezones"

            Dim TranslationFile As String

            If language = Services.Localization.Localization.SystemLocale.ToLower Then
                TranslationFile = Services.Localization.Localization.TimezonesFile
            Else
                TranslationFile = Services.Localization.Localization.TimezonesFile.Replace(".xml", "." + language + ".xml")
            End If

            Dim timeZones As NameValueCollection = CType(DataCache.GetCache(cacheKey), NameValueCollection)

            If timeZones Is Nothing Then
                Dim filePath As String = HttpContext.Current.Server.MapPath(TranslationFile)
                timeZones = New NameValueCollection
                If File.Exists(filePath) = False Then
                    Return timeZones
                End If
                Dim dp As New DNNCacheDependency(filePath)
                Try
                    Dim d As New XmlDocument
                    d.Load(filePath)

                    Dim n As XmlNode
                    For Each n In d.SelectSingleNode("root").ChildNodes
                        If n.NodeType <> XmlNodeType.Comment Then
                            timeZones.Add(n.Attributes("name").Value, n.Attributes("key").Value)
                        End If
                    Next n
                Catch ex As Exception

                End Try
                If Host.PerformanceSetting <> Common.Globals.PerformanceSettings.NoCaching Then
                    DataCache.SetCache(cacheKey, timeZones, dp)
                End If
            End If

            Return timeZones
        End Function    'GetTimeZones

        ''' <summary>
        ''' <para>LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        ''' based on the languages defined in the supported locales file</para>
        ''' </summary>
        ''' <param name="list">DropDownList to load</param>
        ''' <param name="displayType">Format of the culture to display. Must be one the CultureDropDownTypes values. 
        ''' <see cref="CultureDropDownTypes"/> for list of allowable values</param>
        ''' <param name="selectedValue">Name of the default culture to select</param>
        Public Shared Sub LoadCultureDropDownList(ByVal list As DropDownList, ByVal displayType As CultureDropDownTypes, ByVal selectedValue As String)
            Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
            Dim enabledLanguages As Dictionary(Of String, Locale) = GetLocales(objPortalSettings.PortalId)
            Dim _cultureListItems() As ListItem = New ListItem(enabledLanguages.Count - 1) {}
            Dim _cultureListItemsType As CultureDropDownTypes = displayType
            Dim intAdded As Integer = 0

            For Each kvp As KeyValuePair(Of String, Locale) In enabledLanguages
                ' Create a CultureInfo class based on culture
                Dim info As CultureInfo = CultureInfo.CreateSpecificCulture(kvp.Value.Code)

                ' Create and initialize a new ListItem
                Dim item As New ListItem
                item.Value = kvp.Value.Code

                ' Based on the display type desired by the user, select the correct property
                Select Case displayType
                    Case CultureDropDownTypes.EnglishName
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.EnglishName)
                    Case CultureDropDownTypes.Lcid
                        item.Text = info.LCID.ToString()
                    Case CultureDropDownTypes.Name
                        item.Text = info.Name
                    Case CultureDropDownTypes.NativeName
                        item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.NativeName)
                    Case CultureDropDownTypes.TwoLetterIsoCode
                        item.Text = info.TwoLetterISOLanguageName
                    Case CultureDropDownTypes.ThreeLetterIsoCode
                        item.Text = info.ThreeLetterISOLanguageName
                    Case Else
                        item.Text = info.DisplayName
                End Select
                _cultureListItems(intAdded) = item
                intAdded += 1
            Next

            ' If the drop down list already has items, clear the list
            If list.Items.Count > 0 Then
                list.Items.Clear()
            End If

            ReDim Preserve _cultureListItems(intAdded - 1)
            ' add the items to the list
            list.Items.AddRange(_cultureListItems)

            ' select the default item
            If Not selectedValue Is Nothing Then
                Dim item As ListItem = list.Items.FindByValue(selectedValue)
                If Not item Is Nothing Then
                    list.SelectedIndex = -1
                    item.Selected = True
                End If
            End If
        End Sub    'LoadCultureDropDownList

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadTimeZoneDropDownList loads a drop down list with the Timezones
        ''' </summary>
        ''' <param name="list">The list to load</param>
        ''' <param name="language">Language</param>
        ''' <param name="selectedValue">The selected Time Zone</param>
        ''' <history>
        ''' 	[cnurse]	10/29/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub LoadTimeZoneDropDownList(ByVal list As DropDownList, ByVal language As String, ByVal selectedValue As String)

            Dim timeZones As NameValueCollection = GetTimeZones(language)
            'If no Timezones defined get the System Locale Time Zones
            If timeZones.Count = 0 Then
                timeZones = GetTimeZones(Services.Localization.Localization.SystemLocale.ToLower)
            End If
            Dim i As Integer
            For i = 0 To timeZones.Keys.Count - 1
                list.Items.Add(New ListItem(timeZones.GetKey(i).ToString(), timeZones.Get(i).ToString()))
            Next i

            ' select the default item
            If Not selectedValue Is Nothing Then
                Dim item As ListItem = list.Items.FindByValue(selectedValue)
                If item Is Nothing Then
                    'Try system default
                    item = list.Items.FindByValue(SystemTimeZoneOffset.ToString)
                End If
                If Not item Is Nothing Then
                    list.SelectedIndex = -1
                    item.Selected = True
                End If
            End If

        End Sub    'LoadTimeZoneDropDownList

        Public Overloads Shared Function LocaleIsEnabled(ByVal locale As Locale) As Boolean
            Return LocaleIsEnabled(locale.Code)
        End Function

        Public Overloads Shared Function LocaleIsEnabled(ByRef localeCode As String) As Boolean
            Try
                Dim isEnabled As Boolean = False
                Dim _Settings As PortalSettings = PortalController.GetCurrentPortalSettings()
                Dim dicLocales As Dictionary(Of String, Locale) = GetLocales(_Settings.PortalId)
                If dicLocales.Item(localeCode) Is Nothing Then
                    'if localecode is neutral (en, es,...) try to find a locale that has the same language
                    If localeCode.IndexOf("-") = -1 Then
                        For Each strLocale As String In dicLocales.Keys
                            If strLocale.Split("-"c)(0) = localeCode Then
                                'set the requested _localecode to the full locale
                                localeCode = strLocale
                                isEnabled = True
                                Exit For
                            End If
                        Next
                    End If
                Else
                    isEnabled = True
                End If
                Return isEnabled
            Catch ex As Exception
                'item could not be retrieved  or error
                Return False
            End Try
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Localizes ModuleControl Titles
        ''' </summary>
        ''' <param name="moduleControl">ModuleControl</param>
        ''' <returns>
        ''' Localized control title if found
        ''' </returns>
        ''' <remarks>
        ''' Resource keys are: ControlTitle_[key].Text
        ''' Key MUST be lowercase in the resource file
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	08/11/2004	Created
        '''     [cnurse]    11/28/2008  Modified Signature
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function LocalizeControlTitle(ByVal moduleControl As IModuleControl) As String
            Dim controlTitle As String = moduleControl.ModuleContext.Configuration.ModuleTitle
            Dim controlKey As String = moduleControl.ModuleContext.Configuration.ModuleControl.ControlKey.ToLower

            If String.IsNullOrEmpty(controlTitle) AndAlso Not String.IsNullOrEmpty(controlKey) Then
                Dim reskey As String
                reskey = "ControlTitle_" + moduleControl.ModuleContext.Configuration.ModuleControl.ControlKey.ToLower + ".Text"

                Dim localizedvalue As String = Services.Localization.Localization.GetString(reskey, moduleControl.LocalResourceFile)
                If Not localizedvalue Is Nothing Then
                    controlTitle = localizedvalue
                End If
            End If

            Return controlTitle
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LocalizeDataGrid creates localized Headers for a DataGrid
        ''' </summary>
        ''' <param name="grid">Grid to localize</param>
        ''' <param name="ResourceFile">The root name of the Resource File where the localized
        '''   text can be found</param>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub LocalizeDataGrid(ByRef grid As DataGrid, ByVal ResourceFile As String)

            Dim localizedText As String

            For Each col As DataGridColumn In grid.Columns
                'Localize Header Text
                If Not String.IsNullOrEmpty(col.HeaderText) Then
                    localizedText = GetString(col.HeaderText & ".Header", ResourceFile)
                    If localizedText <> "" Then
                        col.HeaderText = localizedText
                    End If
                End If

                If TypeOf col Is EditCommandColumn Then
                    Dim editCol As EditCommandColumn = DirectCast(col, EditCommandColumn)

                    ' Edit Text - maintained for backward compatibility
                    localizedText = GetString(editCol.EditText & ".EditText", ResourceFile)
                    If localizedText <> "" Then editCol.EditText = localizedText

                    ' Edit Text
                    localizedText = GetString(editCol.EditText, ResourceFile)
                    If localizedText <> "" Then editCol.EditText = localizedText

                    ' Cancel Text
                    localizedText = GetString(editCol.CancelText, ResourceFile)
                    If localizedText <> "" Then editCol.CancelText = localizedText

                    ' Update Text
                    localizedText = GetString(editCol.UpdateText, ResourceFile)
                    If localizedText <> "" Then editCol.UpdateText = localizedText
                ElseIf TypeOf col Is ButtonColumn Then
                    Dim buttonCol As ButtonColumn = DirectCast(col, ButtonColumn)

                    ' Edit Text
                    localizedText = GetString(buttonCol.Text, ResourceFile)
                    If localizedText <> "" Then buttonCol.Text = localizedText
                End If

            Next
        End Sub

        ''' <summary>
        ''' Localizes headers and fields on a DetailsView control
        ''' </summary>
        ''' <param name="detailsView"></param>
        ''' <param name="resourceFile">The root name of the resource file where the localized
        '''  texts can be found</param>
        ''' <remarks></remarks>
        Public Shared Sub LocalizeDetailsView(ByRef detailsView As DetailsView, ByVal resourceFile As String)
            For Each field As DataControlField In detailsView.Fields
                LocalizeDataControlField(field, resourceFile)
            Next
        End Sub

        ''' <summary>
        ''' Localizes headers and fields on a GridView control
        ''' </summary>
        ''' <param name="gridView">Grid to localize</param>
        ''' <param name="resourceFile">The root name of the resource file where the localized
        '''  texts can be found</param>
        ''' <remarks></remarks>
        Public Shared Sub LocalizeGridView(ByRef gridView As GridView, ByVal resourceFile As String)
            For Each column As DataControlField In gridView.Columns
                LocalizeDataControlField(column, resourceFile)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Localizes the "Built In" Roles
        ''' </summary>
        ''' <remarks>
        ''' Localizes:
        ''' -DesktopTabs
        ''' -BreadCrumbs
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/01/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function LocalizeRole(ByVal role As String) As String

            Dim localRole As String

            Select Case role
                Case glbRoleAllUsersName, glbRoleSuperUserName, glbRoleUnauthUserName
                    Dim roleKey As String = role.Replace(" ", "")
                    localRole = GetString(roleKey)
                Case Else
                    localRole = role
            End Select

            Return localRole

        End Function

        Public Shared Sub RemoveLanguageFromPortal(ByVal portalID As Integer, ByVal languageID As Integer)
            DataProvider.Instance().DeletePortalLanguages(portalID, languageID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("portalID/languageID", portalID.ToString & "/" & languageID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_DELETED)
            DataCache.ClearPortalCache(portalID, False)
        End Sub

        Public Shared Sub RemoveLanguageFromPortals(ByVal languageID As Integer)
            DataProvider.Instance().DeletePortalLanguages(Null.NullInteger, languageID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("languageID", languageID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_DELETED)
            DataCache.ClearHostCache(True)
        End Sub

        Public Shared Sub RemoveLanguagesFromPortal(ByVal portalID As Integer)
            DataProvider.Instance().DeletePortalLanguages(portalID, Null.NullInteger)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("portalID", portalID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_DELETED)
            DataCache.ClearPortalCache(portalID, False)
        End Sub

        Public Shared Sub SaveLanguage(ByVal locale As Locale)
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            If locale.LanguageID = Null.NullInteger Then
                locale.LanguageID = DataProvider.Instance().AddLanguage(locale.Code, locale.Text, locale.Fallback, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(locale, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGE_CREATED)
            Else
                DataProvider.Instance().UpdateLanguage(locale.LanguageID, locale.Code, locale.Text, locale.Fallback, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(locale, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGE_UPDATED)
            End If
            DataCache.ClearHostCache(True)
        End Sub

        Public Shared Sub SetLanguage(ByVal value As String)
            Try
                Dim Response As HttpResponse = HttpContext.Current.Response
                If Response Is Nothing Then
                    Return
                End If

                ' save the pageculture as a cookie
                Dim cookie As System.Web.HttpCookie = Nothing
                cookie = Response.Cookies.Get("language")
                If (cookie Is Nothing) Then
                    If value <> "" Then
                        cookie = New System.Web.HttpCookie("language", value)
                        Response.Cookies.Add(cookie)
                    End If
                Else
                    cookie.Value = value
                    If value <> "" Then
                        Response.Cookies.Set(cookie)
                    Else
                        Response.Cookies.Remove("language")
                    End If
                End If

            Catch
                Return
            End Try
        End Sub

#End Region

#Region "Obsolete"

        <Obsolete("Deprecated in DNN 5.0. Replaced by GetLocales().")> _
        Public Shared Function GetEnabledLocales() As LocaleCollection
            Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
            Dim enabledLocales As New LocaleCollection
            For Each kvp As KeyValuePair(Of String, Locale) In GetLocales(objPortalSettings.PortalId)
                enabledLocales.Add(kvp.Key, kvp.Value)
            Next
            Return enabledLocales
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by GetLocales().")> _
        Public Shared Function GetSupportedLocales() As LocaleCollection
            Dim supportedLocales As New LocaleCollection
            For Each kvp As KeyValuePair(Of String, Locale) In GetLocales(Null.NullInteger)
                supportedLocales.Add(kvp.Key, kvp.Value)
            Next
            Return supportedLocales
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by LocalizeControlTitle(IModuleControl).")> _
        Public Shared Function LocalizeControlTitle(ByVal ControlTitle As String, ByVal ControlSrc As String, ByVal Key As String) As String
            Dim reskey As String
            reskey = "ControlTitle_" + Key.ToLower + ".Text"
            Dim ResFile As String = ControlSrc.Substring(0, ControlSrc.LastIndexOf("/") + 1) + LocalResourceDirectory + ControlSrc.Substring(ControlSrc.LastIndexOf("/"), ControlSrc.LastIndexOf(".") - ControlSrc.LastIndexOf("/"))
            If ResFile.StartsWith("DesktopModules") Then
                ResFile = "~/" + ResFile
            End If
            Dim localizedvalue As String = Services.Localization.Localization.GetString(reskey, ResFile)
            If Not localizedvalue Is Nothing Then
                Return localizedvalue
            Else
                Return ControlTitle
            End If
        End Function

        <Obsolete("Deprecated in DNN 5.0. This does nothing now as the Admin Tabs are treated like any other tab.")> _
        Public Shared Sub LocalizePortalSettings()
        End Sub

        <Obsolete("Deprecated in DNN 5.0. Replaced by Host.EnableBrowserLanguage OR PortalSettings.EnableBrowserLanguage")> _
        Public Shared Function UseBrowserLanguage() As Boolean
            Return PortalController.GetCurrentPortalSettings().EnableBrowserLanguage
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by Host.EnableUrlLanguage OR PortalSettings.EnableUrlLanguage")> _
        Public Shared Function UseLanguageInUrl() As Boolean
            Return PortalController.GetCurrentPortalSettings().EnableUrlLanguage
        End Function

#End Region

    End Class

End Namespace
