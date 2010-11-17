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
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Xml.Serialization
Imports System.Xml

Namespace DotNetNuke.Services.Log.EventLog


    Partial Public Class EventLogController
        Inherits LogController

        Public Overloads Sub AddLog(ByVal objEventLogInfo As LogInfo)
            Dim objLogController As New LogController
            objLogController.AddLog(objEventLogInfo)
        End Sub

        Public Overloads Sub AddLog(ByVal objCBO As Object, ByVal _PortalSettings As PortalSettings, ByVal UserID As Integer, ByVal UserName As String, ByVal LogType As String)
            'supports adding a custom string for LogType
            Dim objLogController As New LogController
            Dim objLogInfo As New LogInfo
            objLogInfo.LogUserID = UserID
            objLogInfo.LogTypeKey = LogType.ToString
            If _PortalSettings IsNot Nothing Then
                objLogInfo.LogPortalID = _PortalSettings.PortalId
                objLogInfo.LogPortalName = _PortalSettings.PortalName
            End If
            Select Case objCBO.GetType.FullName
                Case "DotNetNuke.Entities.Portals.PortalInfo"
                    Dim objPortal As DotNetNuke.Entities.Portals.PortalInfo = CType(objCBO, DotNetNuke.Entities.Portals.PortalInfo)
                    objLogInfo.LogProperties.Add(New LogDetailInfo("PortalID", objPortal.PortalID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("PortalName", objPortal.PortalName))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("Description", objPortal.Description))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("KeyWords", objPortal.KeyWords))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("LogoFile", objPortal.LogoFile))
                Case "DotNetNuke.Entities.Tabs.TabInfo"
                    Dim objTab As DotNetNuke.Entities.Tabs.TabInfo = CType(objCBO, DotNetNuke.Entities.Tabs.TabInfo)
                    objLogInfo.LogProperties.Add(New LogDetailInfo("TabID", objTab.TabID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("PortalID", objTab.PortalID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("TabName", objTab.TabName))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("Title", objTab.Title))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("Description", objTab.Description))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("KeyWords", objTab.KeyWords))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("Url", objTab.Url))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("ParentId", objTab.ParentId.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("IconFile", objTab.IconFile))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("IsVisible", objTab.IsVisible.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("SkinSrc", objTab.SkinSrc))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("ContainerSrc", objTab.ContainerSrc))
                Case "DotNetNuke.Entities.Modules.ModuleInfo"
                    Dim objModule As DotNetNuke.Entities.Modules.ModuleInfo = CType(objCBO, DotNetNuke.Entities.Modules.ModuleInfo)
                    objLogInfo.LogProperties.Add(New LogDetailInfo("ModuleId", objModule.ModuleID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("ModuleTitle", objModule.ModuleTitle))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("TabModuleID", objModule.TabModuleID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("TabID", objModule.TabID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("PortalID", objModule.PortalID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("ModuleDefId", objModule.ModuleDefID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("FriendlyName", objModule.DesktopModule.FriendlyName))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("IconFile", objModule.IconFile))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("Visibility", objModule.Visibility.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("ContainerSrc", objModule.ContainerSrc))
                Case "DotNetNuke.Entities.Users.UserInfo"
                    Dim objUser As DotNetNuke.Entities.Users.UserInfo = CType(objCBO, DotNetNuke.Entities.Users.UserInfo)
                    objLogInfo.LogProperties.Add(New LogDetailInfo("UserID", objUser.UserID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("FirstName", objUser.Profile.FirstName))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("LastName", objUser.Profile.LastName))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("UserName", objUser.Username))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("Email", objUser.Email))
                Case "DotNetNuke.Security.Roles.RoleInfo"
                    Dim objRole As DotNetNuke.Security.Roles.RoleInfo = CType(objCBO, DotNetNuke.Security.Roles.RoleInfo)
                    objLogInfo.LogProperties.Add(New LogDetailInfo("RoleID", objRole.RoleID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("RoleName", objRole.RoleName))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("PortalID", objRole.PortalID.ToString))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("Description", objRole.Description))
                    objLogInfo.LogProperties.Add(New LogDetailInfo("IsPublic", objRole.IsPublic.ToString))
                Case Else    'Serialise using XmlSerializer
                    objLogInfo.LogProperties.Add(New LogDetailInfo("logdetail", XmlUtils.Serialize(objCBO)))
            End Select
            objLogController.AddLog(objLogInfo)
        End Sub

        Public Overloads Sub AddLog(ByVal objCBO As Object, ByVal _PortalSettings As PortalSettings, ByVal UserID As Integer, ByVal UserName As String, ByVal objLogType As Services.Log.EventLog.EventLogController.EventLogType)
            AddLog(objCBO, _PortalSettings, UserID, UserName, objLogType.ToString)
        End Sub

        Public Overloads Sub AddLog(ByVal _PortalSettings As PortalSettings, ByVal UserID As Integer, ByVal objLogType As Services.Log.EventLog.EventLogController.EventLogType)
            'Used for DotNetNuke native  log types
            Dim objProperties As New LogProperties
            AddLog(objProperties, _PortalSettings, UserID, objLogType.ToString, False)
        End Sub

        Public Overloads Sub AddLog(ByVal PropertyName As String, ByVal PropertyValue As String, ByVal _PortalSettings As PortalSettings, ByVal UserID As Integer, ByVal objLogType As Services.Log.EventLog.EventLogController.EventLogType)
            'Used for DotNetNuke native  log types
            Dim objProperties As New LogProperties
            Dim objLogDetailInfo As New LogDetailInfo
            objLogDetailInfo.PropertyName = PropertyName
            objLogDetailInfo.PropertyValue = PropertyValue
            objProperties.Add(objLogDetailInfo)
            AddLog(objProperties, _PortalSettings, UserID, objLogType.ToString, False)
        End Sub

        Public Overloads Sub AddLog(ByVal PropertyName As String, ByVal PropertyValue As String, ByVal _PortalSettings As PortalSettings, ByVal UserID As Integer, ByVal LogType As String)
            'Used for custom/on-the-fly  log types
            Dim objProperties As New LogProperties
            Dim objLogDetailInfo As New LogDetailInfo
            objLogDetailInfo.PropertyName = PropertyName
            objLogDetailInfo.PropertyValue = PropertyValue
            objProperties.Add(objLogDetailInfo)
            AddLog(objProperties, _PortalSettings, UserID, LogType, False)
        End Sub

        Public Overloads Sub AddLog(ByVal objProperties As LogProperties, ByVal _PortalSettings As PortalSettings, ByVal UserID As Integer, ByVal LogTypeKey As String, ByVal BypassBuffering As Boolean)
            'Used for custom/on-the-fly  log types

            Dim objLogController As New LogController
            Dim objLogInfo As New LogInfo
            objLogInfo.LogUserID = UserID
            objLogInfo.LogTypeKey = LogTypeKey
            objLogInfo.LogProperties = objProperties
            objLogInfo.BypassBuffering = BypassBuffering
            If _PortalSettings IsNot Nothing Then
                objLogInfo.LogPortalID = _PortalSettings.PortalId
                objLogInfo.LogPortalName = _PortalSettings.PortalName
            End If
            objLogController.AddLog(objLogInfo)

        End Sub

    End Class

End Namespace




