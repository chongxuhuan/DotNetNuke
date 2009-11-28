'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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
Imports DotNetNuke.Entities.Modules.Actions

Namespace DotNetNuke.Modules.Admin.Vendors

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The Affiliates PortalModuleBase is used to manage a Vendor's Affiliates
	''' </summary>
	''' <returns></returns>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
	'''                       and localisation
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial  Class Affiliates

		Inherits Entities.Modules.PortalModuleBase
		Implements Entities.Modules.IActionable

#Region "Controls"


#End Region

#Region "Private Members"

		Public VendorID As Integer

#End Region

#Region "Private Methods"

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' BindData gets the affiliates from the Database and binds them to the DataGrid
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub BindData()

			Dim objAffiliates As New Services.Vendors.AffiliateController

			'Localize the Grid
			Services.Localization.Localization.LocalizeDataGrid(grdAffiliates, Me.LocalResourceFile)

			grdAffiliates.DataSource = objAffiliates.GetAffiliates(VendorID)
			grdAffiliates.DataBind()

            cmdAdd.NavigateUrl = FormatURL("AffilId", "-1")
		End Sub

#End Region

#Region "Public Methods"

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' DisplayDate formats a Date
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <paam name="DateValue">The Date to format</param>
		''' <returns>The correctly formatted date</returns>
		''' <history>
		''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Public Function DisplayDate(ByVal DateValue As Date) As String
            If Null.IsNull(DateValue) Then
                Return ""
            Else
                Return DateValue.ToShortDateString
            End If
        End Function

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' FormatURL correctly formats the Url (adding a key/Value pair)
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <paam name="strKeyName">The name of the key to add</param>
		''' <paam name="strKeyValue">The value to add</param>
		''' <returns>The correctly formatted url</returns>
		''' <history>
		''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Public Function FormatURL(ByVal strKeyName As String, ByVal strKeyValue As String) As String
			Return EditUrl(strKeyName, strKeyValue, "Affiliate", "VendorId=" & VendorID.ToString())
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

			Catch exc As Exception			 'Module failed to load
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
                Actions.Add(GetNextActionID, Services.Localization.Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl("VendorId", VendorID.ToString, "Affiliate"), False, SecurityAccessLevel.Admin, Null.IsNull(VendorID) = False, False)
                Return Actions
			End Get
		End Property
#End Region

#Region " Web Form Designer Generated Code "

		'This call is required by the Web Form Designer.
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

		End Sub

		Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
			'CODEGEN: This method call is required by the Web Form Designer
			'Do not modify it using the code editor.
			InitializeComponent()
		End Sub

#End Region

	End Class

End Namespace