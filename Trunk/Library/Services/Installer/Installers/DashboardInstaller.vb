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

Imports System.Xml.XPath
Imports DotNetNuke.Modules.Dashboard.Components

Namespace DotNetNuke.Services.Installer.Installers

    Public Class DashboardInstaller
        Inherits ComponentInstallerBase

#Region "Private Properties"

        Private TempDashboardControl As DashboardControl
        Private Key As String
        Private Src As String
        Private LocalResources As String
        Private ControllerClass As String
        Private IsEnabled As Boolean
        Private ViewOrder As Integer

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of allowable file extensions (in addition to the Host's List)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/05/2009  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property AllowableFiles() As String
            Get
                Return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html"
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub DeleteDashboard()
            Try
                'Attempt to get the Dashboard
                Dim dashboardControl As DashboardControl = DashboardController.GetDashboardControlByPackageID(Package.PackageID)

                If dashboardControl IsNot Nothing Then
                    DashboardController.DeleteControl(dashboardControl)
                End If
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.AUTHENTICATION_UnRegistered)
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub Commit()

        End Sub

        Public Overrides Sub Install()
            Dim bAdd As Boolean = Null.NullBoolean

            Try
                'Attempt to get the Dashboard
                TempDashboardControl = DashboardController.GetDashboardControlByKey(Key)
                Dim dashboardControl As DashboardControl = New DashboardControl

                If TempDashboardControl Is Nothing Then
                    'Enable by default
                    dashboardControl.IsEnabled = True
                    bAdd = True
                Else
                    dashboardControl.DashboardControlID = TempDashboardControl.DashboardControlID
                    dashboardControl.IsEnabled = TempDashboardControl.IsEnabled
                End If
                dashboardControl.DashboardControlKey = Key
                dashboardControl.PackageID = Package.PackageID
                dashboardControl.DashboardControlSrc = Src
                dashboardControl.DashboardControlLocalResources = LocalResources
                dashboardControl.ControllerClass = ControllerClass
                dashboardControl.ViewOrder = ViewOrder

                If bAdd Then
                    'Add new Dashboard
                    DashboardController.AddDashboardControl(dashboardControl)
                Else
                    'Update Dashboard
                    DashboardController.UpdateDashboardControl(dashboardControl)
                End If

                Completed = True
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.DASHBOARD_Registered)
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifest method reads the manifest file for the Authentication compoent.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub ReadManifest(ByVal manifestNav As XPathNavigator)
            'Get the Key
            Key = Util.ReadElement(manifestNav, "dashboardControl/key", Log, Util.DASHBOARD_KeyMissing)

            'Get the Src
            Src = Util.ReadElement(manifestNav, "dashboardControl/src", Log, Util.DASHBOARD_SrcMissing)

            'Get the LocalResources
            LocalResources = Util.ReadElement(manifestNav, "dashboardControl/localResources", Log, Util.DASHBOARD_LocalResourcesMissing)

            'Get the ControllerClass
            ControllerClass = Util.ReadElement(manifestNav, "dashboardControl/controllerClass")

            'Get the IsEnabled Flag
            IsEnabled = Boolean.Parse(Util.ReadElement(manifestNav, "dashboardControl/isEnabled", "true"))

            'Get the ViewOrder
            ViewOrder = Integer.Parse(Util.ReadElement(manifestNav, "dashboardControl/viewOrder", "-1"))

            If Log.Valid Then
                Log.AddInfo(Util.DASHBOARD_ReadSuccess)
            End If
        End Sub

        Public Overrides Sub Rollback()
            'If Temp Dashboard exists then we need to update the DataStore with this 
            If TempDashboardControl Is Nothing Then
                'No Temp Dashboard - Delete newly added system
                DeleteDashboard()
            Else
                'Temp Dashboard - Rollback to Temp
                DashboardController.UpdateDashboardControl(TempDashboardControl)
            End If

        End Sub

        Public Overrides Sub UnInstall()
            Try
                'Attempt to get the DashboardControl
                Dim dashboardControl As DashboardControl = DashboardController.GetDashboardControlByPackageID(Package.PackageID)

                If dashboardControl IsNot Nothing Then
                    DashboardController.DeleteControl(dashboardControl)
                End If
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.DASHBOARD_UnRegistered)
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

#End Region

    End Class

End Namespace
