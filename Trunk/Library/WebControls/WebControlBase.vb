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
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO

Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals

Namespace DotNetNuke.UI.WebControls

    Public MustInherit Class WebControlBase
        Inherits WebControl

#Region "Private Members"
        Private _styleSheetUrl As String = ""
        Private _theme As String = ""
#End Region

#Region "Public Properties"

        Public Property Theme() As String
            Get
                Return _theme
            End Get
            Set(ByVal value As String)
                _theme = value
            End Set
        End Property

        Public ReadOnly Property ResourcesFolderUrl() As String
            Get
                Return Globals.ResolveUrl("~/Resources/")
            End Get
        End Property

        Public Property StyleSheetUrl() As String
            Get
                If (_styleSheetUrl.StartsWith("~")) Then
                    Return Globals.ResolveUrl(_styleSheetUrl)
                Else
                    Return _styleSheetUrl
                End If
            End Get
            Set(ByVal value As String)
                _styleSheetUrl = value
            End Set
        End Property

        Public ReadOnly Property IsHostMenu() As Boolean
            Get
                Dim _IsHost As Boolean = False
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                    _IsHost = True
                End If
                Return _IsHost
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property PortalSettings() As PortalSettings

            Get
                PortalSettings = PortalController.GetCurrentPortalSettings
            End Get

        End Property

        Public MustOverride ReadOnly Property HtmlOutput() As String
#End Region

#Region "Shared Methods"

        Protected Overrides Sub RenderContents(ByVal output As HtmlTextWriter)
            output.Write(HtmlOutput)
        End Sub

#End Region

        <Obsolete("There is no longer the concept of an Admin Page.  All pages are controlled by Permissions", True)> _
        Public ReadOnly Property IsAdminMenu() As Boolean
            Get
                Dim _IsAdmin As Boolean = False
                'If PortalSettings.ActiveTab.ParentId = PortalSettings.AdminTabId Then
                '    _IsAdmin = True
                'End If
                Return _IsAdmin
            End Get
        End Property


    End Class
End Namespace
