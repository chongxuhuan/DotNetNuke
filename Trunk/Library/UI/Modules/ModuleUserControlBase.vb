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

Namespace DotNetNuke.UI.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Modules
    ''' Class	 : ModuleUserControlBase
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleUserControlBase is a base class for Module Controls that inherits from the
    ''' UserControl base class.  As with all MontrolControl base classes it implements
    ''' IModuleControl.
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	12/16/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleUserControlBase
        Inherits UserControl
        Implements IModuleControl

#Region "Private Members"

        Private _localResourceFile As String
        Private _moduleContext As ModuleInstanceContext

#End Region

#Region "IModuleControl Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the underlying base control for this ModuleControl
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/17/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Control() As Control Implements IModuleControl.Control
            Get
                Return Me
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Path for this control (used primarily for UserControls)
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ControlPath() As String Implements IModuleControl.ControlPath
            Get
                Return Me.TemplateSourceDirectory & "/"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Name for this control
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ControlName() As String Implements IModuleControl.ControlName
            Get
                Return Me.GetType.Name.Replace("_", ".")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the local resource file for this control
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LocalResourceFile() As String Implements IModuleControl.LocalResourceFile
            Get
                Dim fileRoot As String

                If String.IsNullOrEmpty(_localResourceFile) Then
                    fileRoot = Path.Combine(Me.ControlPath, Services.Localization.Localization.LocalResourceDirectory & "/" & Me.ID)
                Else
                    fileRoot = _localResourceFile
                End If
                Return fileRoot
            End Get
            Set(ByVal Value As String)
                _localResourceFile = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Module Context for this control
        ''' </summary>
        ''' <returns>A ModuleInstanceContext</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleContext() As ModuleInstanceContext Implements IModuleControl.ModuleContext
            Get
                If _moduleContext Is Nothing Then
                    _moduleContext = New ModuleInstanceContext(Me)
                End If
                Return _moduleContext
            End Get
        End Property

#End Region

    End Class


End Namespace