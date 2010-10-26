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
Imports DotNetNuke.UI.Skins.EventListeners
Imports DotNetNuke.UI.Containers.EventListeners

Namespace DotNetNuke.Application

    Public Class DotNetNukeContext

#Region "Private Members"

        Private _Application As Application
        Private _SkinEventListeners As List(Of SkinEventListener)
        Private _ContainerEventListeners As List(Of ContainerEventListener)

#End Region

#Region "Private Shared Members"

        Private Shared _Current As DotNetNukeContext

#End Region

#Region "Constructors"

        Protected Sub New()
            Me.New(New Application())
        End Sub

        Protected Sub New(ByVal application As Application)
            _Application = application
            _ContainerEventListeners = New List(Of ContainerEventListener)()
            _SkinEventListeners = New List(Of SkinEventListener)()
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property Application() As Application
            Get
                Return _Application
            End Get
        End Property

        Public ReadOnly Property ContainerEventListeners() As List(Of ContainerEventListener)
            Get
                Return _ContainerEventListeners
            End Get
        End Property

        Public ReadOnly Property SkinEventListeners() As List(Of SkinEventListener)
            Get
                Return _SkinEventListeners
            End Get
        End Property

#End Region

#Region "Public Shared Properties"

        Public Shared Property Current() As DotNetNukeContext
            Get
                If _Current Is Nothing Then
                    _Current = New DotNetNukeContext
                End If
                Return _Current
            End Get
            Set(ByVal value As DotNetNukeContext)
                _Current = value
            End Set
        End Property

#End Region

    End Class

End Namespace

