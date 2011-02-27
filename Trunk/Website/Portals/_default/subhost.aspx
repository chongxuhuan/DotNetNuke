<%@ Page Language="VB" %>

<%@ Import Namespace="DotNetNuke" %>
<script runat="server">

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        MyBase.OnInit(e)

        Dim settings As PortalSettings = PortalController.GetCurrentPortalSettings
        Dim pageLocale As CultureInfo = Localization.GetPageLocale(settings)
        If Not settings Is Nothing AndAlso Not pageLocale Is Nothing Then
            Localization.SetThreadCultures(pageLocale, settings)
        End If
    End Sub    
    
    Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

        Dim DomainName As String = Null.NullString
        Dim ServerPath As String
        Dim URL() As String
        Dim intURL As Integer

        ' parse the Request URL into a Domain Name token 
        URL = Split(Request.Url.ToString(), "/")
        For intURL = 2 To URL.GetUpperBound(0)
            Select Case URL(intURL).ToLower
                Case "admin", "desktopmodules", "mobilemodules", "premiummodules"
                    Exit For
                Case Else
                    ' check if filename
                    If InStr(1, URL(intURL), ".aspx") = 0 Then
                        DomainName = DomainName & IIf(DomainName <> "", "/", "") & URL(intURL)
                    Else
                        Exit For
                    End If
            End Select
        Next intURL

        ' format the Request.ApplicationPath
        ServerPath = Request.ApplicationPath
        If Mid(ServerPath, Len(ServerPath), 1) <> "/" Then
            ServerPath = ServerPath & "/"
        End If
        
        Dim portal As Entities.Portals.PortalSettings = Entities.Portals.PortalController.GetCurrentPortalSettings()
        
        If portal.HomeTabId > Null.NullInteger Then
            Response.Redirect(NavigateURL(portal.HomeTabId, portal, String.Empty, String.Empty), True)
        Else
            DomainName = ServerPath & "Default.aspx?alias=" & DomainName

            Response.Redirect(DomainName, True)
        End If
	
    End Sub

</script>
