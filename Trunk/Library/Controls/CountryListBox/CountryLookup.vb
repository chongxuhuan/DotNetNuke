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


'------------------------------------------------------------------------------------------------
' This class uses an IP lookup database from MaxMind, specifically
' the GeoIP Free Database.
'
' The database and the c# implementation of this class
' are available from http://www.maxmind.com/app/csharp
'------------------------------------------------------------------------------------------------

Imports System
Imports System.IO
Imports System.Net

Namespace DotNetNuke.UI.WebControls

    Public Class CountryLookup

#Region " Declarations "
        Public m_MemoryStream As MemoryStream

        Private Shared CountryBegin As Long = 16776960
        Private Shared CountryName As String() = {"N/A", "Asia/Pacific Region", "Europe", "Andorra", "United Arab Emirates", "Afghanistan", "Antigua and Barbuda", "Anguilla", "Albania", "Armenia", "Netherlands Antilles", "Angola", "Antarctica", "Argentina", "American Samoa", "Austria", "Australia", "Aruba", "Azerbaijan", "Bosnia and Herzegovina", "Barbados", "Bangladesh", "Belgium", "Burkina Faso", "Bulgaria", "Bahrain", "Burundi", "Benin", "Bermuda", "Brunei Darussalam", "Bolivia", "Brazil", "Bahamas", "Bhutan", "Bouvet Island", "Botswana", "Belarus", "Belize", "Canada", "Cocos (Keeling) Islands", "Congo, The Democratic Republic of the", "Central African Republic", "Congo", "Switzerland", "Cote D'Ivoire", "Cook Islands", "Chile", "Cameroon", "China", "Colombia", "Costa Rica", "Cuba", "Cape Verde", "Christmas Island", "Cyprus", "Czech Republic", "Germany", "Djibouti", "Denmark", "Dominica", "Dominican Republic", "Algeria", "Ecuador", "Estonia", "Egypt", "Western Sahara", "Eritrea", "Spain", "Ethiopia", "Finland", "Fiji", "Falkland Islands (Malvinas)", "Micronesia, Federated States of", "Faroe Islands", "France", "France, Metropolitan", "Gabon", "United Kingdom", "Grenada", "Georgia", "French Guiana", "Ghana", "Gibraltar", "Greenland", "Gambia", "Guinea", "Guadeloupe", "Equatorial Guinea", "Greece", "South Georgia and the South Sandwich Islands", "Guatemala", "Guam", "Guinea-Bissau", "Guyana", "Hong Kong", "Heard Island and McDonald Islands", "Honduras", "Croatia", "Haiti", "Hungary", "Indonesia", "Ireland", "Israel", "India", "British Indian Ocean Territory", "Iraq", "Iran, Islamic Republic of", "Iceland", "Italy", "Jamaica", "Jordan", "Japan", "Kenya", "Kyrgyzstan", "Cambodia", "Kiribati", "Comoros", "Saint Kitts and Nevis", "Korea, Democratic People's Republic of", "Korea, Republic of", "Kuwait", "Cayman Islands", "Kazakstan", "Lao People's Democratic Republic", "Lebanon", "Saint Lucia", "Liechtenstein", "Sri Lanka", "Liberia", "Lesotho", "Lithuania", "Luxembourg", "Latvia", "Libyan Arab Jamahiriya", "Morocco", "Monaco", "Moldova, Republic of", "Madagascar", "Marshall Islands", "Macedonia, the Former Yugoslav Republic of", "Mali", "Myanmar", "Mongolia", "Macau", "Northern Mariana Islands", "Martinique", "Mauritania", "Montserrat", "Malta", "Mauritius", "Maldives", "Malawi", "Mexico", "Malaysia", "Mozambique", "Namibia", "New Caledonia", "Niger", "Norfolk Island", "Nigeria", "Nicaragua", "Netherlands", "Norway", "Nepal", "Nauru", "Niue", "New Zealand", "Oman", "Panama", "Peru", "French Polynesia", "Papua New Guinea", "Philippines", "Pakistan", "Poland", "Saint Pierre and Miquelon", "Pitcairn", "Puerto Rico", "Palestinian Territory, Occupied", "Portugal", "Palau", "Paraguay", "Qatar", "Reunion", "Romania", "Russian Federation", "Rwanda", "Saudi Arabia", "Solomon Islands", "Seychelles", "Sudan", "Sweden", "Singapore", "Saint Helena", "Slovenia", "Svalbard and Jan Mayen", "Slovakia", "Sierra Leone", "San Marino", "Senegal", "Somalia", "Suriname", "Sao Tome and Principe", "El Salvador", "Syrian Arab Republic", "Swaziland", "Turks and Caicos Islands", "Chad", "French Southern Territories", "Togo", "Thailand", "Tajikistan", "Tokelau", "Turkmenistan", "Tunisia", "Tonga", "East Timor", "Turkey", "Trinidad and Tobago", "Tuvalu", "Taiwan, Province of China", "Tanzania, United Republic of", "Ukraine", "Uganda", "United States Minor Outlying Islands", "United States", "Uruguay", "Uzbekistan", "Holy See (Vatican City State)", "Saint Vincent and the Grenadines", "Venezuela", "Virgin Islands, British", "Virgin Islands, U.S.", "Vietnam", "Vanuatu", "Wallis and Futuna", "Samoa", "Yemen", "Mayotte", "Yugoslavia", "South Africa", "Zambia", "Zaire", "Zimbabwe", "Anonymous Proxy", "Satellite Provider"}
        Private Shared CountryCode As String() = {"--", "AP", "EU", "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AN", "AO", "AQ", "AR", "AS", "AT", "AU", "AW", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BM", "BN", "BO", "BR", "BS", "BT", "BV", "BW", "BY", "BZ", "CA", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EE", "EG", "EH", "ER", "ES", "ET", "FI", "FJ", "FK", "FM", "FO", "FR", "FX", "GA", "GB", "GD", "GE", "GF", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR", "HT", "HU", "ID", "IE", "IL", "IN", "IO", "IQ", "IR", "IS", "IT", "JM", "JO", "JP", "KE", "KG", "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY", "MA", "MC", "MD", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU", "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT", "PW", "PY", "QA", "RE", "RO", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "ST", "SV", "SY", "SZ", "TC", "TD", "TF", "TG", "TH", "TJ", "TK", "TM", "TN", "TO", "TP", "TR", "TT", "TV", "TW", "TZ", "UA", "UG", "UM", "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "YE", "YT", "YU", "ZA", "ZM", "ZR", "ZW", "A1", "A2"}
#End Region

#Region " Methods "
        Public Sub New(ByVal ms As MemoryStream)
            m_MemoryStream = ms
        End Sub

        Public Sub New(ByVal FileLocation As String)
            '------------------------------------------------------------------------------------------------
            ' Load the passed in GeoIP Data file to the memorystream
            '------------------------------------------------------------------------------------------------
            Dim _FileStream As New FileStream(FileLocation, FileMode.Open, FileAccess.Read)
            m_MemoryStream = New MemoryStream
            Dim _Byte(256) As Byte
            While _FileStream.Read(_Byte, 0, _Byte.Length) <> 0
                m_MemoryStream.Write(_Byte, 0, _Byte.Length)
            End While
            _FileStream.Close()
        End Sub
        Private Function ConvertIPAddressToNumber(ByVal _IPAddress As IPAddress) As Long
            '------------------------------------------------------------------------------------------------
            ' Convert an IP Address, (e.g. 127.0.0.1), to the numeric equivalent
            '------------------------------------------------------------------------------------------------
            Dim _Address() As String = Split(_IPAddress.ToString, ".")

            If UBound(_Address) = 3 Then
                Return CType(16777216 * CType(_Address(0), Double) + 65536 * CType(_Address(1), Double) + 256 * CType(_Address(2), Double) + CType(_Address(3), Double), Long)
            Else
                Return 0
            End If
        End Function

        Private Function ConvertIPNumberToAddress(ByVal _IPNumber As Long) As String

            '------------------------------------------------------------------------------------------------
            ' Convert an IP Number to the IP Address equivalent
            '------------------------------------------------------------------------------------------------

            Dim _IPNumberPart1 As String = CStr(Int(_IPNumber / 16777216) Mod 256)
            Dim _IPNumberPart2 As String = CStr(Int(_IPNumber / 65536) Mod 256)
            Dim _IPNumberPart3 As String = CStr(Int(_IPNumber / 256) Mod 256)
            Dim _IPNumberPart4 As String = CStr(Int(_IPNumber) Mod 256)

            Return _IPNumberPart1 & "." & _IPNumberPart2 & "." & _IPNumberPart3 & "." & _IPNumberPart4

        End Function

        Public Shared Function FileToMemory(ByVal FileLocation As String) As MemoryStream

            '------------------------------------------------------------------------------------------------
            ' Read a given file into a Memory Stream to return as the result
            '------------------------------------------------------------------------------------------------
            Dim _FileStream As FileStream
            Dim _MemStream As New MemoryStream
            Dim _Byte(256) As Byte
            Try
                _FileStream = New FileStream(FileLocation, FileMode.Open, FileAccess.Read)
                While _FileStream.Read(_Byte, 0, _Byte.Length) <> 0
                    _MemStream.Write(_Byte, 0, _Byte.Length)
                End While
                _FileStream.Close()
            Catch exc As FileNotFoundException
                Err.Raise(9999, "FileToMemory", Err.Description + "  Please set the ""GeoIPFile"" Property to specify the location of this file.  The property value must be set to the virtual path to GeoIP.dat (i.e. ""/controls/CountryListBox/Data/GeoIP.dat"")")
            End Try
            Return _MemStream
        End Function

        Public Overloads Function LookupCountryCode(ByVal _IPAddress As IPAddress) As String
            '------------------------------------------------------------------------------------------------
            ' Look up the country code, e.g. US, for the passed in IP Address
            '------------------------------------------------------------------------------------------------
            Return CountryCode(CInt(SeekCountry(0, ConvertIPAddressToNumber(_IPAddress), 31)))
        End Function

        Public Overloads Function LookupCountryCode(ByVal _IPAddress As String) As String
            '------------------------------------------------------------------------------------------------
            ' Look up the country code, e.g. US, for the passed in IP Address
            '------------------------------------------------------------------------------------------------
            Dim _Address As IPAddress

            Try
                _Address = IPAddress.Parse(_IPAddress)
            Catch ex As FormatException
                Return "--"
            End Try
            Return LookupCountryCode(_Address)
        End Function

        Public Overloads Function LookupCountryName(ByVal addr As IPAddress) As String
            '------------------------------------------------------------------------------------------------
            ' Look up the country name, e.g. United States, for the IP Address
            '------------------------------------------------------------------------------------------------
            Return CountryName(CInt(SeekCountry(0, ConvertIPAddressToNumber(addr), 31)))
        End Function

        Public Overloads Function LookupCountryName(ByVal _IPAddress As String) As String
            '------------------------------------------------------------------------------------------------
            ' Look up the country name, e.g. United States, for the IP Address
            '------------------------------------------------------------------------------------------------
            Dim _Address As IPAddress
            Try
                _Address = IPAddress.Parse(_IPAddress)
            Catch e As FormatException
                Return "N/A"
            End Try
            Return LookupCountryName(_Address)
        End Function
        Private Function vbShiftLeft(ByVal Value As Long, _
          ByVal Count As Integer) As Long
            '------------------------------------------------------------------------------------------------
            ' Replacement for Bitwise operators which are missing in VB.NET,
            ' these functions are present in .NET 1.1, but for developers
            ' using 1.0, replacement functions must be implemented
            '------------------------------------------------------------------------------------------------

            Dim _Iterator As Integer

            vbShiftLeft = Value
            For _Iterator = 1 To Count
                vbShiftLeft = vbShiftLeft * 2
            Next
        End Function
        Private Function vbShiftRight(ByVal Value As Long, _
          ByVal Count As Integer) As Long
            '------------------------------------------------------------------------------------------------
            ' Replacement for Bitwise operators which are missing in VB.NET,
            ' these functions are present in .NET 1.1, but for developers
            ' using 1.0, replacement functions must be implemented
            '------------------------------------------------------------------------------------------------

            Dim _Iterator As Integer

            vbShiftRight = Value
            For _Iterator = 1 To Count
                vbShiftRight = vbShiftRight \ 2
            Next
        End Function

        Public Function SeekCountry(ByRef Offset As Integer, ByRef Ipnum As Long, ByRef Depth As Short) As Integer
            Try
                Dim Buffer(6) As Byte
                Dim X(2) As Integer
                Dim I As Short
                Dim J As Short
                Dim Y As Byte

                If Depth = 0 Then
                    Throw New Exception
                End If

                m_MemoryStream.Seek(6 * Offset, 0)
                m_MemoryStream.Read(Buffer, 0, 6)

                For I = 0 To 1
                    X(I) = 0

                    For J = 0 To 2
                        Y = Buffer(I * 3 + J)
                        If Y < 0 Then Y = CType(Y + 256, Byte)
                        X(I) = CType(X(I) + vbShiftLeft(Y, J * 8), Integer)
                    Next J
                Next I

                If (Ipnum And vbShiftLeft(1, Depth)) > 0 Then
                    If X(1) >= CountryBegin Then
                        SeekCountry = CType(X(1) - CountryBegin, Integer)
                        Exit Function
                    End If

                    SeekCountry = SeekCountry(X(1), Ipnum, CType(Depth - 1, Short))
                    Exit Function
                Else
                    If X(0) >= CountryBegin Then
                        SeekCountry = CType(X(0) - CountryBegin, Integer)
                        Exit Function
                    End If

                    SeekCountry = SeekCountry(X(0), Ipnum, CType(Depth - 1, Short))
                    Exit Function
                End If

            Catch exc As Exception
                Err.Raise(9999, "CountryLookup.SeekCountry", "Error seeking country: " & exc.ToString)
            End Try
        End Function
#End Region

    End Class

End Namespace