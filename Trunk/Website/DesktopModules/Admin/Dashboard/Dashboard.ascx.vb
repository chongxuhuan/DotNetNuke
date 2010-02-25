' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2010 
'' by DotNetNuke Corporation 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
'' 
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software. 
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE. 
' 


Imports System
Imports DotNetNuke.Entities.Modules

Imports System.Collections.Generic
Imports System.IO
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DotNetNuke.Modules.Dashboard.Components
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Services.Installer

Namespace DotNetNuke.Modules.Admin.Dashboard

    Partial Class Dashboard
        Inherits PortalModuleBase
        Implements IActionable

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            jQuery.RequestRegistration()

            Dim dashboardJs As String = ResolveUrl("~/resources/dashboard/jquery.dashboard.js")

            Page.ClientScript.RegisterClientScriptInclude("DashboardJS", dashboardJs)

            ClientAPI.RegisterClientVariable(Page, "dashboardBaseUrl", ControlPath, False)
            ClientAPI.RegisterClientVariable(Page, "appBaseUrl", DotNetNuke.Common.Globals.ApplicationPath, False)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not UserInfo.IsSuperUser Then
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL("Access Denied"), True)
            End If

            'Get enabled Dashboard Controls 
            Dim controls As List(Of DashboardControl) = DashboardController.GetDashboardControls(True)

            'Bind to tab list 
            rptTabs.DataSource = controls
            rptTabs.DataBind()

            'Bind to control list 
            rptControls.DataSource = controls
            rptControls.DataBind()
        End Sub

        Protected Sub rptControls_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptControls.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim control As DashboardControl = TryCast(e.Item.DataItem, DashboardControl)
                If control IsNot Nothing Then
                    Dim dashboardControl As Control
                    If control.DashboardControlSrc.ToLowerInvariant().EndsWith("ascx") Then
                        ' load from a user control on the file system 
                        dashboardControl = LoadControl("~/" & control.DashboardControlSrc)
                    Else
                        ' load from a typename in an assembly ( ie. server control ) 
                        dashboardControl = LoadControl(Framework.Reflection.CreateType(control.DashboardControlSrc), Nothing)
                    End If
                    dashboardControl.ID = Path.GetFileNameWithoutExtension(control.DashboardControlSrc)

                    Dim placeHolder As PlaceHolder = DirectCast(e.Item.FindControl("phControl"), PlaceHolder)
                    placeHolder.Controls.Add(dashboardControl)
                End If
            End If
        End Sub

#Region "IActionable Members"

        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim actions As New ModuleActionCollection()
                actions.Add(GetNextActionID(), Localization.GetString("Install.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", Util.InstallURL(TabId, "DashboardControl"), False, SecurityAccessLevel.Host, True, False)
                actions.Add(GetNextActionID(), Localization.GetString("Manage.Action", LocalResourceFile), ModuleActionType.AddContent, "", "icon_profile_16px.gif", EditUrl("DashboardControls"), False, SecurityAccessLevel.Host, True, False)
                actions.Add(GetNextActionID(), Localization.GetString("Export.Action", LocalResourceFile), ModuleActionType.AddContent, "", "lt.gif", EditUrl("Export"), False, SecurityAccessLevel.Host, True, False)
                Return actions
            End Get
        End Property

#End Region

    End Class

End Namespace