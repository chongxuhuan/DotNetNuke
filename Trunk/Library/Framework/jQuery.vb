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

Imports System.IO
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.UI.Utilities
Imports System.Text.RegularExpressions

Namespace DotNetNuke.Framework
    Public Class jQuery

#Region "Private Constants"
        Private Const jQueryDebugFile As String = "~/Resources/Shared/Scripts/jquery/jquery.js"
        Private Const jQueryMinFile As String = "~/Resources/Shared/Scripts/jquery/jquery.min.js"
        Private Const jQueryVersionKey As String = "jQueryVersionKey"
        Private Const jQueryVersionMatch As String = "(?<=jquery:\s"")(.*)(?="")"
#End Region

        ''' <summary>
        ''' Returns the default URL for a hosted version of the jQuery script
        ''' </summary>
        ''' <remarks>
        ''' Google hosts versions of many popular javascript libraries on their CDN.
        ''' Using the hosted version increases the likelihood that the file is already
        ''' cached in the users browser. 
        ''' </remarks>
        Public Const DefaultHostedUrl As String = "http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js"

        Private Shared Function GetSettingAsBoolean(ByVal key As String, ByVal defaultValue As Boolean) As Boolean
            Dim retValue As Boolean = defaultValue
            Try
                Dim setting As Object = HttpContext.Current.Items(key)
                If setting IsNot Nothing Then
                    retValue = Convert.ToBoolean(setting)
                End If
            Catch ex As Exception
                Exceptions.LogException(ex)
            End Try
            Return retValue
        End Function

        Private Shared Function IsScriptRegistered() As Boolean
            Return CType(IIf(HttpContext.Current.Items("jquery_registered") IsNot Nothing, True, False), Boolean)
        End Function

#Region "Public Properties"

        ''' <summary>
        ''' Checks whether the jQuery core script file exists locally.
        ''' </summary>
        ''' <remarks>
        ''' This property checks for both the minified version and the full uncompressed version of jQuery.  
        ''' These files should exist in the /Resources/Shared/Scripts/jquery directory.
        ''' </remarks>
        Public Shared ReadOnly Property IsInstalled() As Boolean
            Get
                Dim minFile As String = JQueryFileMapPath(True)
                Dim dbgFile As String = JQueryFileMapPath(False)
                Return File.Exists(minFile) OrElse File.Exists(dbgFile)
            End Get
        End Property

        Public Shared ReadOnly Property IsRequested() As Boolean
            Get
                Return GetSettingAsBoolean("jQueryRequested", False)
            End Get
        End Property

        ''' <summary>
        ''' Gets the HostSetting to determine if we should use the standard jQuery script or the minified jQuery script.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>This is a simple wrapper around the Host.jQueryDebug property</remarks>
        Public Shared ReadOnly Property UseDebugScript() As Boolean
            Get
                Return Host.jQueryDebug
            End Get
        End Property

        ''' <summary>
        ''' Gets the HostSetting to determine if we should use a hosted version of the jQuery script.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>This is a simple wrapper around the Host.jQueryHosted property</remarks>
        Public Shared ReadOnly Property UseHostedScript() As Boolean
            Get
                Return Host.jQueryHosted
            End Get
        End Property

        ''' <summary>
        ''' Gets the HostSetting for the URL of the hosted version of the jQuery script.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>This is a simple wrapper around the Host.jQueryUrl property</remarks>
        Public Shared ReadOnly Property HostedUrl() As String
            Get
                Return Host.jQueryUrl
            End Get
        End Property

        ''' <summary>
        ''' Gets the version string for the local jQuery script
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' This only evaluates the version in the full jQuery file and assumes that the minified script
        ''' is the same version as the full script.
        ''' </remarks>
        Public Shared ReadOnly Property Version() As String
            Get
                Dim ver As String = CType(DataCache.GetCache(jQueryVersionKey), String)
                If String.IsNullOrEmpty(ver) Then
                    If IsInstalled Then
                        Dim jqueryFileName As String = JQueryFileMapPath(False)
                        Dim jfiletext As String = File.ReadAllText(jqueryFileName)
                        Dim verMatch As Match = Regex.Match(jfiletext, jQueryVersionMatch)
                        If verMatch IsNot Nothing Then
                            ver = verMatch.Value
                            DataCache.SetCache(jQueryVersionKey, ver, New System.Web.Caching.CacheDependency(jqueryFileName))
                        Else
                            ver = Localization.GetString("jQuery.UnknownVersion.Text")
                        End If
                    Else
                        ver = Localization.GetString("jQuery.NotInstalled.Text")
                    End If
                End If
                Return ver
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Shared Function JQueryFileMapPath(ByVal GetMinFile As Boolean) As String
            Return HttpContext.Current.Server.MapPath(JQueryFile(GetMinFile))
        End Function

        Public Shared Function JQueryFile(ByVal GetMinFile As Boolean) As String
            Dim jfile As String = jQueryDebugFile
            If GetMinFile Then
                jfile = jQueryMinFile
            End If
            Return ResolveUrl(jfile)

        End Function

        Public Shared Function GetJQueryScriptReference() As String

            Dim scriptsrc As String = HostedUrl
            If Not UseHostedScript Then
                scriptsrc = JQueryFile(Not UseDebugScript)
            End If

            Return String.Format(glbScriptFormat, scriptsrc)
        End Function

        Public Shared Sub RegisterScript(ByVal page As System.Web.UI.Page)
            RegisterScript(page, jQuery.GetJQueryScriptReference)
        End Sub

        Public Shared Sub RegisterScript(ByVal page As System.Web.UI.Page, ByVal script As String)
            ' This method is likely to be called late in the page lifecycle
            ' We need to ensure that jQuery is loaded on the page as early as possible so
            ' we have to hand register the script in the page Head block.
            If Not IsScriptRegistered() Then
                HttpContext.Current.Items("jquery_registered") = True
                Dim headscript As Literal = New Literal()
                headscript.Text = script
                page.Header.Controls.Add(headscript)
            End If

        End Sub

        Public Shared Sub RequestRegistration()
            HttpContext.Current.Items("jQueryRequested") = True
        End Sub
#End Region

        <Obsolete("Deprectaed in DNN 5.1. Replaced by IsRequested.")> _
        Public Shared ReadOnly Property IsRquested() As Boolean
            Get
                Return IsRequested
            End Get
        End Property


    End Class
End Namespace
