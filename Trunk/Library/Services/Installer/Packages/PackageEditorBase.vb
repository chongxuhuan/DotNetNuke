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

Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Entities.Modules.Actions

Namespace DotNetNuke.Services.Installer.Packages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageEditorBase class provides a Base Classs for Package Editors
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	02/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PackageEditorBase
        Inherits ModuleUserControlBase
        Implements IPackageEditor

#Region "Private Members"

        Private _PackageID As Integer = Null.NullInteger
        Private _Package As PackageInfo = Nothing
        Private _IsWizard As Boolean = Null.NullBoolean

#End Region

#Region "IPackageEditor Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Package ID
        ''' </summary>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PackageID() As Integer Implements IPackageEditor.PackageID
            Get
                Return _PackageID
            End Get
            Set(ByVal value As Integer)
                _PackageID = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Editor is in the Wizard
        ''' </summary>
        ''' <value>An Boolean</value>
        ''' <history>
        ''' 	[cnurse]	08/26/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsWizard() As Boolean Implements IPackageEditor.IsWizard
            Get
                Return _IsWizard
            End Get
            Set(ByVal value As Boolean)
                _IsWizard = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Used to Initialize the Control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/21/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub Initialize() Implements IPackageEditor.Initialize

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Used to Update the Package
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/21/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub UpdatePackage() Implements IPackageEditor.UpdatePackage

        End Sub

#End Region

#Region "Protected Properties"

        Protected Overridable ReadOnly Property EditorID() As String
            Get
                Return Null.NullString
            End Get
        End Property

        Protected ReadOnly Property IsSuperTab() As Boolean
            Get
                Return ModuleContext.PortalSettings.ActiveTab.IsSuperTab
            End Get
        End Property

        Protected ReadOnly Property Package() As PackageInfo
            Get
                If _Package Is Nothing Then
                    _Package = PackageController.GetPackage(PackageID)
                End If
                Return _Package
            End Get
        End Property

#End Region

#Region "Private Methods"


#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Me.ID = EditorID

            MyBase.OnInit(e)
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)
        End Sub

#End Region

    End Class

End Namespace
