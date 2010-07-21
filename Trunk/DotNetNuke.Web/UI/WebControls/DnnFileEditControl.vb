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

Imports DotNetNuke.UI.WebControls
Imports System.Collections.Specialized

Namespace DotNetNuke.Web.UI.WebControls

    Public Class DnnFileEditControl
        Inherits IntegerEditControl

#Region "Private Fields"

        Private fileControl As DnnFilePicker
        Private _FileFilter As String
        Private _FilePath As String
        Private _IncludePersonalFolder As Boolean = True

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the current file extension filter.
        ''' </summary>
        ''' <history>
        ''' 	[anurse]	08/11/2006 documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FileFilter() As String
            Get
                Return _FileFilter
            End Get
            Set(ByVal value As String)
                _FileFilter = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the current file path.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/02/2007 created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FilePath() As String
            Get
                Return _FilePath
            End Get
            Set(ByVal value As String)
                _FilePath = value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the control contained within this control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006 created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overloads Overrides Sub CreateChildControls()
            'First clear the controls collection
            Controls.Clear()

            'Create Table
            fileControl = New DnnFilePicker()
            fileControl.ID = String.Format("{0}FileControl", Me.ID)
            fileControl.FileFilter = FileFilter
            fileControl.FilePath = FilePath
            fileControl.Permissions = "ADD"
            fileControl.UsePersonalFolder = True
            fileControl.ShowFolders = False

            'Add table to Control
            Me.Controls.Add(fileControl)

            MyBase.CreateChildControls()
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Me.EnsureChildControls()
            MyBase.OnInit(e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs before the control is rendered.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006 created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overloads Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            fileControl.FileID = IntegerValue

            If Page IsNot Nothing Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Renders the control in edit mode
        ''' </summary>
        ''' <param name="writer">An HtmlTextWriter to render the control to</param>
        ''' <history>
        ''' 	[cnurse]	04/20/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overloads Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            Me.RenderChildren(writer)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads the Post Back Data and determines whether the value has change
        ''' </summary>
        ''' <remarks>
        ''' In this case because the <see cref="FileControl"/> is a contained control, we do not need 
        ''' to process the PostBackData (it has been handled by the File Control).  We just use
        ''' this method as the Framework calls it for us.
        ''' </remarks>
        ''' <param name="postDataKey">A key to the PostBack Data to load</param>
        ''' <param name="postCollection">A name value collection of postback data</param>
        ''' <history>
        '''     [cnurse]	08/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As NameValueCollection) As Boolean
            Dim dataChanged As Boolean = False
            Dim presentValue As String = StringValue
            Dim postedValue As String = postCollection(String.Format("{0}FileControl$File", postDataKey))
            If Not presentValue.Equals(postedValue) Then
                Value = postedValue
                dataChanged = True
            End If
            Return dataChanged
        End Function

#End Region

    End Class

End Namespace

