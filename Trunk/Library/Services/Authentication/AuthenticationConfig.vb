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

Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Globalization

Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Services.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationConfig class providesa configuration class for the DNN
    ''' Authentication provider
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/10/2007  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> _
    Public Class AuthenticationConfig
        Inherits AuthenticationConfigBase

#Region "Private Members"

        Private _Enabled As Boolean = True
        Private _UseCaptcha As Boolean = Null.NullBoolean

        Private Const CACHEKEY As String = "Authentication.DNN"

#End Region

#Region "Constructor(s)"

        Protected Sub New(ByVal portalID As Integer)
            MyBase.New(portalID)

            Try
                Dim setting As String = Null.NullString
                If PortalController.GetPortalSettingsDictionary(portalID).TryGetValue("DNN_Enabled", setting) Then
                    _Enabled = Boolean.Parse(setting)
                End If
                setting = Null.NullString
                If PortalController.GetPortalSettingsDictionary(portalID).TryGetValue("DNN_UseCaptcha", setting) Then
                    _UseCaptcha = Boolean.Parse(setting)
                End If
            Catch
            End Try
        End Sub

#End Region

#Region "Public Properties"

        Public Property Enabled() As Boolean
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property

        Public Property UseCaptcha() As Boolean
            Get
                Return _UseCaptcha
            End Get
            Set(ByVal value As Boolean)
                _UseCaptcha = value
            End Set
        End Property

#End Region

#Region "Public SHared Methods"

        Public Shared Sub ClearConfig(ByVal portalId As Integer)
            Dim key As String = CACHEKEY + "_" + portalId.ToString()
            DataCache.RemoveCache(key)
        End Sub

        Public Shared Function GetConfig(ByVal portalId As Integer) As AuthenticationConfig

            Dim key As String = CACHEKEY + "_" + portalId.ToString()
            Dim config As AuthenticationConfig = CType(DataCache.GetCache(key), AuthenticationConfig)

            If config Is Nothing Then
                config = New AuthenticationConfig(portalId)
                DataCache.SetCache(key, config)
            End If
            Return config
        End Function

        Public Shared Sub UpdateConfig(ByVal config As AuthenticationConfig)
            PortalController.UpdatePortalSetting(config.PortalID, "DNN_Enabled", config.Enabled.ToString())
            PortalController.UpdatePortalSetting(config.PortalID, "DNN_UseCaptcha", config.UseCaptcha.ToString())
            ClearConfig(config.PortalID)
        End Sub

#End Region

    End Class

End Namespace
