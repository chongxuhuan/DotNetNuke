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

Imports System.Diagnostics

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Modules.NavigationProvider

Namespace DotNetNuke.UI.Containers
    Partial Class DropDownActions
        Inherits UI.Containers.ActionBase

        Private m_objControl As NavigationProvider
        Private m_strProviderName As String = "DNNDropDownNavigationProvider"

        Public Property ProviderName() As String
            Get
                Return m_strProviderName
            End Get
            Set(ByVal Value As String)
                'm_strProviderName = Value 'always is dropdown!
            End Set
        End Property

        Public ReadOnly Property Control() As NavigationProvider
            Get
                Return m_objControl
            End Get
        End Property

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Put user code to initialize the page here
            Me.cmdGo.Attributes.Add("onclick", "if (cmdGo_OnClick(dnn.dom.getById('" & Control.NavigationControl.ClientID & "')) == false) return false;")
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            Try
                BindDropDown()
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdGo_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdGo.Click
            Try
                Dim cboActions As DropDownList = CType(Control.NavigationControl, DropDownList)
                If cboActions.SelectedIndex <> -1 Then
                    ProcessAction(cboActions.SelectedItem.Value)
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Sub BindDropDown()
            Dim objNodes As DNNNodeCollection
            objNodes = Navigation.GetActionNodes(ActionRoot, Me)
            Dim objNode As DNNNode
            For Each objNode In objNodes
                ProcessNodes(objNode)
            Next
            Control.Bind(objNodes)

            Me.Visible = DisplayControl(objNodes)
        End Sub

        Private Sub ProcessNodes(ByVal objParent As DNNNode)
            If Len(objParent.JSFunction) > 0 Then
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me.Page, "__dnn_CSAction_" & Me.Control.NavigationControl.ClientID & "_" & objParent.ID, objParent.JSFunction, True)
            End If

            objParent.ClickAction = eClickAction.None            'since GO button is handling actions dont allow selected index change fire postback

            Dim objNode As DNNNode
            For Each objNode In objParent.DNNNodes
                ProcessNodes(objNode)
            Next
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            m_objControl = NavigationProvider.Instance(Me.ProviderName)
            Control.ControlID = "ctl" & Me.ID
            Control.Initialize()
            spActions.Controls.Add(Control.NavigationControl)
        End Sub


    End Class
End Namespace
