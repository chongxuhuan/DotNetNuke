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

Imports DotNetNuke.UI.WebControls
Imports System.Collections.Generic
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.UI.Skins.Controls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.Skins.Controls
    ''' Class:      SkinsEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinsEditControl control provides a standard UI component for editing 
    ''' skins.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/04/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:SkinsEditControl runat=server></{0}:SkinsEditControl>")> _
    Public Class SkinsEditControl
        Inherits EditControl
        Implements IPostBackEventHandler

        Private _AddedItem As String = Null.NullString

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a SkinsEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a SkinsEditControl
        ''' </summary>
        ''' <param name="type">The type of the property</param>
        ''' <history>
        '''     [cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal type As String)
            Me.SystemType = type
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DictionaryValue returns the Dictionary(Of Integer, String) representation of the Value
        ''' </summary>
        ''' <value>A Dictionary(Of Integer, String) representing the Value</value>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property DictionaryValue() As Dictionary(Of Integer, String)
            Get
                Return TryCast(Value, Dictionary(Of Integer, String))
            End Get
            Set(ByVal value As Dictionary(Of Integer, String))
                Me.Value = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OldDictionaryValue returns the Dictionary(Of Integer, String) representation of the OldValue
        ''' </summary>
        ''' <value>A Dictionary(Of Integer, String) representing the OldValue</value>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property OldDictionaryValue() As Dictionary(Of Integer, String)
            Get
                Return TryCast(OldValue, Dictionary(Of Integer, String))
            End Get
            Set(ByVal value As Dictionary(Of Integer, String))
                Me.OldValue = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OldStringValue returns the String representation of the OldValue
        ''' </summary>
        ''' <value>A String representing the OldValue</value>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property OldStringValue() As String
            Get
                Dim strValue As String = Null.NullString
                If OldDictionaryValue IsNot Nothing Then
                    For Each Skin As String In OldDictionaryValue.Values
                        strValue += Skin + ","
                    Next
                End If
                Return strValue
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StringValue is the value of the control expressed as a String
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Property StringValue() As String
            Get
                Dim strValue As String = Null.NullString
                If DictionaryValue IsNot Nothing Then
                    For Each Skin As String In DictionaryValue.Values
                        strValue += Skin + ","
                    Next
                End If
                Return strValue
            End Get
            Set(ByVal Value As String)
                Me.Value = Value
            End Set
        End Property

        Protected Property AddedItem() As String
            Get
                Return _AddedItem
            End Get
            Set(ByVal value As String)
                _AddedItem = value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = DictionaryValue
            args.OldValue = OldDictionaryValue
            args.StringValue = ""
            args.Changed = True
            MyBase.OnValueChanged(args)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnPreRender runs just before the control is due to be rendered
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            'Register control for PostBack
            Page.RegisterRequiresPostBack(Me)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim length As Integer = Null.NullInteger
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As System.Attribute In CustomAttributes
                    If TypeOf attribute Is MaxLengthAttribute Then
                        Dim lengthAtt As MaxLengthAttribute = CType(attribute, MaxLengthAttribute)
                        length = lengthAtt.Length
                        Exit For
                    End If
                Next
            End If

            If DictionaryValue IsNot Nothing Then
                For Each kvp As KeyValuePair(Of Integer, String) In DictionaryValue
                    'Render Hyperlink
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, Page.ClientScript.GetPostBackClientHyperlink(Me, "Delete_" + kvp.Key.ToString(), False))
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "javascript:return confirm('" + ClientAPI.GetSafeJSString(Localization.GetString("DeleteItem")) + "');")
                    writer.AddAttribute(HtmlTextWriterAttribute.Title, Localization.GetString("cmdDelete", Me.LocalResourceFile))
                    writer.RenderBeginTag(HtmlTextWriterTag.A)

                    'Render Image
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl("~/images/delete.gif"))
                    writer.AddAttribute(HtmlTextWriterAttribute.Border, "0")
                    writer.RenderBeginTag(HtmlTextWriterTag.Img)

                    'Render end of Image
                    writer.RenderEndTag()

                    'Render end of Hyperlink
                    writer.RenderEndTag()

                    ControlStyle.AddAttributesToRender(writer)
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, kvp.Value)
                    If length > Null.NullInteger Then
                        writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, length.ToString)
                    End If
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID + "_skin" + kvp.Key.ToString())
                    writer.RenderBeginTag(HtmlTextWriterTag.Input)
                    writer.RenderEndTag()

                    writer.WriteBreak()
                Next

                writer.WriteBreak()

                'Create Add Row
                'Render Hyperlink
                writer.AddAttribute(HtmlTextWriterAttribute.Href, Page.ClientScript.GetPostBackClientHyperlink(Me, "Add", False))
                writer.AddAttribute(HtmlTextWriterAttribute.Title, Localization.GetString("cmdAdd", Me.LocalResourceFile))
                writer.RenderBeginTag(HtmlTextWriterTag.A)

                'Render Image
                writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl("~/images/add.gif"))
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "0")
                writer.RenderBeginTag(HtmlTextWriterTag.Img)

                'Render end of Image
                writer.RenderEndTag()

                'Render end of Hyperlink
                writer.RenderEndTag()

                ControlStyle.AddAttributesToRender(writer)
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString)
                If length > Null.NullInteger Then
                    writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, length.ToString)
                End If
                writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID + "_skinnew")
                writer.RenderBeginTag(HtmlTextWriterTag.Input)
                writer.RenderEndTag()

                writer.WriteBreak()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            If DictionaryValue IsNot Nothing Then
                For Each kvp As KeyValuePair(Of Integer, String) In DictionaryValue
                    ControlStyle.AddAttributesToRender(writer)
                    writer.RenderBeginTag(HtmlTextWriterTag.Span)
                    writer.Write(kvp.Value)
                    writer.RenderEndTag()

                    writer.WriteBreak()
                Next
            End If
        End Sub

#End Region

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean
            Dim dataChanged As Boolean = False
            Dim postedValue As String
            Dim newDictionaryValue As New Dictionary(Of Integer, String)

            For Each kvp As KeyValuePair(Of Integer, String) In DictionaryValue
                postedValue = postCollection(Me.UniqueID + "_skin" + kvp.Key.ToString())

                If kvp.Value.Equals(postedValue) Then
                    newDictionaryValue.Item(kvp.Key) = kvp.Value
                Else
                    newDictionaryValue.Item(kvp.Key) = postedValue
                    dataChanged = True
                End If

            Next

            postedValue = postCollection(Me.UniqueID + "_skinnew")
            If Not String.IsNullOrEmpty(postedValue) Then
                AddedItem = postedValue
            End If

            DictionaryValue = newDictionaryValue

            Return dataChanged
        End Function

#Region "IPostBackEventHandler Implementation"

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent

            Select Case eventArgument.Substring(0, 3)
                Case "Del"
                    Dim args As New PropertyEditorEventArgs(Name)
                    args.Value = DictionaryValue
                    args.OldValue = OldDictionaryValue
                    args.Key = Integer.Parse(eventArgument.Substring(7))
                    args.Changed = True
                    MyBase.OnItemDeleted(args)
                Case "Add"
                    Dim args As New PropertyEditorEventArgs(Name)
                    args.Value = AddedItem
                    args.StringValue = AddedItem
                    args.Changed = True
                    MyBase.OnItemAdded(args)
            End Select
        End Sub

#End Region
    End Class

End Namespace

