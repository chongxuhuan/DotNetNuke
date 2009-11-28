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

Namespace DotNetNuke.HttpModules.Config
    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.HttpModules.Analytics
    ''' Project:    HttpModules
    ''' Module:     AnalyticsEngine
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class definition for an AnalyticsEngine item
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cniknet]	05/03/2009	created
    ''' </history>
    ''' -----------------------------------------------------------------------------

    <Serializable()> _
    Public Class AnalyticsEngine

        Private _engineType As String
        Private _scriptTemplate As String
        Private _elementId As String
        Private _injectTop As Boolean

        Public Property EngineType() As String
            Get
                Return _engineType
            End Get
            Set(ByVal Value As String)
                _engineType = Value
            End Set
        End Property

        Public Property ScriptTemplate() As String
            Get
                Return _scriptTemplate
            End Get
            Set(ByVal Value As String)
                _scriptTemplate = Value
            End Set
        End Property

        Public Property ElementId() As String
            Get
                Return _elementId
            End Get
            Set(ByVal Value As String)
                _elementId = Value
            End Set
        End Property

        Public Property InjectTop() As Boolean
            Get
                Return _injectTop
            End Get
            Set(ByVal Value As Boolean)
                _injectTop = Value
            End Set
        End Property

    End Class

End Namespace

