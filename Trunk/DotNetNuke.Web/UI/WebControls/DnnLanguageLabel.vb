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

Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Globalization
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Personalization

Namespace DotNetNuke.Web.UI.WebControls

    Public Class DnnLanguageLabel
        Inherits CompositeControl
        Implements ILocalizable

#Region "Public Enums"


#End Region

#Region "Controls"

        Dim _Flag As Image
        Dim _Label As Label

#End Region

#Region "Private Members"

        Private _DisplayType As CultureDropDownTypes
        Private _Localize As Boolean = True
        Private _LocalResourceFile As String

#End Region

#Region "Public Properties"

        Public Property DisplayType As CultureDropDownTypes
            Get
                Return _DisplayType
            End Get
            Set(ByVal value As CultureDropDownTypes)
                _DisplayType = value
            End Set
        End Property

        Public Property Language As String
            Get
                Return DirectCast(ViewState("Language"), String)
            End Get
            Set(ByVal value As String)
                ViewState("Language") = value
            End Set
        End Property

#End Region


#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            LocalResourceFile = Utilities.GetLocalResourceFile(Me)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateChildControls overrides the Base class's method to correctly build the
        ''' control based on the configuration
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overloads Overrides Sub CreateChildControls()
            'First clear the controls collection
            Controls.Clear()

            _Flag = New Image
            Me.Controls.Add(_Flag)

            Me.Controls.Add(New LiteralControl("&nbsp;"))

            _Label = New Label
            Me.Controls.Add(_Label)

            'Call base class's method

            MyBase.CreateChildControls()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnPreRender runs just before the control is rendered
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overloads Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            If String.IsNullOrEmpty(Language) Then
                _Flag.ImageUrl = String.Format("~/images/Flags/none.gif", Language)
            Else
                _Flag.ImageUrl = String.Format("~/images/Flags/{0}.gif", Language)
            End If

            If DisplayType = 0 Then
                Dim _PortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
                Dim _ViewTypePersonalizationKey As String = "ViewType" & _PortalSettings.PortalId.ToString
                Dim _ViewType As String = Convert.ToString(Personalization.GetProfile("LanguageDisplayMode", _ViewTypePersonalizationKey))
                Select Case _ViewType
                    Case "NATIVE"
                        DisplayType = CultureDropDownTypes.NativeName
                    Case "ENGLISH"
                        DisplayType = CultureDropDownTypes.EnglishName
                    Case Else
                        DisplayType = CultureDropDownTypes.DisplayName
                End Select

            End If

            Dim localeName As String
            If String.IsNullOrEmpty(Language) Then
                localeName = Localization.GetString("NeutralCulture", Localization.GlobalResourceFile)
            Else
                localeName = Localization.GetLocaleName(Language, DisplayType)
            End If
            _Label.Text = localeName
            _Flag.AlternateText = localeName
        End Sub

#End Region

#Region "ILocalizable Implementation"

        Public Property Localize() As Boolean Implements ILocalizable.Localize
            Get
                Return _Localize
            End Get
            Set(ByVal value As Boolean)
                _Localize = value
            End Set
        End Property

        Public Property LocalResourceFile() As String Implements ILocalizable.LocalResourceFile
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal value As String)
                _LocalResourceFile = value
            End Set
        End Property

        Protected Overridable Sub LocalizeStrings() Implements ILocalizable.LocalizeStrings
        End Sub

#End Region

    End Class
End Namespace
