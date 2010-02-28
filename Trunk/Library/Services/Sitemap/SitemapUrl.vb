
Namespace DotNetNuke.Services.Sitemap
    Public Class SitemapUrl

        Private _url As String
        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property


        Private _lastModified As DateTime
        Public Property LastModified() As DateTime
            Get
                Return _lastModified
            End Get
            Set(ByVal value As DateTime)
                _lastModified = value
            End Set
        End Property


        Private _changeFrequency As SitemapChangeFrequency
        Public Property ChangeFrequency() As SitemapChangeFrequency
            Get
                Return _changeFrequency
            End Get
            Set(ByVal value As SitemapChangeFrequency)
                _changeFrequency = value
            End Set
        End Property


        Private _priority As Single
        Public Property Priority() As Single
            Get
                Return _priority
            End Get
            Set(ByVal value As Single)
                _priority = value
            End Set
        End Property

    End Class

End Namespace
