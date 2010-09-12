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

Imports System.Collections.Generic

Namespace DotNetNuke.ComponentModel

    Public Class SimpleContainer
        Inherits AbstractContainer

        Private componentBuilders As New ComponentBuilderCollection()
        Private componentDependencies As New Dictionary(Of String, IDictionary)
        Private componentLock As New Object
        Private componentTypes As New ComponentTypeCollection()
        Private registeredComponents As New Dictionary(Of System.Type, String)

        Private _Name As String

        ''' <summary>
        ''' Initializes a new instance of the SimpleContainer class.
        ''' </summary>
        Public Sub New()
            Me.New(String.Format("Container_{0}", Guid.NewGuid.ToString()))
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the SimpleContainer class.
        ''' </summary>
        ''' <param name="name"></param>
        Public Sub New(ByVal name As String)
            _Name = name
        End Sub

        Private Sub AddBuilder(ByVal contractType As System.Type, ByVal builder As IComponentBuilder)
            SyncLock componentLock
                If Not componentTypes.Item(contractType).ComponentBuilders.Contains(builder.Name) Then
                    componentTypes.Item(contractType).ComponentBuilders.Add(builder)
                End If
                If Not componentBuilders.Contains(builder.Name) Then
                    componentBuilders.Add(builder)
                End If
            End SyncLock
        End Sub

        Private Overloads Function GetComponent(ByVal builder As IComponentBuilder) As Object
            Dim component As Object
            If builder Is Nothing Then
                component = Nothing
            Else
                component = builder.BuildComponent()
            End If
            Return component
        End Function

        Public Overloads Overrides Function GetComponent(ByVal name As String) As Object
            Dim component As Object = Nothing
            If componentBuilders.Contains(name) Then
                component = GetComponent(componentBuilders.Item(name))
            End If
            Return component
        End Function

        Public Overloads Overrides Function GetComponent(ByVal contractType As System.Type) As Object
            Dim component As Object = Nothing
            If componentTypes.Contains(contractType) Then
                Dim type As ComponentType = componentTypes.Item(contractType)
                If type.ComponentBuilders.Count > 0 Then
                    component = GetComponent(type.ComponentBuilders.Item(0))
                End If
            End If
            Return component
        End Function

        Public Overloads Overrides Function GetComponent(ByVal name As String, ByVal contractType As System.Type) As Object
            Dim component As Object = Nothing
            If componentTypes.Contains(contractType) Then
                Dim type As ComponentType = componentTypes.Item(contractType)
                If type.ComponentBuilders.Contains(name) Then
                    component = GetComponent(type.ComponentBuilders.Item(name))
                End If
            End If
            Return component
        End Function

        Public Overrides Function GetComponentList(ByVal contractType As System.Type) As String()
            Dim components As New List(Of String)
            For Each kvp As KeyValuePair(Of Type, String) In registeredComponents
                If kvp.Key.BaseType Is contractType Then
                    components.Add(kvp.Value)
                End If
            Next
            Return components.ToArray()
        End Function

        Public Overrides Function GetComponentSettings(ByVal name As String) As System.Collections.IDictionary
            Return componentDependencies(name)
        End Function

        Public Overrides ReadOnly Property Name() As String
            Get
                Return _Name
            End Get
        End Property

        Public Overloads Overrides Sub RegisterComponent(ByVal name As String, ByVal contractType As System.Type, ByVal componentType As System.Type, ByVal lifestyle As ComponentLifeStyleType)
            If Not componentTypes.Contains(contractType) Then
                componentTypes.Add(New ComponentType(contractType))
            End If
            Dim builder As IComponentBuilder = Nothing
            Select Case lifestyle
                Case ComponentLifeStyleType.Transient
                    builder = New TransientComponentBuilder(name, componentType)
                Case ComponentLifeStyleType.Singleton
                    builder = New SingletonComponentBuilder(name, componentType)
            End Select
            AddBuilder(contractType, builder)

            registeredComponents(componentType) = name
        End Sub

        Public Overloads Overrides Sub RegisterComponentInstance(ByVal name As String, ByVal contractType As System.Type, ByVal instance As Object)
            If Not componentTypes.Contains(contractType) Then
                componentTypes.Add(New ComponentType(contractType))
            End If
            AddBuilder(contractType, New InstanceComponentBuilder(name, instance))
        End Sub

        Public Overrides Sub RegisterComponentSettings(ByVal name As String, ByVal dependencies As System.Collections.IDictionary)
            componentDependencies(name) = dependencies
        End Sub

    End Class

End Namespace
