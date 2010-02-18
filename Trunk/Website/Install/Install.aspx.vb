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
Imports System.xml

Imports DotNetNuke.Application
Imports DotNetNuke.Common
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Services.Install

    Partial Class Install
        Inherits System.Web.UI.Page

#Region "Private Methods"

        Private Sub ExecuteScripts()

            'Start Timer
            Services.Upgrade.Upgrade.StartTimer()

            'Write out Header
            HtmlUtils.WriteHeader(Response, "executeScripts")

            Response.Write("<h2>Execute Scripts Status Report</h2>")
            Response.Flush()

            Dim strProviderPath As String = DotNetNuke.Data.DataProvider.Instance().GetProviderPath()
            If Not strProviderPath.StartsWith("ERROR:") Then
                Services.Upgrade.Upgrade.ExecuteScripts(strProviderPath)
            End If

            Response.Write("<h2>Execution Complete</h2>")
            Response.Flush()


            'Write out Footer
            HtmlUtils.WriteFooter(Response)
        End Sub

        Private Sub InstallApplication()

            ' the application uses a two step installation process. The first step is used to update 
            ' the Web.config with any configuration settings - which forces an application restart. 
            ' The second step finishes the installation process and provisions the site.

            Dim installationDate As String = Config.GetSetting("InstallationDate")

            If installationDate Is Nothing Or installationDate = "" Then
                Dim strError As String = Config.UpdateMachineKey()
                If strError = "" Then
                    ' send a new request to the application to initiate step 2
                    Response.Redirect(HttpContext.Current.Request.RawUrl, True)
                Else
                    '403-3 Error - Redirect to ErrorPage
                    Dim strURL As String = "~/ErrorPage.aspx?status=403_3&error=" & strError
                    HttpContext.Current.Response.Clear()
                    HttpContext.Current.Server.Transfer(strURL)
                End If
            Else
                'Start Timer
                Services.Upgrade.Upgrade.StartTimer()

                'Write out Header
                HtmlUtils.WriteHeader(Response, "install")

                ' get path to script files
                Dim strProviderPath As String = DotNetNuke.Data.DataProvider.Instance().GetProviderPath()
                If Not strProviderPath.StartsWith("ERROR:") Then

                    Response.Write("<h2>Version: " & FormatVersion(DotNetNukeContext.Current.Application.Version) & "</h2>")
                    Response.Flush()

                    Response.Write("<br><br>")
                    Response.Write("<h2>Installation Status Report</h2>")
                    Response.Flush()
                    Services.Upgrade.Upgrade.InstallDNN(strProviderPath)

                    Response.Write("<h2>Installation Complete</h2>")
                    Response.Write("<br><br><h2><a href='../Default.aspx'>Click Here To Access Your Portal</a></h2><br><br>")
                    Response.Flush()
                Else
                    ' upgrade error
                    Response.Write("<h2>Upgrade Error: " & strProviderPath & "</h2>")
                    Response.Flush()
                End If

                'Write out Footer
                HtmlUtils.WriteFooter(Response)

                'log APPLICATION_START event
                Initialize.LogStart()

                'Start Scheduler
                Initialize.StartScheduler()
            End If

        End Sub

        Private Sub UpgradeApplication()

            'Start Timer
            Services.Upgrade.Upgrade.StartTimer()

            'Write out Header
            HtmlUtils.WriteHeader(Response, "upgrade")

            Response.Write("<h2>Current Assembly Version: " & FormatVersion(DotNetNukeContext.Current.Application.Version) & "</h2>")
            Response.Flush()

            ' get path to script files
            Dim strProviderPath As String = DotNetNuke.Data.DataProvider.Instance().GetProviderPath()
            If Not strProviderPath.StartsWith("ERROR:") Then
                Dim strDatabaseVersion As String

                ' get current database version
                Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().GetDataBaseVersion
                If dr.Read Then
                    'Call Upgrade with the current DB Version to upgrade an
                    'existing DNN installation
                    Dim majVersion As Integer = Convert.ToInt32(dr("Major"))
                    Dim minVersion As Integer = Convert.ToInt32(dr("Minor"))
                    Dim buildVersion As Integer = Convert.ToInt32(dr("Build"))
                    strDatabaseVersion = Format(majVersion, "00") & "." & Format(minVersion, "00") & "." & Format(buildVersion, "00")

                    Response.Write("<h2>Current Database Version: " & strDatabaseVersion & "</h2>")
                    Response.Flush()

                    Dim ignoreWarning As String = Null.NullString
                    Dim strWarning As String = Null.NullString
                    If (majVersion = 3 And minVersion < 3) Or (majVersion = 4 And minVersion < 3) Then
                        'Users and profile have not been transferred

                        ' Get the name of the data provider
                        Dim objProviderConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration("data")

                        'Execute Special Script
                        Services.Upgrade.Upgrade.ExecuteScript(strProviderPath + "Upgrade." + objProviderConfiguration.DefaultProvider)

                        If Not (Request.QueryString("ignoreWarning") Is Nothing) Then
                            ignoreWarning = Request.QueryString("ignoreWarning").ToLower
                        End If

                        strWarning = Services.Upgrade.Upgrade.CheckUpgrade()
                    Else
                        ignoreWarning = "true"
                    End If

                    'Check whether Upgrade is ok
                    If strWarning = Null.NullString Or ignoreWarning = "true" Then
                        Response.Write("<br><br>")
                        Response.Write("<h2>Upgrade Status Report</h2>")
                        Response.Flush()
                        Services.Upgrade.Upgrade.UpgradeDNN(strProviderPath, DotNetNuke.Data.DataProvider.Instance().GetVersion)

                        'Install optional resources if present
                        Upgrade.Upgrade.InstallPackages("Module", True)
                        Upgrade.Upgrade.InstallPackages("Skin", True)
                        Upgrade.Upgrade.InstallPackages("Container", True)
                        Upgrade.Upgrade.InstallPackages("Language", True)
                        Upgrade.Upgrade.InstallPackages("Provider", True)
                        Upgrade.Upgrade.InstallPackages("AuthSystem", True)
                        Upgrade.Upgrade.InstallPackages("Package", True)

                        Response.Write("<h2>Upgrade Complete</h2>")
                        Response.Write("<br><br><h2><a href='../Default.aspx'>Click Here To Access Your Portal</a></h2><br><br>")
                    Else
                        Response.Write("<h2>Warning:</h2>" & strWarning.Replace(vbCrLf, "<br />"))

                        Response.Write("<br><br><a href='Install.aspx?mode=upgrade&ignoreWarning=true'>Click Here To Proceed With The Upgrade.</a>")
                    End If
                    Response.Flush()
                End If
                dr.Close()
            Else
                Response.Write("<h2>Upgrade Error: " & strProviderPath & "</h2>")
                Response.Flush()
            End If

            'Write out Footer
            HtmlUtils.WriteFooter(Response)
        End Sub

        Private Sub AddPortal()

            'Start Timer
            Services.Upgrade.Upgrade.StartTimer()

            'Write out Header
            HtmlUtils.WriteHeader(Response, "addPortal")

            Response.Write("<h2>Add Portal Status Report</h2>")
            Response.Flush()

            ' install new portal(s)
            Dim strNewFile As String = Common.Globals.ApplicationMapPath & "\Install\Portal\Portals.resources"
            If File.Exists(strNewFile) Then
                Dim xmlDoc As New XmlDocument
                Dim node As XmlNode
                Dim nodes As XmlNodeList
                Dim intPortalId As Integer
                xmlDoc.Load(strNewFile)

                ' parse portal(s) if available
                nodes = xmlDoc.SelectNodes("//dotnetnuke/portals/portal")
                For Each node In nodes
                    If Not node Is Nothing Then
                        intPortalId = Services.Upgrade.Upgrade.AddPortal(node, True, 0)
                    End If
                Next

                ' delete the file
                Try
                    File.SetAttributes(strNewFile, FileAttributes.Normal)
                    File.Delete(strNewFile)
                Catch
                    ' error removing the file
                End Try

                Response.Write("<h2>Installation Complete</h2>")
                Response.Write("<br><br><h2><a href='../Default.aspx'>Click Here To Access Your Portal</a></h2><br><br>")
                Response.Flush()
            End If


            'Write out Footer
            HtmlUtils.WriteFooter(Response)
        End Sub

        Private Sub InstallResources()

            'Start Timer
            Services.Upgrade.Upgrade.StartTimer()

            'Write out Header
            HtmlUtils.WriteHeader(Response, "installResources")

            Response.Write("<h2>Install Resources Status Report</h2>")
            Response.Flush()

            ' install new resources(s)
            Upgrade.Upgrade.InstallPackages("Module", True)
            Upgrade.Upgrade.InstallPackages("Skin", True)
            Upgrade.Upgrade.InstallPackages("Container", True)
            Upgrade.Upgrade.InstallPackages("Language", True)
            Upgrade.Upgrade.InstallPackages("Provider", True)
            Upgrade.Upgrade.InstallPackages("AuthSystem", True)
            Upgrade.Upgrade.InstallPackages("Package", True)

            Response.Write("<h2>Installation Complete</h2>")
            Response.Write("<br><br><h2><a href='../Default.aspx'>Click Here To Access Your Portal</a></h2><br><br>")
            Response.Flush()


            'Write out Footer
            HtmlUtils.WriteFooter(Response)
        End Sub

        Private Sub NoUpgrade()
            ' get path to script files
            Dim strProviderPath As String = DotNetNuke.Data.DataProvider.Instance().GetProviderPath()
            If Not strProviderPath.StartsWith("ERROR:") Then
                Dim strDatabaseVersion As String
                ' get current database version
                Try
                    Dim dr As IDataReader = DotNetNuke.Data.DataProvider.Instance().GetDataBaseVersion
                    If dr.Read Then
                        'Write out Header
                        HtmlUtils.WriteHeader(Response, "none")
                        Dim currentAssembly As String = DotNetNukeContext.Current.Application.Version.ToString(3)
                        Dim currentDatabase As String = dr("Major") & "." & dr("Minor") & "." & dr("Build")
                        'do not show versions if the same to stop information leakage
                        If currentAssembly = currentDatabase Then
                            Response.Write("<h2>Current Assembly Version & current Database Version are identical.</h2>")
                        Else
                            Response.Write("<h2>Current Assembly Version: " & currentAssembly.ToString & "</h2>")
                            'Call Upgrade with the current DB Version to upgrade an
                            'existing DNN installation
                            strDatabaseVersion = Format(dr("Major"), "00") & "." & Format(dr("Minor"), "00") & "." & Format(dr("Build"), "00")
                            Response.Write("<h2>Current Database Version: " & strDatabaseVersion & "</h2>")
                        End If
                        
                        Response.Write("<br><br><a href='Install.aspx?mode=Install'>Click Here To Upgrade DotNetNuke</a>")
                        Response.Flush()
                    Else
                        'Write out Header
                        HtmlUtils.WriteHeader(Response, "noDBVersion")
                        Response.Write("<h2>Current Assembly Version: " & DotNetNukeContext.Current.Application.Version.ToString(3) & "</h2>")

                        Response.Write("<h2>Current Database Version: N/A</h2>")
                        Response.Write("<br><br><h2><a href='Install.aspx?mode=Install'>Click Here To Install DotNetNuke</a></h2>")
                        Response.Flush()
                    End If
                    dr.Close()
                Catch ex As Exception
                    'Write out Header
                    HtmlUtils.WriteHeader(Response, "error")
                    Response.Write("<h2>Current Assembly Version: " & DotNetNukeContext.Current.Application.Version.ToString(3) & "</h2>")

                    Response.Write("<h2>" & ex.Message & "</h2>")
                    Response.Flush()
                End Try
            Else
                'Write out Header
                HtmlUtils.WriteHeader(Response, "error")
                Response.Write("<h2>Current Assembly Version: " & DotNetNukeContext.Current.Application.Version.ToString(3) & "</h2>")

                Response.Write("<h2>" & strProviderPath & "</h2>")
                Response.Flush()
            End If
            'Write out Footer
            HtmlUtils.WriteFooter(Response)
        End Sub
#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Get current Script time-out
            Dim scriptTimeOut As Integer = Server.ScriptTimeout

            Dim mode As String = ""
            If Not (Request.QueryString("mode") Is Nothing) Then
                mode = Request.QueryString("mode").ToLower
            End If

            'Disable Client side caching
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache)

            'Check mode is not Nothing
            If mode = "none" Then
                NoUpgrade()
            Else
                'Set Script timeout to MAX value
                Server.ScriptTimeout = Integer.MaxValue

                Select Case Globals.Status
                    Case Globals.UpgradeStatus.Install
                        InstallApplication()
                        'Force an App Restart
                        Config.Touch()
                    Case Globals.UpgradeStatus.Upgrade
                        UpgradeApplication()
                        'Force an App Restart
                        Config.Touch()
                    Case Globals.UpgradeStatus.None
                        'Check mode
                        Select Case mode
                            Case "addportal"
                                AddPortal()
                            Case "installresources"
                                InstallResources()
                            Case "executescripts"
                                ExecuteScripts()
                        End Select
                    Case Globals.UpgradeStatus.Error
                        NoUpgrade()
                End Select

                'restore Script timeout
                Server.ScriptTimeout = scriptTimeOut
            End If

        End Sub

#End Region

    End Class

End Namespace
