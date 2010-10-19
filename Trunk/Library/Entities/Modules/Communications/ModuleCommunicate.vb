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


Imports System

Namespace DotNetNuke.Entities.Modules.Communications

	Public Class ModuleCommunicate

		Private _ModuleCommunicators As New ModuleCommunicators
		Private _ModuleListeners As New ModuleListeners

		Public ReadOnly Property ModuleCommunicators() As ModuleCommunicators
			Get
				Return _ModuleCommunicators
			End Get
		End Property

		Public ReadOnly Property ModuleListeners() As ModuleListeners
			Get
				Return _ModuleListeners
			End Get
		End Property

		Public Sub New()
        End Sub

		Public Sub LoadCommunicator(ByVal ctrl As System.Web.UI.Control)

			' Check and see if the module implements IModuleCommunicator 
			If TypeOf ctrl Is IModuleCommunicator Then
				Me.Add(CType(ctrl, IModuleCommunicator))
			End If

			' Check and see if the module implements IModuleListener 
			If TypeOf ctrl Is IModuleListener Then
				Me.Add(CType(ctrl, IModuleListener))
			End If

		End Sub

        Private Overloads Function Add(ByVal item As IModuleCommunicator) As Integer
            Dim returnData As Integer = _ModuleCommunicators.Add(item)

            Dim i As Integer
            For i = 0 To _ModuleListeners.Count - 1
                AddHandler item.ModuleCommunication, AddressOf _ModuleListeners(i).OnModuleCommunication
            Next i


            Return returnData
        End Function

        Private Overloads Function Add(ByVal item As IModuleListener) As Integer
            Dim returnData As Integer = _ModuleListeners.Add(item)

            Dim i As Integer
            For i = 0 To _ModuleCommunicators.Count - 1
                AddHandler _ModuleCommunicators(i).ModuleCommunication, AddressOf item.OnModuleCommunication
            Next i

            Return returnData
        End Function

	End Class

End Namespace