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

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      EditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditControl control provides a standard UI component for editing 
    ''' properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/14/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ValidationPropertyAttribute("Value")> Public MustInherit Class EditControl
        Inherits WebControl
        Implements IPostBackDataHandler

#Region "Private Members"

        Private _CustomAttributes As Object()
        Private _EditMode As PropertyEditorMode
        Private _LocalResourceFile As String
        Private _Name As String
        Private _OldValue As Object
        Private _Required As Boolean
        Private _SystemType As String
        Private _Value As Object

#End Region

#Region "Events"

        Public Event ItemAdded As PropertyChangedEventHandler
        Public Event ItemDeleted As PropertyChangedEventHandler
        Public Event ValueChanged As PropertyChangedEventHandler

#End Region

#Region "Contructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs an EditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

#Region "Data Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Custom Attributes for this Control
        ''' </summary>
        ''' <value>An array of Attributes</value>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CustomAttributes() As Object()
            Get
                Return _CustomAttributes
            End Get
            Set(ByVal Value As Object())
                _CustomAttributes = Value
                If (Not _CustomAttributes Is Nothing) AndAlso _CustomAttributes.Length > 0 Then
                    OnAttributesChanged()
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Edit Mode of the Editor
        ''' </summary>
        ''' <value>A boolean</value>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditMode() As PropertyEditorMode
            Get
                Return _EditMode
            End Get
            Set(ByVal Value As PropertyEditorMode)
                _EditMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the
        ''' </summary>
        ''' <value>A boolean</value>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable ReadOnly Property IsValid() As Boolean
            Get
                Return True
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Local Resource File for the Control
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        '''     [cnurse]	05/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LocalResourceFile() As String
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal Value As String)
                _LocalResourceFile = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Name is the name of the field as a string
        ''' </summary>
        ''' <value>A string representing the Name of the property</value>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
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
        ''' OldValue is the initial value of the field
        ''' </summary>
        ''' <value>The initial Value of the property</value>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property OldValue() As Object
            Get
                Return _OldValue
            End Get
            Set(ByVal Value As Object)
                _OldValue = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' gets and sets whether the Property is required
        ''' </summary>
        ''' <value>The initial Value of the property</value>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Required() As Boolean
            Get
                Return _Required
            End Get
            Set(ByVal Value As Boolean)
                _Required = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SystemType is the System Data Type for the property
        ''' </summary>
        ''' <value>A string representing the Type of the property</value>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SystemType() As String
            Get
                Return _SystemType
            End Get
            Set(ByVal Value As String)
                _SystemType = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Value is the value of the control
        ''' </summary>
        ''' <value>The Value of the property</value>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
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

#Region "Abstract Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected MustOverride Sub OnDataChanged(ByVal e As System.EventArgs)

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StringValue is the value of the control expressed as a String
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected MustOverride Property StringValue() As String

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
        Protected Overridable Sub OnAttributesChanged()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an item is added to a collection type property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnItemAdded(ByVal e As PropertyEditorEventArgs)
            RaiseEvent ItemAdded(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an item is deleted from a collection type property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnItemDeleted(ByVal e As PropertyEditorEventArgs)
            RaiseEvent ItemDeleted(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnValueChanged runs when the Value has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnValueChanged(ByVal e As PropertyEditorEventArgs)
            RaiseEvent ValueChanged(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim propValue As String = Me.Page.Server.HtmlDecode(CType(Me.Value, String))

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            Dim security As New PortalSecurity
            writer.Write(security.InputFilter(propValue, PortalSecurity.FilterFlag.NoScripting))
            writer.RenderEndTag()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim propValue As String = CType(Me.Value, String)

            ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Value, propValue)
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Render is called by the .NET framework to render the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim strOldValue As String = TryCast(OldValue, String)
            If EditMode = PropertyEditorMode.Edit Or (Required And String.IsNullOrEmpty(strOldValue)) Then
                RenderEditMode(writer)
            Else
                RenderViewMode(writer)
            End If

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
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData

            Dim dataChanged As Boolean = False
            Dim presentValue As String = StringValue
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
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
            'Raise the DataChanged Event
            OnDataChanged(System.EventArgs.Empty)
        End Sub

#End Region

    End Class

End Namespace

