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
Imports DotNetNuke.Services.EventQueue

Namespace DotNetNuke.Entities.Modules

    Public Class EventMessageProcessor
        Inherits EventQueue.EventMessageProcessorBase

#Region "Private Methods"

        Private Sub ImportModule(ByVal message As EventMessage)
            Try
                Dim BusinessControllerClass As String = message.Attributes.Item("BusinessControllerClass")
                Dim objController As Object = Framework.Reflection.CreateObject(BusinessControllerClass, "")
                If TypeOf objController Is IPortable Then
                    Dim ModuleId As Integer = CType(message.Attributes.Item("ModuleId"), Integer)
                    Dim Content As String = HttpContext.Current.Server.HtmlDecode(message.Attributes.Item("Content"))
                    Dim Version As String = message.Attributes.Item("Version")
                    Dim UserID As Integer = CType(message.Attributes.Item("UserId"), Integer)

                    'call the IPortable interface for the module/version
                    CType(objController, IPortable).ImportModule(ModuleId, Content, Version, UserID)

                    'Synchronize Module Cache
                    ModuleController.SynchronizeModule(ModuleId)
                End If
            Catch exc As Exception
                ' an error occurred
                LogException(exc)
            End Try
        End Sub

        Private Sub UpgradeModule(ByVal message As EventMessage)
            Try
                Dim BusinessControllerClass As String = message.Attributes.Item("BusinessControllerClass")
                Dim objController As Object = Framework.Reflection.CreateObject(BusinessControllerClass, "")
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As Services.Log.EventLog.LogInfo
                If TypeOf objController Is IUpgradeable Then
                    ' get the list of applicable versions
                    Dim UpgradeVersions As String() = message.Attributes.Item("UpgradeVersionsList").ToString().Split(",".ToCharArray())
                    For Each Version As String In UpgradeVersions
                        'call the IUpgradeable interface for the module/version
                        Dim Results As String = CType(objController, IUpgradeable).UpgradeModule(Version)
                        'log the upgrade results
                        objEventLogInfo = New Services.Log.EventLog.LogInfo
                        objEventLogInfo.AddProperty("Module Upgraded", BusinessControllerClass)
                        objEventLogInfo.AddProperty("Version", Version)
                        If Not String.IsNullOrEmpty(Results) Then
                            objEventLogInfo.AddProperty("Results", Results)
                        End If
                        objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.MODULE_UPDATED.ToString
                        objEventLog.AddLog(objEventLogInfo)
                    Next
                End If
                UpdateSupportedFeatures(objController, CType(message.Attributes.Item("DesktopModuleId"), Integer))
            Catch exc As Exception
                ' an error occurred
                LogException(exc)
            End Try
        End Sub

        Private Sub UpdateSupportedFeatures(ByVal message As EventMessage)
            Dim BusinessControllerClass As String = message.Attributes.Item("BusinessControllerClass")
            Dim objController As Object = Framework.Reflection.CreateObject(BusinessControllerClass, "")
            UpdateSupportedFeatures(objController, CType(message.Attributes.Item("DesktopModuleId"), Integer))
        End Sub

        Private Sub UpdateSupportedFeatures(ByVal objController As Object, ByVal desktopModuleId As Integer)
            Try
                Dim oDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModule(desktopModuleId, Null.NullInteger)

                If (oDesktopModule IsNot Nothing) Then
                    'Initialise the SupportedFeatures
                    oDesktopModule.SupportedFeatures = 0

                    'Test the interfaces
                    oDesktopModule.IsPortable = (TypeOf objController Is IPortable)
                    oDesktopModule.IsSearchable = (TypeOf objController Is ISearchable)
                    oDesktopModule.IsUpgradeable = (TypeOf objController Is IUpgradeable)

                    DesktopModuleController.SaveDesktopModule(oDesktopModule, False, True)
                End If
            Catch exc As Exception
                ' an error occurred
                LogException(exc)
            End Try
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Function ProcessMessage(ByVal message As EventMessage) As Boolean
            Try
                Select Case message.ProcessorCommand
                    Case "UpdateSupportedFeatures"
                        UpdateSupportedFeatures(message)
                    Case "UpgradeModule"
                        UpgradeModule(message)
                    Case "ImportModule"
                        ImportModule(message)
                    Case Else
                        ' other events can be added here
                End Select
            Catch ex As Exception
                message.ExceptionMessage = ex.Message
                Return False
            End Try
            Return True
        End Function

#End Region

    End Class

End Namespace
