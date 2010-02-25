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

Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Common.Lists

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      EditorInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditorInfo class provides a helper class for the Property Editor
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	12/13/2005	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class EditorInfo

        Private _Attributes As Object()
        Private _Category As String
        Private _ControlStyle As Style
        Private _EditMode As PropertyEditorMode
        Private _Editor As String
        Private _LabelMode As LabelMode
        Private _Name As String
        Private _Required As Boolean
        Private _ResourceKey As String
        Private _Type As String
        Private _Value As Object
        Private _ValidationExpression As String
        Private _Visible As Boolean = True
        Private _Visibility As UserVisibilityMode

        Public Sub New()
        End Sub

        Public Property Attributes() As Object()
            Get
                Return _Attributes
            End Get
            Set(ByVal Value As Object())
                _Attributes = Value
            End Set
        End Property

        Public Property Category() As String
            Get
                Return _Category
            End Get
            Set(ByVal Value As String)
                _Category = Value
            End Set
        End Property

        Public Property ControlStyle() As Style
            Get
                Return _ControlStyle
            End Get
            Set(ByVal Value As Style)
                _ControlStyle = Value
            End Set
        End Property

        Public Property EditMode() As PropertyEditorMode
            Get
                Return _EditMode
            End Get
            Set(ByVal Value As PropertyEditorMode)
                _EditMode = Value
            End Set
        End Property

        Public Property Editor() As String
            Get
                Return _Editor
            End Get
            Set(ByVal Value As String)
                _Editor = Value
            End Set
        End Property

        Public Property LabelMode() As LabelMode
            Get
                Return _LabelMode
            End Get
            Set(ByVal Value As LabelMode)
                _LabelMode = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return _Required
            End Get
            Set(ByVal Value As Boolean)
                _Required = Value
            End Set
        End Property

        Public Property ResourceKey() As String
            Get
                Return _ResourceKey
            End Get
            Set(ByVal Value As String)
                _ResourceKey = Value
            End Set
        End Property

        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(ByVal Value As String)
                _Type = Value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return _Value
            End Get
            Set(ByVal Value As Object)
                _Value = Value
            End Set
        End Property

        Public Property ValidationExpression() As String
            Get
                Return _ValidationExpression
            End Get
            Set(ByVal Value As String)
                _ValidationExpression = Value
            End Set
        End Property

        Public Property Visible() As Boolean
            Get
                Return _Visible
            End Get
            Set(ByVal Value As Boolean)
                _Visible = Value
            End Set
        End Property

        Public Property Visibility() As UserVisibilityMode
            Get
                Return _Visibility
            End Get
            Set(ByVal Value As UserVisibilityMode)
                _Visibility = Value
            End Set
        End Property

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetEditor gets the appropriate Editor based on ID
        ''' properties
        ''' </summary>
        ''' <param name="editorType">The Id of the Editor</param>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetEditor(ByVal editorType As Integer) As String

            Dim editor As String = "UseSystemType"

            If editorType <> Null.NullInteger Then
                Dim objListController As New ListController
                Dim definitionEntry As ListEntryInfo = objListController.GetListEntryInfo(editorType)

                If (Not definitionEntry Is Nothing) AndAlso (definitionEntry.ListName = "DataType") Then
                    editor = definitionEntry.Text
                End If
            End If

            Return editor

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetEditor gets the appropriate Editor based on ID
        ''' properties
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetEditor(ByVal editorValue As String) As String

            Dim editor As String = "UseSystemType"

            Dim objListController As New ListController
            Dim definitionEntry As ListEntryInfo = objListController.GetListEntryInfo("DataType", editorValue)

            If Not definitionEntry Is Nothing Then
                editor = definitionEntry.Text
            End If

            Return editor

        End Function

#End Region


    End Class

End Namespace
