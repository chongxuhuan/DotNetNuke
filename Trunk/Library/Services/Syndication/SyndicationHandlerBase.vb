Imports DotNetNuke.Entities.Portals

Namespace DotNetNuke.Services.Syndication
    Public Class SyndicationHandlerBase
        Inherits GenericRssHttpHandlerBase

        Private _settings As PortalSettings
        Public ReadOnly Property Settings() As PortalSettings
            Get
                Return GetPortalSettings()
            End Get
        End Property

        Private _tabId As Integer = Null.NullInteger
        Public ReadOnly Property TabId() As Integer
            Get
                If _tabId = Null.NullInteger AndAlso Not Request.QueryString("tabid") Is Nothing Then
                    If Not Integer.TryParse(Request.QueryString("tabid"), _tabId) Then
                        _tabId = Null.NullInteger
                    End If
                End If
                Return _tabId
            End Get
        End Property

        Private _moduleId As Integer = Null.NullInteger
        Public ReadOnly Property ModuleId() As Integer
            Get
                If _moduleId = Null.NullInteger AndAlso Not Request.QueryString("moduleid") Is Nothing Then
                    If Not Integer.TryParse(Request.QueryString("moduleid"), _moduleId) Then
                        _moduleId = Null.NullInteger
                    End If
                End If
                Return _moduleId
            End Get
        End Property

        Public ReadOnly Property Request() As HttpRequest
            Get
                Return HttpContext.Current.Request
            End Get
        End Property

    End Class
End Namespace