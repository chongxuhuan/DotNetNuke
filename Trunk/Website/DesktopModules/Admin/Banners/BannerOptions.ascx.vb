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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Vendors
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Modules.Admin.Vendors

    Partial Class BannerOptions
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

        '*******************************************************
        '
        ' The Page_Load event handler on this User Control is used to
        ' obtain a DataReader of banner information from the Banners
        ' table, and then databind the results to a templated DataList
        ' server control.  It uses the DotNetNuke.BannerDB()
        ' data component to encapsulate all data functionality.
        '
        '*******************************************************'
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then

                    ' Obtain banner information from the Banners table and bind to the list control
                    Dim objBannerTypes As New BannerTypeController

                    cboType.DataSource = objBannerTypes.GetBannerTypes
                    cboType.DataBind()
                    cboType.Items.Insert(0, New ListItem(Localization.GetString("AllTypes", LocalResourceFile), "-1"))

                    If ModuleId > 0 Then

                        ' Get settings from the database
                        Dim settings As Hashtable = New ModuleController().GetModuleSettings(ModuleId)

                        If Not optSource.Items.FindByValue(CType(settings("bannersource"), String)) Is Nothing Then
                            optSource.Items.FindByValue(CType(settings("bannersource"), String)).Selected = True
                        Else
                            optSource.Items.FindByValue("L").Selected = True
                        End If
                        If Not cboType.Items.FindByValue(CType(settings("bannertype"), String)) Is Nothing Then
                            cboType.Items.FindByValue(CType(settings("bannertype"), String)).Selected = True
                        End If
                        If CType(settings("bannergroup"), String) <> "" Then
                            DNNTxtBannerGroup.Text = CType(settings("bannergroup"), String)
                        End If
                        If Not optOrientation.Items.FindByValue(CType(settings("orientation"), String)) Is Nothing Then
                            optOrientation.Items.FindByValue(CType(settings("orientation"), String)).Selected = True
                        Else
                            optOrientation.Items.FindByValue("V").Selected = True
                        End If
                        If CType(settings("bannercount"), String) <> "" Then
                            txtCount.Text = CType(settings("bannercount"), String)
                        Else
                            txtCount.Text = "1"
                        End If
                        If CType(settings("border"), String) <> "" Then
                            txtBorder.Text = CType(settings("border"), String)
                        Else
                            txtBorder.Text = "0"
                        End If
                        If CType(settings("padding"), String) <> "" Then
                            txtPadding.Text = CType(settings("padding"), String)
                        Else
                            txtPadding.Text = "4"
                        End If
                        txtBorderColor.Text = CType(settings("bordercolor"), String)
                        txtRowHeight.Text = CType(settings("rowheight"), String)
                        txtColWidth.Text = CType(settings("colwidth"), String)
                        txtBannerClickThroughURL.Text = CType(settings("bannerclickthroughurl"), String)

                    End If
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                If Page.IsValid Then
                    ' Update settings in the database
                    Dim objModules As New ModuleController

                    If Not optSource.SelectedItem Is Nothing Then
                        objModules.UpdateModuleSetting(ModuleId, "bannersource", optSource.SelectedItem.Value)
                    End If
                    If Not cboType.SelectedItem Is Nothing Then
                        objModules.UpdateModuleSetting(ModuleId, "bannertype", cboType.SelectedItem.Value)
                    End If
                    objModules.UpdateModuleSetting(ModuleId, "bannergroup", DNNTxtBannerGroup.Text)
                    If Not optOrientation.SelectedItem Is Nothing Then
                        objModules.UpdateModuleSetting(ModuleId, "orientation", optOrientation.SelectedItem.Value)
                    End If
                    objModules.UpdateModuleSetting(ModuleId, "bannercount", txtCount.Text)
                    objModules.UpdateModuleSetting(ModuleId, "border", txtBorder.Text)
                    objModules.UpdateModuleSetting(ModuleId, "bordercolor", txtBorderColor.Text)
                    objModules.UpdateModuleSetting(ModuleId, "rowheight", txtRowHeight.Text)
                    objModules.UpdateModuleSetting(ModuleId, "colwidth", txtColWidth.Text)
                    objModules.UpdateModuleSetting(ModuleId, "padding", txtPadding.Text)
                    objModules.UpdateModuleSetting(ModuleId, "bannerclickthroughurl", txtBannerClickThroughURL.Text)

                    ' Redirect back to the portal home page
                    Response.Redirect(NavigateURL(), True)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DNNTxtBannerGroup_PopulateOnDemand runs when something is entered on the
        ''' BannerGroup field
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	9/29/2006	Implement a callback to display current groups
        '''  to user so the BannerGroup can be easily selected
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub DNNTxtBannerGroup_PopulateOnDemand(ByVal source As Object, ByVal e As UI.WebControls.DNNTextSuggestEventArgs) Handles DNNTxtBannerGroup.PopulateOnDemand

            Dim dt As DataTable
            Dim objNode As DotNetNuke.UI.WebControls.DNNNode

            Dim objBanners As New BannerController
            dt = objBanners.GetBannerGroups(PortalId)
            Dim dr() As DataRow
            dt.CaseSensitive = False
            dr = dt.Select("GroupName like '" & e.Text & "%'")
            For Each d As DataRow In dr
                objNode = New DotNetNuke.UI.WebControls.DNNNode(d("GroupName").ToString())
                objNode.ID = e.Nodes.Count.ToString
                e.Nodes.Add(objNode)
            Next

        End Sub
    End Class

End Namespace
