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

Imports DotNetNuke.Services.Vendors
Imports DotNetNuke.Entities.Modules.Actions

Namespace DotNetNuke.Modules.Admin.Vendors

    Partial Class DisplayBanners
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

#Region "Event Handlers"

        ''' <summary>
        ''' The Page_Load event handler on this User Control is used to
        ''' obtain a DataReader of banner information from the Banners
        ''' table, and then databind the results to a templated DataList
        ''' server control.  It uses the DotNetNuke.BannerDB()
        ''' data component to encapsulate all data functionality.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'exit without displaying banners to crawlers
            If Request.Browser.Crawler = True Then Exit Sub

            Try
                Dim intPortalId As Integer
                Dim intBannerTypeId As Integer
                Dim strBannerGroup As String
                Dim intBanners As Integer = 0

                ' banner parameters
                Select Case CType(Settings("bannersource"), String)
                    Case "L", ""    ' local
                        intPortalId = PortalId
                    Case "G"    ' global
                        intPortalId = Null.NullInteger
                End Select
                If CType(Settings("bannertype"), String) <> "" Then
                    intBannerTypeId = Int32.Parse(CType(Settings("bannertype"), String))
                End If
                strBannerGroup = CType(Settings("bannergroup"), String)
                If CType(Settings("bannercount"), String) <> "" Then
                    intBanners = Int32.Parse(CType(Settings("bannercount"), String))
                End If
                If CType(Settings("padding"), String) <> "" Then
                    lstBanners.CellPadding = Int32.Parse(CType(Settings("padding"), String))
                End If

                ' load banners
                If intBanners <> 0 Then
                    Dim objBanners As New Services.Vendors.BannerController
                    lstBanners.DataSource = objBanners.LoadBanners(intPortalId, ModuleId, intBannerTypeId, strBannerGroup, intBanners)
                    lstBanners.DataBind()
                End If

                ' set banner display characteristics
                If lstBanners.Items.Count <> 0 Then
                    ' container attributes
                    lstBanners.RepeatLayout = RepeatLayout.Table
                    If CType(Settings("orientation"), String) <> "" Then
                        Select Case CType(Settings("orientation"), String)
                            Case "H"
                                lstBanners.RepeatDirection = RepeatDirection.Horizontal
                            Case "V"
                                lstBanners.RepeatDirection = RepeatDirection.Vertical
                        End Select
                    Else
                        lstBanners.RepeatDirection = RepeatDirection.Vertical
                    End If
                    If CType(Settings("border"), String) <> "" Then
                        lstBanners.ItemStyle.BorderWidth = Unit.Parse(CType(Settings("border"), String) & "px")
                    End If
                    If CType(Settings("bordercolor"), String) <> "" Then
                        Dim objColorConverter As New System.Drawing.ColorConverter
                        lstBanners.ItemStyle.BorderColor = CType(objColorConverter.ConvertFrom(CType(Settings("bordercolor"), String)), System.Drawing.Color)
                    End If

                    ' item attributes
                    If CType(Settings("rowheight"), String) <> "" Then
                        lstBanners.ItemStyle.Height = Unit.Parse(CType(Settings("rowheight"), String) & "px")
                    End If
                    If CType(Settings("colwidth"), String) <> "" Then
                        lstBanners.ItemStyle.Width = Unit.Parse(CType(Settings("colwidth"), String) & "px")
                    End If
                Else
                    lstBanners.Visible = False
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

    
#End Region

#Region "Public Methods"
        Public Function FormatItem(ByVal VendorId As Integer, ByVal BannerId As Integer, ByVal BannerTypeId As Integer, ByVal BannerName As String, ByVal ImageFile As String, ByVal Description As String, ByVal URL As String, ByVal Width As Integer, ByVal Height As Integer) As String
            Dim objBanners As New BannerController
            Return objBanners.FormatBanner(VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, CType(Settings("bannersource"), String), PortalSettings.HomeDirectory, CType(Settings("bannerclickthroughurl"), String))
        End Function
#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
                Actions.Add(GetNextActionID, Services.Localization.Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl(), False, SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property
#End Region

    End Class

End Namespace
