'
' DotNetNuke - http://www.dotnetnuke.com
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
Imports System.ComponentModel
Imports System.Web.UI.WebControls

Namespace DotNetNuke.Web.UI.WebControls

    Public Class DnnImageTextButton
        Inherits System.Web.UI.WebControls.ImageButton
        Implements DotNetNuke.Web.UI.ILocalize

        Public Sub New()
            CssClass = "dnnImageTextButton"
            DisabledCssClass = "dnnImageTextButton disabled"
        End Sub

		<Bindable(True)> _
		<Category("Appearance")> _
		<DefaultValue("")> _
		<Localizable(True)> _
		Public Shadows Property ConfirmMessage() As String
			Get
				Return If(ViewState("ConfirmMessage") Is Nothing, String.Empty, DirectCast(ViewState("ConfirmMessage"), String))
			End Get
			Set(ByVal value As String)
				ViewState("ConfirmMessage") = value
			End Set
		End Property

        <Bindable(True)> _
        <Category("Appearance")> _
        <DefaultValue("")> _
        <Localizable(True)> _
        Public Shadows Property Text() As String
            Get
                Return If(ViewState("Text") Is Nothing, String.Empty, DirectCast(ViewState("Text"), String))
            End Get
            Set(ByVal value As String)
                ViewState("Text") = value
            End Set
        End Property

        <Bindable(True)> _
        <Category("Behavior")> _
        <DefaultValue("")> _
        <Localizable(True)> _
        Public Property DisabledImageUrl() As String
            Get
                Return If(ViewState("DisabledImageUrl") Is Nothing, String.Empty, DirectCast(ViewState("DisabledImageUrl"), String))
            End Get
            Set(ByVal value As String)
                ViewState("DisabledImageUrl") = value
            End Set
        End Property

        <Bindable(True)> _
        <Category("Appearance")> _
        <DefaultValue("")> _
        <Localizable(True)> _
        Public Overrides Property CssClass() As String
            Get
                Return If(ViewState("CssClass") Is Nothing, String.Empty, DirectCast(ViewState("CssClass"), String))
            End Get
            Set(ByVal value As String)
                ViewState("CssClass") = value
            End Set
        End Property

        <Bindable(True)> _
        <Category("Appearance")> _
        <DefaultValue("")> _
        <Localizable(True)> _
        Public Property DisabledCssClass() As String
            Get
                Return If(ViewState("DisabledCssClass") Is Nothing, String.Empty, DirectCast(ViewState("DisabledCssClass"), String))
            End Get
            Set(ByVal value As String)
                ViewState("DisabledCssClass") = value
            End Set
        End Property

        Private _TextLinkControl As HyperLink = Nothing
        Private ReadOnly Property TextLinkControl() As HyperLink
            Get
                If _TextLinkControl Is Nothing Then
                    _TextLinkControl = New HyperLink()
                End If
                Return _TextLinkControl
            End Get
        End Property

        Public Overrides Property ImageUrl() As String
            Get
                Return MyBase.ImageUrl
            End Get
            Set(ByVal value As String)
                MyBase.ImageUrl = value
            End Set
		End Property

		Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
			MyBase.OnPreRender(e)

			If (Not String.IsNullOrEmpty(ConfirmMessage)) Then
				Dim msg As String = ConfirmMessage
				If (Localize) Then
					msg = Utilities.GetLocalizedStringFromParent(ConfirmMessage, Me)
				End If
				'must be done before render
				OnClientClick = Utilities.GetOnClientClickConfirm(Me, msg)
			End If
		End Sub

		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
			LocalizeStrings()

			Dim textLink As New HyperLink()
			textLink.Text = Text
			textLink.ToolTip = ToolTip

			If (Not Enabled) Then
				If (Not String.IsNullOrEmpty(DisabledCssClass)) Then
					CssClass = DisabledCssClass
				End If
				ImageUrl = GetImageUrl(Enabled)
				textLink.NavigateUrl = "javascript:void(0);"
			Else
				textLink.NavigateUrl = "javascript:document.getElementById('" + ClientID + "').click();"
			End If

			writer.AddAttribute("class", CssClass)
			writer.RenderBeginTag("span")

			MyBase.Render(writer)
			textLink.RenderControl(writer)

			writer.RenderEndTag()
		End Sub

        Private Function GetImageUrl(ByVal enabled As Boolean) As String
            If (Not enabled AndAlso Not String.IsNullOrEmpty(Me.DisabledImageUrl)) Then
                Return DisabledImageUrl
            ElseIf (Not String.IsNullOrEmpty(ImageUrl)) Then
                Dim ext As String = System.IO.Path.GetExtension(ImageUrl)
                Dim imageName As String = System.IO.Path.GetFileNameWithoutExtension(ImageUrl)
                Dim fullImgName As String = System.IO.Path.GetFileName(ImageUrl)

                If (Not String.IsNullOrEmpty(ext) AndAlso Not String.IsNullOrEmpty(imageName) AndAlso Not String.IsNullOrEmpty(fullImgName)) Then
                    Try
                        Dim disabledImg As String = ImageUrl.Replace(fullImgName, imageName + "_Disabled" + ext)
                        If (System.IO.File.Exists(Page.Server.MapPath(disabledImg))) Then
                            Return disabledImg
                        End If
                    Catch ex As Exception
                        'return default
                    End Try
                End If
            End If

            Return ImageUrl
        End Function

#Region "Implements ILocalize"
        Private _Localize As Boolean = True
        Public Property Localize() As Boolean Implements ILocalize.Localize
            Get
                Return _Localize
            End Get
            Set(ByVal value As Boolean)
                _Localize = value
            End Set
        End Property

        Protected Overridable Sub LocalizeStrings() Implements ILocalize.LocalizeStrings
			If (Localize) Then
				If (Not String.IsNullOrEmpty(ToolTip)) Then
					ToolTip = Utilities.GetLocalizedStringFromParent(ToolTip, Me)
				End If

				If (Not String.IsNullOrEmpty(Text)) Then
					Text = Utilities.GetLocalizedStringFromParent(Text, Me)

					If (String.IsNullOrEmpty(ToolTip)) Then
						ToolTip = Utilities.GetLocalizedStringFromParent(Text + ".ToolTip", Me)
					End If

					If (String.IsNullOrEmpty(ToolTip)) Then
						ToolTip = Text
					End If
				End If
			End If
        End Sub
#End Region

    End Class

End Namespace
