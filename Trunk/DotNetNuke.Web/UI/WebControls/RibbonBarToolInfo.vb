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

Namespace DotNetNuke.Web.UI.WebControls

	Public Class RibbonBarToolInfo

		Public Sub New()
		End Sub

		Public Sub New(ByVal toolName As String, ByVal isHostTool As Boolean, ByVal useButton As Boolean, ByVal linkWindowTarget As String, ByVal moduleFriendlyName As String, ByVal controlKey As String)
			_ToolName = toolName
			_IsHostTool = isHostTool
			_useButton = useButton
			_LinkWindowTarget = linkWindowTarget
			_ModuleFriendlyName = moduleFriendlyName
			_ControlKey = controlKey
		End Sub

		Private _ToolName As String = ""
		Public Property ToolName() As String
			Get
				Return _ToolName
			End Get
			Set(ByVal value As String)
				_ToolName = value
			End Set
		End Property

		Private _IsHostTool As Boolean = False
		Public Property IsHostTool() As Boolean
			Get
				Return _IsHostTool
			End Get
			Set(ByVal value As Boolean)
				_IsHostTool = value
			End Set
		End Property

		Private _useButton As Boolean = False
		Public Property UseButton() As Boolean
			Get
				Return _useButton
			End Get
			Set(ByVal value As Boolean)
				_useButton = value
			End Set
		End Property

		Private _LinkWindowTarget As String = ""
		Public Property LinkWindowTarget() As String
			Get
				Return _LinkWindowTarget
			End Get
			Set(ByVal value As String)
				_LinkWindowTarget = value
			End Set
		End Property

		Private _ModuleFriendlyName As String = ""
		Public Property ModuleFriendlyName() As String
			Get
				Return _ModuleFriendlyName
			End Get
			Set(ByVal value As String)
				_ModuleFriendlyName = value
			End Set
		End Property

		Private _ControlKey As String = ""
		Public Property ControlKey() As String
			Get
				Return _ControlKey
			End Get
			Set(ByVal value As String)
				_ControlKey = value
			End Set
		End Property

	End Class

End Namespace
