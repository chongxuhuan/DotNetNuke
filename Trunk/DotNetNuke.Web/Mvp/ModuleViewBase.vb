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

Imports WebFormsMvp.Web
Imports DotNetNuke.UI.Modules
Imports System.Globalization
Imports WebFormsMvp
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Web.Mvp
    Public MustInherit Class ModuleViewBase
        Inherits ModuleUserControlBase
        Implements IModuleViewBase

#Region "Private Members"

        Private _AutoDataBind As Boolean
#End Region

#Region "Constructors"

        Protected Sub New()
            AutoDataBind = True
        End Sub

#End Region

#Region "Public Properties"

        Public Property AutoDataBind() As Boolean
            Get
                Return _AutoDataBind
            End Get
            Set(ByVal value As Boolean)
                _AutoDataBind = value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        Protected Function DataItem(Of T As {Class, New})() As T
            Dim _T As T = TryCast(Page.GetDataItem(), T)
            If _T Is Nothing Then
                _T = New T
            End If
            Return _T
        End Function

        Protected Function DataValue(Of T)() As T
            Return DirectCast(Page.GetDataItem(), T)
        End Function

        Protected Function DataValue(Of T)(ByVal format As String) As String
            Return String.Format(CultureInfo.CurrentCulture, format, DataValue(Of T))
        End Function



        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            PageViewHost.Register(Me, Context, False)

            MyBase.OnInit(e)

            AddHandler Page.InitComplete, AddressOf Page_InitComplete
            AddHandler Page.PreRenderComplete, AddressOf Page_PreRenderComplete
            AddHandler Page.Load, AddressOf Page_Load
        End Sub


#End Region

#Region "IModuleView(Of TModel) Implementation"

        Public Event Initialize As EventHandler Implements IModuleViewBase.Initialize

        Public Sub ProcessModuleLoadException(ByVal ex As Exception) Implements IModuleViewBase.ProcessModuleLoadException
            DotNetNuke.Services.Exceptions.ProcessModuleLoadException(Me, ex)
        End Sub

        Public Sub ShowMessage(ByVal messageHeader As String, ByVal message As String, ByVal messageType As ModuleMessageType) Implements IModuleViewBase.ShowMessage
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, messageHeader, message, messageType)
        End Sub

#End Region

#Region "IView(Of TModel) Implementation"

        Public Shadows Event Load(ByVal sender As Object, ByVal e As System.EventArgs) Implements IView.Load


#End Region

#Region "Event Handlers"

        Private Sub Page_InitComplete(ByVal sender As Object, ByVal e As System.EventArgs)
            'Raise Initialize Event to run any initialization code
            RaiseEvent Initialize(Me, EventArgs.Empty)
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
            RaiseEvent Load(Me, e)
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As EventArgs)
            'This event is raised after any async page tasks have completed, so it
            'is safe to data-bind
            If (AutoDataBind) Then DataBind()
        End Sub

#End Region

    End Class
End Namespace

