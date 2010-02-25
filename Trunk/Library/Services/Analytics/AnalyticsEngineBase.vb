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
Imports System.Web.UI
Imports DotNetNuke.Services.Analytics.Config
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.Services.Analytics
    Public MustInherit Class AnalyticsEngineBase

#Region "Public Methods"

        Public MustOverride Function RenderScript(ByVal scriptTemplate As String) As String

        Public Function ReplaceTokens(ByVal s As String) As String
            Dim tokenizer As New DotNetNuke.Services.Tokens.TokenReplace()
            tokenizer.AccessingUser = UserController.GetCurrentUserInfo()
            tokenizer.DebugMessages = False
            Return (tokenizer.ReplaceEnvironmentTokens(s))
        End Function
#End Region

#Region "Public Events"

#End Region

#Region "Private Members"

#End Region

#Region "Public Shared Methods"
        Public Function GetConfig() As AnalyticsConfiguration
            Return AnalyticsConfiguration.GetConfig(Me.EngineName)
        End Function

#End Region

#Region "Public Properties"
        Public MustOverride ReadOnly Property EngineName() As String

        Public Overridable Function RenderCustomScript(ByVal config As AnalyticsConfiguration) As String
            Return ""
        End Function

#End Region

#Region "Protected Event Methods"

#End Region

    End Class

End Namespace

