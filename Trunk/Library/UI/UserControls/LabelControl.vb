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
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.UI.UserControls

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' LabelControl is a user control that provides all the server code to manage a
    ''' label, including localization, 508 support and help.
    ''' </summary>
    ''' <remarks>
    ''' To implement help, the control uses the ClientAPI interface.  In particular
    '''  the javascript function __dnn_Help_OnClick()
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/8/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class LabelControl
        Inherits System.Web.UI.UserControl

#Region "Controls"

        Protected WithEvents label As System.Web.UI.HtmlControls.HtmlGenericControl
        Protected WithEvents cmdHelp As System.Web.UI.WebControls.LinkButton
        Protected WithEvents imgHelp As System.Web.UI.WebControls.Image
        Protected WithEvents lblLabel As System.Web.UI.WebControls.Label
        Protected WithEvents lblHelp As System.Web.UI.WebControls.Label
        Protected WithEvents pnlHelp As System.Web.UI.WebControls.Panel

#End Region

#Region "Private Members"

        Private _ControlName As String  'Associated Edit Control for this Label
        Private _HelpKey As String   'Resource Key for the Help Text
        Private _ResourceKey As String  'Resource Key for the Label Text
        Private _Suffix As String    'Optional Text that appears after the Localized Label Text
        Private _cssClass As String  'CssClass applied to label control

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ControlName is the Id of the control that is associated with the label
        ''' </summary>
        ''' <value>A string representing the id of the associated control</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ControlName() As String
            Get
                Return _ControlName
            End Get
            Set(ByVal Value As String)
                _ControlName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Css style applied to the asp:label control
        ''' </summary>
        ''' <value>A string representing css class name</value>
        ''' <remarks>
        ''' </remarks>
        ''' -----------------------------------------------------------------------------
        Public Property CssClass() As String
            Get
                Return _cssClass
            End Get
            Set(ByVal value As String)
                _cssClass = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HelpKey is the Resource Key for the Help Text
        ''' </summary>
        ''' <value>A string representing the Resource Key for the Help Text</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HelpKey() As String
            Get
                Return _HelpKey
            End Get
            Set(ByVal Value As String)
                _HelpKey = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HelpText is value of the Help Text if no ResourceKey is provided
        ''' </summary>
        ''' <value>A string representing the Text</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HelpText() As String
            Get
                Return lblHelp.Text
            End Get
            Set(ByVal Value As String)
				lblHelp.Text = Value
				'hide the help icon if the help text is ""
				If String.IsNullOrEmpty(Value) Then
					imgHelp.Visible = False
				Else
					imgHelp.AlternateText = HtmlUtils.Clean(Value, False)
                    imgHelp.ToolTip = HtmlUtils.Clean(Value, False)
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ResourceKey is the Resource Key for the Label Text
        ''' </summary>
        ''' <value>A string representing the Resource Key for the Label Text</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ResourceKey() As String
            Get
                Return _ResourceKey
            End Get
            Set(ByVal Value As String)
                _ResourceKey = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Suffix is Optional Text that appears after the Localized Label Text
        ''' </summary>
        ''' <value>A string representing the Optional Text</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Suffix() As String
            Get
                Return _Suffix
            End Get
            Set(ByVal Value As String)
                _Suffix = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Text is value of the Label Text if no ResourceKey is provided
        ''' </summary>
        ''' <value>A string representing the Text</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Text() As String
            Get
                Return lblLabel.Text
            End Get
            Set(ByVal Value As String)
                lblLabel.Text = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Width is value of the Label Width
        ''' </summary>
        ''' <value>A string representing the Text</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Width() As Unit
            Get
                Return lblLabel.Width
            End Get
            Set(ByVal Value As Unit)
                lblLabel.Width = Value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, True, Utilities.DNNClientAPI.MinMaxPersistanceType.None)

                'get the localised text
                If _ResourceKey = "" Then
                    'Set Resource Key to the ID of the control
                    _ResourceKey = Me.ID
                End If
                If (Not String.IsNullOrEmpty(ResourceKey)) Then
                    Dim localText As String = Localization.GetString(_ResourceKey, Me)
                    If Not String.IsNullOrEmpty(localText) Then
                        Me.Text = localText & _Suffix
                    Else
                        Me.Text += _Suffix
                    End If
                End If

                If _HelpKey = "" Then
                    'Set Help Key to the Resource Key plus ".Help"
                    _HelpKey = _ResourceKey & ".Help"
                End If
                Dim helpText As String = Localization.GetString(_HelpKey, Me)
                If (Not String.IsNullOrEmpty(helpText)) OrElse (String.IsNullOrEmpty(Me.HelpText)) Then
                    Me.HelpText = helpText
                End If

                If Not String.IsNullOrEmpty(CssClass) Then
                    lblLabel.CssClass = CssClass
                End If

                'find the reference control in the parents Controls collection
                If ControlName <> "" Then
                    Dim c As Control = Me.Parent.FindControl(ControlName)
                    If Not c Is Nothing Then
                        label.Attributes("for") = c.ClientID
                    End If
                End If

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub imageClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdHelp.Click

            pnlHelp.Visible = True

        End Sub

#End Region

    End Class

End Namespace
