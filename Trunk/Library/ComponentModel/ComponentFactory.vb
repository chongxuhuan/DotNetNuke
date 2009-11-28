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
Imports System.Collections.Generic

Namespace DotNetNuke.ComponentModel

    Public Module ComponentFactory

#Region "Private Members"

        Private _Container As IContainer

#End Region

#Region "Public Properties"

        Public Property Container() As IContainer
            Get
                Return _Container
            End Get
            Set(ByVal value As IContainer)
                _Container = value
            End Set
        End Property

#End Region

        Public Sub InstallComponents(ByVal ParamArray installers As IComponentInstaller())
            If installers Is Nothing Then Throw New ArgumentNullException("installers")

            VerifyContainer()
            For Each installer As IComponentInstaller In installers
                If installer Is Nothing Then Throw New ArgumentNullException("installers")
                installer.InstallComponents(Container)
            Next
        End Sub

        Private Sub VerifyContainer()
            If Container Is Nothing Then
                Throw New InvalidOperationException("Cannot register or retrieve components until ComponentFactory.Container is set")
            End If
        End Sub

#Region "GetComponent"

        Public Function GetComponent(ByVal name As String) As Object
            VerifyContainer()
            Return Container.GetComponent(name)
        End Function

        Public Function GetComponent(Of TContract)() As TContract
            VerifyContainer()
            Return Container.GetComponent(Of TContract)()
        End Function

        Public Function GetComponent(ByVal contractType As Type) As Object
            VerifyContainer()
            Return Container.GetComponent(contractType)
        End Function

        Public Function GetComponent(Of TContract)(ByVal name As String) As TContract
            VerifyContainer()
            Return Container.GetComponent(Of TContract)(name)
        End Function

        Public Function GetComponent(ByVal name As String, ByVal contractType As Type) As Object
            VerifyContainer()
            Return Container.GetComponent(name, contractType)
        End Function

#End Region

#Region "GetComponentList"

        Public Function GetComponentList(ByVal contractType As Type) As String()
            VerifyContainer()
            Return Container.GetComponentList(contractType)
        End Function

        Public Function GetComponentList(Of TContract)() As String()
            VerifyContainer()
            Return Container.GetComponentList(Of TContract)()
        End Function

#End Region

        Public Function GetComponents(Of TContract)() As Dictionary(Of String, TContract)
            VerifyContainer()
            Dim components As New Dictionary(Of String, TContract)
            For Each componentName As String In GetComponentList(Of TContract)()
                components(componentName) = GetComponent(Of TContract)(componentName)
            Next
            Return components
        End Function

#Region "GetComponentSettings"

        Function GetComponentSettings(ByVal name As String) As IDictionary
            VerifyContainer()
            Return Container.GetComponentSettings(name)
        End Function

        Function GetComponentSettings(ByVal component As System.Type) As IDictionary
            VerifyContainer()
            Return Container.GetComponentSettings(component)
        End Function

        Function GetComponentSettings(Of TComponent)() As IDictionary
            VerifyContainer()
            Return Container.GetComponentSettings(Of TComponent)()
        End Function

#End Region

#Region "RegisterComponent"

        Public Sub RegisterComponent(Of TComponent As Class)()
            VerifyContainer()
            Container.RegisterComponent(Of TComponent)()
        End Sub

        Public Sub RegisterComponent(Of TContract, TComponent As Class)()
            VerifyContainer()
            Container.RegisterComponent(Of TContract, TComponent)()
        End Sub

        Public Sub RegisterComponent(ByVal componentType As Type)
            VerifyContainer()
            Container.RegisterComponent(componentType)
        End Sub

        Public Sub RegisterComponent(ByVal contractType As Type, ByVal componentType As Type)
            VerifyContainer()
            Container.RegisterComponent(contractType, componentType)
        End Sub

        Public Sub RegisterComponent(Of TComponent As Class)(ByVal name As String)
            VerifyContainer()
            Container.RegisterComponent(Of TComponent)(name)
        End Sub

        Public Sub RegisterComponent(Of TContract, TComponent As Class)(ByVal name As String)
            VerifyContainer()
            Container.RegisterComponent(Of TContract, TComponent)(name)
        End Sub

        Public Sub RegisterComponent(ByVal name As String, ByVal componentType As Type)
            VerifyContainer()
            Container.RegisterComponent(name, componentType)
        End Sub

        Public Sub RegisterComponent(ByVal name As String, ByVal contractType As Type, ByVal componentType As Type)
            VerifyContainer()
            Container.RegisterComponent(name, contractType, componentType)
        End Sub

#End Region

#Region "RegisterComponentInstance"

        Public Sub RegisterComponentInstance(ByVal name As String, ByVal contractType As Type, ByVal instance As Object)
            VerifyContainer()
            Container.RegisterComponentInstance(name, contractType, instance)
        End Sub

        Public Sub RegisterComponentInstance(ByVal name As String, ByVal instance As Object)
            VerifyContainer()
            Container.RegisterComponentInstance(name, instance)
        End Sub

        Public Sub RegisterComponentInstance(Of TContract)(ByVal name As String, ByVal instance As Object)
            VerifyContainer()
            Container.RegisterComponentInstance(Of TContract)(name, instance)
        End Sub

        Public Sub RegisterComponentInstance(Of TContract)(ByVal instance As Object)
            VerifyContainer()
            Container.RegisterComponentInstance(Of TContract)(instance)
        End Sub

#End Region

#Region "RegisterComponentSettings"

        Public Sub RegisterComponentSettings(ByVal name As String, ByVal dependencies As IDictionary)
            VerifyContainer()
            Container.RegisterComponentSettings(name, dependencies)
        End Sub

        Public Sub RegisterComponentSettings(ByVal component As Type, ByVal dependencies As IDictionary)
            VerifyContainer()
            Container.RegisterComponentSettings(component, dependencies)
        End Sub

        Public Sub RegisterComponentSettings(Of TComponent)(ByVal dependencies As IDictionary)
            VerifyContainer()
            Container.RegisterComponentSettings(Of TComponent)(dependencies)
        End Sub
#End Region

    End Module

End Namespace
