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
Imports DotNetNuke.UI
Imports DotNetNuke.Entities.Modules.Actions

Namespace DotNetNuke.Modules.Admin.Vendors

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The Banners PortalModuleBase is used to manage a Vendor's Banners
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
	'''                       and localisation
	''' </history>
	''' -----------------------------------------------------------------------------
    Partial Class Banners

        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

#Region "Public Properties"

        Public VendorID As Integer

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData gets the banners from the Database and binds them to the DataGrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()
            Dim objBanners As New Services.Vendors.BannerController

            'Localize the Grid
            Services.Localization.Localization.LocalizeDataGrid(grdBanners, Me.LocalResourceFile)

            grdBanners.DataSource = objBanners.GetBanners(VendorID)
            grdBanners.DataBind()

            cmdAdd.NavigateUrl = FormatURL("BannerId", "-1")
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayDate formats a Date
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="DateValue">The Date to format</param>
        ''' <returns>The correctly formatted date</returns>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayDate(ByVal DateValue As Date) As String
            Dim _DisplayDate As String = Null.NullString
            Try
                If Null.IsNull(DateValue) Then
                    _DisplayDate = ""
                Else
                    _DisplayDate = DateValue.ToShortDateString
                End If

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _DisplayDate
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayDate formats a Date
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <returns>The correctly formatted date</returns>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayType(ByVal BannerTypeId As Integer) As String
            Dim _DisplayType As String = Null.NullString
            Try
                Select Case BannerTypeId
                    Case Services.Vendors.BannerType.Banner : _DisplayType = Services.Localization.Localization.GetString("BannerType.Banner.String", Services.Localization.Localization.GlobalResourceFile)
                    Case Services.Vendors.BannerType.MicroButton : _DisplayType = Services.Localization.Localization.GetString("BannerType.MicroButton.String", Services.Localization.Localization.GlobalResourceFile)
                    Case Services.Vendors.BannerType.Button : _DisplayType = Services.Localization.Localization.GetString("BannerType.Button.String", Services.Localization.Localization.GlobalResourceFile)
                    Case Services.Vendors.BannerType.Block : _DisplayType = Services.Localization.Localization.GetString("BannerType.Block.String", Services.Localization.Localization.GlobalResourceFile)
                    Case Services.Vendors.BannerType.Skyscraper : _DisplayType = Services.Localization.Localization.GetString("BannerType.Skyscraper.String", Services.Localization.Localization.GlobalResourceFile)
                    Case Services.Vendors.BannerType.Text : _DisplayType = Services.Localization.Localization.GetString("BannerType.Text.String", Services.Localization.Localization.GlobalResourceFile)
                    Case Services.Vendors.BannerType.Script : _DisplayType = Services.Localization.Localization.GetString("BannerType.Script.String", Services.Localization.Localization.GlobalResourceFile)
                End Select

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _DisplayType
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatURL correctly formats the Url (adding a key/Value pair)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strKeyName">The name of the key to add</param>
        ''' <param name="strKeyValue">The value to add</param>
        ''' <returns>The correctly formatted url</returns>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FormatURL(ByVal strKeyName As String, ByVal strKeyValue As String) As String
            Return EditUrl(strKeyName, strKeyValue, "Banner", "VendorId=" & VendorID.ToString())
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Not Null.IsNull(VendorID) Then
                    BindData()
                Else
                    Me.Visible = False
                End If

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                If Not (Request.QueryString("VendorID") Is Nothing) Then
                    VendorID = Int32.Parse(Request.QueryString("VendorID"))
                Else
                    VendorID = Null.NullInteger
                End If
                Actions.Add(GetNextActionID, Services.Localization.Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl("VendorID", VendorID.ToString, "Banner"), False, SecurityAccessLevel.Admin, Null.IsNull(VendorID) = False, False)
                Return Actions
            End Get
        End Property
#End Region

    End Class

End Namespace