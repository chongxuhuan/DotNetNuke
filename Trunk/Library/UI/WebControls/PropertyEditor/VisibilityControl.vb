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
Imports System.ComponentModel
Imports System.Reflection
Imports System.Web.UI

Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      VisibilityControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The VisibilityControl control provides a base control for defining visibility
    ''' options
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/03/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:VisibilityControl runat=server></{0}:VisibilityControl>")> _
    Public Class VisibilityControl
        Inherits WebControl
        Implements IPostBackDataHandler
        Implements INamingContainer

        Private _Caption As String
        Private _Name As String
        Private _Value As Object

#Region "Events"

        Public Event VisibilityChanged As PropertyChangedEventHandler

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a VisibilityControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Caption
        ''' </summary>
        ''' <value>A string representing the Name of the property</value>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Caption() As String
            Get
                Return _Caption
            End Get
            Set(ByVal Value As String)
                _Caption = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Name is the name of the field as a string
        ''' </summary>
        ''' <value>A string representing the Name of the property</value>
        ''' <history>
        '''     [cnurse]	05/03/2006	created
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
        ''' StringValue is the value of the control expressed as a String
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	05/03/2006	created
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

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnVisibilityChanged runs when the Visibility has changed.  It raises the VisibilityChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/03/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnVisibilityChanged(ByVal e As PropertyEditorEventArgs)
            RaiseEvent VisibilityChanged(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Render renders the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	05/03/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim propValue As UserVisibilityMode = CType(Value, UserVisibilityMode)

            'Render Outer Div
            ControlStyle.AddAttributesToRender(writer)
            AddAttributesToRender(writer)

            'Render Caption
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(Caption)

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            If (propValue = UserVisibilityMode.AllUsers) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "0")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()
            writer.Write(Localization.GetString("Public"))

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            If (propValue = UserVisibilityMode.MembersOnly) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "1")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()
            writer.Write(Localization.GetString("MemberOnly"))

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            If (propValue = UserVisibilityMode.AdminOnly) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "2")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()
            writer.Write(Localization.GetString("AdminOnly"))

            'End render outer div
            writer.RenderEndTag()

        End Sub

#End Region

#Region "IPostBackDataHandler Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadPostData loads the Post Back Data and determines whether the value has change
        ''' </summary>
        ''' <param name="postDataKey">A key to the PostBack Data to load</param>
        ''' <param name="postCollection">A name value collection of postback data</param>
        ''' <history>
        '''     [cnurse]	05/03/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData

            Dim dataChanged As Boolean = False
            Dim presentValue As String = CStr(Value)
            Dim postedValue As String = postCollection(postDataKey)
            If Not presentValue.Equals(postedValue) Then
                Value = postedValue
                dataChanged = True
            End If

            Return dataChanged

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RaisePostDataChangedEvent runs when the PostBackData has changed.  It triggers
        ''' a ValueChanged Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/03/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

            'Raise the VisibilityChanged Event
            Dim intValue As Integer = CType(Value, Integer)
            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = System.Enum.ToObject(GetType(UserVisibilityMode), intValue)
            OnVisibilityChanged(args)

        End Sub

#End Region


    End Class

End Namespace

