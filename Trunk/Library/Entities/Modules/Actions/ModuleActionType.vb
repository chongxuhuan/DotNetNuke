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

Namespace DotNetNuke.Entities.Modules.Actions

	'''-----------------------------------------------------------------------------
	''' Project		: DotNetNuke
	''' Class		: ModuleActionType
	'''-----------------------------------------------------------------------------
	''' <summary>
	''' Identifies common module action types
	''' </summary>
	''' <remarks>
	''' Common action types can be specified in the CommandName attribute of the 
	''' ModuleAction class
	''' </remarks>
	'''-----------------------------------------------------------------------------
    Public Class ModuleActionType

        Public Const AddContent As String = "AddContent.Action"
        Public Const EditContent As String = "EditContent.Action"
        Public Const ContentOptions As String = "ContentOptions.Action"
        Public Const SyndicateModule As String = "SyndicateModule.Action"
        Public Const ImportModule As String = "ImportModule.Action"
        Public Const ExportModule As String = "ExportModule.Action"
        Public Const OnlineHelp As String = "OnlineHelp.Action"
        Public Const ModuleHelp As String = "ModuleHelp.Action"
        Public Const HelpText As String = "ModuleHelp.Text"
        Public Const PrintModule As String = "PrintModule.Action"
        Public Const ModuleSettings As String = "ModuleSettings.Action"
        Public Const DeleteModule As String = "DeleteModule.Action"
        Public Const ClearCache As String = "ClearCache.Action"
        Public Const MoveTop As String = "MoveTop.Action"
        Public Const MoveUp As String = "MoveUp.Action"
        Public Const MoveDown As String = "MoveDown.Action"
        Public Const MoveBottom As String = "MoveBottom.Action"
        Public Const MovePane As String = "MovePane.Action"
        Public Const MoveRoot As String = "MoveRoot.Action"
        Public Const ViewSource As String = "ViewSource.Action"

    End Class

End Namespace
