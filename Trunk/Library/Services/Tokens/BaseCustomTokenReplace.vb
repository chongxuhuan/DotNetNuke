'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009 by DotNetNuke Corp. 
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

Imports DotNetNuke



Namespace DotNetNuke.Services.Tokens

#Region " Type Definitions "
    ''' <summary>
    ''' Scope informs the property access classes about the planned usage of the token
    ''' </summary>
    ''' <remarks>
    ''' The result of a token replace operation depends on the current context, privacy settings 
    ''' and the current scope. The scope should be the lowest scope needed for the current purpose.
    ''' The property access classes should evaluate and use the scope before returning a value.
    ''' </remarks>
    Public Enum Scope
        ''' <summary>
        ''' Only access to Date and Time
        ''' </summary>
        NoSettings = 0

        ''' <summary>
        ''' Tokens for Host, Portal, Tab (, Module), user name
        ''' </summary>
        Configuration = 1

        ''' <summary>
        ''' Configuration, Current User data and user data allowed for registered members
        ''' </summary>
        DefaultSettings = 2

        ''' <summary>
        ''' System notifications to users and adminstrators
        ''' </summary>
        SystemMessages = 3

        ''' <summary>
        ''' internal debugging, error messages, logs
        ''' </summary>
        Debug = 4
    End Enum

    ''' <summary>
    ''' CacheLevel is used to specify the cachability of a string, determined as minimum of the used token cachability
    ''' </summary>
    ''' <remarks>
    ''' CacheLevel is determined as minimum of the used tokens' cachability 
    ''' </remarks>
    Public Enum CacheLevel As Byte
        ''' <summary>
        ''' Caching of the text is not suitable and might expose security risks
        ''' </summary>
        notCacheable = 0
        ''' <summary>
        ''' Caching of the text might result in inaccurate display (e.g. time), but does not expose a security risk
        ''' </summary>
        secureforCaching = 5
        ''' <summary>
        ''' Caching of the text can be done without limitations or any risk
        ''' </summary>
        fullyCacheable = 10
    End Enum
#End Region

    ''' <summary>
    ''' BaseCustomTokenReplace  allows to add multiple sources implementing <see cref="IPropertyAccess">IPropertyAccess</see>
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class BaseCustomTokenReplace
        Inherits BaseTokenReplace

#Region " Private Fields "
        Private _AccessLevel As Scope
        Private _AccessingUser As Entities.Users.UserInfo
        Private _debugMessages As Boolean
#End Region

#Region "Protected Properties"

        ''' <summary>
        ''' Gets/sets the current Access Level controlling access to critical user settings
        ''' </summary>
        ''' <value>A TokenAccessLevel as defined above</value>
        Protected Property CurrentAccessLevel() As Scope
            Get
                Return _AccessLevel
            End Get
            Set(ByVal value As Scope)

                _AccessLevel = value
            End Set
        End Property

        Protected PropertySource As New System.Collections.Generic.Dictionary(Of String, IPropertyAccess)


#End Region

#Region "Public Properties"

        ''' <summary>
        ''' Gets/sets the user object representing the currently accessing user (permission)
        ''' </summary>
        ''' <value>UserInfo oject</value>
        Public Property AccessingUser() As Entities.Users.UserInfo
            Get
                Return _AccessingUser
            End Get
            Set(ByVal value As Entities.Users.UserInfo)
                _AccessingUser = value
            End Set
        End Property

        ''' <summary>
        ''' If DebugMessages are enabled, unknown Tokens are replaced with Error Messages 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DebugMessages() As Boolean
            Get
                Return _debugMessages
            End Get
            Set(ByVal value As Boolean)
                _debugMessages = value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        Protected Overrides Function replacedTokenValue(ByVal strObjectName As String, ByVal strPropertyName As String, ByVal strFormat As String) As String
            Dim PropertyNotFound As Boolean = False
            Dim result As String = String.Empty
            If PropertySource.ContainsKey(strObjectName.ToLower) Then
                result = PropertySource(strObjectName.ToLower).GetProperty(strPropertyName, strFormat, FormatProvider, AccessingUser, CurrentAccessLevel, PropertyNotFound)
            Else
                If DebugMessages Then
                    Dim message As String = Localization.Localization.GetString("TokenReplaceUnknownObject", Localization.Localization.SharedResourceFile, FormatProvider.ToString())
                    If message = String.Empty Then message = "Error accessing [{0}:{1}], {0} is an unknown datasource"
                    result = String.Format(message, strObjectName, strPropertyName)
                End If
            End If
            If DebugMessages And PropertyNotFound Then
                Dim message As String
                If result = PropertyAccess.ContentLocked Then
                    message = Localization.Localization.GetString("TokenReplaceRestrictedProperty", Localization.Localization.GlobalResourceFile, FormatProvider.ToString())
                Else
                    message = Localization.Localization.GetString("TokenReplaceUnknownProperty", Localization.Localization.GlobalResourceFile, FormatProvider.ToString())
                End If

                If message = String.Empty Then message = "Error accessing [{0}:{1}], {1} is unknown for datasource {0}"
                result = String.Format(message, strObjectName, strPropertyName)
            End If
            Return result
        End Function

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Checks for present [Object:Property] tokens
        ''' </summary>
        ''' <param name="strSourceText">String with [Object:Property] tokens</param>
        ''' <returns></returns>
        ''' <history>
        '''    08/10/2007 [sleupold] created
        '''    10/19/2007 [sleupold] corrected to ignore unchanged text returned (issue DNN-6526)
        ''' </history>
        Public Function ContainsTokens(ByVal strSourceText As String) As Boolean
            If Not String.IsNullOrEmpty(strSourceText) Then
                For Each currentMatch As System.Text.RegularExpressions.Match In TokenizerRegex.Matches(strSourceText)
                    If currentMatch.Result("${object}").Length > 0 Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        ''' <summary>
        ''' returns cacheability of the passed text regarding all contained tokens
        ''' </summary>
        ''' <param name="strSourcetext">the text to parse for tokens to replace</param>
        ''' <returns>cacheability level (not - safe - fully)</returns>
        ''' <remarks>always check cacheability before caching a module!</remarks>
        ''' <history>
        '''    10/19/2007 [sleupold] corrected to handle non-empty strings
        ''' </history>
        Public Function Cacheability(ByVal strSourcetext As String) As CacheLevel
            Dim IsSafe As CacheLevel = CacheLevel.fullyCacheable
            If Not (strSourcetext Is Nothing) AndAlso Not String.IsNullOrEmpty(strSourcetext) Then

                'initialize PropertyAccess classes
                Dim DummyResult As String = ReplaceTokens(strSourcetext)

                Dim Result As New System.Text.StringBuilder
                For Each currentMatch As System.Text.RegularExpressions.Match In TokenizerRegex.Matches(strSourcetext)

                    Dim strObjectName As String = currentMatch.Result("${object}")
                    If strObjectName.Length > 0 Then
                        If strObjectName = "[" Then
                            'nothing
                        ElseIf Not PropertySource.ContainsKey(strObjectName.ToLower) Then
                            'end if
                        Else
                            Dim c As CacheLevel = PropertySource(strObjectName.ToLower).Cacheability
                            If c < IsSafe Then IsSafe = c
                        End If
                    End If
                Next
            End If
            Return IsSafe
        End Function

#End Region

    End Class

End Namespace
