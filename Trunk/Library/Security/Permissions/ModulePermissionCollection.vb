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
Imports DotNetNuke
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : ModulePermissionCollection
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModulePermissionCollection provides the a custom collection for ModulePermissionInfo
    ''' objects
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> _
    Public Class ModulePermissionCollection
        Inherits CollectionBase

#Region "Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal modulePermissions As ArrayList)
            AddRange(modulePermissions)
        End Sub

        Public Sub New(ByVal modulePermissions As ModulePermissionCollection)
            AddRange(modulePermissions)
        End Sub

        Public Sub New(ByVal modulePermissions As ArrayList, ByVal ModuleID As Integer)
            For Each permission As ModulePermissionInfo In modulePermissions
                If permission.ModuleID = ModuleID Then
                    Add(permission)
                End If
            Next
        End Sub

        Public Sub New(ByVal objModule As ModuleInfo)
            For Each permission As ModulePermissionInfo In objModule.ModulePermissions
                If permission.ModuleID = objModule.ModuleID Then
                    Add(permission)
                End If
            Next
        End Sub

#End Region

#Region "Public Properties"

        Default Public Property Item(ByVal index As Integer) As ModulePermissionInfo
            Get
                Return CType(List(index), ModulePermissionInfo)
            End Get
            Set(ByVal Value As ModulePermissionInfo)
                List(index) = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Function Add(ByVal value As ModulePermissionInfo) As Integer
            Return List.Add(value)
        End Function

        Public Function Add(ByVal value As ModulePermissionInfo, ByVal checkForDuplicates As Boolean) As Integer
            If Not checkForDuplicates Then
                Add(value)
            Else
                Dim isMatch As Boolean = False
                For Each permission As PermissionInfoBase In Me.List
                    If permission.PermissionID = value.PermissionID AndAlso permission.UserID = value.UserID AndAlso permission.RoleID = value.RoleID Then
                        isMatch = True
                        Exit For
                    End If
                Next
                If Not isMatch Then
                    Add(value)
                End If
            End If
        End Function

        Public Sub AddRange(ByVal modulePermissions As ArrayList)
            For Each permission As ModulePermissionInfo In modulePermissions
                Add(permission)
            Next
        End Sub

        Public Sub AddRange(ByVal modulePermissions As ModulePermissionCollection)
            For Each permission As ModulePermissionInfo In modulePermissions
                Add(permission)
            Next
        End Sub

        Public Function CompareTo(ByVal objModulePermissionCollection As ModulePermissionCollection) As Boolean
            If objModulePermissionCollection.Count <> Me.Count Then
                Return False
            End If
            InnerList.Sort(New CompareModulePermissions)
            objModulePermissionCollection.InnerList.Sort(New CompareModulePermissions)

            For i As Integer = 0 To Me.Count - 1
                If objModulePermissionCollection(i).ModulePermissionID <> Me(i).ModulePermissionID _
                OrElse objModulePermissionCollection(i).AllowAccess <> Me(i).AllowAccess Then
                    Return False
                End If
            Next
            Return True
        End Function

        Public Function Contains(ByVal value As ModulePermissionInfo) As Boolean
            Return List.Contains(value)
        End Function

        Public Function IndexOf(ByVal value As ModulePermissionInfo) As Integer
            Return List.IndexOf(value)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal value As ModulePermissionInfo)
            List.Insert(index, value)
        End Sub

        Public Sub Remove(ByVal value As ModulePermissionInfo)
            List.Remove(value)
        End Sub

        Public Sub Remove(ByVal permissionID As Integer, ByVal roleID As Integer, ByVal userID As Integer)
            For Each permission As PermissionInfoBase In Me.List
                If permission.PermissionID = permissionID AndAlso permission.UserID = userID AndAlso permission.RoleID = roleID Then
                    List.Remove(permission)
                    Exit For
                End If
            Next
        End Sub

        Public Function ToList() As List(Of PermissionInfoBase)
            Dim list As New List(Of PermissionInfoBase)

            For Each permission As PermissionInfoBase In Me.List
                list.Add(permission)
            Next
            Return list
        End Function

        Public Overloads Function ToString(ByVal key As String) As String
            Return PermissionController.BuildPermissions(List, key)
        End Function

#End Region

    End Class

End Namespace
