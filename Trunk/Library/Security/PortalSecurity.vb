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
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Security

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Security.Permissions
Imports System.Collections.Generic

Namespace DotNetNuke.Security

    Public Class PortalSecurity

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' The FilterFlag enum determines which filters are applied by the InputFilter
        ''' function.  The Flags attribute allows the user to include multiple 
        ''' enumerated values in a single variable by OR'ing the individual values
        ''' together.
        ''' </summary>
        ''' <history>
        ''' 	[Joe Brinkman] 	8/15/2003	Created  Bug #000120, #000121
        ''' </history>
        '''-----------------------------------------------------------------------------
        <FlagsAttribute()> _
        Enum FilterFlag
            MultiLine = 1
            NoMarkup = 2
            NoScripting = 4
            NoSQL = 8
            NoAngleBrackets = 16
        End Enum

#Region "Private Methods"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This function uses Regex search strings to remove HTML tags which are 
        ''' targeted in Cross-site scripting (XSS) attacks.  This function will evolve
        ''' to provide more robust checking as additional holes are found.
        ''' </summary>
        ''' <param name="strInput">This is the string to be filtered</param>
        ''' <returns>Filtered UserInput</returns>
        ''' <remarks>
        ''' This is a private function that is used internally by the FormatDisableScripting function
        ''' </remarks>
        ''' <history>
        '''     [cathal]        3/06/2007   Created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Private Function FilterStrings(ByVal strInput As String) As String
            'setup up list of search terms as items may be used twice
            Dim TempInput As String = strInput
            Dim listStrings As New List(Of String)
            listStrings.Add("<script[^>]*>.*?</script[^><]*>")
            listStrings.Add("<input[^>]*>.*?</input[^><]*>")
            listStrings.Add("<object[^>]*>.*?</object[^><]*>")
            listStrings.Add("<embed[^>]*>.*?</embed[^><]*>")
            listStrings.Add("<applet[^>]*>.*?</applet[^><]*>")
            listStrings.Add("<form[^>]*>.*?</form[^><]*>")
            listStrings.Add("<option[^>]*>.*?</option[^><]*>")
            listStrings.Add("<select[^>]*>.*?</select[^><]*>")
            listStrings.Add("<iframe[^>]*>.*?</iframe[^><]*>")
            listStrings.Add("<iframe.*?<")
            listStrings.Add("<ilayer[^>]*>.*?</ilayer[^><]*>")
            listStrings.Add("<form[^>]*>")
            listStrings.Add("</form[^><]*>")
            listStrings.Add("javascript:")
            listStrings.Add("vbscript:")
            listStrings.Add("alert[\s(&nbsp;)]*\([\s(&nbsp;)]*'?[\s(&nbsp;)]*[""(&quot;)]?")

            Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Singleline
            Dim strReplacement As String = " "

            'check if text contains encoded angle brackets, if it does it we decode it to check the plain text
            If TempInput.Contains("&gt;") = True And TempInput.Contains("&lt;") = True Then
                'text is encoded, so decode and try again
                TempInput = HttpContext.Current.Server.HtmlDecode(TempInput)
                For Each s As String In listStrings
                    TempInput = Regex.Replace(TempInput, s, strReplacement, options)
                Next

                'Re-encode
                TempInput = HttpContext.Current.Server.HtmlEncode(TempInput)
            Else
                For Each s As String In listStrings
                    TempInput = Regex.Replace(TempInput, s, strReplacement, options)
                Next
            End If

            Return TempInput
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This function uses Regex search strings to remove HTML tags which are 
        ''' targeted in Cross-site scripting (XSS) attacks.  This function will evolve
        ''' to provide more robust checking as additional holes are found.
        ''' </summary>
        ''' <param name="strInput">This is the string to be filtered</param>
        ''' <returns>Filtered UserInput</returns>
        ''' <remarks>
        ''' This is a private function that is used internally by the InputFilter function
        ''' </remarks>
        ''' <history>
        ''' 	[Joe Brinkman] 	8/15/2003	Created Bug #000120
        '''     [cathal]        3/06/2007   Added check for encoded content
        ''' </history>
        '''-----------------------------------------------------------------------------
        Private Function FormatDisableScripting(ByVal strInput As String) As String
            Dim TempInput As String = strInput
            TempInput = FilterStrings(TempInput)

            Return TempInput
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This filter removes angle brackets i.e. 
        ''' </summary>
        ''' <param name="strInput">This is the string to be filtered</param>
        ''' <returns>Filtered UserInput</returns>
        ''' <remarks>
        ''' This is a private function that is used internally by the InputFilter function
        ''' </remarks>
        ''' <history>
        ''' 	[Cathal] 	6/1/2006	Created to fufill client request
        ''' </history>
        '''-----------------------------------------------------------------------------
        Private Function FormatAngleBrackets(ByVal strInput As String) As String
            Dim TempInput As String = strInput.Replace("<", "")
            TempInput = TempInput.Replace(">", "")
            Return TempInput
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This filter removes CrLf characters and inserts br
        ''' </summary>
        ''' <param name="strInput">This is the string to be filtered</param>
        ''' <returns>Filtered UserInput</returns>
        ''' <remarks>
        ''' This is a private function that is used internally by the InputFilter function
        ''' </remarks>
        ''' <history>
        ''' 	[Joe Brinkman] 	8/15/2003	Created Bug #000120
        ''' </history>
        '''-----------------------------------------------------------------------------
        Private Function FormatMultiLine(ByVal strInput As String) As String
            Dim TempInput As String = strInput.Replace(ControlChars.Cr + ControlChars.Lf, "<br>")
            Return TempInput.Replace(ControlChars.Cr, "<br>")
        End Function    'FormatMultiLine

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This function verifies raw SQL statements to prevent SQL injection attacks 
        ''' and replaces a similar function (PreventSQLInjection) from the Common.Globals.vb module
        ''' </summary>
        ''' <param name="strSQL">This is the string to be filtered</param>
        ''' <returns>Filtered UserInput</returns>
        ''' <remarks>
        ''' This is a private function that is used internally by the InputFilter function
        ''' </remarks>
        ''' <history>
        ''' 	[Joe Brinkman] 	8/15/2003	Created Bug #000121
        '''     [Tom Lucas]     3/8/2004    Fixed   Bug #000114 (Aardvark)
        '''                     8/5/2009 added additional strings and performance tweak
        ''' </history>
        '''-----------------------------------------------------------------------------
        Private Function FormatRemoveSQL(ByVal strSQL As String) As String
            Const BadStatementExpression As String = ";|--|create|drop|select|insert|delete|update|union|sp_|xp_|exec|/\*.*\*/|declare"
            Return Regex.Replace(strSQL, BadStatementExpression, " ", RegexOptions.IgnoreCase And RegexOptions.Compiled).Replace("'", "''")
        End Function



        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This function determines if the Input string contains any markup.
        ''' </summary>
        ''' <param name="strInput">This is the string to be checked</param>
        ''' <returns>True if string contains Markup tag(s)</returns>
        ''' <remarks>
        ''' This is a private function that is used internally by the InputFilter function
        ''' </remarks>
        ''' <history>
        ''' 	[Joe Brinkman] 	8/15/2003	Created Bug #000120
        ''' </history>
        '''-----------------------------------------------------------------------------
        Private Function IncludesMarkup(ByVal strInput As String) As Boolean
            Dim options As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.Singleline
            Dim strPattern As String = "<[^<>]*>"
            Return Regex.IsMatch(strInput, strPattern, options)
        End Function

#End Region

#Region "Public Methods"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This function converts a byte array to a hex string
        ''' </summary>
        ''' <param name="bytes">An array of bytes</param>
        ''' <returns>A string representing the hex converted value</returns>
        ''' <remarks>
        ''' This is a private function that is used internally by the CreateKey function
        ''' </remarks>
        ''' <history>
        ''' </history>
        '''-----------------------------------------------------------------------------
        Private Function BytesToHexString(ByVal bytes() As Byte) As String
            Dim hexString As New StringBuilder(64)

            Dim counter As Integer
            For counter = 0 To bytes.Length - 1
                hexString.Append([String].Format("{0:X2}", bytes(counter)))
            Next counter

            Return hexString.ToString()
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This function creates a random key
        ''' </summary>
        ''' <param name="numBytes">This is the number of bytes for the key</param>
        ''' <returns>A random string</returns>
        ''' <remarks>
        ''' This is a public function used for generating SHA1 keys 
        ''' </remarks>
        ''' <history>
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Function CreateKey(ByVal numBytes As Integer) As String
            Dim rng As New RNGCryptoServiceProvider
            Dim buff(numBytes - 1) As Byte

            rng.GetBytes(buff)

            Return BytesToHexString(buff)
        End Function

        Public Function Decrypt(ByVal strKey As String, ByVal strData As String) As String

            If strData = "" Then
                Return ""
            End If

            Dim strValue As String = ""

            If strKey <> "" Then
                ' convert key to 16 characters for simplicity
                Select Case Len(strKey)
                    Case Is < 16
                        strKey = strKey & Left("XXXXXXXXXXXXXXXX", 16 - Len(strKey))
                    Case Is > 16
                        strKey = Left(strKey, 16)
                End Select

                ' create encryption keys
                Dim byteKey() As Byte = Encoding.UTF8.GetBytes(Left(strKey, 8))
                Dim byteVector() As Byte = Encoding.UTF8.GetBytes(Right(strKey, 8))

                ' convert data to byte array and Base64 decode
                Dim byteData(strData.Length) As Byte
                Try
                    byteData = Convert.FromBase64String(strData)
                Catch    ' invalid length
                    strValue = strData
                End Try

                If strValue = "" Then
                    Try
                        ' decrypt
                        Dim objDES As New DESCryptoServiceProvider
                        Dim objMemoryStream As New MemoryStream
                        Dim objCryptoStream As New CryptoStream(objMemoryStream, objDES.CreateDecryptor(byteKey, byteVector), CryptoStreamMode.Write)
                        objCryptoStream.Write(byteData, 0, byteData.Length)
                        objCryptoStream.FlushFinalBlock()

                        ' convert to string
                        Dim objEncoding As System.Text.Encoding = System.Text.Encoding.UTF8
                        strValue = objEncoding.GetString(objMemoryStream.ToArray())
                    Catch       ' decryption error
                        strValue = ""
                    End Try
                End If
            Else
                strValue = strData
            End If

            Return strValue

        End Function

        Public Function Encrypt(ByVal strKey As String, ByVal strData As String) As String

            Dim strValue As String = ""

            If strKey <> "" Then
                ' convert key to 16 characters for simplicity
                Select Case Len(strKey)
                    Case Is < 16
                        strKey = strKey & Left("XXXXXXXXXXXXXXXX", 16 - Len(strKey))
                    Case Is > 16
                        strKey = Left(strKey, 16)
                End Select

                ' create encryption keys
                Dim byteKey() As Byte = Encoding.UTF8.GetBytes(Left(strKey, 8))
                Dim byteVector() As Byte = Encoding.UTF8.GetBytes(Right(strKey, 8))

                ' convert data to byte array
                Dim byteData() As Byte = Encoding.UTF8.GetBytes(strData)

                ' encrypt 
                Dim objDES As New DESCryptoServiceProvider
                Dim objMemoryStream As New MemoryStream
                Dim objCryptoStream As New CryptoStream(objMemoryStream, objDES.CreateEncryptor(byteKey, byteVector), CryptoStreamMode.Write)
                objCryptoStream.Write(byteData, 0, byteData.Length)
                objCryptoStream.FlushFinalBlock()

                ' convert to string and Base64 encode
                strValue = Convert.ToBase64String(objMemoryStream.ToArray())
            Else
                strValue = strData
            End If

            Return strValue

        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' This function applies security filtering to the UserInput string.
        ''' </summary>
        ''' <param name="UserInput">This is the string to be filtered</param>
        ''' <param name="FilterType">Flags which designate the filters to be applied</param>
        ''' <returns>Filtered UserInput</returns>
        ''' <history>
        ''' 	[Joe Brinkman] 	8/15/2003	Created Bug #000120, #000121
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Function InputFilter(ByVal UserInput As String, ByVal FilterType As FilterFlag) As String
            If UserInput Is Nothing Then Return ""

            Dim TempInput As String = UserInput

            If (FilterType And FilterFlag.NoAngleBrackets) = FilterFlag.NoAngleBrackets Then
                Dim RemoveAngleBrackets As Boolean
                If Config.GetSetting("RemoveAngleBrackets") Is Nothing Then
                    RemoveAngleBrackets = False
                Else
                    RemoveAngleBrackets = Boolean.Parse(Config.GetSetting("RemoveAngleBrackets"))
                End If
                If RemoveAngleBrackets = True Then
                    TempInput = FormatAngleBrackets(TempInput)
                End If
            End If

            If (FilterType And FilterFlag.NoSQL) = FilterFlag.NoSQL Then
                TempInput = FormatRemoveSQL(TempInput)
            Else
                If (FilterType And FilterFlag.NoMarkup) = FilterFlag.NoMarkup AndAlso IncludesMarkup(TempInput) Then
                    TempInput = HttpUtility.HtmlEncode(TempInput)
                End If

                If (FilterType And FilterFlag.NoScripting) = FilterFlag.NoScripting Then
                    TempInput = FormatDisableScripting(TempInput)
                End If

                If (FilterType And FilterFlag.MultiLine) = FilterFlag.MultiLine Then
                    TempInput = FormatMultiLine(TempInput)
                End If
            End If

            Return TempInput
        End Function

        Public Sub SignOut()
            ' Log User Off from Cookie Authentication System
            System.Web.Security.FormsAuthentication.SignOut()

            'remove language cookie
            HttpContext.Current.Response.Cookies("language").Value = ""

            'remove authentication type cookie
            HttpContext.Current.Response.Cookies("authentication").Value = ""

            ' expire cookies
            HttpContext.Current.Response.Cookies("portalaliasid").Value = Nothing
            HttpContext.Current.Response.Cookies("portalaliasid").Path = "/"
            HttpContext.Current.Response.Cookies("portalaliasid").Expires = DateTime.Now.AddYears(-30)

            HttpContext.Current.Response.Cookies("portalroles").Value = Nothing
            HttpContext.Current.Response.Cookies("portalroles").Path = "/"
            HttpContext.Current.Response.Cookies("portalroles").Expires = DateTime.Now.AddYears(-30)
        End Sub

#End Region

#Region "Public Shared/Static Methods"

        Public Shared Sub ClearRoles()
            HttpContext.Current.Response.Cookies("portalroles").Value = Nothing
            HttpContext.Current.Response.Cookies("portalroles").Path = "/"
            HttpContext.Current.Response.Cookies("portalroles").Expires = DateTime.Now.AddYears(-30)
        End Sub

        Public Shared Sub ForceSecureConnection()
            ' get current url
            Dim URL As String = HttpContext.Current.Request.Url.ToString
            ' if unsecure connection
            If URL.StartsWith("http://") Then
                ' switch to secure connection
                URL = URL.Replace("http://", "https://")
                ' append ssl parameter to querystring to indicate secure connection processing has already occurred
                If URL.IndexOf("?") = -1 Then
                    URL = URL & "?ssl=1"
                Else
                    URL = URL & "&ssl=1"
                End If
                ' redirect to secure connection
                HttpContext.Current.Response.Redirect(URL, True)
            End If
        End Sub

        Public Shared Function IsInRole(ByVal role As String) As Boolean
            Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
            Dim context As HttpContext = HttpContext.Current

            If (role <> "" AndAlso Not role Is Nothing AndAlso ((context.Request.IsAuthenticated = False And role = glbRoleUnauthUserName))) Then
                Return True
            Else
                Return objUserInfo.IsInRole(role)
            End If
        End Function

        Public Shared Function IsInRoles(ByVal roles As String) As Boolean
            Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo

            ' super user always has full access
            Dim blnIsInRoles As Boolean = objUserInfo.IsSuperUser

            If Not blnIsInRoles Then
                If Not roles Is Nothing Then
                    Dim context As HttpContext = HttpContext.Current
                    Dim role As String

                    ' permissions strings are encoded with Deny permissions at the beginning and Grant permissions at the end for optimal performance
                    For Each role In roles.Split(New Char() {";"c})
                        If Not String.IsNullOrEmpty(role) Then
                            ' Deny permission
                            If role.StartsWith("!") Then
                                'Portal Admin cannot be denied from his/her portal (so ignore deny permissions if user is portal admin)
                                Dim settings As PortalSettings = PortalController.GetCurrentPortalSettings()
                                If Not (settings.PortalId = objUserInfo.PortalID AndAlso settings.AdministratorId = objUserInfo.UserID) Then
                                    Dim denyRole As String = role.Replace("!", "")
                                    If ((context.Request.IsAuthenticated = False AndAlso denyRole = glbRoleUnauthUserName) OrElse _
                                          denyRole = glbRoleAllUsersName OrElse objUserInfo.IsInRole(denyRole)) Then
                                        blnIsInRoles = False
                                        Exit For
                                    End If
                                End If
                            Else ' Grant permission
                                If ((context.Request.IsAuthenticated = False AndAlso role = glbRoleUnauthUserName) OrElse _
                                      role = glbRoleAllUsersName OrElse objUserInfo.IsInRole(role)) Then
                                    blnIsInRoles = True
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            Return blnIsInRoles
        End Function

#End Region

#Region "Obsoleted Methods, retained for Binary Compatability"

        <Obsolete("Deprecated in DNN 5.0.  Please use HasModuleAccess(SecurityAccessLevel.Edit, PortalSettings, ModuleInfo, Username)")> _
        Public Shared Function HasEditPermissions(ByVal ModuleId As Integer) As Boolean
            Return ModulePermissionController.HasModulePermission(New ModulePermissionCollection(CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByModuleID(ModuleId, -1), GetType(ModulePermissionInfo))), "EDIT")
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use HasModuleAccess(SecurityAccessLevel.Edit, PortalSettings, ModuleInfo)")> _
        Public Shared Function HasEditPermissions(ByVal objModulePermissions As Security.Permissions.ModulePermissionCollection) As Boolean
            Return ModulePermissionController.HasModulePermission(objModulePermissions, "EDIT")
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use HasModuleAccess(SecurityAccessLevel.Edit, PortalSettings, ModuleInfo)")> _
        Public Shared Function HasEditPermissions(ByVal ModuleId As Integer, ByVal Tabid As Integer) As Boolean
            Return ModulePermissionController.HasModulePermission(ModulePermissionController.GetModulePermissions(ModuleId, Tabid), "EDIT")
        End Function

        <Obsolete("Deprecated in DNN 5.1.  Please use ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, PortalSettings, ModuleInfo)")> _
        Public Shared Function HasNecessaryPermission(ByVal AccessLevel As SecurityAccessLevel, ByVal PortalSettings As PortalSettings, ByVal ModuleConfiguration As ModuleInfo, ByVal UserName As String) As Boolean
            Return ModulePermissionController.HasModuleAccess(AccessLevel, "EDIT", ModuleConfiguration)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  Please use ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, PortalSettings, ModuleInfo)")> _
        Public Shared Function HasNecessaryPermission(ByVal AccessLevel As SecurityAccessLevel, ByVal PortalSettings As PortalSettings, ByVal ModuleConfiguration As ModuleInfo, ByVal User As UserInfo) As Boolean
            Return ModulePermissionController.HasModuleAccess(AccessLevel, "EDIT", ModuleConfiguration)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  Please use ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, PortalSettings, ModuleInfo)")> _
        Public Shared Function HasNecessaryPermission(ByVal AccessLevel As SecurityAccessLevel, ByVal PortalSettings As PortalSettings, ByVal ModuleConfiguration As ModuleInfo) As Boolean
            Return ModulePermissionController.HasModuleAccess(AccessLevel, "EDIT", ModuleConfiguration)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  Please use TabPermissionController.CanAdminPage")> _
        Public Shared Function IsPageAdmin() As Boolean
            Return TabPermissionController.CanAdminPage()
        End Function

        <Obsolete("This function has been replaced by UserController.UserLogin")> _
        Public Function UserLogin(ByVal Username As String, ByVal Password As String, ByVal PortalID As Integer, ByVal PortalName As String, ByVal IP As String, ByVal CreatePersistentCookie As Boolean) As Integer

            Dim loginStatus As UserLoginStatus
            Dim UserId As Integer = -1
            Dim objUser As UserInfo = UserController.UserLogin(PortalID, Username, Password, "", PortalName, IP, loginStatus, CreatePersistentCookie)

            If loginStatus = UserLoginStatus.LOGIN_SUCCESS Or loginStatus = UserLoginStatus.LOGIN_SUPERUSER Then
                UserId = objUser.UserID
            End If

            ' return the UserID
            Return UserId

        End Function

#End Region

    End Class

End Namespace