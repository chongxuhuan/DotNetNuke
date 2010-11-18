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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Permissions
Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports DotNetNuke.UI.Utilities
Imports System.Linq

Namespace DotNetNuke.Admin.Modules

    Partial Class ModuleLocalization
        Inherits UserControlBase

#Region "Private Members"

        Private _Modules As List(Of ModuleInfo)
        Private _ShowEditColumn As Boolean = Null.NullBoolean
        Private _ShowFooter As Boolean = True
        Private _ShowLanguageColumn As Boolean = True

#End Region

        Public Event ModuleLocalizationChanged As EventHandler(Of EventArgs)

#Region "Contructors"

        Public Sub New()
            ModuleId = Null.NullInteger
            TabId = Null.NullInteger
        End Sub

#End Region

        Protected ReadOnly Property Modules As List(Of ModuleInfo)
            Get
                If _Modules Is Nothing Then
                    _Modules = LoadTabModules()
                End If
                Return _Modules
            End Get
        End Property


#Region "Public Properties"

        Public ReadOnly Property LocalResourceFile As String
            Get
                Return Localization.GetResourceFile(Me, "ModuleLocalization.ascx")
            End Get
        End Property

        Public Property ModuleId As Integer
            Get
                Return DirectCast(ViewState("ModuleId"), Integer)
            End Get
            Set(ByVal value As Integer)
                ViewState("ModuleId") = value
            End Set
        End Property

        Public Property ShowEditColumn As Boolean
            Get
                Return _ShowEditColumn
            End Get
            Set(ByVal value As Boolean)
                _ShowEditColumn = value
            End Set
        End Property

        Public Property ShowFooter As Boolean
            Get
                Return _ShowFooter
            End Get
            Set(ByVal value As Boolean)
                _ShowFooter = value
            End Set
        End Property

        Public Property ShowLanguageColumn As Boolean
            Get
                Return _ShowLanguageColumn
            End Get
            Set(ByVal value As Boolean)
                _ShowLanguageColumn = value
            End Set
        End Property

        Public Property TabId As Integer
            Get
                Return DirectCast(ViewState("TabId"), Integer)
            End Get
            Set(ByVal value As Integer)
                ViewState("TabId") = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Function LoadTabModules() As List(Of ModuleInfo)
            Dim moduleCtl As New ModuleController
            Dim moduleList As New List(Of ModuleInfo)

            'Check if we have module scope
            If ModuleId > Null.NullInteger Then
                Dim sourceModule As ModuleInfo = moduleCtl.GetModule(ModuleId, TabId)
                If sourceModule.LocalizedModules IsNot Nothing Then
                    For Each localizedModule As ModuleInfo In sourceModule.LocalizedModules.Values
                        moduleList.Add(localizedModule)
                    Next
                End If
            Else
                For Each m As ModuleInfo In moduleCtl.GetTabModules(Me.TabId).Values
                    If Not m.IsDeleted AndAlso Not m.AllTabs Then
                        moduleList.Add(m)
                        If m.LocalizedModules IsNot Nothing Then
                            For Each localizedModule As ModuleInfo In m.LocalizedModules.Values
                                moduleList.Add(localizedModule)
                            Next
                        End If
                    End If
                Next
            End If

            Return moduleList
        End Function

        Private Sub ToggleCheckBox(ByVal dataItem As GridDataItem, ByVal toggleValue As Boolean)
            Dim rowCheckBox As CheckBox = CType(dataItem.FindControl("rowCheckBox"), CheckBox)
            If rowCheckBox.Visible Then
                rowCheckBox.Checked = toggleValue
                dataItem.Selected = toggleValue
            End If
        End Sub


#End Region

#Region "Protected Methods"

        Protected Function ShowHeaderCheckBox() As Boolean
            Dim showCheckBox As Boolean = Null.NullBoolean
            If Modules IsNot Nothing Then
                showCheckBox = Modules.Where(Function(m) Not m.IsDefaultLanguage).Count > 0
            End If
            Return showCheckBox
        End Function

        Protected Sub OnModuleLocalizationChanged(ByVal e As EventArgs)
            RaiseEvent ModuleLocalizationChanged(Me, e)
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub DataBind()
            If TabId <> Null.NullInteger Then
                localizedModulesGrid.DataSource = Modules
            End If
            localizedModulesGrid.DataBind()
        End Sub

        Public Sub LocalizeSelectedItems(ByVal localize As Boolean)
            Dim moduleCtrl As New ModuleController()

            For Each row As GridDataItem In localizedModulesGrid.SelectedItems
                Dim localizedModuleId As Integer = DirectCast(row.OwnerTableView.DataKeyValues(row.ItemIndex)("ModuleId"), Integer)
                Dim localizedTabId As Integer = DirectCast(row.OwnerTableView.DataKeyValues(row.ItemIndex)("TabId"), Integer)
                Dim sourceModule As ModuleInfo = moduleCtrl.GetModule(localizedModuleId, localizedTabId, False)

                If sourceModule IsNot Nothing Then
                    If sourceModule.DefaultLanguageModule IsNot Nothing Then

                        If localize Then
                            'Localize
                            moduleCtrl.LocalizeModule(sourceModule)
                        Else
                            'Delocalize
                            moduleCtrl.DeLocalizeModule(sourceModule)

                            'Mark module as Not Translated
                            moduleCtrl.UpdateTranslationStatus(sourceModule, False)
                        End If
                    End If
                End If
            Next

            moduleCtrl.ClearCache(Me.TabId)

            'Rebind localized Modules
            DataBind()

            'Raise Changed event
            OnModuleLocalizationChanged(EventArgs.Empty)
        End Sub

        Public Sub MarkTranslatedSelectedItems(ByVal translated As Boolean)
            Dim moduleCtrl As New ModuleController()

            For Each row As GridDataItem In localizedModulesGrid.SelectedItems
                Dim localizedModuleId As Integer = DirectCast(row.OwnerTableView.DataKeyValues(row.ItemIndex)("ModuleId"), Integer)
                Dim localizedTabId As Integer = DirectCast(row.OwnerTableView.DataKeyValues(row.ItemIndex)("TabId"), Integer)
                Dim sourceModule As ModuleInfo = moduleCtrl.GetModule(localizedModuleId, localizedTabId, False)

                If sourceModule.IsLocalized Then
                    moduleCtrl.UpdateTranslationStatus(sourceModule, translated)
                End If
            Next

            moduleCtrl.ClearCache(Me.TabId)

            'Rebind localized Modules
            DataBind()

            'Raise Changed event
            OnModuleLocalizationChanged(EventArgs.Empty)
        End Sub

#End Region

#Region "EventHandlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ClientAPI.AddButtonConfirm(delocalizeModuleButton, Localization.GetString("BindConfirm", Me.LocalResourceFile))
        End Sub

        Protected Sub delocalizeModuleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles delocalizeModuleButton.Click
            LocalizeSelectedItems(False)
        End Sub

        Protected Sub localizeModuleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles localizeModuleButton.Click
            LocalizeSelectedItems(True)
        End Sub

        Protected Sub localizedModulesGrid_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles localizedModulesGrid.ItemDataBound
            Dim gridItem As GridDataItem = TryCast(e.Item, GridDataItem)
            If gridItem IsNot Nothing Then
                Dim localizedModule As ModuleInfo = TryCast(gridItem.DataItem, ModuleInfo)
                If localizedModule IsNot Nothing Then
                    Dim selectCheckBox As CheckBox = gridItem.FindControl("rowCheckBox")
                    If selectCheckBox IsNot Nothing Then
                        selectCheckBox.Visible = Not localizedModule.IsDefaultLanguage
                    End If
                End If
            End If
        End Sub

        Protected Sub localizedModulesGrid_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles localizedModulesGrid.PreRender
            For Each column As GridColumn In localizedModulesGrid.Columns
                If (column.UniqueName = "Edit") Then
                    column.Visible = ShowEditColumn
                End If
                If (column.UniqueName = "Language") Then
                    column.Visible = ShowLanguageColumn
                End If
            Next
            localizedModulesGrid.Rebind()

            footerPlaceHolder.Visible = ShowFooter AndAlso Modules.Where(Function(m) Not m.IsDefaultLanguage).Count > 0
        End Sub

        Protected Sub markModuleTranslatedButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markModuleTranslatedButton.Click
            MarkTranslatedSelectedItems(True)
        End Sub

        Protected Sub markModuleUnTranslatedButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markModuleUnTranslatedButton.Click
            MarkTranslatedSelectedItems(False)
        End Sub

        Protected Sub ToggleRowSelection(ByVal sender As Object, ByVal e As EventArgs)
            CType(CType(sender, CheckBox).Parent.Parent, GridItem).Selected = _
              CType(sender, CheckBox).Checked
        End Sub

        Protected Sub ToggleSelectedState(ByVal sender As Object, ByVal e As EventArgs)
            For Each dataItem As GridDataItem In localizedModulesGrid.MasterTableView.Items
                ToggleCheckBox(dataItem, CType(sender, CheckBox).Checked)
            Next
        End Sub

#End Region

    End Class

End Namespace
