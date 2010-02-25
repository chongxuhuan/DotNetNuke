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

Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions

Namespace DotNetNuke.Modules.SearchResults

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.SearchResults
    ''' Project:    DotNetNuke.SearchResults
    ''' Class:      ResultsSettings
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ResultsSettings ModuleSettingsBase is used to manage the 
    ''' settings for the Search Results Module
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/11/2004	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial  Class ResultsSettings
        Inherits Entities.Modules.ModuleSettingsBase

#Region "Controls"


#End Region

#Region "Base Method Implementations"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadSettings loads the settings from the Databas and displays them
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''		[cnurse]	11/11/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub LoadSettings()
            Try
                If (Page.IsPostBack = False) Then
                    If CType(TabModuleSettings("maxresults"), String) <> "" Then
                        txtresults.Text = CType(TabModuleSettings("maxresults"), String)
                    Else
                        txtresults.Text = ""
                    End If
                    If CType(TabModuleSettings("perpage"), String) <> "" Then
                        txtPage.Text = CType(TabModuleSettings("perpage"), String)
                    Else
                        txtPage.Text = ""
                    End If
                    If CType(TabModuleSettings("titlelength"), String) <> "" Then
                        txtTitle.Text = CType(TabModuleSettings("titlelength"), String)
                    Else
                        txtTitle.Text = ""
                    End If
                    If CType(TabModuleSettings("descriptionlength"), String) <> "" Then
                        txtdescription.Text = CType(TabModuleSettings("descriptionlength"), String)
                    Else
                        txtdescription.Text = ""
                    End If
                    chkDescription.Checked = False
                    If CType(TabModuleSettings("showdescription"), String) <> "" Then
                        If CType(TabModuleSettings("showdescription"), String) = "Y" Then
                            chkDescription.Checked = True
                        End If
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
        '''		[cnurse]	11/11/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UpdateSettings()
            Try
                If Page.IsValid Then
                    Dim objModules As New Entities.Modules.ModuleController

                    objModules.UpdateTabModuleSetting(TabModuleId, "maxresults", txtresults.Text)
                    objModules.UpdateTabModuleSetting(TabModuleId, "perpage", txtPage.Text)
                    objModules.UpdateTabModuleSetting(TabModuleId, "titlelength", txtTitle.Text)
                    objModules.UpdateTabModuleSetting(TabModuleId, "descriptionlength", txtdescription.Text)
                    objModules.UpdateTabModuleSetting(TabModuleId, "showdescription", CType(IIf(chkDescription.Checked, "Y", "N"), String))
                End If
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
