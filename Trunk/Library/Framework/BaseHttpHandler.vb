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

Imports System.Web
Imports System.Net
Imports System.IO

Namespace DotNetNuke.Framework
    Public MustInherit Class BaseHttpHandler
        Implements IHttpHandler

        Private _context As HttpContext

        ''' <summary>
        ''' Processs the incoming HTTP request.
        ''' </summary>
        ''' <param name="context">Context.</param>
        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
            _context = context

            SetResponseCachePolicy(Response.Cache)

            If Not ValidateParameters() Then
                RespondInternalError()
                Return
            End If

            If RequiresAuthentication AndAlso Not HasPermission Then
                RespondForbidden()
                Return
            End If

            Response.ContentType = ContentMimeType
            Response.ContentEncoding = ContentEncoding

            HandleRequest()
        End Sub

        Public Overridable ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Returns the <see cref="HttpContext" /> object for the incoming HTTP request.
        ''' </summary>
        Public ReadOnly Property Context() As HttpContext
            Get
                Return _context
            End Get
        End Property

        ''' <summary>
        ''' Returns the <see cref="HttpRequest" /> object for the incoming HTTP request.
        ''' </summary>
        Public ReadOnly Property Request() As HttpRequest
            Get
                Return Context.Request
            End Get
        End Property

        ''' <summary>
        ''' Gets the <see cref="HttpResponse" /> object associated with the Page object. This object 
        ''' allows you to send HTTP response data to a client and contains information about that response.
        ''' </summary>
        Public ReadOnly Property Response() As HttpResponse
            Get
                Return Context.Response
            End Get
        End Property

        ''' <summary>
        ''' Gets the string representation of the body of the incoming request.
        ''' </summary>
        Public ReadOnly Property Content() As String
            Get
                Dim _content As String = String.Empty
                Request.InputStream.Position = 0
                Using Reader As StreamReader = New StreamReader(Request.InputStream)
                    _content = Reader.ReadToEnd()
                End Using

                Return _content
            End Get
        End Property

        ''' <summary>
        ''' Handles the request.  This is where you put your
        ''' business logic.
        ''' </summary>
        ''' <remarks>
        ''' <p>This method should result in a call to one 
        ''' (or more) of the following methods:</p>
        ''' <p><code>context.Response.BinaryWrite();</code></p>
        ''' <p><code>context.Response.Write();</code></p>
        ''' <p><code>context.Response.WriteFile();</code></p>
        ''' <p>
        ''' <code>
        ''' someStream.Save(context.Response.OutputStream);
        ''' </code>
        ''' </p>
        ''' <p>etc...</p>
        ''' <p>
        ''' If you want a download box to show up with a 
        ''' pre-populated filename, add this call here 
        ''' (supplying a real filename).
        ''' </p>
        ''' <p>
        ''' <code>Response.AddHeader("Content-Disposition"
        ''' , "attachment; filename=\"" + Filename + "\"");</code>
        ''' </p>
        ''' </remarks>
        Public MustOverride Sub HandleRequest()

        ''' <summary>
        ''' Validates the parameters.  Inheriting classes must
        ''' implement this and return true if the parameters are
        ''' valid, otherwise false.
        ''' </summary>
        ''' <returns><c>true</c> if the parameters are valid,
        ''' otherwise <c>false</c></returns>
        Public MustOverride Function ValidateParameters() As Boolean

        ''' <summary>
        ''' Gets a value indicating whether this handler
        ''' requires users to be authenticated.
        ''' </summary>
        ''' <value>
        '''    <c>true</c> if authentication is required
        '''    otherwise, <c>false</c>.
        ''' </value>
        Public Overridable ReadOnly Property RequiresAuthentication() As Boolean
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the requester
        ''' has the necessary permissions.
        ''' </summary>
        ''' <remarks>
        ''' By default all authenticated users have permssions.  
        ''' This property is only enforced if <see cref="RequiresAuthentication" /> is <c>true</c>
        ''' </remarks>
        ''' <value>
        '''    <c>true</c> if the user has the appropriate permissions
        '''    otherwise, <c>false</c>.
        ''' </value>
        Public Overridable ReadOnly Property HasPermission() As Boolean
            Get
                Return Context.User.Identity.IsAuthenticated
            End Get
        End Property

        ''' <summary>
        ''' Gets the content MIME type for the response object.
        ''' </summary>
        ''' <value></value>
        Public Overridable ReadOnly Property ContentMimeType() As String
            Get
                Return "text/plain"
            End Get
        End Property

        ''' <summary>
        ''' Gets the content encoding for the response object.
        ''' </summary>
        ''' <value></value>
        Public Overridable ReadOnly Property ContentEncoding() As System.Text.Encoding
            Get
                Return System.Text.Encoding.UTF8
            End Get
        End Property

        ''' <summary>
        ''' Sets the cache policy.  Unless a handler overrides
        ''' this method, handlers will not allow a respons to be
        ''' cached.
        ''' </summary>
        ''' <param name="cache">Cache.</param>
        Public Overridable Sub SetResponseCachePolicy(ByVal cache As HttpCachePolicy)
            cache.SetCacheability(HttpCacheability.NoCache)
            cache.SetNoStore()
            cache.SetExpires(DateTime.MinValue)
        End Sub

        ''' <summary>
        ''' Helper method used to Respond to the request
        ''' that the file was not found.
        ''' </summary>
        Protected Sub RespondFileNotFound()
            Response.StatusCode = CInt(HttpStatusCode.NotFound)
            Response.[End]()
        End Sub

        ''' <summary>
        ''' Helper method used to Respond to the request
        ''' that an error occurred in processing the request.
        ''' </summary>
        Protected Sub RespondInternalError()
            ' It's really too bad that StatusCode property
            ' is not of type HttpStatusCode.
            Response.StatusCode = CInt(HttpStatusCode.InternalServerError)
            Response.End()
        End Sub

        ''' <summary>
        ''' Helper method used to Respond to the request
        ''' that the request in attempting to access a resource
        ''' that the user does not have access to.
        ''' </summary>
        Protected Sub RespondForbidden()
            Response.StatusCode = CInt(HttpStatusCode.Forbidden)
            Response.End()
        End Sub

    End Class
End Namespace
