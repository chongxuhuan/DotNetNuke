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

Namespace DotNetNuke.ComponentModel

    Public MustInherit Class AbstractContainer
        Implements IContainer

        Public MustOverride ReadOnly Property Name() As String Implements IContainer.Name

#Region "GetComponent"

        Public MustOverride Function GetComponent(ByVal name As String) As Object Implements IContainer.GetComponent
        Public MustOverride Function GetComponent(ByVal name As String, ByVal contractType As System.Type) As Object Implements IContainer.GetComponent

        Public MustOverride Function GetComponent(ByVal contractType As System.Type) As Object Implements IContainer.GetComponent

        Public Overridable Function GetComponent(Of TContract)() As TContract Implements IContainer.GetComponent
            Return DirectCast(GetComponent(GetType(TContract)), TContract)
        End Function

        Public Overridable Function GetComponent(Of TContract)(ByVal name As String) As TContract Implements IContainer.GetComponent
            Return DirectCast(GetComponent(name, GetType(TContract)), TContract)
        End Function

#End Region

#Region "GetComponentList"

        Public MustOverride Function GetComponentList(ByVal contractType As Type) As String() Implements IContainer.GetComponentList

        Public Overridable Function GetComponentList(Of TContract)() As String() Implements IContainer.GetComponentList
            Return GetComponentList(GetType(TContract))
        End Function

#End Region


#Region "GetComponentSettings"

        Public MustOverride Function GetComponentSettings(ByVal name As String) As IDictionary Implements IContainer.GetComponentSettings

        Public Overridable Function GetComponentSettings(ByVal component As System.Type) As IDictionary Implements IContainer.GetComponentSettings
            Return GetComponentSettings(component.FullName)
        End Function

        Public Overridable Function GetCustomDependencies(Of TComponent)() As IDictionary Implements IContainer.GetComponentSettings
            Return GetComponentSettings(GetType(TComponent).FullName)
        End Function

#End Region

#Region "RegisterComponent"

        Public MustOverride Sub RegisterComponent(ByVal name As String, ByVal contractType As System.Type, ByVal componentType As System.Type, ByVal lifestyle As ComponentLifeStyleType) Implements IContainer.RegisterComponent

        Public Overridable Sub RegisterComponent(ByVal name As String, ByVal contractType As System.Type, ByVal componentType As System.Type) Implements IContainer.RegisterComponent
            RegisterComponent(name, contractType, componentType, ComponentLifeStyleType.Singleton)
        End Sub

        Public Overridable Sub RegisterComponent(ByVal name As String, ByVal componentType As System.Type) Implements IContainer.RegisterComponent
            RegisterComponent(name, componentType, componentType, ComponentLifeStyleType.Singleton)
        End Sub

        Public Overridable Sub RegisterComponent(ByVal contractType As System.Type, ByVal componentType As System.Type) Implements IContainer.RegisterComponent
            RegisterComponent(componentType.FullName, contractType, componentType, ComponentLifeStyleType.Singleton)
        End Sub

        Public Overridable Sub RegisterComponent(ByVal contractType As System.Type, ByVal componentType As System.Type, ByVal lifestyle As ComponentLifeStyleType) Implements IContainer.RegisterComponent
            RegisterComponent(componentType.FullName, contractType, componentType, lifestyle)
        End Sub

        Public Overridable Sub RegisterComponent(ByVal componentType As System.Type) Implements IContainer.RegisterComponent
            RegisterComponent(componentType.FullName, componentType, componentType, ComponentLifeStyleType.Singleton)
        End Sub

        Public Overridable Sub RegisterComponent(Of TComponent As Class)() Implements IContainer.RegisterComponent
            RegisterComponent(GetType(TComponent))
        End Sub

        Public Overridable Sub RegisterComponent(Of TComponent As Class)(ByVal name As String) Implements IContainer.RegisterComponent
            RegisterComponent(name, GetType(TComponent), GetType(TComponent), ComponentLifeStyleType.Singleton)
        End Sub

        Public Overridable Sub RegisterComponent(Of TComponent As Class)(ByVal name As String, ByVal lifestyle As ComponentLifeStyleType) Implements IContainer.RegisterComponent
            RegisterComponent(name, GetType(TComponent), GetType(TComponent), lifestyle)
        End Sub

        Public Overridable Sub RegisterComponent(Of TContract, TComponent As Class)() Implements IContainer.RegisterComponent
            RegisterComponent(GetType(TContract), GetType(TComponent))
        End Sub

        Public Overridable Sub RegisterComponent(Of TContract, TComponent As Class)(ByVal name As String) Implements IContainer.RegisterComponent
            RegisterComponent(name, GetType(TContract), GetType(TComponent), ComponentLifeStyleType.Singleton)
        End Sub

        Public Overridable Sub RegisterComponent(Of TContract, TComponent As Class)(ByVal name As String, ByVal lifestyle As ComponentLifeStyleType) Implements IContainer.RegisterComponent
            RegisterComponent(name, GetType(TContract), GetType(TComponent), lifestyle)
        End Sub

#End Region

#Region "RegisterComponentSettings"

        Public MustOverride Sub RegisterComponentSettings(ByVal name As String, ByVal dependencies As System.Collections.IDictionary) Implements IContainer.RegisterComponentSettings

        Public Overridable Sub RegisterComponentSettings(ByVal component As System.Type, ByVal dependencies As System.Collections.IDictionary) Implements IContainer.RegisterComponentSettings
            RegisterComponentSettings(component.FullName, dependencies)
        End Sub

        Public Overridable Sub RegisterComponentSettings(Of TComponent)(ByVal dependencies As System.Collections.IDictionary) Implements IContainer.RegisterComponentSettings
            RegisterComponentSettings(GetType(TComponent).FullName, dependencies)
        End Sub

#End Region

#Region "RegisterComponentInstance"

        Public MustOverride Sub RegisterComponentInstance(ByVal name As String, ByVal contractType As System.Type, ByVal instance As Object) Implements IContainer.RegisterComponentInstance

        Public Sub RegisterComponentInstance(ByVal name As String, ByVal instance As Object) Implements IContainer.RegisterComponentInstance
            RegisterComponentInstance(name, instance.GetType(), instance)
        End Sub

        Public Sub RegisterComponentInstance(Of TContract)(ByVal instance As Object) Implements IContainer.RegisterComponentInstance
            RegisterComponentInstance(instance.GetType().FullName, GetType(TContract), instance)
        End Sub

        Public Sub RegisterComponentInstance(Of TContract)(ByVal name As String, ByVal instance As Object) Implements IContainer.RegisterComponentInstance
            RegisterComponentInstance(name, GetType(TContract), instance)
        End Sub

#End Region

    End Class

End Namespace
