'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Messaging
    Public Class MessagingBusinessController
        Implements DotNetNuke.Entities.Modules.IUpgradeable

        Public Function UpgradeModule(ByVal Version As String) As String Implements DotNetNuke.Entities.Modules.IUpgradeable.UpgradeModule

            Try
                Select Case Version
                    Case "01.00.00"
                        Dim moduleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Messaging")

                        If moduleDefinition IsNot Nothing Then
                            'Add Module to User Profile Page for all Portals

                            Dim settings As PortalSettings = PortalController.GetCurrentPortalSettings()
                            Dim objPortalController As New PortalController
                            Dim objTabController As New TabController
                            Dim objModuleController As New ModuleController

                            Dim portals As ArrayList = objPortalController.GetPortals()
                            For Each portal As PortalInfo In portals
                                Dim tabID As Integer = TabController.GetTabByTabPath(portal.PortalID, "//UserProfile")
                                If (tabID <> Null.NullInteger) Then
                                    Dim tab As TabInfo = objTabController.GetTab(tabID, portal.PortalID, True)
                                    If (tab IsNot Nothing) Then
                                        Dim moduleId As Integer = DotNetNuke.Services.Upgrade.Upgrade.AddModuleToPage(tab, moduleDefinition.ModuleDefID, "My Inbox", "", True)
                                        Dim objModule As ModuleInfo = objModuleController.GetModule(moduleId, tabID, False)

                                        Dim permissions As ArrayList = New PermissionController().GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "EDIT")
                                        Dim permission As PermissionInfo
                                        If permissions.Count = 1 Then
                                            permission = TryCast(permissions(0), PermissionInfo)
                                        End If
                                        If permission IsNot Nothing Then
                                            Dim modulePermission As ModulePermissionInfo = New ModulePermissionInfo(permission)
                                            modulePermission.ModuleID = moduleId
                                            modulePermission.RoleID = settings.RegisteredRoleId
                                            modulePermission.UserID = Null.NullInteger
                                            modulePermission.AllowAccess = True

                                            objModule.ModulePermissions.Add(modulePermission)

                                            ModulePermissionController.SaveModulePermissions(objModule)
                                        End If
                                    End If
                                End If
                            Next
                        End If
                End Select
                Return "Success"
            Catch ex As Exception
                Return "Failed"
            End Try
        End Function

    End Class
End Namespace
