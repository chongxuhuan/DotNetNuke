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

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.Html
    ''' Project:    DotNetNuke
    ''' Class:      HtmlTextUserInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Defines an instance of an HtmlTextUser object
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class HtmlTextUserInfo

        ' local property declarations
        Private _ItemID As Integer
        Private _StateID As Integer
        Private _StateName As String
        Private _ModuleID As Integer
        Private _ModuleTitle As String
        Private _TabID As Integer
        Private _UserID As Integer
        Private _CreatedOnDate As Date

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property ItemID() As Integer
            Get
                Return _ItemID
            End Get
            Set(ByVal Value As Integer)
                _ItemID = Value
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

        Public Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal Value As Integer)
                _ModuleID = Value
            End Set
        End Property

        Public Property ModuleTitle() As String
            Get
                Return _ModuleTitle
            End Get
            Set(ByVal Value As String)
                _ModuleTitle = Value
            End Set
        End Property

        Public Property TabID() As Integer
            Get
                Return _TabID
            End Get
            Set(ByVal Value As Integer)
                _TabID = Value
            End Set
        End Property

        Public Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal Value As Integer)
                _UserID = Value
            End Set
        End Property

        Public Property CreatedOnDate() As Date
            Get
                Return _CreatedOnDate
            End Get
            Set(ByVal Value As Date)
                _CreatedOnDate = Value
            End Set
        End Property

    End Class

End Namespace

