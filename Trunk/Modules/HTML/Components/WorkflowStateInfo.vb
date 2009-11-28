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

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.Html
    ''' Project:    DotNetNuke
    ''' Class:      WorkflowStateInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Defines an instance of a WorkflowState object
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class WorkflowStateInfo

        ' local property declarations
        Private _PortalID As Integer
        Private _WorkflowID As Integer
        Private _WorkflowName As String
        Private _Description As String
        Private _IsDeleted As Boolean = False
        Private _StateID As Integer
        Private _StateName As String
        Private _Order As Integer
        Private _Notify As Boolean = False
        Private _IsActive As Boolean = True

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        Public Property WorkflowID() As Integer
            Get
                Return _WorkflowID
            End Get
            Set(ByVal Value As Integer)
                _WorkflowID = Value
            End Set
        End Property

        Public Property WorkflowName() As String
            Get
                Return _WorkflowName
            End Get
            Set(ByVal Value As String)
                _WorkflowName = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        Public Property IsDeleted() As Boolean
            Get
                Return _IsDeleted
            End Get
            Set(ByVal Value As Boolean)
                _IsDeleted = Value
            End Set
        End Property

        Public Property StateID() As Integer
            Get
                Return _StateID
            End Get
            Set(ByVal Value As Integer)
                _StateID = Value
            End Set
        End Property

        Public Property StateName() As String
            Get
                Return _StateName
            End Get
            Set(ByVal Value As String)
                _StateName = Value
            End Set
        End Property

        Public Property Order() As Integer
            Get
                Return _Order
            End Get
            Set(ByVal Value As Integer)
                _Order = Value
            End Set
        End Property

        Public Property Notify() As Boolean
            Get
                Return _Notify
            End Get
            Set(ByVal Value As Boolean)
                _Notify = Value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return _IsActive
            End Get
            Set(ByVal Value As Boolean)
                _IsActive = Value
            End Set
        End Property

    End Class

End Namespace

