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

Namespace DotNetNuke.Common

    <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> _
    Public Module Assembly

#Region "Public Constants"

        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbAppType As String = "Framework"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbAppVersion As String = "05.01.00"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbAppName As String = "DNNCORP.PE"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbAppTitle As String = "DotNetNuke"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbAppDescription As String = "DotNetNuke Professional Edition"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbAppCompany As String = "DotNetNuke Corporation"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbAppUrl As String = "http://www.dotnetnuke.com"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbUpgradeUrl As String = "http://update.dotnetnuke.com"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbLegalCopyright As String = "DotNetNuke® is copyright 2002-YYYY by DotNetNuke Corporation"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbTrademark As String = "DotNetNuke,DNN"
        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public Const glbHelpUrl As String = "http://www.dotnetnuke.com/default.aspx?tabid=787"

#End Region

        <Obsolete("Deprecated in DNN 5.1. Use DotNetNukeContext.Current.Application properties.")> Public ReadOnly Property ApplicationVersion() As System.Version
            Get
                Return New System.Version("05.01.00")
            End Get
        End Property

    End Module

End Namespace

