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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Lists
Imports System.Collections.Generic
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.UI.UserControls

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Address UserControl is used to manage User Addresses
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class Address
        Inherits Framework.UserControlBase

#Region "Controls"

        Protected rowStreet As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plStreet As UI.UserControls.LabelControl
        Protected txtStreet As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkStreet As System.Web.UI.WebControls.CheckBox
        Protected lblStreetRequired As System.Web.UI.WebControls.Label
        Protected valStreet As System.Web.UI.WebControls.RequiredFieldValidator

        Protected rowUnit As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plUnit As UI.UserControls.LabelControl
        Protected txtUnit As System.Web.UI.WebControls.TextBox

        Protected rowCity As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plCity As UI.UserControls.LabelControl
        Protected txtCity As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkCity As System.Web.UI.WebControls.CheckBox
        Protected lblCityRequired As System.Web.UI.WebControls.Label
        Protected valCity As System.Web.UI.WebControls.RequiredFieldValidator

        Protected rowCountry As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plCountry As UI.UserControls.LabelControl
        Protected WithEvents cboCountry As DotNetNuke.UI.WebControls.CountryListBox
        Protected WithEvents chkCountry As System.Web.UI.WebControls.CheckBox
        Protected lblCountryRequired As System.Web.UI.WebControls.Label
        Protected valCountry As System.Web.UI.WebControls.RequiredFieldValidator

        Protected rowRegion As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plRegion As UI.UserControls.LabelControl
        Protected lblRegion As System.Web.UI.WebControls.Label
        Protected WithEvents cboRegion As System.Web.UI.WebControls.DropDownList
        Protected txtRegion As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkRegion As System.Web.UI.WebControls.CheckBox
        Protected lblRegionRequired As System.Web.UI.WebControls.Label
        Protected valRegion1 As System.Web.UI.WebControls.RequiredFieldValidator
        Protected valRegion2 As System.Web.UI.WebControls.RequiredFieldValidator

        Protected rowPostal As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plPostal As UI.UserControls.LabelControl
        Protected txtPostal As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkPostal As System.Web.UI.WebControls.CheckBox
        Protected lblPostalRequired As System.Web.UI.WebControls.Label
        Protected valPostal As System.Web.UI.WebControls.RequiredFieldValidator

        Protected rowTelephone As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plTelephone As UI.UserControls.LabelControl
        Protected txtTelephone As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkTelephone As System.Web.UI.WebControls.CheckBox
        Protected lblTelephoneRequired As System.Web.UI.WebControls.Label
        Protected valTelephone As System.Web.UI.WebControls.RequiredFieldValidator

        Protected rowCell As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plCell As UI.UserControls.LabelControl
        Protected txtCell As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkCell As System.Web.UI.WebControls.CheckBox
        Protected lblCellRequired As System.Web.UI.WebControls.Label
        Protected valCell As System.Web.UI.WebControls.RequiredFieldValidator

        Protected rowFax As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plFax As UI.UserControls.LabelControl
        Protected txtFax As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkFax As System.Web.UI.WebControls.CheckBox
        Protected lblFaxRequired As System.Web.UI.WebControls.Label
        Protected valFax As System.Web.UI.WebControls.RequiredFieldValidator
#End Region

#Region "Private Members"

        Private _ModuleId As Integer
        Private _LabelColumnWidth As String = ""
        Private _ControlColumnWidth As String = ""
        Private _StartTabIndex As Integer = 1

        Private _Street As String
        Private _Unit As String
        Private _City As String
        Private _Country As String
        Private _Region As String
        Private _Postal As String
        Private _Telephone As String
        Private _Cell As String
        Private _Fax As String

        Private _ShowStreet As Boolean = True
        Private _ShowUnit As Boolean = True
        Private _ShowCity As Boolean = True
        Private _ShowCountry As Boolean = True
        Private _ShowRegion As Boolean = True
        Private _ShowPostal As Boolean = True
        Private _ShowTelephone As Boolean = True
        Private _ShowCell As Boolean = True
        Private _ShowFax As Boolean = True

        Private _CountryData As String = "Text"
        Private _RegionData As String = "Text"

        Private MyFileName As String = "Address.ascx"

#End Region

#Region "Properties"

        Public Property ModuleId() As Integer
            Get
                ModuleId = Convert.ToInt32(ViewState("ModuleId"))
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property
        Public Property LabelColumnWidth() As String
            Get
                LabelColumnWidth = Convert.ToString(ViewState("LabelColumnWidth"))
            End Get
            Set(ByVal Value As String)
                _LabelColumnWidth = Value
            End Set
        End Property
        Public Property ControlColumnWidth() As String
            Get
                ControlColumnWidth = Convert.ToString(ViewState("ControlColumnWidth"))
            End Get
            Set(ByVal Value As String)
                _ControlColumnWidth = Value
            End Set
        End Property
        Public WriteOnly Property StartTabIndex() As Integer
            Set(ByVal Value As Integer)
                _StartTabIndex = Value
            End Set
        End Property


        Public Property Street() As String
            Get
                Street = txtStreet.Text
            End Get
            Set(ByVal Value As String)
                _Street = Value
            End Set
        End Property
        Public Property Unit() As String
            Get
                Unit = txtUnit.Text
            End Get
            Set(ByVal Value As String)
                _Unit = Value
            End Set
        End Property
        Public Property City() As String
            Get
                City = txtCity.Text
            End Get
            Set(ByVal Value As String)
                _City = Value
            End Set
        End Property
        Public Property Country() As String
            Get
                Dim retValue As String = ""
                If Not cboCountry.SelectedItem Is Nothing Then
                    Select Case LCase(_CountryData)
                        Case "text"
                            If cboCountry.SelectedIndex = 0 Then 'Return blank if 'Not_Specified' selected 
                                retValue = ""
                            Else
                                retValue = cboCountry.SelectedItem.Text
                            End If
                        Case "value"
                            retValue = cboCountry.SelectedItem.Value
                    End Select
                End If
                Return retValue
            End Get
            Set(ByVal Value As String)
                _Country = Value
            End Set
        End Property
        Public Property Region() As String
            Get
                Dim retValue As String = ""
                If cboRegion.Visible Then
                    If Not cboRegion.SelectedItem Is Nothing Then
                        Select Case LCase(_RegionData)
                            Case "text"
                                If cboRegion.SelectedIndex > 0 Then
                                    retValue = cboRegion.SelectedItem.Text
                                End If
                            Case "value"
                                retValue = cboRegion.SelectedItem.Value
                        End Select
                    End If
                Else
                    retValue = txtRegion.Text
                End If
                Return retValue
            End Get
            Set(ByVal Value As String)
                _Region = Value
            End Set
        End Property
        Public Property Postal() As String
            Get
                Postal = txtPostal.Text
            End Get
            Set(ByVal Value As String)
                _Postal = Value
            End Set
        End Property
        Public Property Telephone() As String
            Get
                Telephone = txtTelephone.Text
            End Get
            Set(ByVal Value As String)
                _Telephone = Value
            End Set
        End Property
        Public Property Cell() As String
            Get
                Cell = txtCell.Text
            End Get
            Set(ByVal Value As String)
                _Cell = Value
            End Set
        End Property
        Public Property Fax() As String
            Get
                Fax = txtFax.Text
            End Get
            Set(ByVal Value As String)
                _Fax = Value
            End Set
        End Property


        Public WriteOnly Property ShowStreet() As Boolean
            Set(ByVal Value As Boolean)
                _ShowStreet = Value
            End Set
        End Property
        Public WriteOnly Property ShowUnit() As Boolean
            Set(ByVal Value As Boolean)
                _ShowUnit = Value
            End Set
        End Property
        Public WriteOnly Property ShowCity() As Boolean
            Set(ByVal Value As Boolean)
                _ShowCity = Value
            End Set
        End Property
        Public WriteOnly Property ShowCountry() As Boolean
            Set(ByVal Value As Boolean)
                _ShowCountry = Value
            End Set
        End Property
        Public WriteOnly Property ShowRegion() As Boolean
            Set(ByVal Value As Boolean)
                _ShowRegion = Value
            End Set
        End Property
        Public WriteOnly Property ShowPostal() As Boolean
            Set(ByVal Value As Boolean)
                _ShowPostal = Value
            End Set
        End Property
        Public WriteOnly Property ShowTelephone() As Boolean
            Set(ByVal Value As Boolean)
                _ShowTelephone = Value
            End Set
        End Property
        Public WriteOnly Property ShowCell() As Boolean
            Set(ByVal Value As Boolean)
                _ShowCell = Value
            End Set
        End Property
        Public WriteOnly Property ShowFax() As Boolean
            Set(ByVal Value As Boolean)
                _ShowFax = Value
            End Set
        End Property


        Public WriteOnly Property CountryData() As String
            Set(ByVal Value As String)
                _CountryData = Value
            End Set
        End Property
        Public WriteOnly Property RegionData() As String
            Set(ByVal Value As String)
                _RegionData = Value
            End Set
        End Property


        Public ReadOnly Property LocalResourceFile() As String
            Get
                Return Services.Localization.Localization.GetResourceFile(Me, MyFileName)
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Localize correctly sets up the control for US/Canada/Other Countries
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Localize()
            Dim countryCode As String = cboCountry.SelectedItem.Value
            Dim ctlEntry As New ListController
            ' listKey in format "Country.US:Region"
            Dim listKey As String = "Country." & countryCode
            Dim entryCollection As ListEntryInfoCollection = ctlEntry.GetListEntryInfoCollection("Region", listKey)

            If entryCollection.Count <> 0 Then
                cboRegion.Visible = True
                txtRegion.Visible = False
                With cboRegion
                    .Items.Clear()
                    .DataSource = entryCollection
                    .DataBind()
                    cboRegion.Items.Insert(0, New ListItem("<" & Services.Localization.Localization.GetString("Not_Specified", Services.Localization.Localization.SharedResourceFile) & ">", ""))
                End With
                If countryCode.ToLower = "us" Then
                    valRegion1.Enabled = True
                    valRegion2.Enabled = False
                    valRegion1.ErrorMessage = Services.Localization.Localization.GetString("StateRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plRegion.Text = Services.Localization.Localization.GetString("plState", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plRegion.HelpText = Services.Localization.Localization.GetString("plState.Help", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plPostal.Text = Services.Localization.Localization.GetString("plZip", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plPostal.HelpText = Services.Localization.Localization.GetString("plZip.Help", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                Else
                    valRegion1.ErrorMessage = Services.Localization.Localization.GetString("ProvinceRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plRegion.Text = Services.Localization.Localization.GetString("plProvince", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plRegion.HelpText = Services.Localization.Localization.GetString("plProvince.Help", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plPostal.Text = Services.Localization.Localization.GetString("plPostal", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    plPostal.HelpText = Services.Localization.Localization.GetString("plPostal.Help", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                End If
                valRegion1.Enabled = True
                valRegion2.Enabled = False
            Else
                cboRegion.ClearSelection()
                cboRegion.Visible = False
                txtRegion.Visible = True
                valRegion1.Enabled = False
                valRegion2.Enabled = True
                valRegion2.ErrorMessage = Services.Localization.Localization.GetString("RegionRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                plRegion.Text = Services.Localization.Localization.GetString("plRegion", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                plRegion.HelpText = Services.Localization.Localization.GetString("plRegion.Help", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                plPostal.Text = Services.Localization.Localization.GetString("plPostal", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                plPostal.HelpText = Services.Localization.Localization.GetString("plPostal.Help", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
            End If

            If lblRegionRequired.Text = "" Then
                valRegion1.Enabled = False
                valRegion2.Enabled = False
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ShowRequiredFields sets up displaying which fields are required
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ShowRequiredFields()

            Dim settings As Dictionary(Of String, String) = PortalController.GetPortalSettingsDictionary(PortalSettings.PortalId)

            lblStreetRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addressstreet", PortalSettings.PortalId, True), "*", "").ToString
            lblCityRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addresscity", PortalSettings.PortalId, True), "*", "").ToString
            lblCountryRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addresscountry", PortalSettings.PortalId, True), "*", "").ToString
            lblRegionRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addressregion", PortalSettings.PortalId, True), "*", "").ToString
            lblPostalRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addresspostal", PortalSettings.PortalId, True), "*", "").ToString
            lblTelephoneRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addresstelephone", PortalSettings.PortalId, True), "*", "").ToString
            lblCellRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addresscell", PortalSettings.PortalId, True), "*", "").ToString
            lblFaxRequired.Text = IIf(PortalController.GetPortalSettingAsBoolean("addressfax", PortalSettings.PortalId, True), "*", "").ToString

            If TabPermissionController.CanAdminPage() Then
                If lblCountryRequired.Text = "*" Then
                    chkCountry.Checked = True
                    valCountry.Enabled = True
                End If
                If lblRegionRequired.Text = "*" Then
                    chkRegion.Checked = True
                    If cboRegion.Visible = True Then
                        valRegion1.Enabled = True
                        valRegion2.Enabled = False
                    Else
                        valRegion1.Enabled = False
                        valRegion2.Enabled = True
                    End If
                End If
                If lblCityRequired.Text = "*" Then
                    chkCity.Checked = True
                    valCity.Enabled = True
                End If
                If lblStreetRequired.Text = "*" Then
                    chkStreet.Checked = True
                    valStreet.Enabled = True
                End If
                If lblPostalRequired.Text = "*" Then
                    chkPostal.Checked = True
                    valPostal.Enabled = True
                End If
                If lblTelephoneRequired.Text = "*" Then
                    chkTelephone.Checked = True
                    valTelephone.Enabled = True
                End If
                If lblCellRequired.Text = "*" Then
                    chkCell.Checked = True
                    valCell.Enabled = True
                End If
                If lblFaxRequired.Text = "*" Then
                    chkFax.Checked = True
                    valFax.Enabled = True
                End If
            End If

            If lblCountryRequired.Text = "" Then
                valCountry.Enabled = False
            End If
            If lblRegionRequired.Text = "" Then
                valRegion1.Enabled = False
                valRegion2.Enabled = False
            End If
            If lblCityRequired.Text = "" Then
                valCity.Enabled = False
            End If
            If lblStreetRequired.Text = "" Then
                valStreet.Enabled = False
            End If
            If lblPostalRequired.Text = "" Then
                valPostal.Enabled = False
            End If
            If lblTelephoneRequired.Text = "" Then
                valTelephone.Enabled = False
            End If
            If lblCellRequired.Text = "" Then
                valCell.Enabled = False
            End If
            If lblFaxRequired.Text = "" Then
                valFax.Enabled = False
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateRequiredFields updates which fields are required
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UpdateRequiredFields()

            If chkCountry.Checked = False Then
                chkRegion.Checked = False
            End If

            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addressstreet", IIf(chkStreet.Checked, "", "N").ToString)
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresscity", IIf(chkCity.Checked, "", "N").ToString)
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresscountry", IIf(chkCountry.Checked, "", "N").ToString)
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addressregion", IIf(chkRegion.Checked, "", "N").ToString)
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresspostal", IIf(chkPostal.Checked, "", "N").ToString)
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresstelephone", IIf(chkTelephone.Checked, "", "N").ToString)
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresscell", IIf(chkCell.Checked, "", "N").ToString)
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addressfax", IIf(chkFax.Checked, "", "N").ToString)

            ShowRequiredFields()

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                valStreet.ErrorMessage = Services.Localization.Localization.GetString("StreetRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                valCity.ErrorMessage = Services.Localization.Localization.GetString("CityRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                valCountry.ErrorMessage = Services.Localization.Localization.GetString("CountryRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                valPostal.ErrorMessage = Services.Localization.Localization.GetString("PostalRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                valTelephone.ErrorMessage = Services.Localization.Localization.GetString("TelephoneRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                valCell.ErrorMessage = Services.Localization.Localization.GetString("CellRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                valFax.ErrorMessage = Services.Localization.Localization.GetString("FaxRequired", Services.Localization.Localization.GetResourceFile(Me, MyFileName))

                If Not Page.IsPostBack Then

                    If _LabelColumnWidth <> "" Then
                        plCountry.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plRegion.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plCity.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plStreet.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plUnit.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plPostal.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plTelephone.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plCell.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        plFax.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                    End If

                    If _ControlColumnWidth <> "" Then
                        cboCountry.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        cboRegion.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtRegion.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtCity.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtStreet.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtUnit.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtPostal.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtTelephone.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtCell.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtFax.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                    End If

                    txtStreet.TabIndex = Convert.ToInt16(_StartTabIndex)
                    txtUnit.TabIndex = Convert.ToInt16(_StartTabIndex + 1)
                    txtCity.TabIndex = Convert.ToInt16(_StartTabIndex + 2)
                    cboCountry.TabIndex = Convert.ToInt16(_StartTabIndex + 3)
                    cboRegion.TabIndex = Convert.ToInt16(_StartTabIndex + 4)
                    txtRegion.TabIndex = Convert.ToInt16(_StartTabIndex + 5)
                    txtPostal.TabIndex = Convert.ToInt16(_StartTabIndex + 6)
                    txtTelephone.TabIndex = Convert.ToInt16(_StartTabIndex + 7)
                    txtCell.TabIndex = Convert.ToInt16(_StartTabIndex + 8)
                    txtFax.TabIndex = Convert.ToInt16(_StartTabIndex + 9)

                    ' <tam:note modified to test Lists
                    'Dim objRegionalController As New RegionalController
                    'cboCountry.DataSource = objRegionalController.GetCountries
                    ' <this test using method 2: get empty collection then get each entry list on demand & store into cache

                    Dim ctlEntry As New ListController
                    Dim entryCollection As ListEntryInfoCollection = ctlEntry.GetListEntryInfoCollection("Country")

                    cboCountry.DataSource = entryCollection
                    cboCountry.DataBind()
                    cboCountry.Items.Insert(0, New ListItem("<" & Services.Localization.Localization.GetString("Not_Specified", Services.Localization.Localization.SharedResourceFile) & ">", ""))

                    Select Case LCase(_CountryData)
                        Case "text"
                            If _Country = "" Then
                                cboCountry.SelectedIndex = 0
                            Else
                                If Not cboCountry.Items.FindByText(_Country) Is Nothing Then
                                    cboCountry.ClearSelection()
                                    cboCountry.Items.FindByText(_Country).Selected = True
                                End If
                            End If
                        Case "value"
                            If Not cboCountry.Items.FindByValue(_Country) Is Nothing Then
                                cboCountry.ClearSelection()
                                cboCountry.Items.FindByValue(_Country).Selected = True
                            End If
                    End Select

                    Localize()

                    If cboRegion.Visible Then
                        Select Case LCase(_RegionData)
                            Case "text"
                                If _Region = "" Then
                                    cboRegion.SelectedIndex = 0
                                Else
                                    If Not cboRegion.Items.FindByText(_Region) Is Nothing Then
                                        cboRegion.Items.FindByText(_Region).Selected = True
                                    End If
                                End If
                            Case "value"
                                If Not cboRegion.Items.FindByValue(_Region) Is Nothing Then
                                    cboRegion.Items.FindByValue(_Region).Selected = True
                                End If
                        End Select
                    Else
                        txtRegion.Text = _Region
                    End If

                    txtStreet.Text = _Street
                    txtUnit.Text = _Unit
                    txtCity.Text = _City
                    txtPostal.Text = _Postal
                    txtTelephone.Text = _Telephone
                    txtCell.Text = _Cell
                    txtFax.Text = _Fax

                    rowStreet.Visible = _ShowStreet
                    rowUnit.Visible = _ShowUnit
                    rowCity.Visible = _ShowCity
                    rowCountry.Visible = _ShowCountry
                    rowRegion.Visible = _ShowRegion
                    rowPostal.Visible = _ShowPostal
                    rowTelephone.Visible = _ShowTelephone
                    rowCell.Visible = _ShowCell
                    rowFax.Visible = _ShowFax

                    If TabPermissionController.CanAdminPage() Then
                        chkStreet.Visible = True
                        chkCity.Visible = True
                        chkCountry.Visible = True
                        chkRegion.Visible = True
                        chkPostal.Visible = True
                        chkTelephone.Visible = True
                        chkCell.Visible = True
                        chkFax.Visible = True
                    End If

                    ViewState("ModuleId") = Convert.ToString(_ModuleId)
                    ViewState("LabelColumnWidth") = _LabelColumnWidth
                    ViewState("ControlColumnWidth") = _ControlColumnWidth

                    ShowRequiredFields()

                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cboCountry_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCountry.SelectedIndexChanged
            Try
                Localize()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkCity_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCity.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkCountry_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCountry.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkPostal_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkPostal.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkRegion_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRegion.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkStreet_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkStreet.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkTelephone_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTelephone.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkCell_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCell.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkFax_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFax.CheckedChanged
            Try
                UpdateRequiredFields()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
