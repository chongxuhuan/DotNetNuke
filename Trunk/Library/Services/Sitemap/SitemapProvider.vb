Imports System
Imports System.Configuration.Provider
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Portals

Namespace DotNetNuke.Services.Sitemap

    Public MustInherit Class SitemapProvider
        '        Inherits ProviderBase
        '
        '        Public Overrides Sub Initialize(ByVal name As String, ByVal config As System.Collections.Specialized.NameValueCollection)
        '
        '            ' Verify that config isn't null
        '            If config Is Nothing Then
        '                Throw New ArgumentNullException("config")
        '            End If
        '
        '            ' Assign the provider a default name if it doesn't have one
        '            If String.IsNullOrEmpty(name) Then
        '                name = "SitemapProvider"
        '            End If
        '
        '            ' Add a default "description" attribute to config if the
        '            ' attribute doesn't exist or is empty
        '            If String.IsNullOrEmpty(config("description")) Then
        '                config.Remove("description")
        '                config.Add("description", "DotNetNuke Sitemap provider")
        '            End If
        '
        '            MyBase.Initialize(name, config)
        '
        '        End Sub


        Private _name As String
        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Private _description As String
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property


        Public Property Enabled() As Boolean
            Get
                Return Boolean.Parse(PortalController.GetPortalSetting(Name + "Enabled", PortalController.GetCurrentPortalSettings.PortalId, "True"))
            End Get
            Set(ByVal value As Boolean)
                PortalController.UpdatePortalSetting(PortalController.GetCurrentPortalSettings.PortalId, Name + "Enabled", value.ToString())
            End Set
        End Property



        Public Property OverridePriority() As Boolean
            Get
                Return Boolean.Parse(PortalController.GetPortalSetting(Name + "Override", PortalController.GetCurrentPortalSettings.PortalId, "False"))
            End Get
            Set(ByVal value As Boolean)
                PortalController.UpdatePortalSetting(PortalController.GetCurrentPortalSettings.PortalId, Name + "Override", value.ToString())
            End Set
        End Property

        Public Property Priority() As Single
            Get
                Dim value As Single = 0
                If (OverridePriority) Then
                    ' stored as an integer (pr * 100) to prevent from translating errors with the decimal point
                    value = Single.Parse(PortalController.GetPortalSetting(Name + "Value", PortalController.GetCurrentPortalSettings.PortalId, "50")) / 100
                End If
                Return value
            End Get

            Set(ByVal value As Single)
                PortalController.UpdatePortalSetting(PortalController.GetCurrentPortalSettings.PortalId, Name + "Value", (value * 100).ToString())
            End Set

        End Property


        Public MustOverride Function GetUrls(ByVal portalId As Integer, ByVal ps As PortalSettings, ByVal version As String) As List(Of Sitemap.SitemapUrl)

    End Class

End Namespace

