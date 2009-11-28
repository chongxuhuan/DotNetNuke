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
Imports System.Reflection

Namespace DotNetNuke.Application

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Application
    ''' Project:    DotNetNuke
    ''' Module:     Application
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Application class contains properties that describe the Application.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	09/10/2009  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Application

#Region "Private Members"

        Private Shared _status As ReleaseMode = ReleaseMode.None

#End Region

#Region "Constructors"

        Protected Friend Sub New()

        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property Company() As String
            Get
                Return "DotNetNuke Corporation"
            End Get
        End Property

        Public Overridable ReadOnly Property Description() As String
            Get
                Return "DotNetNuke Community Edition"
            End Get
        End Property

        Public ReadOnly Property HelpUrl() As String
            Get
                Return "http://www.dotnetnuke.com/default.aspx?tabid=787"
            End Get
        End Property

        Public ReadOnly Property LegalCopyright() As String
            Get
                Return "DotNetNuke® is copyright 2002-" + DateTime.Today.ToString("yyyy") + " by DotNetNuke Corporation"
            End Get
        End Property

        Public Overridable ReadOnly Property Name() As String
            Get
                Return "DNNCORP.CE"
            End Get
        End Property

        Public Overridable ReadOnly Property SKU() As String
            Get
                Return "DNN"
            End Get
        End Property

        Public ReadOnly Property Status() As ReleaseMode
            Get
                If _status = ReleaseMode.None Then
                    Dim assy As Assembly = System.Reflection.Assembly.GetExecutingAssembly
                    If Attribute.IsDefined(assy, GetType(AssemblyStatusAttribute)) Then
                        Dim attr As Attribute = Attribute.GetCustomAttribute(assy, GetType(AssemblyStatusAttribute))
                        If attr IsNot Nothing Then
                            _status = CType(attr, AssemblyStatusAttribute).Status
                        End If
                    End If
                End If

                Return _status
            End Get
        End Property

        Public ReadOnly Property Title() As String
            Get
                Return "DotNetNuke"
            End Get
        End Property

        Public ReadOnly Property Trademark() As String
            Get
                Return "DotNetNuke,DNN"
            End Get
        End Property

        Public ReadOnly Property Type() As String
            Get
                Return "Framework"
            End Get
        End Property

        Public ReadOnly Property UpgradeUrl() As String
            Get
                Return "http://update.dotnetnuke.com"
            End Get
        End Property

        Public ReadOnly Property Url() As String
            Get
                Return "http://www.dotnetnuke.com"
            End Get
        End Property

        Public ReadOnly Property Version() As System.Version
            Get
                Return System.Reflection.Assembly.GetExecutingAssembly().GetName.Version
            End Get
        End Property

#End Region

    End Class

End Namespace

