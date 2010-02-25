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
Imports System.Reflection
Imports System.Collections.Specialized
Imports System.Collections.Generic

Namespace DotNetNuke.UI.WebControls

    Public Class DualListBox
        Inherits WebControl
        Implements IPostBackEventHandler
        Implements IPostBackDataHandler

#Region "Private Members"

        Private _AvailableListBoxStyle As New Style
        Private _ContainerStyle As New TableStyle
        Private _ButtonStyle As New Style
        Private _HeaderStyle As New Style
        Private _SelectedListBoxStyle As New Style

        Private _AvailableDataSource As Object
        Private _SelectedDataSource As Object

        Private _DataTextField As String
        Private _DataValueField As String
        Private _LocalResourceFile As String

        Private _AddAllImageURL As String
        Private _AddAllKey As String
        Private _AddAllText As String
        Private _AddImageURL As String
        Private _AddKey As String
        Private _AddText As String
        Private _RemoveAllImageURL As String
        Private _RemoveAllKey As String
        Private _RemoveAllText As String
        Private _RemoveImageURL As String
        Private _RemoveKey As String
        Private _RemoveText As String

        Private _AddValues As List(Of String)
        Private _RemoveValues As List(Of String)

#End Region

#Region "Events"

        Public Event AddButtonClick As DualListBoxEventHandler
        Public Event AddAllButtonClick As EventHandler
        Public Event RemoveButtonClick As DualListBoxEventHandler
        Public Event RemoveAllButtonClick As EventHandler

#End Region

#Region "Public Properties"

        Public Property AddAllImageURL() As String
            Get
                Return _AddAllImageURL
            End Get
            Set(ByVal value As String)
                _AddAllImageURL = value
            End Set
        End Property

        Public Property AddAllKey() As String
            Get
                Return _AddAllKey
            End Get
            Set(ByVal value As String)
                _AddAllKey = value
            End Set
        End Property

        Public Property AddAllText() As String
            Get
                Return _AddAllText
            End Get
            Set(ByVal value As String)
                _AddAllText = value
            End Set
        End Property

        Public Property AddImageURL() As String
            Get
                Return _AddImageURL
            End Get
            Set(ByVal value As String)
                _AddImageURL = value
            End Set
        End Property

        Public Property AddKey() As String
            Get
                Return _AddKey
            End Get
            Set(ByVal value As String)
                _AddKey = value
            End Set
        End Property

        Public Property AddText() As String
            Get
                Return _AddText
            End Get
            Set(ByVal value As String)
                _AddText = value
            End Set
        End Property

        Public Property AvailableDataSource() As Object
            Get
                Return _AvailableDataSource
            End Get
            Set(ByVal Value As Object)
                _AvailableDataSource = Value
            End Set
        End Property

        Public WriteOnly Property DataTextField() As String
            Set(ByVal Value As String)
                _DataTextField = Value
            End Set
        End Property

        Public WriteOnly Property DataValueField() As String
            Set(ByVal Value As String)
                _DataValueField = Value
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal value As String)
                _LocalResourceFile = value
            End Set
        End Property

        Public Property RemoveAllImageURL() As String
            Get
                Return _RemoveAllImageURL
            End Get
            Set(ByVal value As String)
                _RemoveAllImageURL = value
            End Set
        End Property

        Public Property RemoveAllKey() As String
            Get
                Return _RemoveAllKey
            End Get
            Set(ByVal value As String)
                _RemoveAllKey = value
            End Set
        End Property

        Public Property RemoveAllText() As String
            Get
                Return _RemoveAllText
            End Get
            Set(ByVal value As String)
                _RemoveAllText = value
            End Set
        End Property

        Public Property RemoveImageURL() As String
            Get
                Return _RemoveImageURL
            End Get
            Set(ByVal value As String)
                _RemoveImageURL = value
            End Set
        End Property

        Public Property RemoveKey() As String
            Get
                Return _RemoveKey
            End Get
            Set(ByVal value As String)
                _RemoveKey = value
            End Set
        End Property

        Public Property RemoveText() As String
            Get
                Return _RemoveText
            End Get
            Set(ByVal value As String)
                _RemoveText = value
            End Set
        End Property

        Public Property SelectedDataSource() As Object
            Get
                Return _SelectedDataSource
            End Get
            Set(ByVal Value As Object)
                _SelectedDataSource = Value
            End Set
        End Property


#Region "Style Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Available List Box Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	02/15/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Available List Box.")> _
        Public ReadOnly Property AvailableListBoxStyle() As Style
            Get
                Return _AvailableListBoxStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Button Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	02/15/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Button.")> _
        Public ReadOnly Property ButtonStyle() As Style
            Get
                Return _ButtonStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Container Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	02/15/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Container.")> _
        Public ReadOnly Property ContainerStyle() As TableStyle
            Get
                Return _ContainerStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Header Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	02/15/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Header.")> _
        Public ReadOnly Property HeaderStyle() As Style
            Get
                Return _HeaderStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Selected List Box Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	02/15/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Selected List Box.")> _
        Public ReadOnly Property SelectedListBoxStyle() As Style
            Get
                Return _SelectedListBoxStyle
            End Get
        End Property

#End Region

#End Region

#Region "Private Methods"

        Private Function GetList(ByVal listType As String, ByVal dataSource As Object) As NameValueCollection
            Dim dataList As IEnumerable = TryCast(dataSource, IEnumerable)
            Dim list As New NameValueCollection

            If dataList Is Nothing Then
                Throw New ArgumentException("The " + listType + "DataSource must implement the IEnumerable Interface")
            Else
                For Each item As Object In dataList
                    Dim bindings As BindingFlags = BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.Static
                    Dim objTextProperty As PropertyInfo = item.GetType().GetProperty(_DataTextField, bindings)
                    Dim objValueProperty As PropertyInfo = item.GetType().GetProperty(_DataValueField, bindings)
                    Dim objValue As String = Convert.ToString(objValueProperty.GetValue(item, Nothing))
                    Dim objText As String = Convert.ToString(objTextProperty.GetValue(item, Nothing))

                    list.Add(objText, objValue)
                Next
            End If

            Return list
        End Function

        Private Sub RenderButton(ByVal buttonType As String, ByVal writer As HtmlTextWriter)
            Dim buttonText As String = Null.NullString
            Dim imageURL As String = Null.NullString

            'Begin Button Row
            writer.RenderBeginTag(HtmlTextWriterTag.Tr)

            'Begin Button Cell
            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            Select Case buttonType
                Case "Add"
                    If String.IsNullOrEmpty(AddKey) Then
                        buttonText = AddText
                    Else
                        buttonText = Localization.GetString(AddKey, LocalResourceFile)
                    End If
                    imageURL = AddImageURL
                Case "AddAll"
                    If String.IsNullOrEmpty(AddAllKey) Then
                        buttonText = AddAllText
                    Else
                        buttonText = Localization.GetString(AddAllKey, LocalResourceFile)
                    End If
                    imageURL = AddAllImageURL
                Case "Remove"
                    If String.IsNullOrEmpty(RemoveKey) Then
                        buttonText = RemoveText
                    Else
                        buttonText = Localization.GetString(RemoveKey, LocalResourceFile)
                    End If
                    imageURL = RemoveImageURL
                Case "RemoveAll"
                    If String.IsNullOrEmpty(RemoveAllKey) Then
                        buttonText = RemoveAllText
                    Else
                        buttonText = Localization.GetString(RemoveAllKey, LocalResourceFile)
                    End If
                    imageURL = RemoveAllImageURL
            End Select

            'Render Hyperlink
            writer.AddAttribute(HtmlTextWriterAttribute.Href, Page.ClientScript.GetPostBackClientHyperlink(Me, buttonType))
            writer.AddAttribute(HtmlTextWriterAttribute.Title, buttonText)
            writer.RenderBeginTag(HtmlTextWriterTag.A)

            'Render Image
            If Not String.IsNullOrEmpty(imageURL) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveClientUrl(imageURL))
                writer.AddAttribute(HtmlTextWriterAttribute.Title, buttonText)
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "0")
                writer.RenderBeginTag(HtmlTextWriterTag.Img)
                writer.RenderEndTag()
            Else
                writer.Write(buttonText)
            End If

            'End of Hyperlink
            writer.RenderEndTag()

            'End of Button Cell
            writer.RenderEndTag()

            'Render end of Button Row
            writer.RenderEndTag()
        End Sub

        Private Sub RenderButtons(ByVal writer As HtmlTextWriter)
            'render table
            writer.RenderBeginTag(HtmlTextWriterTag.Table)

            RenderButton("Add", writer)
            RenderButton("AddAll", writer)

            'Begin Button Row
            writer.RenderBeginTag(HtmlTextWriterTag.Tr)

            'Begin Button Cell
            writer.RenderBeginTag(HtmlTextWriterTag.Td)

            writer.Write("&nbsp;")

            'End of Button Cell
            writer.RenderEndTag()

            'Render end of Button Row
            writer.RenderEndTag()

            RenderButton("Remove", writer)
            RenderButton("RemoveAll", writer)

            'Render end of table
            writer.RenderEndTag()
        End Sub

        Private Sub RenderListBox(ByVal listType As String, ByVal dataSource As Object, ByVal style As Style, ByVal writer As HtmlTextWriter)
            If dataSource IsNot Nothing Then
                Dim list As NameValueCollection = GetList(listType, dataSource)

                If list IsNot Nothing Then
                    If style IsNot Nothing Then
                        style.AddAttributesToRender(writer)
                    End If

                    'Render ListBox
                    writer.AddAttribute(HtmlTextWriterAttribute.Multiple, "multiple")
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID + "_" + listType)
                    writer.RenderBeginTag(HtmlTextWriterTag.Select)

                    For i As Integer = 0 To list.Count - 1
                        'Render option tags for each item
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, list.Get(i))
                        writer.RenderBeginTag(HtmlTextWriterTag.Option)
                        writer.Write(list.GetKey(i))

                        writer.RenderEndTag()
                    Next i

                    'Render ListBox end
                    writer.RenderEndTag()
                End If
            End If
        End Sub

        Private Sub RenderHeader(ByVal writer As HtmlTextWriter)
            'render Header row
            writer.RenderBeginTag(HtmlTextWriterTag.Tr)

            If HeaderStyle IsNot Nothing Then
                HeaderStyle.AddAttributesToRender(writer)
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            writer.Write(Localization.GetString(Me.ID + "_Available", LocalResourceFile))
            writer.RenderEndTag()

            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            writer.RenderEndTag()

            If HeaderStyle IsNot Nothing Then
                HeaderStyle.AddAttributesToRender(writer)
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            writer.Write(Localization.GetString(Me.ID + "_Selected", LocalResourceFile))
            writer.RenderEndTag()

            'Render end of Header Row
            writer.RenderEndTag()
        End Sub

        Private Sub RenderListBoxes(ByVal writer As HtmlTextWriter)
            'render List Boxes row
            writer.RenderBeginTag(HtmlTextWriterTag.Tr)

            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            RenderListBox("Available", AvailableDataSource, AvailableListBoxStyle, writer)
            writer.RenderEndTag()

            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            RenderButtons(writer)
            writer.RenderEndTag()

            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            RenderListBox("Selected", SelectedDataSource, SelectedListBoxStyle, writer)
            writer.RenderEndTag()

            'Render end of List Boxes Row
            writer.RenderEndTag()
        End Sub

#End Region

#Region "Protected Methods"

        Protected Sub OnAddButtonClick(ByVal e As DualListBoxEventArgs)
            RaiseEvent AddButtonClick(Me, e)
        End Sub

        Protected Sub OnAddAllButtonClick(ByVal e As EventArgs)
            RaiseEvent AddAllButtonClick(Me, e)
        End Sub

        Protected Sub OnRemoveButtonClick(ByVal e As DualListBoxEventArgs)
            RaiseEvent RemoveButtonClick(Me, e)
        End Sub

        Protected Sub OnRemoveAllButtonClick(ByVal e As EventArgs)
            RaiseEvent RemoveAllButtonClick(Me, e)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            If Page IsNot Nothing Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub

        Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)

            'render table
            If ContainerStyle IsNot Nothing Then
                ContainerStyle.AddAttributesToRender(writer)
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Table)

            'Render Header Row
            RenderHeader(writer)

            'Render ListBox row
            RenderListBoxes(writer)

            'Render end of table
            writer.RenderEndTag()
        End Sub

#End Region

#Region "IPostBackEventHandler Implementation"

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent
            Select Case eventArgument
                Case "Add"
                    OnAddButtonClick(New DualListBoxEventArgs(_AddValues))
                Case "AddAll"
                    OnAddAllButtonClick(New EventArgs())
                Case "Remove"
                    OnRemoveButtonClick(New DualListBoxEventArgs(_RemoveValues))
                Case "RemoveAll"
                    OnRemoveAllButtonClick(New EventArgs())
            End Select
        End Sub

#End Region

#Region "IPostBackDataHandler Implementation"

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            Dim addItems As String = postCollection(postDataKey + "_Available")
            If Not String.IsNullOrEmpty(addItems) Then
                _AddValues = New List(Of String)
                For Each addItem As String In addItems.Split(","c)
                    _AddValues.Add(addItem)
                Next
            End If

            Dim removeItems As String = postCollection(postDataKey + "_Selected")
            If Not String.IsNullOrEmpty(removeItems) Then
                _RemoveValues = New List(Of String)
                For Each removeItem As String In removeItems.Split(","c)
                    _RemoveValues.Add(removeItem)
                Next
            End If

        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub

#End Region

    End Class

End Namespace
