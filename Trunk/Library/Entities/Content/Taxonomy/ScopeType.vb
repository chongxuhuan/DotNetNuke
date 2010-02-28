'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Entities.Content.Taxonomy

    <Serializable()> Public Class ScopeType
        Implements IHydratable

#Region "Private Members"

        Private _ScopeTypeId As Integer
        Private _ScopeType As String

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(Null.NullString)
        End Sub

        Public Sub New(ByVal scopeType As String)
            _ScopeTypeId = Null.NullInteger
            _ScopeType = scopeType
        End Sub

#End Region

#Region "Public Properties"

        Public Property ScopeTypeId() As Integer
            Get
                Return _ScopeTypeId
            End Get
            Set(ByVal value As Integer)
                _ScopeTypeId = value
            End Set
        End Property

        Public Property ScopeType() As String
            Get
                Return _ScopeType
            End Get
            Set(ByVal value As String)
                _ScopeType = value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            ScopeTypeId = Null.SetNullInteger(dr("ScopeTypeID"))
            ScopeType = Null.SetNullString(dr("ScopeType"))
        End Sub

        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return ScopeTypeId
            End Get
            Set(ByVal value As Integer)
                ScopeTypeId = value
            End Set
        End Property

#End Region

        Public Overrides Function ToString() As String
            Return ScopeType
        End Function

    End Class

End Namespace

