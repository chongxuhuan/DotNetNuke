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
    ''' -----------------------------------------------------------------------------
    ''' <summary></summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    '''                             brackets from property names
    ''' </history>
    ''' -----------------------------------------------------------------------------


	Public Interface IModuleCommunicator

		Event ModuleCommunication As ModuleCommunicationEventHandler
	End Interface	'IModuleCommunicator
    _

	Public Interface IModuleListener
		Sub OnModuleCommunication(ByVal s As Object, ByVal e As ModuleCommunicationEventArgs)
	End Interface	'IModuleListener


	Public Delegate Sub ModuleCommunicationEventHandler(ByVal sender As Object, ByVal e As ModuleCommunicationEventArgs)
    _

	Public Class RoleChangeEventArgs
		Inherits ModuleCommunicationEventArgs
		Private _RoleId As String = Nothing
		Private _PortalId As String = Nothing
		Public Property PortalId() As String
			Get
				Return _PortalId
			End Get
			Set(ByVal Value As String)
				_PortalId = Value
			End Set
		End Property


		Public Property RoleId() As String
			Get
				Return _RoleId
			End Get
			Set(ByVal Value As String)
				_RoleId = Value
			End Set
		End Property

	End Class	'ModuleCommunicationEventArgs

	Public Class ModuleCommunicationEventArgs
		Inherits System.EventArgs

		Private _Type As String = Nothing
		Private _Value As Object = Nothing
		Private _Sender As String = Nothing
		Private _Target As String = Nothing

		Private _Text As String = Nothing

        Public Property Text() As String
            Get
                Return _Text
            End Get
            Set(ByVal Value As String)
                _Text = Value
            End Set
        End Property

        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(ByVal Value As String)
                _Type = Value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return _Value
            End Get
            Set(ByVal Value As Object)
                _Value = Value
            End Set
        End Property

        Public Property Sender() As String
            Get
                Return _Sender
            End Get
            Set(ByVal Value As String)
                _Sender = Value
            End Set
        End Property

        Public Property Target() As String
            Get
                Return _Target
            End Get
            Set(ByVal Value As String)
                _Target = Value
            End Set
        End Property

        Public Sub New()
        End Sub    'New


        Public Sub New(ByVal Type As String, ByVal Value As Object, ByVal Sender As String, ByVal Target As String)
            _Type = Type
            _Value = Value
            _Sender = Sender
            _Target = Target

        End Sub

        Public Sub New(ByVal Text As String)
            _Text = Text
        End Sub    'New
    End Class 'ModuleCommunicationEventArgs
    _ 


	Public Class ModuleCommunicators
		Inherits System.Collections.CollectionBase


		Default Public Property Item(ByVal index As Integer) As IModuleCommunicator
			Get
				Return CType(Me.List(index), IModuleCommunicator)
			End Get
			Set(ByVal Value As IModuleCommunicator)
				Me.List(index) = Value
			End Set
		End Property

		Public Sub New()
		End Sub	   'New


		Public Function Add(ByVal item As IModuleCommunicator) As Integer
			Return Me.List.Add(item)
		End Function	   'Add
	End Class	'ModuleCommunicators
    _ 

	Public Class ModuleListeners
		Inherits System.Collections.CollectionBase


		Default Public Property Item(ByVal index As Integer) As IModuleListener
			Get
				Return CType(Me.List(index), IModuleListener)
			End Get
			Set(ByVal Value As IModuleListener)
				Me.List(index) = Value
			End Set
		End Property

		Public Sub New()
		End Sub	   'New


		Public Function Add(ByVal item As IModuleListener) As Integer
			Return Me.List.Add(item)
		End Function	   'Add

	End Class	'ModuleListeners

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
		End Sub	   'New 

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
		End Function	   'Add 


		Private Overloads Function Add(ByVal item As IModuleListener) As Integer
			Dim returnData As Integer = _ModuleListeners.Add(item)

			Dim i As Integer
			For i = 0 To _ModuleCommunicators.Count - 1
				AddHandler _ModuleCommunicators(i).ModuleCommunication, AddressOf item.OnModuleCommunication
			Next i

			Return returnData
		End Function	   'Add 

	End Class

End Namespace