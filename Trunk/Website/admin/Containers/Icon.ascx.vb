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

Namespace DotNetNuke.UI.Containers
	''' -----------------------------------------------------------------------------
	''' Project	 : DotNetNuke
	''' Class	 : DotNetNuke.UI.Containers.Icon
	''' 
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' Contains the attributes of an Icon.  
	''' These are read into the PortalModuleBase collection as attributes for the icons within the module controls.
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[sun1]	    2/1/2004	Created
	''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
	'''                             brackets from property names
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial  Class Icon
        Inherits UI.Skins.SkinObjectBase

#Region "Private Members"

        Private _borderWidth As String

#End Region

#Region "Public Members"

        Public Property BorderWidth() As String
            Get
                Return _borderWidth
            End Get
            Set(ByVal Value As String)
                _borderWidth = Value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ' public attributes
                If BorderWidth <> "" Then
                    imgIcon.BorderWidth = System.Web.UI.WebControls.Unit.Parse(BorderWidth)
                End If

                Me.Visible = False

                If (Me.ModuleControl IsNot Nothing) AndAlso (ModuleControl.ModuleContext.Configuration IsNot Nothing) Then
                    If ModuleControl.ModuleContext.Configuration.IconFile <> "" Then
                        If ModuleControl.ModuleContext.Configuration.IconFile.StartsWith("~/") Then
                            imgIcon.ImageUrl = ModuleControl.ModuleContext.Configuration.IconFile
                        Else
                            If IsAdminControl() Then
                                imgIcon.ImageUrl = ModuleControl.ModuleContext.Configuration.DesktopModule.FolderName & "/" & ModuleControl.ModuleContext.Configuration.IconFile
                            Else
                                imgIcon.ImageUrl = ModuleControl.ModuleContext.PortalSettings.HomeDirectory & ModuleControl.ModuleContext.Configuration.IconFile
                            End If
                        End If
                        imgIcon.AlternateText = ModuleControl.ModuleContext.Configuration.ModuleTitle
                        Me.Visible = True
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
