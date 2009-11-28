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

Imports System
Imports System.IO
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls

Imports DotNetNuke
Imports DotNetNuke.Services.Analytics
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common

Namespace DotNetNuke.HttpModules.Analytics
    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.HttpModules.Analytics
    ''' Project:    HttpModules
    ''' Module:     AnalyticsModule
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' This module contains functionality for injecting web analytics scripts into the page
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cniknet]	05/03/2009	created
    ''' </history>
    ''' -----------------------------------------------------------------------------

    Public Class AnalyticsModule
        Implements IHttpModule

        Public ReadOnly Property ModuleName() As String
            Get
                Return "AnalyticsModule"
            End Get
        End Property

        Public Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init
            AddHandler application.PreRequestHandlerExecute, AddressOf Me.OnPreRequestHandlerExecute
        End Sub

        Private Sub OnPreRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)

            Try
                'First check if we are upgrading/installing or if it is a non-page request
                Dim app As HttpApplication = CType(sender, HttpApplication)
                Dim Request As HttpRequest = app.Request

                'First check if we are upgrading/installing
                If Request.Url.LocalPath.ToLower.EndsWith("install.aspx") OrElse Request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") Then
                    Exit Sub
                End If

                'exit if a request for a .net mapping that isn't a content page is made i.e. axd
                If Request.Url.LocalPath.ToLower.EndsWith(".aspx") = False _
                        AndAlso Request.Url.LocalPath.ToLower.EndsWith(".asmx") = False _
                        AndAlso Request.Url.LocalPath.ToLower.EndsWith(".ashx") = False Then
                    Exit Sub
                End If

                If Not (HttpContext.Current Is Nothing) Then
                    Dim context As HttpContext = HttpContext.Current
                    If (context Is Nothing) Then Exit Sub
                    Dim page As DotNetNuke.Framework.CDefault = TryCast(context.Handler, DotNetNuke.Framework.CDefault)
                    If (page Is Nothing) Then Exit Sub
                    AddHandler page.Load, AddressOf OnPageLoad
                End If

            Catch ex As Exception
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("Analytics.AnalyticsModule", "OnPreRequestHandlerExecute")
                objEventLogInfo.AddProperty("ExceptionMessage", ex.Message)
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLog.AddLog(objEventLogInfo)
            End Try
        End Sub


        Private Sub OnPageLoad(ByVal sender As Object, ByVal e As EventArgs)

            Try
                Dim analyticsEngines As Config.AnalyticsEngineCollection = Config.AnalyticsEngineConfiguration.GetConfig().AnalyticsEngines
                If analyticsEngines Is Nothing OrElse analyticsEngines.Count = 0 Then Exit Sub

                Dim page As Page = CType(sender, Page)
                If (page Is Nothing) Then Exit Sub

                For Each engine As Config.AnalyticsEngine In analyticsEngines
                    If (engine.ElementId <> "") Then
                        Dim objEngine As AnalyticsEngineBase = Nothing
                        If (engine.EngineType <> "") Then
                            Dim engineType As Type = Type.GetType(engine.EngineType)
                            objEngine = CType(Activator.CreateInstance(engineType), AnalyticsEngineBase)
                        Else
                            objEngine = CType(New DotNetNuke.Services.Analytics.GenericAnalyticsEngine(), AnalyticsEngineBase)
                        End If
                        If Not (objEngine Is Nothing) Then
                            Dim script As String = engine.ScriptTemplate
                            If (script <> "") Then
                                script = objEngine.RenderScript(script)
                                If (script <> "") Then
                                    Dim element As HtmlContainerControl = CType(page.FindControl(engine.ElementId), HtmlContainerControl)
                                    If Not element Is Nothing Then
                                        Dim scriptControl As New LiteralControl()
                                        scriptControl.Text = script
                                        If engine.InjectTop Then
                                            element.Controls.AddAt(0, scriptControl)
                                        Else
                                            element.Controls.Add(scriptControl)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("Analytics.AnalyticsModule", "OnPageLoad")
                objEventLogInfo.AddProperty("ExceptionMessage", ex.Message)
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLog.AddLog(objEventLogInfo)
            End Try

        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

    End Class

End Namespace
