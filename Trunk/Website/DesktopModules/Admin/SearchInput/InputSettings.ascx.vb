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

Imports DotNetNuke
Imports System.Text.RegularExpressions
Imports System.Web

Namespace DotNetNuke.Modules.SearchInput

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.SearchInput
    ''' Project:    DotNetNuke.SearchInput
    ''' Class:      InputSettings
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The InputSettings ModuleSettingsBase is used to manage the 
    ''' settings for the Search Input Module
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/30/2004	converted to SettingsBase
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial  Class InputSettings
        Inherits Entities.Modules.ModuleSettingsBase

#Region "Controls"
        Protected WithEvents cmdUpdate As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdCancel As System.Web.UI.WebControls.LinkButton

#End Region

#Region "Private Methods"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindSearchResults gets the Search Results Modules available and binds them to the
        ''' drop-down combo
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''		[cnurse]	11/30/2004	converted to SettingsBase
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindSearchResults()
            Dim objSearch As New SearchInputController

            cboModule.DataSource = objSearch.GetSearchResultModules(PortalId)
            cboModule.DataTextField = "SearchTabName"
            cboModule.DataValueField = "TabID"
            cboModule.DataBind()
            If cboModule.Items.Count < 2 Then
                cboModule.Visible = False
                txtModule.Visible = True
            Else
                cboModule.Visible = True
                txtModule.Visible = False
            End If
        End Sub
#End Region

#Region "Base Method Implementations"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadSettings loads the settings from the Database and displays them
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''		[cnurse]	11/30/2004	converted to SettingsBase
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub LoadSettings()
            Try
                If (Page.IsPostBack = False) Then
                    BindSearchResults()

                    Dim SearchTabID As String = CType(ModuleSettings("SearchResultsModule"), String)
                    Dim ShowGoImage As String = CType(ModuleSettings("ShowGoImage"), String)
                    Dim ShowSearchImage As String = CType(ModuleSettings("ShowSearchImage"), String)

                    If Not cboModule.Items.FindByValue(SearchTabID) Is Nothing Then
                        cboModule.Items.FindByValue(SearchTabID).Selected = True
                    End If
                    If cboModule.Items.Count > 0 Then
                        txtModule.Text = cboModule.SelectedItem.Text
                    Else
                        txtModule.Text = Localization.GetString("NoSearchModule", LocalResourceFile)
                    End If

                    If Not ShowGoImage Is Nothing Then
                        chkGo.Checked() = CType(ShowGoImage, Boolean)
                    End If

                    If Not ShowSearchImage Is Nothing Then
                        chkSearchImage.Checked() = CType(ShowSearchImage, Boolean)
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateSettings saves the modified settings to the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''		[cnurse]	11/30/2004	converted to SettingsBase
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UpdateSettings()
            Try
                Dim objModules As New Entities.Modules.ModuleController

                If Not cboModule.SelectedIndex = -1 Then
                    objModules.UpdateModuleSetting(Me.ModuleId, "SearchResultsModule", cboModule.SelectedItem.Value)
                End If

                objModules.UpdateModuleSetting(Me.ModuleId, "ShowGoImage", chkGo.Checked.ToString)
                objModules.UpdateModuleSetting(Me.ModuleId, "ShowSearchImage", chkSearchImage.Checked.ToString)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region " Web Form Designer Generated Code "
        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub
#End Region

    End Class

End Namespace
