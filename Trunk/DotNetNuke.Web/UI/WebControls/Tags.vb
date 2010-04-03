'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports System.Web.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Web.UI
Imports DotNetNuke.Security

Namespace DotNetNuke.Web.UI.WebControls

    Public Class Tags
        Inherits WebControl
        Implements IPostBackEventHandler
        Implements IPostBackDataHandler

        Public Event TagsUpdated As EventHandler(Of EventArgs)

#Region "Private Members"

        Private _AddImageUrl As String
        Private _AllowTagging As Boolean = False
        Private _CancelImageUrl As String
        Private _CategoryImageUrl As String
        Private _CategoryLabelCssClass As String
        Private _ContentItem As ContentItem
        Private _NavigateUrlFormatString As String
        Private _SaveImageUrl As String
        Private _Separator As String = ",&nbsp;"
        Private _ShowCategories As Boolean
        Private _ShowTags As Boolean
        Private _TagImageUrl As String
        Private _Tags As String
        Private _TagLabelCssClass As String

        Private ReadOnly Property TagVocabulary() As Vocabulary
            Get
                Dim vocabularyController As New VocabularyController()
                Return (From v In vocabularyController.GetVocabularies() _
                       Where v.IsSystem AndAlso v.Name = "Tags" _
                       Select v) _
                       .SingleOrDefault()
            End Get
        End Property

#End Region

#Region "Public Properties"

        Public Property AddImageUrl() As String
            Get
                Return _AddImageUrl
            End Get
            Set(ByVal value As String)
                _AddImageUrl = value
            End Set
        End Property

        Public Property AllowTagging() As Boolean
            Get
                Return _AllowTagging
            End Get
            Set(ByVal value As Boolean)
                _AllowTagging = value
            End Set
        End Property

        Public Property CancelImageUrl() As String
            Get
                Return _CancelImageUrl
            End Get
            Set(ByVal value As String)
                _CancelImageUrl = value
            End Set
        End Property

        Public Property CategoryImageUrl() As String
            Get
                Return _CategoryImageUrl
            End Get
            Set(ByVal value As String)
                _CategoryImageUrl = value
            End Set
        End Property

        Public Property CategoryLabelCssClass() As String
            Get
                Return _CategoryLabelCssClass
            End Get
            Set(ByVal value As String)
                _CategoryLabelCssClass = value
            End Set
        End Property

        Public Property ContentItem() As ContentItem
            Get
                Return _ContentItem
            End Get
            Set(ByVal value As ContentItem)
                _ContentItem = value
            End Set
        End Property

        Public Property IsEditMode() As Boolean
            Get
                Dim _IsEditMode As Boolean = False
                If ViewState("IsEditMode") IsNot Nothing Then
                    _IsEditMode = Convert.ToBoolean(ViewState("IsEditMode"))
                End If
                Return _IsEditMode
            End Get
            Set(ByVal value As Boolean)
                ViewState("IsEditMode") = value
            End Set
        End Property

        Public Property NavigateUrlFormatString() As String
            Get
                Return _NavigateUrlFormatString
            End Get
            Set(ByVal value As String)
                _NavigateUrlFormatString = value
            End Set
        End Property

        Public Property SaveImageUrl() As String
            Get
                Return _SaveImageUrl
            End Get
            Set(ByVal value As String)
                _SaveImageUrl = value
            End Set
        End Property

        Public Property Separator() As String
            Get
                Return _Separator
            End Get
            Set(ByVal Value As String)
                _Separator = Value
            End Set
        End Property

        Public Property ShowCategories() As Boolean
            Get
                Return _ShowCategories
            End Get
            Set(ByVal value As Boolean)
                _ShowCategories = value
            End Set
        End Property

        Public Property ShowTags() As Boolean
            Get
                Return _ShowTags
            End Get
            Set(ByVal value As Boolean)
                _ShowTags = value
            End Set
        End Property

        Public Property TagImageUrl() As String
            Get
                Return _TagImageUrl
            End Get
            Set(ByVal value As String)
                _TagImageUrl = value
            End Set
        End Property

        Public Property TagLabelCssClass() As String
            Get
                EnsureChildControls()
                Return _TagLabelCssClass
            End Get
            Set(ByVal value As String)
                EnsureChildControls()
                _TagLabelCssClass = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Function LocalizeString(ByVal key As String) As String
            Dim LocalResourceFile As String = Utilities.GetLocalResourceFile(Me)
            Dim localizedString As String
            If Not String.IsNullOrEmpty(key) AndAlso Not String.IsNullOrEmpty(LocalResourceFile) Then
                localizedString = Localization.GetString(key, LocalResourceFile)
            Else
                localizedString = Null.NullString
            End If
            Return localizedString
        End Function

        Private Sub RenderButton(ByVal writer As HtmlTextWriter, ByVal buttonType As String, ByVal imageUrl As String)
            writer.AddAttribute(HtmlTextWriterAttribute.Title, LocalizeString(String.Format("{0}.ToolTip", buttonType)))
            writer.AddAttribute(HtmlTextWriterAttribute.Href, Page.ClientScript.GetPostBackClientHyperlink(Me, buttonType))
            writer.RenderBeginTag(HtmlTextWriterTag.A)

            'Image
            If Not String.IsNullOrEmpty(imageUrl) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl(imageUrl))
                writer.RenderBeginTag(HtmlTextWriterTag.Img)
                writer.RenderEndTag()
            End If

            writer.Write(LocalizeString(buttonType))
            writer.RenderEndTag()
        End Sub

        Private Sub RenderTerm(ByVal writer As HtmlTextWriter, ByVal term As Term, ByRef count As Integer)
            If count = 0 Then
                writer.Write("&nbsp;")
            Else
                writer.Write(Separator)
            End If

            writer.AddAttribute(HtmlTextWriterAttribute.Href, String.Format(NavigateUrlFormatString, term.Name))
            writer.AddAttribute(HtmlTextWriterAttribute.Title, term.Name)
            writer.AddAttribute(HtmlTextWriterAttribute.Rel, "tag")
            writer.RenderBeginTag(HtmlTextWriterTag.A)
            writer.Write(term.Name)
            writer.RenderEndTag()

            count += 1
        End Sub

        Private Sub SaveTags()
            Dim tags As String = New PortalSecurity().InputFilter(_Tags, PortalSecurity.FilterFlag.NoMarkup Or PortalSecurity.FilterFlag.NoScripting)
            If Not String.IsNullOrEmpty(tags) Then
                For Each tag As String In tags.Split(","c)
                    Dim existingTerm As Term = (From t As Term In ContentItem.Terms.AsQueryable _
                                                Where t.Name.Equals(tag, StringComparison.CurrentCultureIgnoreCase) _
                                                Select t) _
                                                .SingleOrDefault()

                    If existingTerm Is Nothing Then
                        'Not tagged
                        Dim termController As New TermController()
                        Dim term As Term = (From t As Term In termController.GetTermsByVocabulary(TagVocabulary.VocabularyId) _
                                                Where t.Name.Equals(tag, StringComparison.CurrentCultureIgnoreCase) _
                                                Select t) _
                                                .SingleOrDefault()
                        If term Is Nothing Then
                            'Add term
                            term = New Term(TagVocabulary.VocabularyId)
                            term.Name = tag
                            termController.AddTerm(term)
                        End If

                        'Add term to content
                        ContentItem.Terms.Add(term)
                        termController.AddTermToContent(term, ContentItem)
                    End If
                Next
            End If

            IsEditMode = False

            'Raise the Tags Updated Event
            OnTagsUpdate(EventArgs.Empty)
        End Sub

#End Region

        Protected Sub OnTagsUpdate(ByVal e As EventArgs)
            RaiseEvent TagsUpdated(Me, e)
        End Sub

#Region "Public Methods"

        Public Overrides Sub RenderControl(ByVal writer As HtmlTextWriter)

            'Render Categories
            If ShowCategories Then
                If Not String.IsNullOrEmpty(CategoryLabelCssClass) Then
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, CategoryLabelCssClass)
                End If
                writer.RenderBeginTag(HtmlTextWriterTag.Span)

                'Render Category Image
                If Not String.IsNullOrEmpty(CategoryImageUrl) Then
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl(CategoryImageUrl))
                    writer.RenderBeginTag(HtmlTextWriterTag.Img)
                    writer.RenderEndTag()
                End If

                writer.Write(LocalizeString("Category"))
                writer.RenderEndTag()

                'Render Category Links
                Dim categoryCount As Integer = 0
                For Each term As Term In ContentItem.Terms
                    If term.VocabularyId <> TagVocabulary.VocabularyId Then
                        RenderTerm(writer, term, categoryCount)
                    End If
                Next
            End If

            If ShowCategories AndAlso ShowTags Then
                writer.Write("&nbsp;&nbsp;")
            End If

            If ShowTags Then
                If Not String.IsNullOrEmpty(TagLabelCssClass) Then
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, TagLabelCssClass)
                End If
                writer.RenderBeginTag(HtmlTextWriterTag.Span)

                'Render Category Image
                If Not String.IsNullOrEmpty(TagImageUrl) Then
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl(TagImageUrl))
                    writer.RenderBeginTag(HtmlTextWriterTag.Img)
                    writer.RenderEndTag()
                End If

                writer.Write(LocalizeString("Tag"))
                writer.RenderEndTag()

                If Not IsEditMode Then
                    'Render Tag Links
                    Dim tagCount As Integer = 0
                    For Each term As Term In ContentItem.Terms
                        If term.VocabularyId = TagVocabulary.VocabularyId Then
                            RenderTerm(writer, term, tagCount)
                        End If
                    Next
                End If

                If AllowTagging Then
                    If IsEditMode Then
                        writer.Write("&nbsp;&nbsp;")

                        writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
                        writer.RenderBeginTag(HtmlTextWriterTag.Input)
                        writer.RenderEndTag()

                        writer.Write("&nbsp;&nbsp;")

                        'Render Save Button
                        RenderButton(writer, "Save", SaveImageUrl)

                        writer.Write("&nbsp;&nbsp;")

                        'Render Add Button
                        RenderButton(writer, "Cancel", CancelImageUrl)
                    Else
                        writer.Write("&nbsp;&nbsp;")

                        'Render Add Button
                        RenderButton(writer, "Add", AddImageUrl)
                    End If
                End If
            End If

        End Sub

#End Region

#Region "IPostBackDataHandler Implementation"

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            _Tags = postCollection(postDataKey)

            Return True
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub

#End Region

#Region "IPostBackEventHandler Implementation"

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent
            Select Case eventArgument
                Case "Add"
                    IsEditMode = True
                Case "Cancel"
                    IsEditMode = False
                Case "Save"
                    SaveTags()
                Case Else
                    IsEditMode = False
            End Select
        End Sub

#End Region

    End Class

End Namespace

