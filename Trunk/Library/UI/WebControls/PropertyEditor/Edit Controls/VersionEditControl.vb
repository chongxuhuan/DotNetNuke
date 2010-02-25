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

Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      VersionEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The VersionEditControl control provides a standard UI component for editing 
    ''' System.Version properties.
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/21/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:VersionEditControl runat=server></{0}:VersionEditControl>")> _
    Public Class VersionEditControl
        Inherits EditControl

        Private EnumType As Type

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StringValue is the value of the control expressed as a String
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	02/21/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Property StringValue() As String
            Get
                Return Value.ToString(3)
            End Get
            Set(ByVal Value As String)
                Me.Value = New System.Version(Value)
            End Set
        End Property

        Protected ReadOnly Property Version() As System.Version
            Get
                Return TryCast(Value, System.Version)
            End Get
        End Property

#End Region

        Protected Sub RenderDropDownList(ByVal writer As System.Web.UI.HtmlTextWriter, ByVal type As String, ByVal val As Integer)

            'Render the Select Tag
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID + "_" + type)
            writer.AddStyleAttribute("width", "40px")
            writer.RenderBeginTag(HtmlTextWriterTag.Select)

            For i As Integer = 0 To 99
                'Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, i.ToString())

                If val = i Then
                    'Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
                End If

                'Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option)
                writer.Write(i.ToString("00"))
                writer.RenderEndTag()
            Next

            'Close Select Tag
            writer.RenderEndTag()
        End Sub

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/21/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)

            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = Value
            args.OldValue = OldValue
            args.StringValue = StringValue

            MyBase.OnValueChanged(args)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnPreRender runs just before the control is rendered.  It forces a postback to the
        ''' Control.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/21/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/21/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            'Render a containing span Tag
            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)

            'Render Major
            RenderDropDownList(writer, "Major", Version.Major)

            writer.Write("&nbsp;")

            'Render Minor
            RenderDropDownList(writer, "Minor", Version.Minor)

            writer.Write("&nbsp;")

            'Render Build
            RenderDropDownList(writer, "Build", Version.Build)

            'Close Select Tag
            writer.RenderEndTag()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/21/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            If Version IsNot Nothing Then
                writer.Write(Version.ToString(3))
            End If
            writer.RenderEndTag()

        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean

            Dim majorVersion As String = postCollection(postDataKey + "_Major")
            Dim minorVersion As String = postCollection(postDataKey + "_Minor")
            Dim buildVersion As String = postCollection(postDataKey + "_Build")

            Dim dataChanged As Boolean = False
            Dim presentValue As System.Version = Version
            Dim postedValue As System.Version = New System.Version(majorVersion + "." + minorVersion + "." + buildVersion)
            If Not presentValue.Equals(postedValue) Then
                Value = postedValue
                dataChanged = True
            End If

            Return dataChanged
        End Function

#End Region

    End Class

End Namespace

