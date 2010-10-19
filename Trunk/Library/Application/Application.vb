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
Imports System.Reflection

Namespace DotNetNuke.Application

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Application
    ''' Project:    DotNetNuke
    ''' Module:     Application
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Application class contains properties that describe the DotNetNuke Application.
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

        ''' <summary>
        ''' Gets the company to which the DotNetNuke application is related.
        ''' </summary>
        ''' <value>Fixed result: DotNetNuke Corporation</value>
        Public ReadOnly Property Company() As String
            Get
                Return "DotNetNuke Corporation"
            End Get
        End Property

        ''' <summary>
        ''' Gets the description of the application
        ''' </summary>
        ''' <value>Fixed result: DotNetNuke Community Edition</value>
        Public Overridable ReadOnly Property Description() As String
            Get
                Return "DotNetNuke Community Edition"
            End Get
        End Property

        ''' <summary>
        ''' Gets the help URL related to the DotNetNuke application
        ''' </summary>
        ''' <value>Fixed result: http://www.dotnetnuke.com/default.aspx?tabid=787 </value>
        Public ReadOnly Property HelpUrl() As String
            Get
                Return "http://www.dotnetnuke.com/default.aspx?tabid=787"
            End Get
        End Property

        ''' <summary>
        ''' Gets the legal copyright.
        ''' </summary>
        ''' <value>Dynamic: DotNetNuke® is copyright 2002-todays year by DotNetNuke Corporation"</value>
        Public ReadOnly Property LegalCopyright() As String
            Get
                Return "DotNetNuke® is copyright 2002-" + DateTime.Today.ToString("yyyy") + " by DotNetNuke Corporation"
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the application
        ''' </summary>
        ''' <value>Fixed result: DNNCORP.CE</value>
        Public Overridable ReadOnly Property Name() As String
            Get
                Return "DNNCORP.CE"
            End Get
        End Property

        ''' <summary>
        ''' Gets the SKU (Stock Keeping Unit)
        ''' </summary>
        ''' <value>Fixed result: DNN</value>
        Public Overridable ReadOnly Property SKU() As String
            Get
                Return "DNN"
            End Get
        End Property

        ''' <summary>
        ''' Gets the status of the DotnetNuke application
        ''' </summary>
        ''' <value>The status. This can be (enumeration)
        '''<code>Enum ReleaseMode
        '''    None
        '''    Alpha
        '''    Beta
        '''    RC
        '''   Stable
        ''' End Enum</code>
        ''' </value>
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

        ''' <summary>
        ''' Gets the title of the application
        ''' </summary>
        ''' <value>Fixed value "DotNetNuke".</value>
        Public ReadOnly Property Title() As String
            Get
                Return "DotNetNuke"
            End Get
        End Property

        ''' <summary>
        ''' Gets the trademark.
        ''' </summary>
        ''' <value>Fixed value: DotNetNuke,DNN</value>
        Public ReadOnly Property Trademark() As String
            Get
                Return "DotNetNuke,DNN"
            End Get
        End Property

        ''' <summary>
        ''' Gets the type of the application
        ''' </summary>
        ''' <value>Fixed value: Framework</value>
        Public ReadOnly Property Type() As String
            Get
                Return "Framework"
            End Get
        End Property

        ''' <summary>
        ''' Gets the upgrade URL.
        ''' </summary>
        ''' <value>Fixed value: http://update.dotnetnuke.com </value>
        Public ReadOnly Property UpgradeUrl() As String
            Get
                Return "http://update.dotnetnuke.com"
            End Get
        End Property

        ''' <summary>
        ''' Gets the URL of the application
        ''' </summary>
        ''' <value>Fixed value: http://www.dotnetnuke.com </value>
        Public ReadOnly Property Url() As String
            Get
                Return "http://www.dotnetnuke.com"
            End Get
        End Property

        ''' <summary>
        ''' Gets the version of the DotNetNuke framework/application
        ''' </summary>
        ''' <value>The version as retreieved from the Executing assembly.</value>
        Public ReadOnly Property Version() As System.Version
            Get
                Return System.Reflection.Assembly.GetExecutingAssembly().GetName.Version
            End Get
        End Property

#End Region

#Region "Public Functions"
        ''' <summary>
        ''' Determine whether a product specific change is to be applied
        ''' </summary>
        ''' <param name="productNames">list of product names</param>
        ''' <returns>true if product is within list of names</returns>
        ''' <remarks></remarks>
        Public Function ApplyToProduct(ByVal productNames As String) As Boolean
            Return productNames.Contains(Me.Name)
        End Function

#End Region
    End Class

End Namespace

