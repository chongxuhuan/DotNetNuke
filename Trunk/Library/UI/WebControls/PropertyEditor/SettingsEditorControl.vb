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

Imports System.ComponentModel
Imports System.Reflection


Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      SettingsEditorControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SettingsEditorControl control provides an Editor to edit DotNetNuke
    ''' Settings
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/14/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:SettingsEditorControl runat=server></{0}:SettingsEditorControl>")> _
    Public Class SettingsEditorControl
        Inherits PropertyEditorControl

#Region "Private Members"

        Private _CustomEditors As Hashtable
        Private _Visibility As Hashtable
        Private _UnderlyingDataSource As IEnumerable

#End Region

#Region "Protected Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Underlying DataSource
        ''' </summary>
        ''' <value>An IEnumerable</value>
        ''' <history>
        ''' 	[cnurse]	03/09/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property UnderlyingDataSource() As IEnumerable
            Get
                If _UnderlyingDataSource Is Nothing Then
                    _UnderlyingDataSource = GetSettings()
                End If
                Return _UnderlyingDataSource
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the CustomEditors that are used by this control
        ''' </summary>
        ''' <value>The CustomEditors object</value>
        ''' <history>
        ''' 	[cnurse]	03/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property CustomEditors() As Hashtable
            Get
                Return _CustomEditors
            End Get
            Set(ByVal Value As Hashtable)
                _CustomEditors = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Visibility values that are used by this control
        ''' </summary>
        ''' <value>The CustomEditors object</value>
        ''' <history>
        ''' 	[cnurse]	08/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Visibility() As Hashtable
            Get
                Return _Visibility
            End Get
            Set(ByVal value As Hashtable)
                _Visibility = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSettings converts the DataSource into an ArrayList (IEnumerable)
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetSettings() As ArrayList

            Dim settings As Hashtable = CType(DataSource, Hashtable)
            Dim arrSettings As New ArrayList
            Dim settingsEnumerator As IDictionaryEnumerator = settings.GetEnumerator()
            While settingsEnumerator.MoveNext()
                Dim info As SettingInfo = New SettingInfo(settingsEnumerator.Key, settingsEnumerator.Value)
                If (Not CustomEditors Is Nothing) AndAlso (Not CustomEditors(settingsEnumerator.Key) Is Nothing) Then
                    info.Editor = CType(CustomEditors(settingsEnumerator.Key), String)
                End If
                arrSettings.Add(info)
            End While

            arrSettings.Sort(New SettingNameComparer)

            Return arrSettings

        End Function

#End Region

#Region "Protected Methods"

        Protected Overloads Overrides Sub AddEditorRow(ByRef tbl As Table, ByVal obj As Object)

            Dim info As SettingInfo = CType(obj, SettingInfo)

            AddEditorRow(tbl, info.Name, New SettingsEditorInfoAdapter(DataSource, obj, Me.ID))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetRowVisibility determines the Visibility of a row in the table
        ''' </summary>
        ''' <param name="obj">The property</param>
        ''' <history>
        '''     [cnurse]	03/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetRowVisibility(ByVal obj As Object) As Boolean

            Dim info As SettingInfo = CType(obj, SettingInfo)
            Dim _IsVisible As Boolean = True

            If (Not Visibility Is Nothing) AndAlso (Not Visibility(info.Name) Is Nothing) Then
                _IsVisible = CType(Visibility(info.Name), Boolean)
            End If

            Return _IsVisible

        End Function

#End Region

    End Class

End Namespace

