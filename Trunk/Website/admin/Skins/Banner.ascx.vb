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
Imports System.Collections.Generic

Namespace DotNetNuke.UI.Skins.Controls
    ''' -----------------------------------------------------------------------------
    ''' <summary></summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    '''                             brackets from property names
    ''' </history>
    ''' -----------------------------------------------------------------------------

    Public MustInherit Class Banner

        Inherits UI.Skins.SkinObjectBase

        ' private members
        Private _AllowNullBannerType As Boolean = False
        Private _groupName As String
        Private _bannerTypeId As String
        Private _bannerCount As String
        Private _width As String
        Private _orientation As String
        Private _borderWidth As String
        Private _borderColor As String
        Private _rowHeight As String
        Private _colWidth As String

        Const MyFileName As String = "Banner.ascx"

#Region "Public Members"

        Public Property AllowNullBannerType As Boolean
            Get
                Return _AllowNullBannerType
            End Get
            Set(ByVal Value As Boolean)
                _AllowNullBannerType = Value
            End Set
        End Property


        Public Property GroupName() As String
            Get
                Return _groupName
            End Get
            Set(ByVal Value As String)
                _groupName = Value
            End Set
        End Property

        Public Property BannerTypeId() As String
            Get
                Return _bannerTypeId
            End Get
            Set(ByVal Value As String)
                _bannerTypeId = Value
            End Set
        End Property

        Public Property BannerCount() As String
            Get
                Return _bannerCount
            End Get
            Set(ByVal Value As String)
                _bannerCount = Value
            End Set
        End Property

        Public Property Width() As String
            Get
                Return _width
            End Get
            Set(ByVal Value As String)
                _width = Value
            End Set
        End Property

        Public Property Orientation() As String
            Get
                Return _orientation
            End Get
            Set(ByVal Value As String)
                _orientation = Value
            End Set
        End Property

        Public Property BorderWidth() As String
            Get
                Return _borderWidth
            End Get
            Set(ByVal Value As String)
                _borderWidth = Value
            End Set
        End Property

        Public Property BorderColor() As String
            Get
                Return _borderColor
            End Get
            Set(ByVal Value As String)
                _borderColor = Value
            End Set
        End Property

        Public Property RowHeight() As String
            Get
                Return _rowHeight
            End Get
            Set(ByVal Value As String)
                _rowHeight = Value
            End Set
        End Property

        Public Property ColWidth() As String
            Get
                Return _colWidth
            End Get
            Set(ByVal Value As String)
                _colWidth = Value
            End Set
        End Property
#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If PortalSettings.BannerAdvertising <> 0 And Me.Visible = True Then

                Dim BannerType As Integer
                If AllowNullBannerType Then
                    If Not String.IsNullOrEmpty(BannerTypeId) Then
                        BannerType = Int32.Parse(CType(BannerTypeId, String))
                    End If
                Else
                    If String.IsNullOrEmpty(BannerTypeId) Then
                        BannerType = PortalController.GetPortalSettingAsInteger("BannerTypeId", PortalSettings.PortalId, 1)
                    End If
                End If

                ' public attributes
                If GroupName = "" Then
                    GroupName = PortalController.GetPortalSetting("BannerGroupName", PortalSettings.PortalId, "")
                End If
                If BannerCount = "" Then
                    BannerCount = "1"
                End If

                Dim intPortalId As Integer
                If PortalSettings.BannerAdvertising = 1 Then
                    intPortalId = PortalSettings.PortalId ' portal
                Else
                    intPortalId = Null.NullInteger ' host
                End If

                ' load banners
                Dim objBanners As New BannerController
                Dim arrBanners As ArrayList = objBanners.LoadBanners(intPortalId, Null.NullInteger, BannerType, GroupName, Integer.Parse(BannerCount))

                ' bind to datalist
                lstBanners.DataSource = arrBanners
                lstBanners.DataBind()

                ' set banner display characteristics
                If lstBanners.Items.Count <> 0 Then
                    ' container attributes
                    lstBanners.RepeatLayout = RepeatLayout.Table
                    If Width <> "" Then
                        lstBanners.Width = Unit.Parse(Width & "px")
                    End If
                    If lstBanners.Items.Count = 1 Then
                        lstBanners.CellPadding = 0
                        lstBanners.CellSpacing = 0
                    Else
                        lstBanners.CellPadding = 4
                        lstBanners.CellSpacing = 0
                    End If

                    If Orientation <> "" Then
                        Select Case Orientation
                            Case "H"
                                lstBanners.RepeatDirection = RepeatDirection.Horizontal
                            Case "V"
                                lstBanners.RepeatDirection = RepeatDirection.Vertical
                        End Select
                    Else
                        lstBanners.RepeatDirection = RepeatDirection.Vertical
                    End If

                    If BorderWidth <> "" Then
                        lstBanners.ItemStyle.BorderWidth = Unit.Parse(BorderWidth & "px")
                    End If
                    If BorderColor <> "" Then
                        Dim objColorConverter As New System.Drawing.ColorConverter
                        lstBanners.ItemStyle.BorderColor = CType(objColorConverter.ConvertFrom(BorderColor), System.Drawing.Color)
                    End If

                    ' item attributes
                    If RowHeight <> "" Then
                        lstBanners.ItemStyle.Height = Unit.Parse(RowHeight & "px")
                    End If
                    If ColWidth <> "" Then
                        lstBanners.ItemStyle.Width = Unit.Parse(ColWidth & "px")
                    End If
                Else
                    lstBanners.Visible = False
                End If
            Else
                lstBanners.Visible = False
            End If

        End Sub
#End Region

#Region "Public Methods"
        Public Function FormatItem(ByVal VendorId As Integer, ByVal BannerId As Integer, ByVal BannerTypeId As Integer, ByVal BannerName As String, ByVal ImageFile As String, ByVal Description As String, ByVal URL As String, ByVal Width As Integer, ByVal Height As Integer) As String
            Dim objBanners As New BannerController
            Return objBanners.FormatBanner(VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, CType(IIf(PortalSettings.BannerAdvertising = 1, "L", "G"), String), PortalSettings.HomeDirectory)
        End Function
#End Region

    End Class
End Namespace
