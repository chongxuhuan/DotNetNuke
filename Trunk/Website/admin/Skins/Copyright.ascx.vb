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

Namespace DotNetNuke.UI.Skins.Controls
    ''' -----------------------------------------------------------------------------
    ''' <summary></summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    '''                             brackets from property names
    ''' </history>
    ''' -----------------------------------------------------------------------------


    Partial  Class Copyright

        Inherits UI.Skins.SkinObjectBase

        ' private members
        Private _cssClass As String

        Const MyFileName As String = "Copyright.ascx"

#Region "Public Members"

        Public Property CssClass() As String
            Get
                Return _cssClass
            End Get
            Set(ByVal Value As String)
                _cssClass = Value
            End Set
        End Property

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' public attributes
            If CssClass <> "" Then
                lblCopyright.CssClass = CssClass
            End If

            If PortalSettings.FooterText <> "" Then
                lblCopyright.Text = PortalSettings.FooterText
            Else
                lblCopyright.Text = String.Format(Services.Localization.Localization.GetString("Copyright", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), Year(Now()), PortalSettings.PortalName)
            End If

        End Sub

    End Class

End Namespace
