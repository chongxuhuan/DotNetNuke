' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2010 
'' by DotNetNuke Corporation 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
'' 
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software. 
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE. 
' 


Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Modules.Dashboard.Components

    <Serializable()> _
    Public Class DashboardControl
        Implements IComparable

#Region "Private Members"

        Private _ControllerClass As String
        Private _DashboardControlID As Integer
        Private _DashboardControlKey As String
        Private _DashboardControlLocalResources As String
        Private _DashboardControlSrc As String
        Private _IsDirty As Boolean
        Private _IsEnabled As Boolean
        Private _PackageID As Integer
        Private _ViewOrder As Integer

#End Region

#Region "Public Properties"

        Public Property ControllerClass() As String
            Get
                Return _ControllerClass
            End Get
            Set(ByVal value As String)
                _ControllerClass = value
            End Set
        End Property

        Public Property DashboardControlID() As Integer
            Get
                Return _DashboardControlID
            End Get
            Set(ByVal value As Integer)
                _DashboardControlID = value
            End Set
        End Property

        Public Property DashboardControlKey() As String
            Get
                Return _DashboardControlKey
            End Get
            Set(ByVal value As String)
                _DashboardControlKey = value
            End Set
        End Property

        Public Property DashboardControlLocalResources() As String
            Get
                Return _DashboardControlLocalResources
            End Get
            Set(ByVal value As String)
                _DashboardControlLocalResources = value
            End Set
        End Property

        Public Property DashboardControlSrc() As String
            Get
                Return _DashboardControlSrc
            End Get
            Set(ByVal value As String)
                _DashboardControlSrc = value
            End Set
        End Property

        Public ReadOnly Property IsDirty() As Boolean
            Get
                Return _IsDirty
            End Get
        End Property

        Public Property IsEnabled() As Boolean
            Get
                Return _IsEnabled
            End Get
            Set(ByVal value As Boolean)
                If _IsEnabled <> value Then _IsDirty = True
                _IsEnabled = value
            End Set
        End Property

        Public ReadOnly Property LocalizedTitle() As String
            Get
                Return Localization.GetString(DashboardControlKey & ".Title", "~/" & DashboardControlLocalResources)
            End Get
        End Property

        Public Property PackageID() As Integer
            Get
                Return _PackageID
            End Get
            Set(ByVal value As Integer)
                _PackageID = value
            End Set
        End Property

        Public Property ViewOrder() As Integer
            Get
                Return _ViewOrder
            End Get
            Set(ByVal value As Integer)
                If _ViewOrder <> value Then _IsDirty = True
                _ViewOrder = value
            End Set
        End Property

#End Region

#Region "IComparable Implementation"

        Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
            Dim dashboardControl As DashboardControl = TryCast(obj, DashboardControl)
            If dashboardControl Is Nothing Then
                Throw New ArgumentException("object is not a DashboardControl")
            End If

            Return Me.ViewOrder.CompareTo(dashboardControl.ViewOrder)
        End Function

#End Region

    End Class

End Namespace