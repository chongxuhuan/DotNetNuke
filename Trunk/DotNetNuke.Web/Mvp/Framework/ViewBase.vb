'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports System.Globalization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.UI.Skins.Controls
Imports System.Web

Namespace DotNetNuke.Web.Mvp.Framework

    Public MustInherit Class ViewBase(Of TView As IView, _
                              TPresenter As {Presenter(Of TView, TPresenterModel), New}, _
                              TPresenterModel As {PresenterModel, New})
        Inherits ModuleUserControlBase
        Implements IView

#Region "Private Members"

        Private _Presenter As TPresenter

#End Region

#Region "IView Implementation"

        Protected MustOverride Sub Localize() Implements IView.Localize

        Public Sub ShowMessage(ByVal resourceKey As String, ByVal messageType As ModuleMessage.ModuleMessageType) Implements IView.ShowMessage
            Dim heading As String = Localization.GetString(String.Format(CultureInfo.InvariantCulture, "{0}.MessageHeader", resourceKey), LocalResourceFile)
            Dim message As String = Localization.GetString(String.Format(CultureInfo.InvariantCulture, "{0}.Message", resourceKey), LocalResourceFile)
            If (String.IsNullOrEmpty(heading) OrElse heading.StartsWith("RESX:", StringComparison.OrdinalIgnoreCase)) Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, message, messageType)
            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, heading, message, messageType)
            End If
        End Sub

#End Region

#Region "Protected Properties"

        Protected Property Presenter() As TPresenter
            Get
                Return _Presenter
            End Get
            Private Set(ByVal value As TPresenter)
                _Presenter = value
            End Set
        End Property

        'HACK: Think this is the only way to do this, since I can't seem to cast "Me" to TView anywhere :(
        Protected MustOverride ReadOnly Property View() As TView

        Protected Overridable ReadOnly Property ViewName() As String
            Get
                Return ID
            End Get
        End Property

#End Region

#Region "Public Properties"

        Public Shadows ReadOnly Property Context() As HttpContextBase
            Get
                Return New HttpContextWrapper(MyBase.Context)
            End Get
        End Property

#End Region

        Private Function RunIfValid(ByVal func As Func(Of TPresenter, Boolean), ByVal validationGroup As String) As Boolean
            If (String.IsNullOrEmpty(validationGroup)) Then
                Page.Validate()
            Else
                Page.Validate(validationGroup)
            End If
            If (Page.IsValid) Then
                Return func.Invoke(Presenter)
            Else
                Return Nothing
            End If
        End Function

#Region "Protected Methods"

        Protected Overridable Sub ConnectEvents()
        End Sub

        Protected Overridable Function CreateSaveHandler(ByVal presenterFunction As Func(Of TPresenter, Boolean)) As EventHandler
            Return CreateSaveHandler(presenterFunction, Nothing)
        End Function

        Protected Overridable Function CreateSaveHandler(ByVal presenterFunction As Func(Of TPresenter, Boolean), ByVal validationGroup As String) As EventHandler
            Return Function(sender, args) Presenter.RunWithAuthPolicy(Function(func1) RunIfValid(Function(func2) presenterFunction(Presenter), validationGroup))
        End Function

        Protected Overridable Function CreateSimpleHandler(ByVal presenterFunction As Func(Of TPresenter, Boolean)) As EventHandler
            Return Function(sender, args) presenterFunction(Presenter)
        End Function

        Protected Overridable Function CreateSimpleHandler(Of TEventArgs As EventArgs)(ByVal presenterFunction As Func(Of TPresenter, TEventArgs, Boolean)) As EventHandler(Of TEventArgs)
            Return Function(sender, args) presenterFunction(Presenter, args)
        End Function

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            MyBase.LoadViewState(savedState)

            'Restore the Presenter's State
            Presenter.RestoreState(ViewState)
        End Sub

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            Dim model As TPresenterModel = New TPresenterModel()
            model.LoadFromView(Me)

            Presenter = New TPresenter()
            Presenter.Initialize(View, model, New PresenterEnvironment(ModuleContext, Context))

            'Call base class
            MyBase.OnInit(e)

            ConnectEvents()
            SetupClientScript()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            'Call localalize to carry out any localization
            Localize()

            Presenter.LoadInternal()

            'Call base class
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Function SaveViewState() As Object
            'Get the ViewState from the Presenter
            Presenter.SaveState(ViewState)

            'Call the base class to save the View State
            Return MyBase.SaveViewState()
        End Function

        Protected Overridable Sub SetupClientScript()
            'jQuery.RequestRegistration();

            'string jsFileUrl = String.Format(CultureInfo.InvariantCulture,
            '                                 "{0}.js",
            '                                 AppRelativeVirtualPath);
            'string jsFilePath = Server.MapPath(jsFileUrl);
            'if (File.Exists(jsFilePath)) {
            '    Page.ClientScript.RegisterClientScriptInclude(GetType(),
            '                                                  "CoreJS",
            '                                                  ResolveClientUrl(jsFileUrl));
            '    Page.ClientScript.RegisterClientScriptBlock(GetType(),
            '                                                "CoreJSStartup",
            '                                                String.Format(CultureInfo.InvariantCulture, "$(document).ready({0}_Load)", ViewName),
            '                                                true);
            '}
        End Sub


#End Region

    End Class

End Namespace

