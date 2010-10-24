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

Imports DotNetNuke.Entities.Controllers
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : LanguageSettings
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies LanguageSettings functionality for the Extensions module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]   04/03/2008    Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class LanguageSettings
        Inherits ModuleSettingsBase


        Public Overrides Sub UpdateSettings()
            MyBase.UpdateSettings()

            PortalController.UpdatePortalSetting(Me.ModuleContext.PortalId, "EnableUrlLanguage", chkUrl.Checked.ToString())
            Dim modController As New ModuleController
            modController.UpdateModuleSetting(Me.ModuleContext.ModuleId, "UsePaging", chkUsePaging.Checked)
            modController.UpdateModuleSetting(Me.ModuleContext.ModuleId, "PageSize", txtPageSize.Text)


        End Sub

        Public Overrides Sub LoadSettings()
            MyBase.LoadSettings()

            valPageSize.MinimumValue = 1
            valPageSize.MaximumValue = Int32.MaxValue

            chkUrl.Checked = Me.ModuleContext.PortalSettings.EnableUrlLanguage
            chkUsePaging.Checked = CType(Me.ModuleContext.Settings("UsePaging"), Boolean)

            Dim _PageSize As Integer = 10 'default page size
            If CType(Me.ModuleContext.Settings("PageSize"), Integer) = 0 Then
                txtPageSize.Text = _PageSize
            Else
                txtPageSize.Text = CType(Me.ModuleContext.Settings("PageSize"), Integer)
            End If

            urlRow.Visible = Not PortalSettings.Current.ContentLocalizationEnabled

        End Sub


    End Class

End Namespace


