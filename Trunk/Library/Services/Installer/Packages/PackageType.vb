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

Namespace DotNetNuke.Services.Installer.Packages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageType class represents a single Installer Package Type
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	09/04/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PackageType

#Region "Private Members"

        Private _PackageType As String = Null.NullString
        Private _Description As String
        Private _EditorControlSrc As String
        Private _SecurityAccessLevel As SecurityAccessLevel

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallPackage instance
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Description of this package type
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	09/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the EditorControlSrc of this package type
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/04/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditorControlSrc() As String
            Get
                Return _EditorControlSrc
            End Get
            Set(ByVal value As String)
                _EditorControlSrc = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Name of this package type
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	09/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PackageType() As String
            Get
                Return _PackageType
            End Get
            Set(ByVal value As String)
                _PackageType = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the security Access Level required to install this package type
        ''' </summary>
        ''' <value>A SecurityAccessLevel enumeration</value>
        ''' <history>
        ''' 	[cnurse]	09/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SecurityAccessLevel() As SecurityAccessLevel
            Get
                Return _SecurityAccessLevel
            End Get
            Set(ByVal value As SecurityAccessLevel)
                _SecurityAccessLevel = value
            End Set
        End Property

#End Region

    End Class

End Namespace
