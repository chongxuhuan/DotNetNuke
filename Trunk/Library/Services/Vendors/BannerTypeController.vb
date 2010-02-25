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
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Services.Vendors

	Public Class BannerTypeController

		Public Function GetBannerTypes() As ArrayList

			Dim arrBannerTypes As New ArrayList

            arrBannerTypes.Add(New BannerTypeInfo(BannerType.Banner, Services.Localization.Localization.GetString("BannerType.Banner.String", Services.Localization.Localization.GlobalResourceFile)))
            arrBannerTypes.Add(New BannerTypeInfo(BannerType.MicroButton, Services.Localization.Localization.GetString("BannerType.MicroButton.String", Services.Localization.Localization.GlobalResourceFile)))
            arrBannerTypes.Add(New BannerTypeInfo(BannerType.Button, Services.Localization.Localization.GetString("BannerType.Button.String", Services.Localization.Localization.GlobalResourceFile)))
            arrBannerTypes.Add(New BannerTypeInfo(BannerType.Block, Services.Localization.Localization.GetString("BannerType.Block.String", Services.Localization.Localization.GlobalResourceFile)))
            arrBannerTypes.Add(New BannerTypeInfo(BannerType.Skyscraper, Services.Localization.Localization.GetString("BannerType.Skyscraper.String", Services.Localization.Localization.GlobalResourceFile)))
            arrBannerTypes.Add(New BannerTypeInfo(BannerType.Text, Services.Localization.Localization.GetString("BannerType.Text.String", Services.Localization.Localization.GlobalResourceFile)))
            arrBannerTypes.Add(New BannerTypeInfo(BannerType.Script, Services.Localization.Localization.GetString("BannerType.Script.String", Services.Localization.Localization.GlobalResourceFile)))

			Return arrBannerTypes

		End Function
	End Class

End Namespace

