' DotNetNuke® - http:'www.dotnetnuke.com
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

Imports System
Imports System.IO
Imports System.Web

Imports System.Collections
Imports System.Collections.Specialized

Namespace DotNetNuke.HttpModules.Compression

    ''' <summary>
    ''' An HttpModule that hooks onto the Response.Filter property of the
    ''' current request and tries to compress the output, based on what
    ''' the browser supports
    ''' </summary>
    ''' <remarks>
    ''' <p>This HttpModule uses classes that inherit from <see cref="CompressingFilter"/>.
    ''' We already support gzip and deflate (aka zlib), if you'd like to add 
    ''' support for compress (which uses LZW, which is licensed), add in another
    ''' class that inherits from HttpFilter to do the work.</p>
    ''' 
    ''' <p>This module checks the Accept-Encoding HTTP header to determine if the
    ''' client actually supports any notion of compression.  Currently, we support
    ''' the deflate (zlib) and gzip compression schemes.  I chose to not implement
    ''' compress because it uses lzw which requires a license from 
    ''' Unisys.  For more information about the common compression types supported,
    ''' see http:'www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.11 for details.</p> 
    ''' </remarks>
    ''' <seealso cref="CompressingFilter"/>
    ''' <seealso cref="Stream"/>
    Public Class CompressionModule
        Implements IHttpModule

        Private Const INSTALLED_KEY As String = "httpcompress.attemptedinstall"
        Private Shared ReadOnly INSTALLED_TAG As Object = New Object()

        ''' <summary>
        ''' Init the handler and fulfill <see cref="IHttpModule"/>
        ''' </summary>
        ''' <remarks>
        ''' This implementation hooks the ReleaseRequestState and PreSendRequestHeaders events to 
        ''' figure out as late as possible if we should install the filter.  Previous versions did
        ''' not do this as well.
        ''' </remarks>
        ''' <param name="context">The <see cref="HttpApplication"/> this handler is working for.</param>
        Public Sub Init(ByVal context As HttpApplication) Implements IHttpModule.Init
            AddHandler context.ReleaseRequestState, AddressOf CompressContent
            AddHandler context.PreSendRequestHeaders, AddressOf CompressContent
        End Sub

        ''' <summary>
        ''' Implementation of <see cref="IHttpModule"/>
        ''' </summary>
        ''' <remarks>
        ''' Currently empty.  Nothing to really do, as I have no member variables.
        ''' </remarks>
        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

        ''' <summary>
        ''' EventHandler that gets ahold of the current request context and attempts to compress the output.
        ''' </summary>
        ''' <param name="sender">The <see cref="HttpApplication"/> that is firing this event.</param>
        ''' <param name="e">Arguments to the event</param>
        Private Sub CompressContent(ByVal sender As Object, ByVal e As EventArgs)

            Dim app As HttpApplication = CType(sender, HttpApplication)
            If (app Is Nothing) OrElse (app.Context Is Nothing) OrElse (app.Context.Items Is Nothing) Then
                Exit Sub
            Else
                'Check if page is a content page
                Dim page As DotNetNuke.Framework.CDefault = TryCast(app.Context.Handler, DotNetNuke.Framework.CDefault)
                If (page Is Nothing) Then Exit Sub
            End If

            If app.Response Is Nothing OrElse app.Response.ContentType Is Nothing OrElse app.Response.ContentType.ToLower() <> "text/html" Then
                Exit Sub
            End If

            ' only do this if we havn't already attempted an install.  This prevents PreSendRequestHeaders from
            ' trying to add this item way to late.  We only want the first run through to do anything.
            ' also, we use the context to store whether or not we've attempted an add, as it's thread-safe and
            ' scoped to the request.  An instance of this module can service multiple requests at the same time,
            ' so we cannot use a member variable.
            If Not app.Context.Items.Contains(INSTALLED_KEY) Then
                ' log the install attempt in the HttpContext
                ' must do this first as several IF statements
                ' below skip full processing of this method
                app.Context.Items.Add(INSTALLED_KEY, INSTALLED_TAG)

                ' path comparison is based on filename and querystring parameters ( ie. default.aspx?tabid=## )
                Dim realPath As String = app.Request.Url.PathAndQuery

                ' get the config settings
                Dim _Settings As Settings = Settings.GetSettings()
                If _Settings Is Nothing Then
                    Exit Sub
                End If

                Dim compress As Boolean = True

                If _Settings.PreferredAlgorithm = Algorithms.None Then
                    compress = False

                    ' Terminate processing if both compression and whitespace handling are disabled
                    If Not _Settings.Whitespace Then
                        Exit Sub
                    End If
                End If

                Dim acceptedTypes As String = app.Request.Headers("Accept-Encoding")
                If _Settings.IsExcludedPath(realPath) OrElse acceptedTypes Is Nothing Then
                    ' skip if the file path excludes compression
                    ' if we couldn't find the header, bail out
                    Exit Sub
                End If

                ' fix to handle caching appropriately
                ' see http:'www.pocketsoap.com/weblog/2003/07/1330.html
                ' Note, this header is added only when the request
                ' has the possibility of being compressed...
                ' i.e. it is not added when the request is excluded from
                ' compression by CompressionLevel, Path, or MimeType
                app.Response.Cache.VaryByHeaders("Accept-Encoding") = True

                Dim filter As CompressingFilter = Nothing
                If compress Then
                    ' the actual types could be , delimited.  split 'em out.
                    Dim types As String() = acceptedTypes.Split(","c)

                    filter = GetFilterForScheme(types, app.Response.Filter, _Settings)
                End If

                If filter Is Nothing Then
                    If _Settings.Whitespace Then
                        app.Response.Filter = New WhitespaceFilter(app.Response.Filter, _Settings.Reg)
                    Else
                        Exit Sub
                    End If
                Else
                    If _Settings.Whitespace Then
                        app.Response.Filter = New WhitespaceFilter(filter, _Settings.Reg)
                    Else
                        app.Response.Filter = filter
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Get ahold of a <see cref="CompressingFilter"/> for the given encoding scheme.
        ''' If no encoding scheme can be found, it returns null.
        ''' </summary>
        ''' <remarks>
        ''' See http:'www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.3 for details
        ''' on how clients are supposed to construct the Accept-Encoding header.  This
        ''' implementation follows those rules, though we allow the server to override
        ''' the preference given to different supported algorithms.  I'm doing this as 
        ''' I would rather give the server control over the algorithm decision than 
        ''' the client.  If the clients send up * as an accepted encoding with highest
        ''' quality, we use the preferred algorithm as specified in the config file.
        ''' </remarks>
        Public Shared Function GetFilterForScheme(ByVal schemes As String(), ByVal output As Stream, ByVal prefs As Settings) As CompressingFilter

            Dim foundDeflate As Boolean = False
            Dim foundGZip As Boolean = False
            Dim foundStar As Boolean = False

            Dim deflateQuality As Single = 0.0
            Dim gZipQuality As Single = 0.0
            Dim starQuality As Single = 0.0

            Dim isAcceptableDeflate As Boolean
            Dim isAcceptableGZip As Boolean
            Dim isAcceptableStar As Boolean

            For i As Integer = 0 To schemes.Length - 1
                Dim acceptEncodingValue As String = schemes(i).Trim().ToLower()

                If acceptEncodingValue.StartsWith("deflate") Then
                    foundDeflate = True
                    Dim newDeflateQuality As Single = GetQuality(acceptEncodingValue)
                    If deflateQuality < newDeflateQuality Then
                        deflateQuality = newDeflateQuality
                    End If
                ElseIf (acceptEncodingValue.StartsWith("gzip") OrElse acceptEncodingValue.StartsWith("x-gzip")) Then
                    foundGZip = True
                    Dim newGZipQuality As Single = GetQuality(acceptEncodingValue)
                    If (gZipQuality < newGZipQuality) Then
                        gZipQuality = newGZipQuality
                    End If
                ElseIf (acceptEncodingValue.StartsWith("*")) Then
                    foundStar = True
                    Dim newStarQuality As Single = GetQuality(acceptEncodingValue)
                    If (starQuality < newStarQuality) Then
                        starQuality = newStarQuality
                    End If
                End If
            Next

            isAcceptableStar = foundStar And (starQuality > 0)
            isAcceptableDeflate = (foundDeflate And (deflateQuality > 0)) OrElse (Not foundDeflate And isAcceptableStar)
            isAcceptableGZip = (foundGZip And (gZipQuality > 0)) OrElse (Not foundGZip And isAcceptableStar)

            If isAcceptableDeflate AndAlso Not foundDeflate Then
                deflateQuality = starQuality
            End If

            If isAcceptableGZip AndAlso Not foundGZip Then
                gZipQuality = starQuality
            End If

            ' do they support any of our compression methods?
            If (Not (isAcceptableDeflate OrElse isAcceptableGZip OrElse isAcceptableStar)) Then
                Return Nothing
            End If

            ' if deflate is better according to client
            If (isAcceptableDeflate AndAlso (Not isAcceptableGZip OrElse (deflateQuality > gZipQuality))) Then
                Return New DeflateFilter(output)
            End If

            ' if gzip is better according to client
            If (isAcceptableGZip AndAlso (Not isAcceptableDeflate OrElse (deflateQuality < gZipQuality))) Then
                Return New GZipFilter(output)
            End If

            ' if we're here, the client either didn't have a preference or they don't support compression
            If (isAcceptableDeflate AndAlso (prefs.PreferredAlgorithm = Algorithms.Deflate OrElse prefs.PreferredAlgorithm = Algorithms.Default)) Then
                Return New DeflateFilter(output)
            End If

            If (isAcceptableGZip AndAlso prefs.PreferredAlgorithm = Algorithms.GZip) Then
                Return New GZipFilter(output)
            End If


            If (isAcceptableDeflate OrElse isAcceptableStar) Then
                Return New DeflateFilter(output)
            End If

            If (isAcceptableGZip) Then
                Return New GZipFilter(output)
            End If

            ' return null.  we couldn't find a filter.
            Return Nothing
        End Function

        Public Shared Function GetQuality(ByVal acceptEncodingValue As String) As Single
            Dim qParam As Integer = acceptEncodingValue.IndexOf("q=")

            If (qParam >= 0) Then
                Dim Val As Single = 0.0
                Try
                    Val = Single.Parse(acceptEncodingValue.Substring(qParam + 2, acceptEncodingValue.Length - (qParam + 2)))
                Catch exc As FormatException
                End Try

                Return Val
            Else
                Return 1
            End If

        End Function

    End Class

End Namespace
