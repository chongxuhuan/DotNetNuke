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
Imports System.Collections.Generic
Imports System.Data
Imports DotNetNuke.Entities.Tabs
Imports System.Xml
Imports DotNetNuke.Entities.Modules.Definitions
Imports System.Xml.Serialization
Imports System.IO
Imports DotNetNuke.Services.EventQueue
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : ModuleController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleController provides the Business Layer for Modules
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleController

#Region "Private Members"

        Private Shared dataProvider As DataProvider = dataProvider.Instance()

#End Region

#Region "Private Methods"

        Private Sub ClearCache(ByVal TabId As Integer)
            DataCache.ClearModuleCache(TabId)
        End Sub

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds module content to the node module
        ''' </summary>
        ''' <param name="nodeModule">Node where to add the content</param>
        ''' <param name="objModule">Module</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddContent(ByVal nodeModule As XmlNode, ByVal objModule As ModuleInfo)
            Dim xmlattr As XmlAttribute

            If objModule.DesktopModule.BusinessControllerClass <> "" And objModule.DesktopModule.IsPortable Then
                Try
                    Dim objObject As Object = Framework.Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass)
                    If TypeOf objObject Is IPortable Then
                        Dim Content As String = CType(CType(objObject, IPortable).ExportModule(objModule.ModuleID), String)
                        If Content <> "" Then
                            ' add attributes to XML document
                            Dim newnode As XmlNode = nodeModule.OwnerDocument.CreateElement("content")
                            xmlattr = nodeModule.OwnerDocument.CreateAttribute("type")
                            xmlattr.Value = CleanName(objModule.DesktopModule.ModuleName)
                            newnode.Attributes.Append(xmlattr)
                            xmlattr = nodeModule.OwnerDocument.CreateAttribute("version")
                            xmlattr.Value = objModule.DesktopModule.Version
                            newnode.Attributes.Append(xmlattr)

                            Content = HttpContext.Current.Server.HtmlEncode(Content)
                            newnode.InnerXml = XmlUtils.XMLEncode(Content)

                            nodeModule.AppendChild(newnode)
                        End If
                    End If
                Catch
                    'ignore errors
                End Try
            End If
        End Sub

        Private Shared Function CheckIsInstance(ByVal templateModuleID As Integer, ByVal hModules As Hashtable) As Boolean
            ' will be instance or module?
            Dim IsInstance As Boolean = False
            If templateModuleID > 0 Then
                If Not hModules(templateModuleID) Is Nothing Then
                    ' this module has already been processed -> process as instance
                    IsInstance = True
                End If
            End If

            Return IsInstance
        End Function

        Private Shared Sub CreateEventQueueMessage(ByVal objModule As ModuleInfo, ByVal strContent As String, ByVal strVersion As String, ByVal userID As Integer)
            Dim oAppStartMessage As New EventQueue.EventMessage
            oAppStartMessage.Priority = MessagePriority.High
            oAppStartMessage.ExpirationDate = Now.AddYears(-1)
            oAppStartMessage.SentDate = System.DateTime.Now
            oAppStartMessage.Body = ""
            oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke"
            oAppStartMessage.ProcessorCommand = "ImportModule"

            'Add custom Attributes for this message
            oAppStartMessage.Attributes.Add("BusinessControllerClass", objModule.DesktopModule.BusinessControllerClass)
            oAppStartMessage.Attributes.Add("ModuleId", objModule.ModuleID.ToString())
            oAppStartMessage.Attributes.Add("Content", strContent)
            oAppStartMessage.Attributes.Add("Version", strVersion)
            oAppStartMessage.Attributes.Add("UserID", userID.ToString)

            'send it to occur on next App_Start Event
            EventQueueController.SendMessage(oAppStartMessage, "Application_Start_FirstRequest")
        End Sub

        Private Shared Function DeserializeModule(ByVal nodeModule As XmlNode, ByVal nodePane As XmlNode, ByVal PortalId As Integer, ByVal TabId As Integer, ByVal ModuleDefId As Integer) As ModuleInfo
            Dim objModules As New ModuleController

            'Create New Module
            Dim objModule As New ModuleInfo
            objModule.PortalID = PortalId
            objModule.TabID = TabId
            objModule.ModuleOrder = -1
            objModule.ModuleTitle = XmlUtils.GetNodeValue(nodeModule, "title")
            objModule.PaneName = XmlUtils.GetNodeValue(nodePane, "name")
            objModule.ModuleDefID = ModuleDefId
            objModule.CacheTime = XmlUtils.GetNodeValueInt(nodeModule, "cachetime")
            objModule.CacheMethod = XmlUtils.GetNodeValue(nodeModule, "cachemethod")
            objModule.Alignment = XmlUtils.GetNodeValue(nodeModule, "alignment")
            objModule.IconFile = ImportFile(PortalId, XmlUtils.GetNodeValue(nodeModule, "iconfile"))
            objModule.AllTabs = XmlUtils.GetNodeValueBoolean(nodeModule, "alltabs")
            Select Case XmlUtils.GetNodeValue(nodeModule, "visibility")
                Case "Maximized" : objModule.Visibility = VisibilityState.Maximized
                Case "Minimized" : objModule.Visibility = VisibilityState.Minimized
                Case "None" : objModule.Visibility = VisibilityState.None
            End Select
            objModule.Color = XmlUtils.GetNodeValue(nodeModule, "color", "")
            objModule.Border = XmlUtils.GetNodeValue(nodeModule, "border", "")
            objModule.Header = XmlUtils.GetNodeValue(nodeModule, "header", "")
            objModule.Footer = XmlUtils.GetNodeValue(nodeModule, "footer", "")
            objModule.InheritViewPermissions = XmlUtils.GetNodeValueBoolean(nodeModule, "inheritviewpermissions", False)

            objModule.StartDate = XmlUtils.GetNodeValueDate(nodeModule, "startdate", Null.NullDate)
            objModule.EndDate = XmlUtils.GetNodeValueDate(nodeModule, "enddate", Null.NullDate)

            If XmlUtils.GetNodeValue(nodeModule, "containersrc", "") <> "" Then
                objModule.ContainerSrc = XmlUtils.GetNodeValue(nodeModule, "containersrc", "")
            End If
            objModule.DisplayTitle = XmlUtils.GetNodeValueBoolean(nodeModule, "displaytitle", True)
            objModule.DisplayPrint = XmlUtils.GetNodeValueBoolean(nodeModule, "displayprint", True)
            objModule.DisplaySyndicate = XmlUtils.GetNodeValueBoolean(nodeModule, "displaysyndicate", False)
            objModule.IsWebSlice = XmlUtils.GetNodeValueBoolean(nodeModule, "iswebslice", False)
            If objModule.IsWebSlice Then
                objModule.WebSliceTitle = XmlUtils.GetNodeValue(nodeModule, "webslicetitle", objModule.ModuleTitle)
                objModule.WebSliceExpiryDate = XmlUtils.GetNodeValueDate(nodeModule, "websliceexpirydate", objModule.EndDate)
                objModule.WebSliceTTL = XmlUtils.GetNodeValueInt(nodeModule, "webslicettl", objModule.CacheTime \ 60)
            End If

            Return objModule
        End Function

        Private Shared Sub DeserializeModulePermissions(ByVal nodeModulePermissions As XmlNodeList, ByVal PortalId As Integer, ByVal TabId As Integer, ByVal objModule As ModuleInfo)
            Dim objRoleController As New RoleController
            Dim objPermissionController As New PermissionController
            Dim objPermission As PermissionInfo
            Dim PermissionID As Integer
            Dim arrPermissions As ArrayList
            Dim i As Integer

            For Each node As XmlNode In nodeModulePermissions
                Dim PermissionKey As String = XmlUtils.GetNodeValue(node, "permissionkey")
                Dim PermissionCode As String = XmlUtils.GetNodeValue(node, "permissioncode")
                Dim RoleName As String = XmlUtils.GetNodeValue(node, "rolename")
                Dim AllowAccess As Boolean = XmlUtils.GetNodeValueBoolean(node, "allowaccess")

                Dim RoleID As Integer = Integer.MinValue
                Select Case RoleName
                    Case glbRoleAllUsersName
                        RoleID = Convert.ToInt32(glbRoleAllUsers)
                    Case Common.Globals.glbRoleUnauthUserName
                        RoleID = Convert.ToInt32(glbRoleUnauthUser)
                    Case Else
                        Dim objRole As RoleInfo = objRoleController.GetRoleByName(PortalId, RoleName)
                        If Not objRole Is Nothing Then
                            RoleID = objRole.RoleID
                        End If
                End Select
                If RoleID <> Integer.MinValue Then
                    PermissionID = -1
                    arrPermissions = objPermissionController.GetPermissionByCodeAndKey(PermissionCode, PermissionKey)

                    For i = 0 To arrPermissions.Count - 1
                        objPermission = CType(arrPermissions(i), PermissionInfo)
                        PermissionID = objPermission.PermissionID
                    Next

                    ' if role was found add, otherwise ignore
                    If PermissionID <> -1 Then
                        Dim objModulePermission As New ModulePermissionInfo
                        objModulePermission.ModuleID = objModule.ModuleID
                        objModulePermission.PermissionID = PermissionID
                        objModulePermission.RoleID = RoleID
                        objModulePermission.AllowAccess = Convert.ToBoolean(XmlUtils.GetNodeValue(node, "allowaccess"))
                        objModule.ModulePermissions.Add(objModulePermission)
                    End If
                End If
            Next
        End Sub

        Private Shared Function FindModule(ByVal nodeModule As XmlNode, ByVal TabId As Integer, ByVal mergeTabs As PortalTemplateModuleAction) As Boolean
            Dim objModules As New ModuleController
            Dim dicModules As Dictionary(Of Integer, ModuleInfo) = objModules.GetTabModules(TabId)
            Dim objModule As ModuleInfo

            Dim moduleFound As Boolean = False
            Dim modTitle As String = XmlUtils.GetNodeValue(nodeModule, "title")
            If mergeTabs = PortalTemplateModuleAction.Merge Then
                For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In dicModules
                    objModule = kvp.Value
                    If modTitle = objModule.ModuleTitle Then
                        moduleFound = True
                        Exit For
                    End If
                Next
            End If

            Return moduleFound
        End Function

        Private Shared Sub GetModuleContent(ByVal nodeModule As XmlNode, ByVal ModuleId As Integer, ByVal TabId As Integer, ByVal PortalId As Integer)
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModule(ModuleId, TabId, True)
            Dim strVersion As String = nodeModule.SelectSingleNode("content").Attributes.ItemOf("version").Value
            Dim strType As String = nodeModule.SelectSingleNode("content").Attributes.ItemOf("type").Value
            Dim strcontent As String = nodeModule.SelectSingleNode("content").InnerXml
            strcontent = strcontent.Substring(9, strcontent.Length - 12)

            If objModule.DesktopModule.BusinessControllerClass <> "" And strcontent <> "" Then
                Dim objportal As PortalInfo
                Dim objportals As New PortalController
                objportal = objportals.GetPortal(PortalId)

                'Determine if the Module is copmpletely installed 
                '(ie are we running in the same request that installed the module).
                If objModule.DesktopModule.SupportedFeatures = Null.NullInteger Then
                    ' save content in eventqueue for processing after an app restart,
                    ' as modules Supported Features are not updated yet so we
                    ' cannot determine if the module supports IsPortable
                    CreateEventQueueMessage(objModule, strcontent, strVersion, objportal.AdministratorId)
                Else
                    strcontent = HttpContext.Current.Server.HtmlDecode(strcontent)
                    If objModule.DesktopModule.IsPortable Then
                        Try
                            Dim objObject As Object = Framework.Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass)
                            If TypeOf objObject Is IPortable Then
                                CType(objObject, IPortable).ImportModule(objModule.ModuleID, strcontent, strVersion, objportal.AdministratorId)
                            End If
                        Catch
                            'if there is an error then the type cannot be loaded at this time, so add to EventQueeue
                            CreateEventQueueMessage(objModule, strcontent, strVersion, objportal.AdministratorId)
                        End Try
                    End If
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTabModulesCallBack gets a Dictionary of Modules by 
        ''' Tab from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetTabModulesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim tabID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Return CBO.FillDictionary(Of Integer, ModuleInfo)("ModuleID", dataProvider.GetTabModules(tabID), New Dictionary(Of Integer, ModuleInfo))
        End Function

        Private Shared Function GetModuleDefinition(ByVal nodeModule As XmlNode) As ModuleDefinitionInfo
            Dim objModuleDefinition As ModuleDefinitionInfo = Nothing

            ' Templates prior to v4.3.5 only have the <definition> node to define the Module Type
            ' This <definition> node was populated with the DesktopModuleInfo.ModuleName property
            ' Thus there is no mechanism to determine to which module definition the module belongs.
            '
            ' Template from v4.3.5 on also have the <moduledefinition> element that is populated
            ' with the ModuleDefinitionInfo.FriendlyName.  Therefore the module Instance identifies
            ' which Module Definition it belongs to.

            'Get the DesktopModule defined by the <definition> element
            Dim objDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName(XmlUtils.GetNodeValue(nodeModule, "definition"), Null.NullInteger)
            If Not objDesktopModule Is Nothing Then

                'Get the moduleDefinition from the <moduledefinition> element
                Dim friendlyName As String = XmlUtils.GetNodeValue(nodeModule, "moduledefinition")

                If String.IsNullOrEmpty(friendlyName) Then
                    'Module is pre 4.3.5 so get the first Module Definition (at least it won't throw an error then)
                    For Each objModuleDefinition In ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(objDesktopModule.DesktopModuleID).Values
                        Exit For
                    Next
                Else
                    'Module is 4.3.5 or later so get the Module Defeinition by its friendly name
                    objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(friendlyName, objDesktopModule.DesktopModuleID)
                End If
            End If

            Return objModuleDefinition
        End Function

#End Region

#Region "Public Shared Methods"

        Private Shared Sub DeserializeModuleSettings(ByVal nodeModuleSettings As XmlNodeList, ByVal objModule As ModuleInfo)
            Dim oModuleSettingNode As XmlNode
            Dim sKey As String
            Dim sValue As String

            For Each oModuleSettingNode In nodeModuleSettings
                sKey = XmlUtils.GetNodeValue(oModuleSettingNode, "settingname")
                sValue = XmlUtils.GetNodeValue(oModuleSettingNode, "settingvalue")

                objModule.ModuleSettings(sKey) = sValue
            Next
        End Sub

        Private Shared Sub DeserializeTabModuleSettings(ByVal nodeTabModuleSettings As XmlNodeList, ByVal objModule As ModuleInfo)
            Dim oTabModuleSettingNode As XmlNode
            Dim sKey As String
            Dim sValue As String
            Dim mc As New ModuleController

            For Each oTabModuleSettingNode In nodeTabModuleSettings
                sKey = XmlUtils.GetNodeValue(oTabModuleSettingNode, "settingname")
                sValue = XmlUtils.GetNodeValue(oTabModuleSettingNode, "settingvalue")
                '  mc.UpdateTabModuleSettings()
                objModule.TabModuleSettings(sKey) = sValue
            Next
        End Sub

        Public Shared Sub DeserializeModule(ByVal nodeModule As XmlNode, ByVal nodePane As XmlNode, ByVal PortalId As Integer, ByVal TabId As Integer, ByVal mergeTabs As PortalTemplateModuleAction, ByVal hModules As Hashtable)
            Dim objModules As New ModuleController
            Dim objModuleDefinition As ModuleDefinitionInfo = GetModuleDefinition(nodeModule)
            Dim objModule As ModuleInfo
            Dim intModuleId As Integer

            ' will be instance or module?
            Dim templateModuleID As Integer = XmlUtils.GetNodeValueInt(nodeModule, "moduleID")
            Dim IsInstance As Boolean = CheckIsInstance(templateModuleID, hModules)

            If Not objModuleDefinition Is Nothing Then
                'If Mode is Merge Check if Module exists
                If Not FindModule(nodeModule, TabId, mergeTabs) Then
                    objModule = DeserializeModule(nodeModule, nodePane, PortalId, TabId, objModuleDefinition.ModuleDefID)

                    'deserialize Module's settings
                    Dim nodeModuleSettings As XmlNodeList = nodeModule.SelectNodes("modulesettings/modulesetting")
                    DeserializeModuleSettings(nodeModuleSettings, objModule)

                    Dim nodeTabModuleSettings As XmlNodeList = nodeModule.SelectNodes("tabmodulesettings/tabmodulesetting")
                    DeserializeTabModuleSettings(nodeTabModuleSettings, objModule)

                    If Not IsInstance Then
                        'Add new module
                        intModuleId = objModules.AddModule(objModule)
                        If templateModuleID > 0 Then
                            hModules.Add(templateModuleID, intModuleId)
                        End If
                    Else
                        'Add instance
                        objModule.ModuleID = Convert.ToInt32(hModules(templateModuleID))
                        intModuleId = objModules.AddModule(objModule)
                    End If

                    If XmlUtils.GetNodeValue(nodeModule, "content") <> "" And Not IsInstance Then
                        GetModuleContent(nodeModule, intModuleId, TabId, PortalId)
                    End If

                    ' Process permissions only once
                    If Not IsInstance Then
                        Dim nodeModulePermissions As XmlNodeList = nodeModule.SelectNodes("modulepermissions/permission")
                        DeserializeModulePermissions(nodeModulePermissions, PortalId, TabId, objModule)

                        'Persist the permissions to the Data base
                        ModulePermissionController.SaveModulePermissions(objModule)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' SerializeModule
        ''' </summary>
        ''' <param name="xmlModule">The Xml Document to use for the Module</param>
        ''' <param name="objModule">The ModuleInfo object to serialize</param>
        ''' <param name="includeContent">A flak that determines whether the content of the module is serialised.</param>
        Public Shared Function SerializeModule(ByVal xmlModule As XmlDocument, ByVal objModule As ModuleInfo, ByVal includeContent As Boolean) As XmlNode
            Dim xserModules As New XmlSerializer(GetType(ModuleInfo))
            Dim nodeModule, nodeDefinition, newnode As XmlNode

            Dim objmodules As New ModuleController

            Dim sw As New StringWriter
            xserModules.Serialize(sw, objModule)

            xmlModule.LoadXml(sw.GetStringBuilder().ToString())
            nodeModule = xmlModule.SelectSingleNode("module")
            nodeModule.Attributes.Remove(nodeModule.Attributes.ItemOf("xmlns:xsd"))
            nodeModule.Attributes.Remove(nodeModule.Attributes.ItemOf("xmlns:xsi"))

            'remove unwanted elements
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("portalid"))
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("tabid"))
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("tabmoduleid"))
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("moduleorder"))
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("panename"))
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("isdeleted"))

            For Each nodePermission As XmlNode In nodeModule.SelectNodes("modulepermissions/permission")
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("modulepermissionid"))
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("permissionid"))
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("moduleid"))
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("roleid"))
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("userid"))
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("username"))
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("displayname"))
            Next

            If includeContent Then
                AddContent(nodeModule, objModule)
            End If

            'serialize ModuleSettings and TabModuleSettings
            XmlUtils.SerializeHashtable(objModule.ModuleSettings, xmlModule, nodeModule, "modulesetting", "settingname", "settingvalue")
            XmlUtils.SerializeHashtable(objModule.TabModuleSettings, xmlModule, nodeModule, "tabmodulesetting", "settingname", "settingvalue")

            newnode = xmlModule.CreateElement("definition")

            Dim objModuleDef As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByID(objModule.ModuleDefID)
            newnode.InnerText = DesktopModuleController.GetDesktopModule(objModuleDef.DesktopModuleID, objModule.PortalID).ModuleName
            nodeModule.AppendChild(newnode)

            'Add Module Definition Info
            nodeDefinition = xmlModule.CreateElement("moduledefinition")
            nodeDefinition.InnerText = objModuleDef.FriendlyName
            nodeModule.AppendChild(nodeDefinition)

            Return nodeModule
        End Function

        Public Shared Sub SynchronizeModule(ByVal moduleID As Integer)
            Dim objModules As New ModuleController
            Dim arrModules As ArrayList = objModules.GetModuleTabs(moduleID)
            Dim tabController As New TabController
            Dim tabSettings As Hashtable
            For Each objModule As ModuleInfo In arrModules
                tabSettings = tabController.GetTabSettings(objModule.TabID)
                If tabSettings("CacheProvider") IsNot Nothing AndAlso tabSettings("CacheProvider").ToString().Length > 0 Then
                    Dim provider As OutputCache.OutputCachingProvider = OutputCache.OutputCachingProvider.Instance(tabSettings("CacheProvider").ToString())
                    If provider IsNot Nothing Then
                        provider.Remove(objModule.TabID)
                    End If
                End If

                If HttpContext.Current IsNot Nothing Then
                    Dim provider As ModuleCache.ModuleCachingProvider = ModuleCache.ModuleCachingProvider.Instance(objModule.GetEffectiveCacheMethod)
                    If provider IsNot Nothing Then
                        provider.Remove(objModule.TabModuleID)
                    End If
                End If
            Next
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' add a module to a page
        ''' </summary>
        ''' <param name="objModule">moduleInfo for the module to create</param>
        ''' <returns>ID of the created module</returns>
        ''' <history>
        '''    [sleupold] 2007-09-24 documented
        ''' </history>
        Public Function AddModule(ByVal objModule As ModuleInfo) As Integer
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            ' add module
            If Null.IsNull(objModule.ModuleID) Then
                Dim typeController As IContentTypeController = New ContentTypeController
                Dim contentType As ContentType = (From t In typeController.GetContentTypes() _
                                                 Where t.ContentType = "Module" _
                                                 Select t) _
                                                 .SingleOrDefault

                Dim contentController As IContentController = New ContentController()
                objModule.Content = objModule.ModuleTitle
                objModule.ContentTypeId = contentType.ContentTypeId
                objModule.Indexed = False
                Dim contentItemID As Integer = contentController.AddContentItem(objModule)

                'Add Module
                objModule.ModuleID = dataProvider.AddModule(contentItemID, objModule.PortalID, objModule.ModuleDefID, objModule.ModuleTitle, objModule.AllTabs, objModule.Header, objModule.Footer, objModule.StartDate, objModule.EndDate, objModule.InheritViewPermissions, objModule.IsDeleted, UserController.GetCurrentUserInfo.UserID)

                'Now we have the ModuleID - update the contentItem
                contentController.UpdateContentItem(objModule)

                objEventLog.AddLog(objModule, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED)
                ' set module permissions
                ModulePermissionController.SaveModulePermissions(objModule)
            End If

            'Lets see if the module already exists
            Dim tmpModule As ModuleInfo = GetModule(objModule.ModuleID, objModule.TabID)
            If tmpModule IsNot Nothing Then
                'Module Exists already
                If tmpModule.IsDeleted Then
                    'Restore Module
                    RestoreModule(objModule)
                End If
            Else
                ' add tabmodule
                dataProvider.AddTabModule(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName, objModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo.UserID)
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("TabId", objModule.TabID.ToString))
                objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("ModuleID", objModule.ModuleID.ToString))
                objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.TABMODULE_CREATED.ToString
                objEventLog.AddLog(objEventLogInfo)

                If objModule.ModuleOrder = -1 Then
                    ' position module at bottom of pane
                    UpdateModuleOrder(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName)
                Else
                    ' position module in pane
                    UpdateTabModuleOrder(objModule.TabID)
                End If
            End If

            'Save ModuleSettings
            UpdateModuleSettings(objModule)

            'Save ModuleSettings
            If objModule.TabModuleID = -1 Then
                If tmpModule Is Nothing Then tmpModule = GetModule(objModule.ModuleID, objModule.TabID)
                objModule.TabModuleID = tmpModule.TabModuleID
            End If
            UpdateTabModuleSettings(objModule)

            ClearCache(objModule.TabID)

            Return objModule.ModuleID

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CopyModule copies a Module from one Tab to another optionally including all the 
        '''	TabModule settings
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="moduleId">The Id of the module to copy</param>
        '''	<param name="fromTabId">The Id of the source tab</param>
        '''	<param name="toTabId">The Id of the destination tab</param>
        '''	<param name="toPaneName">The name of the Pane on the destination tab where the module will end up</param>
        '''	<param name="includeSettings">A flag to indicate whether the settings are copied to the new Tab</param>
        ''' <history>
        ''' 	[cnurse]	10/21/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CopyModule(ByVal moduleId As Integer, ByVal fromTabId As Integer, ByVal toTabId As Integer, ByVal toPaneName As String, ByVal includeSettings As Boolean)
            'First Get the Module itself
            Dim objModule As ModuleInfo = GetModule(moduleId, fromTabId, False)

            'If the Destination Pane Name is not set, assume the same name as the source
            If toPaneName = "" Then
                toPaneName = objModule.PaneName
            End If

            'This will fail if the page already contains this module
            Try
                'Add a copy of the module to the bottom of the Pane for the new Tab
                dataProvider.AddTabModule(toTabId, moduleId, -1, toPaneName, objModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo.UserID)

                'Optionally copy the TabModuleSettings
                If includeSettings Then
                    Dim toModule As ModuleInfo = GetModule(moduleId, toTabId, False)
                    CopyTabModuleSettings(objModule, toModule)
                End If
            Catch
                ' module already in the page, ignore error
            End Try

            ClearCache(fromTabId)
            ClearCache(toTabId)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CopyModule copies a Module from one Tab to a collection of Tabs optionally
        '''	including the TabModule settings
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="moduleId">The Id of the module to copy</param>
        '''	<param name="fromTabId">The Id of the source tab</param>
        '''	<param name="toTabs">An ArrayList of TabItem objects</param>
        '''	<param name="includeSettings">A flag to indicate whether the settings are copied to the new Tab</param>
        ''' <history>
        ''' 	[cnurse]	2004-10-22	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CopyModule(ByVal moduleId As Integer, ByVal fromTabId As Integer, ByVal toTabs As List(Of TabInfo), ByVal includeSettings As Boolean)
            'Iterate through collection copying the module to each Tab (except the source)
            For Each objTab As TabInfo In toTabs
                If objTab.TabID <> fromTabId Then
                    CopyModule(moduleId, fromTabId, objTab.TabID, "", includeSettings)
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CopyTabModuleSettings copies the TabModuleSettings from one instance to another
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="fromModule">The module to copy from</param>
        '''	<param name="toModule">The module to copy to</param>
        ''' <history>
        ''' 	[cnurse]	2005-01-11	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CopyTabModuleSettings(ByVal fromModule As ModuleInfo, ByVal toModule As ModuleInfo)

            'Get the TabModuleSettings
            Dim settings As Hashtable = GetTabModuleSettings(fromModule.TabModuleID)

            'Copy each setting to the new TabModule instance
            For Each setting As DictionaryEntry In settings
                UpdateTabModuleSetting(toModule.TabModuleID, CType(setting.Key, String), CType(setting.Value, String))
            Next

        End Sub

        Public Sub CreateContentItem(ByVal updatedModule As ModuleInfo)
            Dim typeController As IContentTypeController = New ContentTypeController
            Dim contentType As ContentType = (From t In typeController.GetContentTypes() _
                                             Where t.ContentType = "Module" _
                                             Select t) _
                                             .SingleOrDefault
            'This module does not have a valid ContentItem
            'create ContentItem
            Dim contentController As IContentController = DotNetNuke.Entities.Content.Common.GetContentController()
            updatedModule.Content = updatedModule.ModuleTitle
            updatedModule.Indexed = False
            updatedModule.ContentTypeId = contentType.ContentTypeId
            updatedModule.ContentItemId = contentController.AddContentItem(updatedModule)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteAllModules deletes all instances of a Module (from a collection).  This overload
        ''' soft deletes the instances
        ''' </summary>
        '''	<param name="moduleId">The Id of the module to copy</param>
        '''	<param name="tabId">The Id of the current tab</param>
        '''	<param name="fromTabs">An ArrayList of TabItem objects</param>
        ''' <history>
        ''' 	[cnurse]	2009-03-24	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteAllModules(ByVal moduleId As Integer, ByVal tabId As Integer, ByVal fromTabs As List(Of TabInfo))
            DeleteAllModules(moduleId, tabId, fromTabs, True, False, False)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteAllModules deletes all instances of a Module (from a collection), optionally excluding the
        '''	current instance, and optionally including deleting the Module itself.
        ''' </summary>
        ''' <remarks>
        '''	Note - the base module is not removed unless both the flags are set, indicating
        '''	to delete all instances AND to delete the Base Module
        ''' </remarks>
        '''	<param name="moduleId">The Id of the module to copy</param>
        '''	<param name="tabId">The Id of the current tab</param>
        ''' <param name="softDelete">A flag that determines whether the instance should be soft-deleted</param>
        '''	<param name="fromTabs">An ArrayList of TabItem objects</param>
        '''	<param name="includeCurrent">A flag to indicate whether to delete from the current tab
        '''		as identified ny tabId</param>
        '''	<param name="deleteBaseModule">A flag to indicate whether to delete the Module itself</param>
        ''' <history>
        ''' 	[cnurse]	2004-10-22	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteAllModules(ByVal moduleId As Integer, ByVal tabId As Integer, ByVal fromTabs As List(Of TabInfo), ByVal softDelete As Boolean, ByVal includeCurrent As Boolean, ByVal deleteBaseModule As Boolean)
            'Iterate through collection deleting the module from each Tab (except the current)
            For Each objTab As TabInfo In fromTabs
                If objTab.TabID <> tabId OrElse includeCurrent Then
                    DeleteTabModule(objTab.TabID, moduleId, softDelete)
                End If
            Next

            'Optionally delete the Module
            If includeCurrent AndAlso deleteBaseModule AndAlso Not softDelete Then
                DeleteModule(moduleId)
            End If

            ClearCache(tabId)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Delete a module instance permanently from the database
        ''' </summary>
        ''' <param name="ModuleId">ID of the module instance</param>
        ''' <history>
        '''    [sleupold]   1007-09-24 documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteModule(ByVal moduleId As Integer)
            'Get the module
            Dim objModule As ModuleInfo = GetModule(moduleId)

            'Delete Module
            dataProvider.DeleteModule(moduleId)

            'Remove the Content Item
            If objModule.ContentItemId > Null.NullInteger Then
                Dim ctl As IContentController = DotNetNuke.Entities.Content.Common.GetContentController()
                ctl.DeleteContentItem(objModule)
            End If

            'Log deletion
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("ModuleId", moduleId.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.MODULE_DELETED)

            'Delete Search Items for this Module
            dataProvider.DeleteSearchItems(moduleId)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Delete a module reference permanently from the database.
        ''' if there are no other references, the module instance is deleted as well
        ''' </summary>
        ''' <param name="tabId">ID of the page</param>
        ''' <param name="moduleId">ID of the module instance</param>
        ''' <param name="softDelete">A flag that determines whether the instance should be soft-deleted</param>
        ''' <history>
        '''    [sleupold]   1007-09-24 documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteTabModule(ByVal tabId As Integer, ByVal moduleId As Integer, ByVal softDelete As Boolean)
            ' save moduleinfo
            Dim objModule As ModuleInfo = GetModule(moduleId, tabId, False)

            If Not objModule Is Nothing Then
                ' delete the module instance for the tab
                dataProvider.DeleteTabModule(tabId, moduleId, softDelete)
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("tabId", tabId.ToString))
                objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("moduleId", moduleId.ToString))
                objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.TABMODULE_DELETED.ToString
                objEventLog.AddLog(objEventLogInfo)

                ' reorder all modules on tab
                UpdateTabModuleOrder(tabId)

                ' check if all modules instances have been deleted
                If GetModule(moduleId, Null.NullInteger, True).TabID = Null.NullInteger Then
                    'hard delete the module
                    DeleteModule(moduleId)
                End If
            End If

            ClearCache(tabId)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' get info of all modules in any portal of the installation
        ''' </summary>
        ''' <returns>moduleInfo of all modules</returns>
        ''' <remarks>created for upgrade purposes</remarks>
        ''' <history>
        '''    [sleupold] 2007-09-24 documented
        '''</history>
        ''' -----------------------------------------------------------------------------
        Public Function GetAllModules() As ArrayList
            Return CBO.FillCollection(dataProvider.GetAllModules(), GetType(ModuleInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' get a Module object
        ''' </summary>
        ''' <param name="moduleID">ID of the module</param>
        ''' <returns>a ModuleInfo object - note that this method will always hit the database as no TabID cachekey is provided</returns>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetModule(ByVal moduleID As Integer) As ModuleInfo
            Return GetModule(moduleID, Null.NullInteger, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' get a Module object
        ''' </summary>
        ''' <param name="moduleID">ID of the module</param>
        ''' <param name="tabID">ID of the page</param>
        ''' <returns>a ModuleInfo object</returns>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetModule(ByVal moduleID As Integer, ByVal tabID As Integer) As ModuleInfo
            Return GetModule(moduleID, tabID, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' get a Module object
        ''' </summary>
        ''' <param name="moduleID">ID of the module</param>
        ''' <param name="tabID">ID of the page</param>
        ''' <param name="ignoreCache">flag, if data shall not be taken from cache</param>
        ''' <returns>ArrayList of ModuleInfo objects</returns>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetModule(ByVal moduleID As Integer, ByVal tabID As Integer, ByVal ignoreCache As Boolean) As ModuleInfo
            Dim modInfo As ModuleInfo = Nothing
            Dim bFound As Boolean = False
            If Not ignoreCache Then
                'First try the cache
                Dim dicModules As Dictionary(Of Integer, ModuleInfo) = GetTabModules(tabID)
                bFound = dicModules.TryGetValue(moduleID, modInfo)
            End If

            If ignoreCache Or Not bFound Then
                modInfo = CBO.FillObject(Of ModuleInfo)(dataProvider.GetModule(moduleID, tabID))
            End If
            Return modInfo
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' get all Module objects of a portal
        ''' </summary>
        ''' <param name="portalID">ID of the portal</param>
        ''' <returns>ArrayList of ModuleInfo objects</returns>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetModules(ByVal portalID As Integer) As ArrayList
            Return CBO.FillCollection(dataProvider.GetModules(portalID), GetType(ModuleInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' get Module objects of a portal, either only those, to be placed on all tabs or not
        ''' </summary>
        ''' <param name="portalID">ID of the portal</param>
        ''' <param name="allTabs">specify, whether to return modules to be shown on all tabs or those to be shown on specified tabs</param>
        ''' <returns>ArrayList of TabModuleInfo objects</returns>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetAllTabsModules(ByVal portalID As Integer, ByVal allTabs As Boolean) As ArrayList
            Return CBO.FillCollection(dataProvider.GetAllTabsModules(portalID, allTabs), GetType(ModuleInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get ModuleInfo object of first module instance with a given friendly name of the module definition
        ''' </summary>
        ''' <param name="PortalId">ID of the portal, where to look for the module</param>
        ''' <param name="FriendlyName">friendly name of module definition</param>
        ''' <returns>ModuleInfo of first module instance</returns>
        ''' <remarks>preferably used for admin and host modules</remarks>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetModuleByDefinition(ByVal portalID As Integer, ByVal friendlyName As String) As ModuleInfo
            ' declare return object
            Dim objModule As ModuleInfo = Nothing

            ' format cache key
            Dim key As String = String.Format(DataCache.ModuleCacheKey, portalID)

            ' get module dictionary from cache
            Dim modules As Dictionary(Of String, ModuleInfo) = DataCache.GetCache(Of Dictionary(Of String, ModuleInfo))(key)

            If modules Is Nothing Then
                ' create new dictionary
                modules = New Dictionary(Of String, ModuleInfo)
            End If

            ' return module if it exists
            If modules.ContainsKey(friendlyName) Then
                objModule = modules(friendlyName)
            Else
                ' clone the dictionary so that we have a local copy
                Dim clonemodules As Dictionary(Of String, ModuleInfo) = New Dictionary(Of String, ModuleInfo)
                For Each objModule In modules.Values
                    clonemodules(objModule.ModuleDefinition.FriendlyName) = objModule
                Next

                ' get from database
                Dim dr As IDataReader = dataProvider.Instance().GetModuleByDefinition(portalID, friendlyName)
                Try
                    ' hydrate object
                    objModule = CBO.FillObject(Of ModuleInfo)(dr)
                Finally
                    ' close connection
                    CBO.CloseDataReader(dr, True)
                End Try

                If objModule IsNot Nothing Then
                    ' add the module to the dictionary
                    clonemodules(objModule.ModuleDefinition.FriendlyName) = objModule

                    ' set module caching settings
                    Dim timeOut As Int32 = DataCache.ModuleCacheTimeOut * Convert.ToInt32(Host.Host.PerformanceSetting)

                    ' cache module dictionary
                    If timeOut > 0 Then
                        DataCache.SetCache(key, clonemodules, TimeSpan.FromMinutes(timeOut))
                    End If
                End If
            End If

            ' return module object
            Return objModule
        End Function

        Public Function GetModulesByDefinition(ByVal portalID As Integer, ByVal friendlyName As String) As ArrayList
            Return CBO.FillCollection(dataProvider.Instance().GetModuleByDefinition(portalID, friendlyName), GetType(ModuleInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' For a portal get a list of all active module and tabmodule references that support iSearchable
        ''' </summary>
        ''' <param name="portalID">ID of the portal to be searched</param>
        ''' <returns>Arraylist of ModuleInfo for modules supporting search.</returns>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSearchModules(ByVal portalID As Integer) As ArrayList
            Return CBO.FillCollection(dataProvider.GetSearchModules(portalID), GetType(ModuleInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get all Module references on a tab
        ''' </summary>
        ''' <param name="TabId"></param>
        ''' <returns>Dictionary of ModuleID and ModuleInfo</returns>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetTabModules(ByVal tabID As Integer) As Dictionary(Of Integer, ModuleInfo)
            Dim cacheKey As String = String.Format(DataCache.TabModuleCacheKey, tabID.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, ModuleInfo))(New CacheItemArgs(cacheKey, DataCache.TabModuleCacheTimeOut, DataCache.TabModuleCachePriority, tabID), _
                                                                                        AddressOf GetTabModulesCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MoveModule moes a Module from one Tab to another including all the 
        '''	TabModule settings
        ''' </summary>
        '''	<param name="moduleId">The Id of the module to move</param>
        '''	<param name="fromTabId">The Id of the source tab</param>
        '''	<param name="toTabId">The Id of the destination tab</param>
        '''	<param name="toPaneName">The name of the Pane on the destination tab where the module will end up</param>
        ''' <history>
        ''' 	[cnurse]	10/21/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub MoveModule(ByVal moduleId As Integer, ByVal fromTabId As Integer, ByVal toTabId As Integer, ByVal toPaneName As String)
            'Get Module
            Dim objModule As ModuleInfo = GetModule(moduleId, fromTabId)

            'Move the module to the Tab
            dataProvider.MoveTabModule(fromTabId, moduleId, toTabId, toPaneName, UserController.GetCurrentUserInfo.UserID)

            'Update Module Order for source tab
            UpdateTabModuleOrder(fromTabId)

            'Update Module Order for target tab
            UpdateTabModuleOrder(toTabId)
        End Sub

        Public Sub RestoreModule(ByVal objModule As ModuleInfo)
            dataProvider.RestoreTabModule(objModule.TabID, objModule.ModuleID)
            ClearCache(objModule.TabID)
        End Sub

        Private Sub UpdateModuleSettings(ByVal updatedModule As ModuleInfo)
            Dim sKey As String
            For Each sKey In updatedModule.ModuleSettings.Keys
                UpdateModuleSetting(updatedModule.ModuleID, sKey, CType(updatedModule.ModuleSettings(sKey), String))
            Next
        End Sub

        Private Sub UpdateTabModuleSettings(ByVal updatedTabModule As ModuleInfo)
            Dim sKey As String
            For Each sKey In updatedTabModule.TabModuleSettings.Keys
                UpdateTabModuleSetting(updatedTabModule.TabModuleID, sKey, CType(updatedTabModule.TabModuleSettings(sKey), String))
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Update module settings and permissions in database from ModuleInfo
        ''' </summary>
        ''' <param name="objModule">ModuleInfo of the module to update</param>
        ''' <history>
        '''    [sleupold]   2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateModule(ByVal objModule As ModuleInfo)
            'Update ContentItem If neccessary
            If objModule.ContentItemId = Null.NullInteger AndAlso objModule.ModuleID <> Null.NullInteger Then
                CreateContentItem(objModule)
            End If

            ' update module
            dataProvider.UpdateModule(objModule.ModuleID, objModule.ContentItemId, objModule.ModuleTitle, objModule.AllTabs, objModule.Header, objModule.Footer, objModule.StartDate, objModule.EndDate, objModule.InheritViewPermissions, objModule.IsDeleted, UserController.GetCurrentUserInfo.UserID)

            'Update Tags
            Dim termController As ITermController = DotNetNuke.Entities.Content.Common.GetTermController()
            termController.RemoveTermsFromContent(objModule)
            For Each _Term As Term In objModule.Terms
                termController.AddTermToContent(_Term, objModule)
            Next
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objModule, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_UPDATED)

            ' save module permissions
            ModulePermissionController.SaveModulePermissions(objModule)
            UpdateModuleSettings(objModule)

            If Not Null.IsNull(objModule.TabID) Then

                ' update tabmodule
                dataProvider.UpdateTabModule(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName, objModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(objModule, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.TABMODULE_UPDATED)
                ' update module order in pane
                UpdateModuleOrder(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName)

                ' set the default module
                If PortalSettings.Current IsNot Nothing Then
                    If objModule.IsDefaultModule Then
                        If objModule.ModuleID <> PortalSettings.Current.DefaultModuleId Then
                            'Update Setting
                            PortalController.UpdatePortalSetting(objModule.PortalID, "defaultmoduleid", objModule.ModuleID.ToString)
                        End If
                        If objModule.TabID <> PortalSettings.Current.DefaultTabId Then
                            'Update Setting
                            PortalController.UpdatePortalSetting(objModule.PortalID, "defaulttabid", objModule.TabID.ToString)
                        End If
                    Else
                        If objModule.ModuleID = PortalSettings.Current.DefaultModuleId AndAlso objModule.TabID = PortalSettings.Current.DefaultTabId Then
                            'Clear setting
                            PortalController.DeletePortalSetting(objModule.PortalID, "defaultmoduleid")
                            PortalController.DeletePortalSetting(objModule.PortalID, "defaulttabid")
                        End If
                    End If
                End If

                ' apply settings to all desktop modules in portal
                If objModule.AllModules Then
                    Dim objTabs As New TabController
                    For Each tabPair As KeyValuePair(Of Integer, TabInfo) In objTabs.GetTabsByPortal(objModule.PortalID)
                        Dim objTab As TabInfo = tabPair.Value
                        For Each modulePair As KeyValuePair(Of Integer, ModuleInfo) In GetTabModules(objTab.TabID)
                            Dim objTargetModule As ModuleInfo = modulePair.Value
                            dataProvider.UpdateTabModule(objTargetModule.TabID, objTargetModule.ModuleID, objTargetModule.ModuleOrder, objTargetModule.PaneName, objTargetModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo.UserID)
                        Next
                    Next
                End If
            End If

            'Clear Cache for all TabModules
            For Each tabModule As ModuleInfo In GetModuleTabs(objModule.ModuleID)
                ClearCache(tabModule.TabID)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' set/change the module position within a pane on a page
        ''' </summary>
        ''' <param name="TabId">ID of the page</param>
        ''' <param name="ModuleId">ID of the module on the page</param>
        ''' <param name="ModuleOrder">position within the controls list on page, -1 if to be added at the end</param>
        ''' <param name="PaneName">name of the pane, the module is placed in on the page</param>
        ''' <history>
        '''    [sleupold] 2007-09-24 commented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateModuleOrder(ByVal TabId As Integer, ByVal ModuleId As Integer, ByVal ModuleOrder As Integer, ByVal PaneName As String)
            Dim objModule As ModuleInfo = GetModule(ModuleId, TabId, False)
            If Not objModule Is Nothing Then
                ' adding a module to a new pane - places the module at the bottom of the pane 
                If ModuleOrder = -1 Then
                    Dim dr As IDataReader = Nothing
                    Try
                        dr = dataProvider.GetTabModuleOrder(TabId, PaneName)
                        While dr.Read
                            ModuleOrder = Convert.ToInt32(dr("ModuleOrder"))
                        End While
                    Catch ex As Exception
                        LogException(ex)
                    Finally
                        CBO.CloseDataReader(dr, True)
                    End Try
                    ModuleOrder += 2
                End If

                dataProvider.UpdateModuleOrder(TabId, ModuleId, ModuleOrder, PaneName)

                ' clear cache
                If objModule.AllTabs = False Then
                    ClearCache(TabId)
                Else
                    Dim objTabs As New TabController
                    For Each tabPair As KeyValuePair(Of Integer, TabInfo) In objTabs.GetTabsByPortal(objModule.PortalID)
                        Dim objTab As TabInfo = tabPair.Value
                        ClearCache(objTab.TabID)
                    Next
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' set/change all module's positions within a page
        ''' </summary>
        ''' <param name="TabId">ID of the page</param>
        ''' <history>
        '''    [sleupold]   2007-09-24 documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateTabModuleOrder(ByVal TabId As Integer)
            Dim ModuleCounter As Integer
            Dim dr As IDataReader = Nothing
            dr = dataProvider.GetTabPanes(TabId)
            Try
                While dr.Read
                    ModuleCounter = 0
                    Dim dr2 As IDataReader = Nothing
                    dr2 = dataProvider.GetTabModuleOrder(TabId, Convert.ToString(dr("PaneName")))
                    Try
                        While dr2.Read
                            ModuleCounter += 1
                            dataProvider.UpdateModuleOrder(TabId, Convert.ToInt32(dr2("ModuleID")), (ModuleCounter * 2) - 1, Convert.ToString(dr("PaneName")))
                        End While
                    Catch ex2 As Exception
                        LogException(ex2)
                    Finally
                        CBO.CloseDataReader(dr2, True)
                    End Try
                End While
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try

            'clear module cache
            ClearCache(TabId)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get a list of all TabModule references of a module instance
        ''' </summary>
        ''' <param name="ModuleID">ID of the Module</param>
        ''' <returns>ArrayList of ModuleInfo</returns>
        ''' <history>
        '''    [sleupold]   2007-09-24 documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetModuleTabs(ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(dataProvider.GetModule(moduleID, Null.NullInteger), GetType(ModuleInfo))
        End Function

#Region "ModuleSettings"

        ''' <summary>
        ''' read all settings for a module from ModuleSettings table
        ''' </summary>
        ''' <param name="ModuleId">ID of the module</param>
        ''' <returns>(cached) hashtable containing all settings</returns>
        ''' <remarks>TabModuleSettings are not included</remarks>
        ''' <history>
        '''    [sleupold] 2007-09-24 commented
        ''' </history>
        Public Function GetModuleSettings(ByVal ModuleId As Integer) As Hashtable
            Dim objSettings As Hashtable
            Dim strCacheKey As String = "GetModuleSettings" & ModuleId.ToString

            objSettings = CType(DataCache.GetCache(strCacheKey), Hashtable)

            If objSettings Is Nothing Then
                objSettings = New Hashtable
                Dim dr As IDataReader = Nothing
                Try
                    dr = dataProvider.GetModuleSettings(ModuleId)
                    While dr.Read()
                        If Not dr.IsDBNull(1) Then
                            objSettings(dr.GetString(0)) = dr.GetString(1)
                        Else
                            objSettings(dr.GetString(0)) = String.Empty
                        End If
                    End While
                Catch ex As Exception
                    LogException(ex)
                Finally
                    ' Ensure DataReader is closed
                    CBO.CloseDataReader(dr, True)
                End Try

                ' cache data
                Dim intCacheTimeout As Integer = 20 * Convert.ToInt32(Host.Host.PerformanceSetting)
                DataCache.SetCache(strCacheKey, objSettings, TimeSpan.FromMinutes(intCacheTimeout))
            End If

            Return objSettings
        End Function

        ''' <summary>
        ''' Adds or updates a module's setting value
        ''' </summary>
        ''' <param name="ModuleId">ID of the module, the setting belongs to</param>
        ''' <param name="SettingName">name of the setting property</param>
        ''' <param name="SettingValue">value of the setting (String).</param>
        ''' <remarks>empty SettingValue will remove the setting, if not preserveIfEmpty is true</remarks>
        ''' <history>
        '''    [sleupold] 2007-09-24 added removal for empty settings
        ''' </history>
        Public Sub UpdateModuleSetting(ByVal ModuleId As Integer, ByVal SettingName As String, ByVal SettingValue As String)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("ModuleId", ModuleId.ToString))
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString))
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("SettingValue", SettingValue.ToString))

            Dim dr As IDataReader = Nothing
            Try
                dr = dataProvider.GetModuleSetting(ModuleId, SettingName)
                If dr.Read Then
                    dataProvider.UpdateModuleSetting(ModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo.UserID)
                    objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_UPDATED.ToString
                    objEventLog.AddLog(objEventLogInfo)
                Else
                    dataProvider.AddModuleSetting(ModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo.UserID)
                    objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_CREATED.ToString
                    objEventLog.AddLog(objEventLogInfo)
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                ' Ensure DataReader is closed
                CBO.CloseDataReader(dr, True)
            End Try

            DataCache.RemoveCache("GetModuleSettings" & ModuleId.ToString)

        End Sub

        ''' <summary>
        ''' Delete a Setting of a module instance
        ''' </summary>
        ''' <param name="ModuleId">ID of the affected module</param>
        ''' <param name="SettingName">Name of the setting to be deleted</param>
        ''' <history>
        '''    [sleupold] 2007-09-24 documented
        ''' </history>
        Public Sub DeleteModuleSetting(ByVal ModuleId As Integer, ByVal SettingName As String)
            dataProvider.DeleteModuleSetting(ModuleId, SettingName)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("ModuleId", ModuleId.ToString))
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString))
            objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_DELETED.ToString
            objEventLog.AddLog(objEventLogInfo)
            DataCache.RemoveCache("GetModuleSettings" & ModuleId.ToString)
        End Sub

        ''' <summary>
        ''' Delete all Settings of a module instance
        ''' </summary>
        ''' <param name="ModuleId">ID of the affected module</param>
        ''' <history>
        '''    [sleupold] 2007-09-24 documented
        ''' </history>
        Public Sub DeleteModuleSettings(ByVal ModuleId As Integer)
            dataProvider.DeleteModuleSettings(ModuleId)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("ModuleId", ModuleId.ToString))
            objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_DELETED.ToString
            objEventLog.AddLog(objEventLogInfo)
            DataCache.RemoveCache("GetModuleSettings" & ModuleId.ToString)
        End Sub

#End Region

#Region "TabModuleSettings"

        ''' <summary>
        ''' read all settings for a module on a page from TabModuleSettings Table
        ''' </summary>
        ''' <param name="TabModuleId">ID of the tabModule</param>
        ''' <returns>(cached) hashtable containing all settings</returns>
        ''' <remarks>ModuleSettings are not included</remarks>
        ''' <history>
        '''    [sleupold] 2007-09-24 documented
        ''' </history>
        Public Function GetTabModuleSettings(ByVal TabModuleId As Integer) As Hashtable

            Dim strCacheKey As String = "GetTabModuleSettings" & TabModuleId.ToString
            Dim objSettings As Hashtable = CType(DataCache.GetCache(strCacheKey), Hashtable)

            If objSettings Is Nothing Then
                objSettings = New Hashtable
                Dim dr As IDataReader = Nothing
                Try
                    dr = dataProvider.GetTabModuleSettings(TabModuleId)
                    While dr.Read()
                        If Not dr.IsDBNull(1) Then
                            objSettings(dr.GetString(0)) = dr.GetString(1)
                        Else
                            objSettings(dr.GetString(0)) = String.Empty
                        End If
                    End While
                Catch ex As Exception
                    LogException(ex)
                Finally
                    ' Ensure DataReader is closed
                    CBO.CloseDataReader(dr, True)
                End Try

                ' cache data
                Dim intCacheTimeout As Integer = 20 * Convert.ToInt32(Host.Host.PerformanceSetting)
                DataCache.SetCache(strCacheKey, objSettings, TimeSpan.FromMinutes(intCacheTimeout))
            End If

            Return objSettings

        End Function

        ''' <summary>
        ''' Adds or updates a module's setting value
        ''' </summary>
        ''' <param name="TabModuleId">ID of the tabmodule, the setting belongs to</param>
        ''' <param name="SettingName">name of the setting property</param>
        ''' <param name="SettingValue">value of the setting (String).</param>
        ''' <remarks>empty SettingValue will relove the setting</remarks>
        ''' <history>
        '''    [sleupold] 2007-09-24 added removal for empty settings
        ''' </history>
        Public Sub UpdateTabModuleSetting(ByVal TabModuleId As Integer, ByVal SettingName As String, ByVal SettingValue As String)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("TabModuleId", TabModuleId.ToString))
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString))
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("SettingValue", SettingValue.ToString))

            Dim dr As IDataReader = Nothing
            Try
                dr = dataProvider.GetTabModuleSetting(TabModuleId, SettingName)
                If dr.Read Then
                    dataProvider.UpdateTabModuleSetting(TabModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo.UserID)
                    objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_UPDATED.ToString
                    objEventLog.AddLog(objEventLogInfo)
                Else
                    dataProvider.AddTabModuleSetting(TabModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo.UserID)
                    objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_CREATED.ToString
                    objEventLog.AddLog(objEventLogInfo)
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                ' Ensure DataReader is closed
                CBO.CloseDataReader(dr, True)
            End Try

            DataCache.RemoveCache("GetTabModuleSettings" & TabModuleId.ToString)
        End Sub

        ''' <summary>
        ''' Delete a specific setting of a tabmodule reference
        ''' </summary>
        ''' <param name="TabModuleId">ID of the affected tabmodule</param>
        ''' <param name="SettingName">Name of the setting to remove</param>
        ''' <history>
        '''    [sleupold] 2007-09-24 documented
        ''' </history>
        Public Sub DeleteTabModuleSetting(ByVal TabModuleId As Integer, ByVal SettingName As String)
            dataProvider.DeleteTabModuleSetting(TabModuleId, SettingName)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("TabModuleId", TabModuleId.ToString))
            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString))
            objEventLogInfo.LogTypeKey = Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_DELETED.ToString
            objEventLog.AddLog(objEventLogInfo)

            DataCache.RemoveCache("GetTabModuleSettings" & TabModuleId.ToString)
        End Sub

        ''' <summary>
        ''' Delete all settings of a tabmodule reference
        ''' </summary>
        ''' <param name="TabModuleId">ID of the affected tabmodule</param>
        ''' <history>
        '''    [sleupold] 2007-09-24 documented
        ''' </history>
        Public Sub DeleteTabModuleSettings(ByVal TabModuleId As Integer)
            dataProvider.DeleteTabModuleSettings(TabModuleId)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("TabModuleID", TabModuleId.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_DELETED)
            DataCache.RemoveCache("GetTabModuleSettings" & TabModuleId.ToString)
        End Sub

#End Region

#End Region

#Region "Obsolete"

        <Obsolete("Replaced in DotNetNuke 5.0 by CopyModule(Integer, integer, List(Of TabInfo), Boolean)")> _
        Public Sub CopyModule(ByVal moduleId As Integer, ByVal fromTabId As Integer, ByVal toTabs As ArrayList, ByVal includeSettings As Boolean)
            'Iterate through collection copying the module to each Tab (except the source)
            For Each objTab As TabInfo In toTabs
                If objTab.TabID <> fromTabId Then
                    CopyModule(moduleId, fromTabId, objTab.TabID, "", includeSettings)
                End If
            Next
        End Sub

        <Obsolete("Deprectaed in DNN 5.1.  Replaced By DeleteAllModules(Integer,Integer, List(Of TabInfo), Boolean, Boolean, Boolean)")> _
        Public Sub DeleteAllModules(ByVal moduleId As Integer, ByVal tabId As Integer, ByVal fromTabs As List(Of TabInfo), ByVal includeCurrent As Boolean, ByVal deleteBaseModule As Boolean)
            DeleteAllModules(moduleId, tabId, fromTabs, True, includeCurrent, deleteBaseModule)
        End Sub

        <Obsolete("Replaced in DotNetNuke 5.0 by DeleteAllModules(Integer, integer, List(Of TabInfo), Boolean, boolean)")> _
        Public Sub DeleteAllModules(ByVal moduleId As Integer, ByVal tabId As Integer, ByVal fromTabs As ArrayList, ByVal includeCurrent As Boolean, ByVal deleteBaseModule As Boolean)
            Dim listTabs As New List(Of TabInfo)
            For Each objTab As TabInfo In fromTabs
                listTabs.Add(objTab)
            Next
            DeleteAllModules(moduleId, tabId, listTabs, True, includeCurrent, deleteBaseModule)
        End Sub

        <Obsolete("Deprectaed in DNN 5.1. Replaced by DeleteTabModule(Integer, integer, boolean)")> _
        Public Sub DeleteTabModule(ByVal tabId As Integer, ByVal moduleId As Integer)
            DeleteTabModule(tabId, moduleId, True)
        End Sub

        <Obsolete("Replaced in DotNetNuke 5.0 by GetTabModules(Integer)")> _
        Public Function GetPortalTabModules(ByVal portalID As Integer, ByVal tabID As Integer) As ArrayList
            Dim arr As New ArrayList
            For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In GetTabModules(tabID)
                arr.Add(kvp.Value)
            Next
            Return arr
        End Function

        <Obsolete("Replaced in DotNetNuke 5.0 by GetModules(Integer)")> _
        Public Function GetModules(ByVal portalID As Integer, ByVal includePermissions As Boolean) As ArrayList
            Return CBO.FillCollection(dataProvider.GetModules(portalID), GetType(ModuleInfo))
        End Function

        <Obsolete("Replaced in DotNetNuke 5.0 by UpdateTabModuleOrder(Integer)")> _
        Public Sub UpdateTabModuleOrder(ByVal tabId As Integer, ByVal portalId As Integer)
            UpdateTabModuleOrder(tabId)
        End Sub

        <Obsolete("The module caching feature has been updated in version 5.2.0.  This method is no longer used.")> _
        Public Shared Function CacheDirectory() As String
            Return PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Cache"
        End Function

        <Obsolete("The module caching feature has been updated in version 5.2.0.  This method is no longer used.")> _
        Public Shared Function CacheFileName(ByVal TabModuleID As Integer) As String
            Dim strCacheKey As String = "TabModule:"
            strCacheKey += TabModuleID.ToString & ":"
            strCacheKey += System.Threading.Thread.CurrentThread.CurrentCulture.ToString
            Return PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Cache" & "\" & CleanFileName(strCacheKey) & ".resources"
        End Function

        <Obsolete("The module caching feature has been updated in version 5.2.0.  This method is no longer used.")> _
        Public Shared Function CacheKey(ByVal TabModuleID As Integer) As String
            Dim strCacheKey As String = "TabModule:"
            strCacheKey += TabModuleID.ToString & ":"
            strCacheKey += System.Threading.Thread.CurrentThread.CurrentCulture.ToString
            Return strCacheKey
        End Function

#End Region

    End Class


End Namespace
