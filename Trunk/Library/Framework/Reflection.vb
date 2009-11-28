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

Imports System
Imports System.Configuration
Imports System.Data
Imports DotNetNuke.Framework.Providers
Imports System.Web.Compilation

Namespace DotNetNuke.Framework

    ''' -----------------------------------------------------------------------------
    ''' Namespace: DotNetNuke.Framework
    ''' Project	 : DotNetNuke
    ''' Class	 : Reflection
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Library responsible for reflection
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Nik Kalyani]	10/15/2004	Replaced brackets in parameter names
    ''' 	[cnurse]	    10/13/2005	Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Reflection

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload for creating an object from a Provider configured in web.config</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal ObjectProviderType As String) As Object
            Return CreateObject(ObjectProviderType, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        ''' <param name="UseCache">Caching switch</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload for creating an object from a Provider configured in web.config</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal ObjectProviderType As String, ByVal UseCache As Boolean) As Object
            Return CreateObject(ObjectProviderType, "", "", "", UseCache)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        ''' <param name="ObjectNamespace">The namespace of the object to create.</param>
        ''' <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload for creating an object from a Provider including NameSpace and 
        ''' AssemblyName ( this allows derived providers to share the same config )</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal ObjectProviderType As String, ByVal ObjectNamespace As String, ByVal ObjectAssemblyName As String) As Object
            Return CreateObject(ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        ''' <param name="ObjectNamespace">The namespace of the object to create.</param>
        ''' <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        ''' <param name="UseCache">Caching switch</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload for creating an object from a Provider including NameSpace and 
        ''' AssemblyName ( this allows derived providers to share the same config )</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal ObjectProviderType As String, ByVal ObjectNamespace As String, ByVal ObjectAssemblyName As String, ByVal UseCache As Boolean) As Object
            Return CreateObject(ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, UseCache)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        ''' <param name="ObjectProviderName">The name of the Provider</param>
        ''' <param name="ObjectNamespace">The namespace of the object to create.</param>
        ''' <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload for creating an object from a Provider including NameSpace, 
        ''' AssemblyName and ProviderName</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal ObjectProviderType As String, ByVal ObjectProviderName As String, ByVal ObjectNamespace As String, ByVal ObjectAssemblyName As String) As Object
            Return CreateObject(ObjectProviderType, ObjectProviderName, ObjectNamespace, ObjectAssemblyName, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        ''' <param name="ObjectProviderName">The name of the Provider</param>
        ''' <param name="ObjectNamespace">The namespace of the object to create.</param>
        ''' <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        ''' <param name="UseCache">Caching switch</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload for creating an object from a Provider including NameSpace, 
        ''' AssemblyName and ProviderName</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal ObjectProviderType As String, ByVal ObjectProviderName As String, ByVal ObjectNamespace As String, ByVal ObjectAssemblyName As String, ByVal UseCache As Boolean) As Object

            Dim TypeName As String = ""

            ' get the provider configuration based on the type
            Dim objProviderConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ObjectProviderType)

            ' if both the Namespace and AssemblyName are provided then we will construct an "assembly qualified typename" - ie. "NameSpace.ClassName, AssemblyName" 
            If ObjectNamespace <> "" AndAlso ObjectAssemblyName <> "" Then
                If ObjectProviderName = "" Then
                    ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                    TypeName = ObjectNamespace & "." & objProviderConfiguration.DefaultProvider & ", " & ObjectAssemblyName & "." & objProviderConfiguration.DefaultProvider
                Else
                    ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                    TypeName = ObjectNamespace & "." & ObjectProviderName & ", " & ObjectAssemblyName & "." & ObjectProviderName
                End If
            Else
                ' if only the Namespace is provided then we will construct an "full typename" - ie. "NameSpace.ClassName" 
                If ObjectNamespace <> "" Then
                    If ObjectProviderName = "" Then
                        ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                        TypeName = ObjectNamespace & "." & objProviderConfiguration.DefaultProvider
                    Else
                        ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                        TypeName = ObjectNamespace & "." & ObjectProviderName
                    End If
                Else
                    ' if neither Namespace or AssemblyName are provided then we will get the typename from the default provider 
                    If ObjectProviderName = "" Then
                        ' get the typename of the default Provider from web.config
                        TypeName = CType(objProviderConfiguration.Providers(objProviderConfiguration.DefaultProvider), Provider).Type
                    Else
                        ' get the typename of the specified ProviderName from web.config 
                        TypeName = CType(objProviderConfiguration.Providers(ObjectProviderName), Provider).Type
                    End If
                End If
            End If

            Return CreateObject(TypeName, TypeName, UseCache)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="TypeName">The fully qualified TypeName</param>
        ''' <param name="CacheKey">The Cache Key</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload that takes a fully-qualified typename and a Cache Key</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal TypeName As String, ByVal CacheKey As String) As Object

            Return CreateObject(TypeName, CacheKey, True)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="TypeName">The fully qualified TypeName</param>
        ''' <param name="CacheKey">The Cache Key</param>
        ''' <param name="UseCache">Caching switch</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload that takes a fully-qualified typename and a Cache Key</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal TypeName As String, ByVal CacheKey As String, ByVal UseCache As Boolean) As Object

            ' dynamically create the object
            Return Activator.CreateInstance(CreateType(TypeName, CacheKey, UseCache))

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <typeparam name="T">The type of object to create</typeparam>
        ''' <returns></returns>
        ''' <remarks>Generic version</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(Of T)() As T

            ' dynamically create the object
            Return Activator.CreateInstance(Of T)()

        End Function

        Public Shared Function CreateType(ByVal TypeName As String) As Type
            Return CreateType(TypeName, "", True, False)
        End Function

        Public Shared Function CreateType(ByVal TypeName As String, ByVal IgnoreErrors As Boolean) As Type
            Return CreateType(TypeName, "", True, IgnoreErrors)
        End Function

        Public Shared Function CreateType(ByVal TypeName As String, ByVal CacheKey As String, ByVal UseCache As Boolean) As Type
            Return CreateType(TypeName, CacheKey, UseCache, False)
        End Function

        Public Shared Function CreateType(ByVal TypeName As String, ByVal CacheKey As String, ByVal UseCache As Boolean, ByVal IgnoreErrors As Boolean) As Type

            If CacheKey = "" Then
                CacheKey = TypeName
            End If

            Dim objType As Type = Nothing

            ' use the cache for performance
            If UseCache Then
                objType = CType(DataCache.GetCache(CacheKey), Type)
            End If

            ' is the type in the cache?
            If objType Is Nothing Then
                Try
                    ' use reflection to get the type of the class
                    objType = BuildManager.GetType(TypeName, True, True)

                    If UseCache Then
                        ' insert the type into the cache
                        DataCache.SetCache(CacheKey, objType)
                    End If
                Catch exc As Exception
                    ' could not load the type
                    If Not IgnoreErrors Then
                        LogException(exc)
                    End If
                End Try
            End If

            Return objType
        End Function

        Public Shared Function CreateInstance(ByVal Type As Type) As Object
            If Not Type Is Nothing Then
                Return Type.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, Nothing, Nothing, Nothing, Nothing)
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function GetProperty(ByVal Type As Type, ByVal PropertyName As String, ByVal Target As Object) As Object
            If Not Type Is Nothing Then
                Return Type.InvokeMember(PropertyName, System.Reflection.BindingFlags.GetProperty, Nothing, Target, Nothing)
            Else
                Return Nothing
            End If
        End Function

        Public Shared Sub SetProperty(ByVal Type As Type, ByVal PropertyName As String, ByVal Target As Object, ByVal Args() As Object)
            If Not Type Is Nothing Then
                Type.InvokeMember(PropertyName, System.Reflection.BindingFlags.SetProperty, Nothing, Target, Args)
            End If
        End Sub

        Public Shared Sub InvokeMethod(ByVal Type As Type, ByVal PropertyName As String, ByVal Target As Object, ByVal Args() As Object)
            If Not Type Is Nothing Then
                Type.InvokeMember(PropertyName, System.Reflection.BindingFlags.InvokeMethod, Nothing, Target, Args)
            End If
        End Sub

#End Region

#Region "Deprecated Methods"

        ' dynamically create a default Provider from a ProviderType - this method was used by the CachingProvider to avoid a circular dependency
        <Obsolete("This method has been deprecated. Please use CreateObject(ByVal ObjectProviderType As String, ByVal UseCache As Boolean) As Object")> _
        Friend Shared Function CreateObjectNotCached(ByVal ObjectProviderType As String) As Object

            Dim TypeName As String = ""
            Dim objType As Type = Nothing

            ' get the provider configuration based on the type
            Dim objProviderConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ObjectProviderType)

            ' get the typename of the Base DataProvider from web.config
            TypeName = CType(objProviderConfiguration.Providers(objProviderConfiguration.DefaultProvider), Provider).Type

            Try
                ' use reflection to get the type of the class
                objType = BuildManager.GetType(TypeName, True, True)

            Catch exc As Exception

                ' could not load the type
                LogException(exc)

            End Try

            ' dynamically create the object
            Return Activator.CreateInstance(objType)

        End Function

#End Region

    End Class

End Namespace