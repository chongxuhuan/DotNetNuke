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
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SearchConfig class provides a configuration class for Search
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/10/2007  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class SearchConfig

#Region "Private Members"

        Private _SearchIncludeCommon As Boolean
        Private _SearchIncludeNumeric As Boolean
        Private _SearchMaxWordlLength As Integer
        Private _SearchMinWordlLength As Integer

        Private Shared CACHEKEY As String = "SearchConfig {0}"

#End Region

#Region "Constructor(s)"

        Public Sub New(ByVal portalID As Integer)
            Me.New(PortalController.GetPortalSettingsDictionary(portalID))
        End Sub

        Public Sub New(ByVal settings As Dictionary(Of String, String))
            _SearchIncludeCommon = GetSettingAsBoolean("SearchIncludeCommon", settings, Host.SearchIncludeCommon)
            _SearchIncludeNumeric = GetSettingAsBoolean("SearchIncludeNumeric", settings, Host.SearchIncludeNumeric)
            _SearchMaxWordlLength = GetSettingAsInteger("MaxSearchWordLength", settings, Host.SearchMaxWordlLength)
            _SearchMinWordlLength = GetSettingAsInteger("MinSearchWordLength", settings, Host.SearchMinWordlLength)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to inlcude Common Words in the Search Index
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchIncludeCommon() As Boolean
            Get
                Return _SearchIncludeCommon
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to inlcude Numbers in the Search Index
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchIncludeNumeric() As Boolean
            Get
                Return _SearchIncludeNumeric
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the maximum Search Word length to index
        ''' </summary>
        ''' <remarks>Defaults to 25</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchMaxWordlLength() As Integer
            Get
                Return _SearchMaxWordlLength
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the maximum Search Word length to index
        ''' </summary>
        ''' <remarks>Defaults to 3</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchMinWordlLength() As Integer
            Get
                Return _SearchMinWordlLength
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function GetSettingAsBoolean(ByVal key As String, ByVal settings As Dictionary(Of String, String), ByVal defaultValue As Boolean) As Boolean
            Dim retValue As Boolean
            Try
                Dim setting As String = Null.NullString
                settings.TryGetValue(key, setting)
                If String.IsNullOrEmpty(setting) Then
                    retValue = defaultValue
                Else
                    retValue = (setting.ToUpperInvariant().StartsWith("Y") OrElse setting.ToUpperInvariant = "TRUE")
                End If
            Catch ex As Exception
                'we just want to trap the error as we may not be installed so there will be no Settings
            End Try
            Return retValue
        End Function

        Private Function GetSettingAsInteger(ByVal key As String, ByVal settings As Dictionary(Of String, String), ByVal defaultValue As Integer) As Integer
            Dim retValue As Integer
            Try
                Dim setting As String = Null.NullString
                settings.TryGetValue(key, setting)
                If String.IsNullOrEmpty(setting) Then
                    retValue = defaultValue
                Else
                    retValue = Convert.ToInt32(setting)
                End If
            Catch ex As Exception
                'we just want to trap the error as we may not be installed so there will be no Settings
            End Try
            Return retValue
        End Function

#End Region

    End Class

End Namespace
