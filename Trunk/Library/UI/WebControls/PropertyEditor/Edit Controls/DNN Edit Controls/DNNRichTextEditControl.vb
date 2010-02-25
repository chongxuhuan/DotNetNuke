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

Imports System.Collections.Specialized

Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DNNRichTextEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNRichTextEditControl control provides a standard UI component for editing
    ''' RichText
    ''' </summary>
    ''' <history>
    '''     [cnurse]	03/31/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:DNNRichTextEditControl runat=server></{0}:DNNRichTextEditControl>")> _
    Public Class DNNRichTextEditControl
        Inherits TextEditControl

        Private RichTextEditor As HTMLEditorProvider.HtmlEditorProvider

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a DNNRichTextEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateChildControls creates the controls collection
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CreateChildControls()
            If Me.EditMode = PropertyEditorMode.Edit Then
                RichTextEditor = HTMLEditorProvider.HtmlEditorProvider.Instance
                RichTextEditor.ControlID = Me.ID + "edit"
                RichTextEditor.Initialize()
                RichTextEditor.Height = Me.ControlStyle.Height
                RichTextEditor.Width = Me.ControlStyle.Width

                If RichTextEditor.Height.IsEmpty Then
                    RichTextEditor.Height = New Unit(300)
                End If
                If RichTextEditor.Width.IsEmpty Then
                    RichTextEditor.Width = New Unit(450)
                End If

                Controls.Clear()
                Controls.Add(RichTextEditor.HtmlEditorControl)
            End If

            MyBase.CreateChildControls()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadPostData loads the Post Back Data and determines whether the value has change
        ''' </summary>
        ''' <param name="postDataKey">A key to the PostBack Data to load</param>
        ''' <param name="postCollection">A name value collection of postback data</param>
        ''' <history>
        '''     [cnurse]	03/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean
            Dim dataChanged As Boolean = False
            Dim presentValue As String = StringValue
            Dim postedValue As String = RichTextEditor.Text
            If Not presentValue.Equals(postedValue) Then
                Value = postedValue
                dataChanged = True
            End If

            Return dataChanged
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim strValue As String = CType(Value, String)
            Dim strOldValue As String = CType(OldValue, String)

            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = Me.Page.Server.HtmlEncode(strValue)
            args.OldValue = Me.Page.Server.HtmlEncode(strOldValue)
            args.StringValue = Me.Page.Server.HtmlEncode(StringValue)

            MyBase.OnValueChanged(args)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnInit runs when teh control is intialized
        ''' </summary>
        ''' <history>
        '''     [cnurse]	09/16/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Me.EnsureChildControls()
            MyBase.OnInit(e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnPreRender runs just before the control is rendered.  It forces a postback to the
        ''' Control.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            If Me.EditMode = PropertyEditorMode.Edit Then
                RichTextEditor.Text = Me.Page.Server.HtmlDecode(CType(Me.Value, String))
            End If

            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Render is called by the .NET framework to render the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	03/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            Me.RenderChildren(writer)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	05/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim propValue As String = Me.Page.Server.HtmlDecode(CType(Me.Value, String))
            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(propValue)
            writer.RenderEndTag()
        End Sub

#End Region

    End Class

End Namespace

