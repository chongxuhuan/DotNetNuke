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

Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      SettingInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SettingInfo class provides a helper class for the Settings Editor
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	12/13/2005	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SettingInfo

#Region "Private Members"

        Private _Name As String
        Private _Type As Type
        Private _Value As Object
        Private _Editor As String

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new SettingInfo obect
        ''' </summary>
        ''' <param name="name">The name of the setting</param>
        ''' <param name="value">The value of the setting</param>
        ''' <history>
        ''' 	[cnurse]	03/23/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal name As Object, ByVal value As Object)

            _Name = CType(name, String)
            _Value = value
            _Type = value.GetType
            _Editor = EditorInfo.GetEditor(-1)

            Dim strValue As String = CType(value, String)
            Dim IsFound As Boolean = False

            If _Type.IsEnum Then
                IsFound = True
            End If

            If Not IsFound Then
                Try
                    Dim boolValue As Boolean = Boolean.Parse(strValue)
                    Editor = EditorInfo.GetEditor("TrueFalse")
                    IsFound = True
                Catch ex As Exception
                End Try
            End If
            If Not IsFound Then
                Try
                    Dim intValue As Integer = Integer.Parse(strValue)
                    Editor = EditorInfo.GetEditor("Integer")
                    IsFound = True
                Catch ex As Exception
                End Try
            End If

        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Setting Name
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Setting Value
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Value() As Object
            Get
                Return _Value
            End Get
            Set(ByVal Value As Object)
                _Value = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Editor to use for the Setting
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Editor() As String
            Get
                Return _Editor
            End Get
            Set(ByVal Value As String)
                _Editor = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Setting Type
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Type() As Type
            Get
                Return _Type
            End Get
            Set(ByVal Value As Type)
                _Type = Value
            End Set
        End Property

#End Region

    End Class

End Namespace
