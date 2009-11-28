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

Imports System.Collections.Specialized
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Framework


Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DNNLocaleEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNLocaleEditControl control provides a standard UI component for selecting
    ''' a Locale
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/23/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:DNNLocaleEditControl runat=server></{0}:DNNLocaleEditControl>")> _
    Public Class DNNLocaleEditControl
        Inherits TextEditControl
        Implements IPostBackEventHandler

        Private _ListType As LanguagesListType = LanguagesListType.Enabled
        Private _DisplayMode As String = "Native"

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a DNNLocaleEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

        Protected ReadOnly Property ListType() As LanguagesListType
            Get
                Return _ListType
            End Get
        End Property

        Protected ReadOnly Property DisplayMode() As String
            Get
                Return _DisplayMode
            End Get
        End Property

        Private Sub RenderModeButtons(ByVal writer As HtmlTextWriter)

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            If DisplayMode = "English" Then
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            Else
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, Page.ClientScript.GetPostBackEventReference(Me, "English"))
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()

            writer.Write(Localization.GetString("DisplayEnglish", Localization.SharedResourceFile))

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            If DisplayMode = "Native" Then
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            Else
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, Page.ClientScript.GetPostBackEventReference(Me, "Native"))
            End If
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()

            writer.Write(Localization.GetString("DisplayNative", Localization.SharedResourceFile))

        End Sub

        Private Sub RenderOption(ByVal writer As HtmlTextWriter, ByVal culture As CultureInfo)
            Dim localeValue As String = Convert.ToString(culture.Name)
            Dim isSelected As Boolean = (localeValue = StringValue)
            Dim localeName As String

            If DisplayMode = "Native" Then
                localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(culture.NativeName)
            Else
                localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(culture.EnglishName)
            End If

            'Add the Value Attribute
            writer.AddAttribute(HtmlTextWriterAttribute.Value, localeValue)

            If isSelected Then
                'Add the Selected Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
            End If

            'Render Option Tag
            writer.RenderBeginTag(HtmlTextWriterTag.Option)
            writer.Write(localeName)
            writer.RenderEndTag()

        End Sub


#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnAttributesChanged runs when the CustomAttributes property has changed.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/18/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnAttributesChanged()
            'Get the List settings out of the "Attributes"
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As System.Attribute In CustomAttributes
                    Dim listAtt As LanguagesListTypeAttribute = TryCast(attribute, LanguagesListTypeAttribute)
                    If listAtt IsNot Nothing Then
                        _ListType = listAtt.ListType
                        Exit For
                    End If
                Next
            End If
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	05/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As HtmlTextWriter)
            Dim locale As Locale = Localization.GetLocale(StringValue)

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            If locale IsNot Nothing Then
                writer.Write(locale.Text)
            End If
            writer.RenderEndTag()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)

            'Render div
            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Div)

            'Render Button Row
            RenderModeButtons(writer)

            'Render break
            writer.WriteBreak()

            'Render the Select Tag
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Select)

            Select Case ListType
                Case LanguagesListType.All
                    Dim cultures As CultureInfo() = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    Array.Sort(cultures, New CultureInfoComparer(DisplayMode))

                    'Render None selected option
                    'Add the Value Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString)

                    If StringValue = Null.NullString Then
                        'Add the Selected Attribute
                        writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
                    End If
                    writer.RenderBeginTag(HtmlTextWriterTag.Option)
                    writer.Write("<" & Localization.GetString("Not_Specified", Localization.SharedResourceFile) & ">")
                    writer.RenderEndTag()

                    For Each culture As CultureInfo In cultures
                        RenderOption(writer, culture)
                    Next
                Case LanguagesListType.Supported
                    For Each language As Locale In Localization.GetLocales(Null.NullInteger).Values
                        RenderOption(writer, CultureInfo.CreateSpecificCulture(language.Code))
                    Next
                Case LanguagesListType.Enabled
                    Dim settings As PortalSettings = PortalController.GetCurrentPortalSettings()
                    For Each language As Locale In Localization.GetLocales(settings.PortalId).Values
                        RenderOption(writer, CultureInfo.CreateSpecificCulture(language.Code))
                    Next
            End Select

            'Close Select Tag
            writer.RenderEndTag()

            'Clode Div
            writer.RenderEndTag()
        End Sub

#End Region

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            _DisplayMode = eventArgument
        End Sub

    End Class

End Namespace

