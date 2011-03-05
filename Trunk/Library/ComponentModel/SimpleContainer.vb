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
Imports DotNetNuke.Collections.Internal

Namespace DotNetNuke.ComponentModel

    Public Class SimpleContainer
        Inherits AbstractContainer

        Private componentBuilders As New ComponentBuilderCollection()
        Private componentDependencies As New SharedDictionary(Of String, IDictionary)
        Private componentTypes As New ComponentTypeCollection()
        Private registeredComponents As New SharedDictionary(Of System.Type, String)

        Private _Name As String

#Region "Constructors"

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

#End Region

#Region "Private Methods"

        Private Sub AddBuilder(ByVal contractType As System.Type, ByVal builder As IComponentBuilder)
            Dim componentType As ComponentType = GetComponentType(contractType)
            If componentType IsNot Nothing Then
                Dim builders As ComponentBuilderCollection = componentType.ComponentBuilders

                Using writeLock As ISharedCollectionLock = builders.GetWriteLock()
                    builders.AddBuilder(builder, True)
                End Using

                Using writeLock As ISharedCollectionLock = componentBuilders.GetWriteLock()
                    componentBuilders.AddBuilder(builder, False)
                End Using
            End If

        End Sub

        Private Sub AddComponentType(ByVal contractType As System.Type)
            Dim componentType As ComponentType = GetComponentType(contractType)

            If componentType Is Nothing Then
                componentType = New ComponentType(contractType)

                Using writeLock As ISharedCollectionLock = componentTypes.GetWriteLock()
                    componentTypes(componentType.BaseType) = componentType
                End Using
            End If
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

        Private Function GetComponentBuilder(ByVal name As String) As IComponentBuilder
            Dim builder As IComponentBuilder = Nothing

            Using writeLock As ISharedCollectionLock = componentBuilders.GetReadLock()
                componentBuilders.TryGetValue(name, builder)
            End Using

            Return builder
        End Function

        Private Function GetDefaultComponentBuilder(ByVal componentType As ComponentType) As IComponentBuilder
            Dim builder As IComponentBuilder = Nothing

            Using writeLock As ISharedCollectionLock = componentType.ComponentBuilders.GetReadLock()
                builder = componentType.ComponentBuilders.DefaultBuilder
            End Using

            Return builder
        End Function

        Private Function GetComponentType(ByVal contractType As System.Type) As ComponentType
            Dim componentType As ComponentType = Nothing

            Using writeLock As ISharedCollectionLock = componentTypes.GetReadLock()
                componentTypes.TryGetValue(contractType, componentType)
            End Using

            Return componentType
        End Function

        Private Overloads Sub RegisterComponent(ByVal name As String, ByVal type As System.Type)
            Using writeLock As ISharedCollectionLock = registeredComponents.GetWriteLock()
                registeredComponents(type) = name
            End Using
        End Sub

#End Region

        Public Overloads Overrides Function GetComponent(ByVal name As String) As Object
            Dim builder As IComponentBuilder = GetComponentBuilder(name)

            Return GetComponent(builder)
        End Function

        Public Overloads Overrides Function GetComponent(ByVal contractType As System.Type) As Object
            Dim componentType As ComponentType = GetComponentType(contractType)
            Dim component As Object = Nothing

            If componentType IsNot Nothing Then
                Dim builderCount As Integer = 0

                Using readLock As ISharedCollectionLock = componentType.ComponentBuilders.GetReadLock()
                    builderCount = componentType.ComponentBuilders.Count
                End Using

                If builderCount > 0 Then
                    Dim builder As IComponentBuilder = GetDefaultComponentBuilder(componentType)

                    component = GetComponent(builder)
                End If
            End If

            Return component
        End Function

        Public Overloads Overrides Function GetComponent(ByVal name As String, ByVal contractType As System.Type) As Object
            Dim componentType As ComponentType = GetComponentType(contractType)
            Dim component As Object = Nothing

            If componentType IsNot Nothing Then
                Dim builder As IComponentBuilder = GetComponentBuilder(name)

                component = GetComponent(builder)
            End If
            Return component
        End Function

        Public Overrides Function GetComponentList(ByVal contractType As System.Type) As String()
            Dim components As New List(Of String)

            Using readLock As ISharedCollectionLock = registeredComponents.GetReadLock()
                For Each kvp As KeyValuePair(Of Type, String) In registeredComponents
                    If kvp.Key.BaseType Is contractType Then
                        components.Add(kvp.Value)
                    End If
                Next
            End Using
            Return components.ToArray()
        End Function

        Public Overrides Function GetComponentSettings(ByVal name As String) As System.Collections.IDictionary
            Dim settings As IDictionary
            Using readLock As ISharedCollectionLock = componentDependencies.GetReadLock()
                settings = componentDependencies(name)
            End Using
            Return settings
        End Function

        Public Overrides ReadOnly Property Name() As String
            Get
                Return _Name
            End Get
        End Property

        Public Overloads Overrides Sub RegisterComponent(ByVal name As String, ByVal contractType As System.Type, ByVal type As System.Type, ByVal lifestyle As ComponentLifeStyleType)
            AddComponentType(contractType)

            Dim builder As IComponentBuilder = Nothing
            Select Case lifestyle
                Case ComponentLifeStyleType.Transient
                    builder = New TransientComponentBuilder(name, type)
                Case ComponentLifeStyleType.Singleton
                    builder = New SingletonComponentBuilder(name, type)
            End Select
            AddBuilder(contractType, builder)

            RegisterComponent(name, type)
        End Sub

        Public Overloads Overrides Sub RegisterComponentInstance(ByVal name As String, ByVal contractType As System.Type, ByVal instance As Object)
            AddComponentType(contractType)

            AddBuilder(contractType, New InstanceComponentBuilder(name, instance))
        End Sub

        Public Overrides Sub RegisterComponentSettings(ByVal name As String, ByVal dependencies As System.Collections.IDictionary)
            Using writeLock As ISharedCollectionLock = componentDependencies.GetWriteLock()
                componentDependencies(name) = dependencies
            End Using
        End Sub

    End Class

End Namespace
