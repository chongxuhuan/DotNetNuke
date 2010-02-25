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

Imports System.Runtime.Serialization
Imports System.Security.Permissions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports System.Xml.Serialization

Namespace DotNetNuke.Services.Exceptions
    <Serializable()> Public Class SecurityException
        Inherits BasePortalException

        Private m_IP As string
        Private m_Querystring As string

        ' default constructor
        Public Sub New()
            MyBase.New()
        End Sub

        'constructor with exception message
        Public Sub New(ByVal message As String)
            MyBase.New(message)
            InitilizePrivateVariables()
        End Sub

        ' constructor with message and inner exception
        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
            InitilizePrivateVariables()
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
            InitilizePrivateVariables()
            m_IP = info.GetString("m_IP")
            m_Querystring = info.GetString("m_Querystring")
        End Sub

        Private Sub InitilizePrivateVariables()
            'Try and get the Portal settings from httpcontext
            Try
                If Not HttpContext.Current.Request.UserHostAddress Is Nothing Then
                    m_IP = HttpContext.Current.Request.UserHostAddress
                End If
                m_Querystring = HttpContext.Current.Request.MapPath(Querystring, HttpContext.Current.Request.ApplicationPath, False)
            Catch exc As Exception
                m_IP = ""
                m_Querystring = ""
            End Try
        End Sub

        <SecurityPermission(SecurityAction.Demand, SerializationFormatter:=True)> _
        Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            ' Serialize this class' state and then call the base class GetObjectData
            'info.AddValue("m_IP", m_IP, GetType(String))
            'info.AddValue("m_Querystring", m_Querystring, GetType(String))
            info.AddValue("m_IP", m_IP, GetType(String))
            info.AddValue("m_Querystring", m_Querystring, GetType(String))
            MyBase.GetObjectData(info, context)
        End Sub


        <XmlElement("IP")> Public ReadOnly Property IP() As String
            Get
                Return m_IP
            End Get
        End Property

        <XmlElement("Querystring")> Public ReadOnly Property Querystring() As String
            Get
                Return m_Querystring
            End Get
        End Property

    End Class

End Namespace