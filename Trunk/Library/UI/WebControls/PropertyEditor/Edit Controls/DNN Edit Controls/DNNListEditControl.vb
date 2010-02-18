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

Imports System.Collections.Specialized

Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DNNListEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNListEditControl control provides a standard UI component for selecting
    ''' from Lists
    ''' </summary>
    ''' <history>
    '''     [cnurse]	05/04/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:DNNListEditControl runat=server></{0}:DNNListEditControl>")> _
    Public Class DNNListEditControl
        Inherits EditControl
        Implements IPostBackEventHandler

        Private _AutoPostBack As Boolean
        Private _ListName As String = Null.NullString
        Private _ParentKey As String = Null.NullString
        Private _TextField As ListBoundField = ListBoundField.Text
        Private _ValueField As ListBoundField = ListBoundField.Value
        Private _List As ListEntryInfoCollection

#Region "Events"

        Public Event ItemChanged As PropertyChangedEventHandler

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a DNNListEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()

        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the List Auto Posts Back
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property AutoPostBack() As Boolean
            Get
                Return _AutoPostBack
            End Get
            Set(ByVal Value As Boolean)
                _AutoPostBack = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IntegerValue returns the Integer representation of the Value
        ''' </summary>
        ''' <value>An integer representing the Value</value>
        ''' <history>
        '''     [cnurse]	06/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property IntegerValue() As Integer
            Get
                Dim intValue As Integer = Null.NullInteger
                Try
                    'Try and cast the value to an Integer
                    intValue = CType(Value, Integer)
                Catch ex As Exception
                End Try
                Return intValue
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' List gets the List associated with the control
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property List() As ListEntryInfoCollection
            Get
                If _List Is Nothing Then
                    Dim objListController As New ListController
                    _List = objListController.GetListEntryInfoCollection(ListName, ParentKey, Me.PortalId)
                End If
                Return _List
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ListName is the name of the List to display
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Property ListName() As String
            Get
                If _ListName = Null.NullString Then
                    _ListName = Me.Name
                End If
                Return _ListName
            End Get
            Set(ByVal Value As String)
                _ListName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OldIntegerValue returns the Integer representation of the OldValue
        ''' </summary>
        ''' <value>An integer representing the OldValue</value>
        ''' <history>
        '''     [cnurse]	06/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property OldIntegerValue() As Integer
            Get
                Dim intValue As Integer = Null.NullInteger
                Try
                    'Try and cast the value to an Integer
                    intValue = CType(OldValue, Integer)
                Catch ex As Exception
                End Try
                Return intValue
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ParentKey is the parent key of the List to display
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Property ParentKey() As String
            Get
                Return _ParentKey
            End Get
            Set(ByVal Value As String)
                _ParentKey = Value
            End Set
        End Property

        Protected ReadOnly Property PortalId() As Integer
            Get
                Return PortalSettings.Current.PortalId
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TextField is the field to display in the combo
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Property TextField() As ListBoundField
            Get
                Return _TextField
            End Get
            Set(ByVal Value As ListBoundField)
                _TextField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ValueField is the field to use as the combo item values
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Property ValueField() As ListBoundField
            Get
                Return _ValueField
            End Get
            Set(ByVal Value As ListBoundField)
                _ValueField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OldStringValue returns the Boolean representation of the OldValue
        ''' </summary>
        ''' <value>A String representing the OldValue</value>
        ''' <history>
        '''     [cnurse]	06/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property OldStringValue() As String
            Get
                Return CType(OldValue, String)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StringValue is the value of the control expressed as a String
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Property StringValue() As String
            Get
                Return CType(Value, String)
            End Get
            Set(ByVal Value As String)
                If ValueField = ListBoundField.Id Then
                    'Integer type field
                    Me.Value = Int32.Parse(Value)
                Else
                    'String type Field
                    Me.Value = Value
                End If
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Function GetEventArgs() As PropertyEditorEventArgs
            Dim args As New PropertyEditorEventArgs(Name)
            If ValueField = ListBoundField.Id Then
                'This is an Integer Value
                args.Value = IntegerValue
                args.OldValue = OldIntegerValue
            Else
                'This is a String Value
                args.Value = StringValue
                args.OldValue = OldStringValue
            End If
            args.StringValue = StringValue
            Return args
        End Function

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnAttributesChanged runs when the CustomAttributes property has changed.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	06/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnAttributesChanged()
            'Get the List settings out of the "Attributes"
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As System.Attribute In CustomAttributes
                    If TypeOf attribute Is ListAttribute Then
                        Dim listAtt As ListAttribute = CType(attribute, ListAttribute)
                        ListName = listAtt.ListName
                        ParentKey = listAtt.ParentKey
                        TextField = listAtt.TextField
                        ValueField = listAtt.ValueField
                        Exit For
                    End If
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            MyBase.OnValueChanged(GetEventArgs())
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnItemChanged runs when the Item has changed
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnItemChanged(ByVal e As PropertyEditorEventArgs)
            RaiseEvent ItemChanged(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim objListController As New ListController
            Dim entry As ListEntryInfo = Nothing
            Dim entryText As String = Null.NullString

            Select Case ValueField
                Case ListBoundField.Id
                    entry = objListController.GetListEntryInfo(CType(Value, Integer))
                Case ListBoundField.Text
                    entryText = StringValue
                Case ListBoundField.Value
                    entry = objListController.GetListEntryInfo(ListName, StringValue)
            End Select

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)

            If Not entry Is Nothing Then
                Select Case TextField
                    Case ListBoundField.Id
                        writer.Write(entry.EntryID.ToString)
                    Case ListBoundField.Text
                        writer.Write(entry.Text)
                    Case ListBoundField.Value
                        writer.Write(entry.Value)
                End Select
            Else
                writer.Write(entryText)
            End If

            'Close Select Tag
            writer.RenderEndTag()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            'Render the Select Tag
            ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            If AutoPostBack Then
                writer.AddAttribute(HtmlTextWriterAttribute.Onchange, Page.ClientScript.GetPostBackEventReference(Me, Me.ID.ToString()))
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Select)

            'Add the Not Specified Option
            If ValueField = ListBoundField.Text Then
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString)
            Else
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString)
            End If
            If StringValue = Null.NullString Then
                'Add the Selected Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Option)
            writer.Write("<" & Localization.GetString("Not_Specified", Localization.SharedResourceFile) & ">")
            writer.RenderEndTag()

            For I As Integer = 0 To List.Count - 1
                Dim item As ListEntryInfo = List.Item(I)
                Dim itemValue As String = Null.NullString

                'Add the Value Attribute
                Select Case ValueField
                    Case ListBoundField.Id
                        itemValue = item.EntryID.ToString
                    Case ListBoundField.Text
                        itemValue = item.Text
                    Case ListBoundField.Value
                        itemValue = item.Value
                End Select
                writer.AddAttribute(HtmlTextWriterAttribute.Value, itemValue)
                If StringValue = itemValue Then
                    'Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
                End If
                'Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option)
                Select Case TextField
                    Case ListBoundField.Id
                        writer.Write(item.EntryID.ToString)
                    Case ListBoundField.Text
                        writer.Write(item.Text)
                    Case ListBoundField.Value
                        writer.Write(item.Value)
                End Select
                writer.RenderEndTag()
            Next

            'Close Select Tag
            writer.RenderEndTag()
        End Sub

#End Region

#Region "IPostBackEventHandler Members"

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            If AutoPostBack Then
                OnItemChanged(GetEventArgs())
            End If
        End Sub

#End Region

    End Class

End Namespace

