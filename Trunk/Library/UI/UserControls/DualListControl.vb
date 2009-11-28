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
Imports System.IO

Namespace DotNetNuke.UI.UserControls

    Public MustInherit Class DualListControl

        Inherits Framework.UserControlBase

        Private _ListBoxWidth As String = ""
        Private _ListBoxHeight As String = ""
        Private _Available As ArrayList
        Private _Assigned As ArrayList
        Private _DataTextField As String = ""
        Private _DataValueField As String = ""
        Private _Enabled As Boolean = True

        Protected WithEvents lstAvailable As System.Web.UI.WebControls.ListBox
        Protected WithEvents cmdAdd As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdRemove As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdAddAll As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdRemoveAll As System.Web.UI.WebControls.LinkButton
        Protected Label1 As System.Web.UI.WebControls.Label
        Protected Label2 As System.Web.UI.WebControls.Label
        Protected WithEvents lstAssigned As System.Web.UI.WebControls.ListBox

        Private MyFileName As String = "DualListControl.ascx"


        ' public properties
        Public Property ListBoxWidth() As String
            Get
                ListBoxWidth = Convert.ToString(ViewState(Me.ClientID & "_ListBoxWidth"))
            End Get
            Set(ByVal Value As String)
                _ListBoxWidth = Value
            End Set
        End Property

        Public Property ListBoxHeight() As String
            Get
                ListBoxHeight = Convert.ToString(ViewState(Me.ClientID & "_ListBoxHeight"))
            End Get
            Set(ByVal Value As String)
                _ListBoxHeight = Value
            End Set
        End Property

        Public Property Available() As ArrayList
            Get
                Dim objListItem As ListItem

                Dim objList As ArrayList = New ArrayList

                For Each objListItem In lstAvailable.Items
                    objList.Add(objListItem)
                Next

                Available = objList
            End Get
            Set(ByVal Value As ArrayList)
                _Available = Value
            End Set
        End Property

        Public Property Assigned() As ArrayList
            Get
                Dim objListItem As ListItem

                Dim objList As ArrayList = New ArrayList

                For Each objListItem In lstAssigned.Items
                    objList.Add(objListItem)
                Next

                Assigned = objList
            End Get
            Set(ByVal Value As ArrayList)
                _Assigned = Value
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

        Public WriteOnly Property Enabled() As Boolean
            Set(ByVal Value As Boolean)
                _Enabled = Value
            End Set
        End Property

        '*******************************************************
        '
        ' The Page_Load server event handler on this page is used
        ' to populate the role information for the page
        '
        '*******************************************************

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                'Localization
                Label1.Text = Services.Localization.Localization.GetString("Available", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                Label2.Text = Services.Localization.Localization.GetString("Assigned", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                cmdAdd.ToolTip = Services.Localization.Localization.GetString("Add", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                cmdAddAll.ToolTip = Services.Localization.Localization.GetString("AddAll", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                cmdRemove.ToolTip = Services.Localization.Localization.GetString("Remove", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                cmdRemoveAll.ToolTip = Services.Localization.Localization.GetString("RemoveAll", Services.Localization.Localization.GetResourceFile(Me, MyFileName))

                If Not Page.IsPostBack Then

                    ' set dimensions of control
                    If _ListBoxWidth <> "" Then
                        lstAvailable.Width = System.Web.UI.WebControls.Unit.Parse(_ListBoxWidth)
                        lstAssigned.Width = System.Web.UI.WebControls.Unit.Parse(_ListBoxWidth)
                    End If
                    If _ListBoxHeight <> "" Then
                        lstAvailable.Height = System.Web.UI.WebControls.Unit.Parse(_ListBoxHeight)
                        lstAssigned.Height = System.Web.UI.WebControls.Unit.Parse(_ListBoxHeight)
                    End If

                    ' load available
                    lstAvailable.DataTextField = _DataTextField
                    lstAvailable.DataValueField = _DataValueField
                    lstAvailable.DataSource = _Available
                    lstAvailable.DataBind()
                    Sort(lstAvailable)

                    ' load selected
                    lstAssigned.DataTextField = _DataTextField
                    lstAssigned.DataValueField = _DataValueField
                    lstAssigned.DataSource = _Assigned
                    lstAssigned.DataBind()
                    Sort(lstAssigned)

                    ' set enabled
                    lstAvailable.Enabled = _Enabled
                    lstAssigned.Enabled = _Enabled

                    ' save persistent values
                    ViewState(Me.ClientID & "_ListBoxWidth") = _ListBoxWidth
                    ViewState(Me.ClientID & "_ListBoxHeight") = _ListBoxHeight

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdd.Click

            Dim objListItem As ListItem

            Dim objList As ArrayList = New ArrayList

            For Each objListItem In lstAvailable.Items
                objList.Add(objListItem)
            Next

            For Each objListItem In objList
                If objListItem.Selected Then
                    lstAvailable.Items.Remove(objListItem)
                    lstAssigned.Items.Add(objListItem)
                End If
            Next

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAssigned)

        End Sub

        Private Sub cmdRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRemove.Click

            Dim objListItem As ListItem

            Dim objList As ArrayList = New ArrayList

            For Each objListItem In lstAssigned.Items
                objList.Add(objListItem)
            Next

            For Each objListItem In objList
                If objListItem.Selected Then
                    lstAssigned.Items.Remove(objListItem)
                    lstAvailable.Items.Add(objListItem)
                End If
            Next

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAvailable)

        End Sub

        Private Sub cmdAddAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddAll.Click

            Dim objListItem As ListItem

            For Each objListItem In lstAvailable.Items
                lstAssigned.Items.Add(objListItem)
            Next

            lstAvailable.Items.Clear()

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAssigned)

        End Sub

        Private Sub cmdRemoveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRemoveAll.Click

            Dim objListItem As ListItem

            For Each objListItem In lstAssigned.Items
                lstAvailable.Items.Add(objListItem)
            Next

            lstAssigned.Items.Clear()

            lstAvailable.ClearSelection()
            lstAssigned.ClearSelection()

            Sort(lstAvailable)
        End Sub

        Private Sub Sort(ByVal ctlListBox As ListBox)

            Dim arrListItems As New ArrayList
            Dim objListItem As ListItem

            ' store listitems in temp arraylist
            For Each objListItem In ctlListBox.Items
                arrListItems.Add(objListItem)
            Next

            ' sort arraylist based on text value
            arrListItems.Sort(New ListItemComparer)

            ' clear control
            ctlListBox.Items.Clear()

            ' add listitems to control
            For Each objListItem In arrListItems
                ctlListBox.Items.Add(objListItem)
            Next

        End Sub

    End Class

    Public Class ListItemComparer
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim a As ListItem = CType(x, ListItem)
            Dim b As ListItem = CType(y, ListItem)
            Dim c As New CaseInsensitiveComparer
            Return c.Compare(a.Text, b.Text)
        End Function
    End Class

End Namespace
