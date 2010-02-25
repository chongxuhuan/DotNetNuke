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
Imports System
Imports System.Collections

Namespace DotNetNuke.HttpModules.Config
    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.HttpModules.Analytics
    ''' Project:    HttpModules
    ''' Module:     AnalyticsEngineCollection
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class definition for a collection of AnalyticsEngine items
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cniknet]	05/03/2009	created
    ''' </history>
    ''' -----------------------------------------------------------------------------

    <Serializable()> _
    Public Class AnalyticsEngineCollection

        Inherits CollectionBase

        Public Sub Add(ByVal a As AnalyticsEngine)

            Me.InnerList.Add(a)

        End Sub

        Default Public Overridable Property Item(ByVal index As Integer) As AnalyticsEngine
            Get
                Return DirectCast(MyBase.List.Item(index), AnalyticsEngine)
            End Get
            Set(ByVal Value As AnalyticsEngine)
                MyBase.List.Item(index) = Value
            End Set
        End Property

    End Class

End Namespace
