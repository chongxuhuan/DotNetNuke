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

Imports System.Collections.Specialized
Imports System.Text
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Security.Permissions
Imports System.Web
Imports System.Reflection
Imports System.Diagnostics
Imports System.Xml.Serialization
Imports DotNetNuke.Application


Namespace DotNetNuke.Services.Exceptions

    <Serializable()> Public Class BasePortalException
        Inherits Exception

        Private m_AssemblyVersion As String
        Private m_PortalID As Integer
        Private m_PortalName As String
        Private m_UserID As Integer
        Private m_UserName As String
        Private m_ActiveTabID As Integer
        Private m_ActiveTabName As String
        Private m_RawURL As String
        Private m_AbsoluteURL As String
        Private m_AbsoluteURLReferrer As String
        Private m_UserAgent As String
        Private m_DefaultDataProvider As String
        Private m_ExceptionGUID As String
        Private m_InnerExceptionString As String
        Private m_FileName As String
        Private m_FileLineNumber As Integer
        Private m_FileColumnNumber As Integer
        Private m_Method As String
        Private m_StackTrace As String
        Private m_Message As String
        Private m_Source As String

        ' default constructor
        Public Sub New()
            MyBase.New()
        End Sub

        'constructor with exception message
        Public Sub New(ByVal message As String)
            MyBase.New(message)
            InitializePrivateVariables()
        End Sub

        ' constructor with message and inner exception
        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
            InitializePrivateVariables()
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
            InitializePrivateVariables()
            m_AssemblyVersion = info.GetString("m_AssemblyVersion")
            m_PortalID = info.GetInt32("m_PortalID")
            m_PortalName = info.GetString("m_PortalName")
            m_UserID = info.GetInt32("m_UserID")
            m_UserName = info.GetString("m_Username")
            m_ActiveTabID = info.GetInt32("m_ActiveTabID")
            m_ActiveTabName = info.GetString("m_ActiveTabName")
            m_RawURL = info.GetString("m_RawURL")
            m_AbsoluteURL = info.GetString("m_AbsoluteURL")
            m_AbsoluteURLReferrer = info.GetString("m_AbsoluteURLReferrer")
            m_UserAgent = info.GetString("m_UserAgent")
            m_DefaultDataProvider = info.GetString("m_DefaultDataProvider")
            m_ExceptionGUID = info.GetString("m_ExceptionGUID")
            m_InnerExceptionString = info.GetString("m_InnerExceptionString")
            m_FileName = info.GetString("m_FileName")
            m_FileLineNumber = info.GetInt32("m_FileLineNumber")
            m_FileColumnNumber = info.GetInt32("m_FileColumnNumber")
            m_Method = info.GetString("m_Method")
            m_StackTrace = info.GetString("m_StackTrace")
            m_Message = info.GetString("m_Message")
            m_Source = info.GetString("m_Source")
        End Sub

        Private Sub InitializePrivateVariables()
            'Try and get the Portal settings from context
            'If an error occurs getting the context then set the variables to -1
            Try
                Dim _context As HttpContext = HttpContext.Current
                Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Dim _objInnermostException As Exception
                _objInnermostException = New Exception(Me.Message, Me)
                Do Until _objInnermostException.InnerException Is Nothing
                    _objInnermostException = _objInnermostException.InnerException
                Loop
                Dim _exceptionInfo As ExceptionInfo = GetExceptionInfo(_objInnermostException)

                Try
                    m_AssemblyVersion = DotNetNukeContext.Current.Application.Version.ToString(3)
                Catch
                    m_AssemblyVersion = "-1"
                End Try

                Try
                    m_PortalID = _portalSettings.PortalId
                    m_PortalName = _portalSettings.PortalName
                Catch
                    m_PortalID = -1
                    m_PortalName = ""
                End Try

                Try
                    Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                    m_UserID = objUserInfo.UserID
                Catch
                    m_UserID = -1
                End Try

                Try
                    If m_UserID <> -1 Then
                        Dim objUserInfo As UserInfo = UserController.GetUserById(m_PortalID, m_UserID)
                        If Not objUserInfo Is Nothing Then
                            m_UserName = objUserInfo.Username
                        Else
                            m_UserName = ""
                        End If

                    Else
                        m_UserName = ""
                    End If
                Catch
                    m_UserName = ""
                End Try

                Try
                    m_ActiveTabID = _portalSettings.ActiveTab.TabID
                Catch ex As Exception
                    m_ActiveTabID = -1
                End Try

                Try
                    m_ActiveTabName = _portalSettings.ActiveTab.TabName
                Catch ex As Exception
                    m_ActiveTabName = ""
                End Try

                Try
                    m_RawURL = _context.Request.RawUrl
                Catch
                    m_RawURL = ""
                End Try

                Try
                    m_AbsoluteURL = _context.Request.Url.AbsolutePath
                Catch
                    m_AbsoluteURL = ""
                End Try

                Try
                    m_AbsoluteURLReferrer = _context.Request.UrlReferrer.AbsoluteUri
                Catch
                    m_AbsoluteURLReferrer = ""
                End Try

                Try
                    m_UserAgent = _context.Request.UserAgent
                Catch ex As Exception
                    m_UserAgent = ""
                End Try

                Try
                    Dim objProviderConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data")
                    Dim strTypeName As String = CType(objProviderConfiguration.Providers(objProviderConfiguration.DefaultProvider), Framework.Providers.Provider).Type
                    m_DefaultDataProvider = strTypeName
                Catch ex As Exception
                    m_DefaultDataProvider = ""
                End Try

                Try
                    m_ExceptionGUID = Guid.NewGuid.ToString
                Catch ex As Exception
                    m_ExceptionGUID = ""
                End Try

                Try
                    m_FileName = _exceptionInfo.FileName
                Catch
                    m_FileName = ""
                End Try

                Try
                    m_FileLineNumber = _exceptionInfo.FileLineNumber
                Catch
                    m_FileLineNumber = -1
                End Try

                Try
                    m_FileColumnNumber = _exceptionInfo.FileColumnNumber
                Catch
                    m_FileColumnNumber = -1
                End Try

                Try
                    m_Method = _exceptionInfo.Method
                Catch
                    m_Method = ""
                End Try

                Try
                    m_StackTrace = Me.StackTrace
                Catch ex As Exception
                    m_StackTrace = ""
                End Try

                Try
                    m_Message = Me.Message
                Catch ex As Exception
                    m_Message = ""
                End Try

                Try
                    m_Source = Me.Source
                Catch ex As Exception
                    m_Source = ""
                End Try

            Catch exc As Exception
                m_PortalID = -1
                m_UserID = -1
                m_AssemblyVersion = "-1"
                m_ActiveTabID = -1
                m_ActiveTabName = ""
                m_RawURL = ""
                m_AbsoluteURL = ""
                m_AbsoluteURLReferrer = ""
                m_UserAgent = ""
                m_DefaultDataProvider = ""
                m_ExceptionGUID = ""
                m_FileName = ""
                m_FileLineNumber = -1
                m_FileColumnNumber = -1
                m_Method = ""
                m_StackTrace = ""
                m_Message = ""
                m_Source = ""
            End Try
        End Sub

        <SecurityPermission(SecurityAction.Demand, SerializationFormatter:=True)> _
        Public Overrides Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            ' Serialize this class' state and then call the base class GetObjectData
            info.AddValue("m_AssemblyVersion", m_AssemblyVersion, GetType(String))
            info.AddValue("m_PortalID", m_PortalID, GetType(Int32))
            info.AddValue("m_PortalName", m_PortalName, GetType(String))
            info.AddValue("m_UserID", m_UserID, GetType(Int32))
            info.AddValue("m_UserName", m_UserName, GetType(String))
            info.AddValue("m_ActiveTabID", m_ActiveTabID, GetType(Int32))
            info.AddValue("m_ActiveTabName", m_ActiveTabName, GetType(String))
            info.AddValue("m_RawURL", m_RawURL, GetType(String))
            info.AddValue("m_AbsoluteURL", m_AbsoluteURL, GetType(String))
            info.AddValue("m_AbsoluteURLReferrer", m_AbsoluteURLReferrer, GetType(String))
            info.AddValue("m_UserAgent", m_UserAgent, GetType(String))
            info.AddValue("m_DefaultDataProvider", m_DefaultDataProvider, GetType(String))
            info.AddValue("m_ExceptionGUID", m_ExceptionGUID, GetType(String))
            info.AddValue("m_FileName", m_FileName, GetType(String))
            info.AddValue("m_FileLineNumber", m_FileLineNumber, GetType(Int32))
            info.AddValue("m_FileColumnNumber", m_FileColumnNumber, GetType(Int32))
            info.AddValue("m_Method", m_Method, GetType(String))
            info.AddValue("m_StackTrace", m_StackTrace, GetType(String))
            info.AddValue("m_Message", m_Message, GetType(String))
            info.AddValue("m_Source", m_Source, GetType(String))

            MyBase.GetObjectData(info, context)
        End Sub

        Public ReadOnly Property AssemblyVersion() As String
            Get
                Return m_AssemblyVersion
            End Get
        End Property

        Public ReadOnly Property PortalID() As Integer
            Get
                Return m_PortalID
            End Get
        End Property

        Public ReadOnly Property PortalName() As String
            Get
                Return m_PortalName
            End Get
        End Property

        Public ReadOnly Property UserID() As Integer
            Get
                Return m_UserID
            End Get
        End Property

        Public ReadOnly Property UserName() As String
            Get
                Return m_UserName
            End Get
        End Property

        Public ReadOnly Property ActiveTabID() As Integer
            Get
                Return m_ActiveTabID
            End Get
        End Property

        Public ReadOnly Property ActiveTabName() As String
            Get
                Return m_ActiveTabName
            End Get
        End Property

        Public ReadOnly Property RawURL() As String
            Get
                Return m_RawURL
            End Get
        End Property

        Public ReadOnly Property AbsoluteURL() As String
            Get
                Return m_AbsoluteURL
            End Get
        End Property

        Public ReadOnly Property AbsoluteURLReferrer() As String
            Get
                Return m_AbsoluteURLReferrer
            End Get
        End Property

        Public ReadOnly Property UserAgent() As String
            Get
                Return m_UserAgent
            End Get
        End Property

        Public ReadOnly Property DefaultDataProvider() As String
            Get
                Return m_DefaultDataProvider
            End Get
        End Property

        Public ReadOnly Property ExceptionGUID() As String
            Get
                Return m_ExceptionGUID
            End Get
        End Property

        Public ReadOnly Property FileName() As String
            Get
                Return m_FileName
            End Get
        End Property
        Public ReadOnly Property FileLineNumber() As Integer
            Get
                Return m_FileLineNumber
            End Get
        End Property
        Public ReadOnly Property FileColumnNumber() As Integer
            Get
                Return m_FileColumnNumber
            End Get
        End Property

        Public ReadOnly Property Method() As String
            Get
                Return m_Method
            End Get
        End Property

        <XmlIgnore()> Public Shadows ReadOnly Property TargetSite() As MethodBase
            Get
                Return MyBase.TargetSite()
            End Get
        End Property

    End Class
End Namespace
