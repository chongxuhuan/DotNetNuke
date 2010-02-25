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
' CountryListBox ASP.NET Web Control, lists	countries and
' automatically detects	country	of visitors.
'
' This	web	control	will load a	listbox	with all countries and
' upon	loading	will attempt to	automatically recognize	the
' country that the	visitor	is visiting	the	website	from.
'------------------------------------------------------------------------------------------------

Imports System
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports System.Configuration
Imports System.Web.Caching
Imports System.IO
Imports System.Collections

<Assembly: TagPrefix("DotNetNuke.UI.WebControls.CountryListBox", "DotNetNuke")> 

Namespace DotNetNuke.UI.WebControls
	<ToolboxData("<{0}:CountryListBox runat=server></{0}:CountryListBox>")> _
	Public Class CountryListBox
		Inherits System.Web.UI.WebControls.DropDownList
#Region " Declarations "
		Private _CacheGeoIPData As Boolean = True
		Private _TestIP As String
		Private _LocalhostCountryCode As String
		Private _GeoIPFile As String
#End Region

#Region " Properties "
		<Bindable(True), Category("Caching"), DefaultValue(True)> Public Property CacheGeoIPData() As Boolean
			Get
				Return _CacheGeoIPData
			End Get

			Set(ByVal Value As Boolean)
				_CacheGeoIPData = Value
				If Value = False Then
					Context.Cache.Remove("GeoIPData")
				End If
			End Set
		End Property
		<Bindable(True), Category("Appearance"), DefaultValue("")> Public Property GeoIPFile() As String
			Get
				Return _GeoIPFile
			End Get
			Set(ByVal Value As String)
				_GeoIPFile = Value
			End Set
		End Property
		<Bindable(True), Category("Appearance"), DefaultValue("")> Public Property TestIP() As String
			Get
				Return _TestIP
			End Get
			Set(ByVal Value As String)
				_TestIP = Value
			End Set
		End Property
		<Bindable(True), Category("Appearance"), DefaultValue("")> Public Property LocalhostCountryCode() As String
			Get
				Return _LocalhostCountryCode
			End Get
			Set(ByVal Value As String)
				_LocalhostCountryCode = Value
			End Set
		End Property
#End Region

#Region " DataBinding "
		Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
			Dim IsLocal As Boolean = False
			Dim IP As String

			If Not Page.IsPostBack Then
				'If GeoIPFile is not provided, assume they put it in BIN.
				If _GeoIPFile = "" Then _GeoIPFile = "controls/CountryListBox/Data/GeoIP.dat"

				EnsureChildControls()

				'Check to see if a TestIP is specified
				If _TestIP <> "" Then
					'TestIP is specified, let's use it
					IP = _TestIP
				ElseIf Me.Page.Request.UserHostAddress = "127.0.0.1" Then
					'The country cannot be detected because the user is local.
					IsLocal = True
					'Set the IP address in case they didn't specify LocalhostCountryCode
					IP = Me.Page.Request.UserHostAddress
				Else
					'Set the IP address so we can find the country
					IP = Me.Page.Request.UserHostAddress
				End If

				'Check to see if we need to generate the Cache for the GeoIPData file
				If Context.Cache.Get("GeoIPData") Is Nothing And _CacheGeoIPData Then
					'Store it as	well as	setting	a dependency on	the	file
					Context.Cache.Insert("GeoIPData", CountryLookup.FileToMemory(Context.Server.MapPath(_GeoIPFile)), New CacheDependency(Context.Server.MapPath(_GeoIPFile)))
				End If

				'Check to see if the request is a localhost request
				'and see if the LocalhostCountryCode is specified
				If IsLocal And _LocalhostCountryCode <> "" Then
					'Bing the data
					MyBase.OnDataBinding(e)
					'Pre-Select the value in the drop-down based
					'on the LocalhostCountryCode specified.
					If Not Me.Items.FindByValue(_LocalhostCountryCode) Is Nothing Then
						Me.Items.FindByValue(_LocalhostCountryCode).Selected = True
					End If
				Else
					'Either this is a remote request or it is a local
					'request with no LocalhostCountryCode specified
					Dim _CountryLookup As CountryLookup

					'Check to see if we are using the Cached
					'version of the GeoIPData file
					If _CacheGeoIPData Then
						'Yes, get it from cache
						_CountryLookup = New CountryLookup(CType(Context.Cache.Get("GeoIPData"), MemoryStream))
					Else
						'No, get it from file
						_CountryLookup = New CountryLookup(Context.Server.MapPath(_GeoIPFile))
					End If

					'Get the country code based on the IP address
					Dim _UserCountryCode As String = _CountryLookup.LookupCountryCode(IP)

					'Bind the datasource
					MyBase.OnDataBinding(e)

					'Make sure the value returned is actually
					'in the drop-down list.
					If Not Me.Items.FindByValue(_UserCountryCode) Is Nothing Then
						'Yes, it's there, select it based on its value
						Me.Items.FindByValue(_UserCountryCode).Selected = True
					Else
						'No it's not there.  Let's get the Country description
						'and add a new list item for the Country detected
						Dim _UserCountry As String = _CountryLookup.LookupCountryName(IP)
						If _UserCountry <> "N/A" Then
							Dim newItem As New ListItem
							newItem.Value = _UserCountryCode
							newItem.Text = _UserCountry
							Me.Items.Insert(0, newItem)
							'Now let's Pre-Select it
							Me.Items.FindByValue(_UserCountryCode).Selected = True
						End If
					End If
				End If
			End If
		End Sub
#End Region

	End Class

End Namespace
