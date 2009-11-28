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

Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Modules

Namespace DotNetNuke.Modules.Admin.Extensions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : BatchInstall
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies the functionality to BatchInstall  multiple Extensions(packages) to 
    ''' the Portal
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]   05/13/2008    Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class BatchInstall
        Inherits ModuleUserControlBase

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindAuthSystems binds the Authentication Systems checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindAuthSystems()
            BindPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "NoAuthSystems", lblAuthSystemsError)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindLanguages binds the languages checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindLanguages()
            BindPackageItems("Language", lstLanguages, lblNoLanguages, "NoLanguages", lblLanguagesError)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindModules binds the modules checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindModules()
            BindPackageItems("Module", lstModules, lblNoModules, "NoModules", lblModulesError)
        End Sub

        Private Sub BindPackageItems(ByVal packageType As String, ByVal list As CheckBoxList, ByVal noItemsLabel As Label, ByVal noItemsKey As String, ByVal errorLabel As Label)
            Dim arrFiles As String()
            Dim strFile As String

            Dim InstallPath As String = ApplicationMapPath & "\Install\" + packageType
            list.Items.Clear()
            If Directory.Exists(InstallPath) Then
                arrFiles = Directory.GetFiles(InstallPath)
                Dim iFile As Integer = 0
                For Each strFile In arrFiles
                    Dim strResource As String = strFile.Replace(InstallPath + "\", "")
                    If strResource.ToLower.EndsWith(".zip") OrElse strResource.ToLower.EndsWith(".resources") Then
                        Dim packageItem As ListItem = New ListItem()
                        packageItem.Value = strResource
                        strResource = strResource.Replace(".zip", "")
                        strResource = strResource.Replace(".resources", "")
                        strResource = strResource.Replace("_Install", ")")
                        strResource = strResource.Replace("_install", ")")
                        strResource = strResource.Replace("_Source", ")")
                        strResource = strResource.Replace("_source", ")")
                        strResource = strResource.Replace("_", " (")
                        packageItem.Text = strResource

                        list.Items.Add(packageItem)
                    End If
                Next
            End If

            If list.Items.Count > 0 Then
                noItemsLabel.Visible = False
            Else
                noItemsLabel.Visible = True
                noItemsLabel.Text = Localization.GetString(noItemsKey, Me.LocalResourceFile)
            End If
            If errorLabel IsNot Nothing Then
                errorLabel.Text = Null.NullString
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindSkins binds the skins checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindSkins()
            BindPackageItems("Skin", lstSkins, lblNoSkins, "NoSkins", lblSkinsError)

            BindPackageItems("Container", lstContainers, lblNoContainers, "NoContainers", Nothing)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallLanguages installs the Optional Languages
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallAuthSystems() As Boolean
            Return InstallPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "InstallAuthSystemError")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallLanguages installs the Optional Languages
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallLanguages() As Boolean
            Return InstallPackageItems("Language", lstLanguages, lblLanguagesError, "InstallLanguageError")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallModules installs the Optional Modules
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallModules() As Boolean
            Return InstallPackageItems("Module", lstModules, lblModulesError, "InstallModuleError")
        End Function

        Private Function InstallPackageItems(ByVal packageType As String, ByVal list As CheckBoxList, ByVal errorLabel As Label, ByVal errorKey As String) As Boolean
            Dim success As Boolean = False
            Dim strErrorMessage As String = Null.NullString

            'Get current Script time-out
            Dim scriptTimeOut As Integer = Server.ScriptTimeout

            Try
                'Set Script timeout to MAX value
                Server.ScriptTimeout = Integer.MaxValue

                Dim InstallPath As String = ApplicationMapPath & "\Install\" + packageType
                For Each packageItem As ListItem In list.Items
                    If packageItem.Selected Then
                        success = Upgrade.Upgrade.InstallPackage(InstallPath + "\" + packageItem.Value, packageType, False)
                        If Not success Then
                            strErrorMessage += String.Format(Localization.GetString(errorKey, Me.LocalResourceFile), packageItem.Text)
                        End If
                    End If
                Next

                success = String.IsNullOrEmpty(strErrorMessage)
            Catch ex As Exception
                strErrorMessage = ex.StackTrace
            Finally
                'restore Script timeout
                Server.ScriptTimeout = scriptTimeOut
            End Try

            If Not success Then
                errorLabel.Text += strErrorMessage
            End If

            Return success
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallSkins installs the Optional Skins
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   05/13/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallSkins() As Boolean
            Dim skinSuccess As Boolean = InstallPackageItems("Skin", lstSkins, lblSkinsError, "InstallSkinError")
            Dim containerSuccess As Boolean = InstallPackageItems("Container", lstContainers, lblSkinsError, "InstallContainerError")
            Return skinSuccess And containerSuccess
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then
                BindModules()
                BindSkins()
                BindLanguages()
                BindAuthSystems()
            End If
        End Sub

        Protected Sub cmdInstall_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdInstall.Click
            Dim moduleSuccess As Boolean
            Dim skinSuccess As Boolean
            Dim languagesSuccess As Boolean
            Dim AuthSystemSuccess As Boolean

            If lstAuthSystems.SelectedIndex = Null.NullInteger AndAlso _
                        lstContainers.SelectedIndex = Null.NullInteger AndAlso _
                        lstSkins.SelectedIndex = Null.NullInteger AndAlso _
                        lstModules.SelectedIndex = Null.NullInteger AndAlso _
                        lstLanguages.SelectedIndex = Null.NullInteger Then
                Skins.Skin.AddModuleMessage(Me, Localization.GetString("NoneSelected", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                Exit Sub
            End If

            moduleSuccess = InstallModules()
            skinSuccess = InstallSkins()
            languagesSuccess = InstallLanguages()
            AuthSystemSuccess = InstallAuthSystems()

            If moduleSuccess And skinSuccess And languagesSuccess And AuthSystemSuccess Then
                'Refesh page to update lists
                Response.Redirect(Request.RawUrl, True)
            End If
        End Sub

    End Class

End Namespace
