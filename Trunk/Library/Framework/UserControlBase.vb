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
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Threading

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals


Namespace DotNetNuke.Framework

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserControlBase class defines a custom base class inherited by all
    ''' user controls within the Portal. 
    ''' </summary>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserControlBase
        Inherits UserControl

#Region "Public Properties"

        Public ReadOnly Property IsHostMenu() As Boolean
            Get
                Dim _IsHost As Boolean = False
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId OrElse PortalSettings.ActiveTab.TabID = PortalSettings.SuperTabId Then
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

#End Region

        <Obsolete("There is no longer the concept of an Admin Page.  All pages are controlled by Permissions")> _
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
