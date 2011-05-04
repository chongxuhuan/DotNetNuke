#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.IO;
using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;

#endregion

namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// An HttpModule that hooks onto the Response.Filter property of the
    /// current request and tries to compress the output, based on what
    /// the browser supports
    /// </summary>
    /// <remarks>
    /// <p>This HttpModule uses classes that inherit from <see cref="CompressingFilter"/>.
    /// We already support gzip and deflate (aka zlib), if you'd like to add
    /// support for compress (which uses LZW, which is licensed), add in another
    /// class that inherits from HttpFilter to do the work.</p>
    ///
    /// <p>This module checks the Accept-Encoding HTTP header to determine if the
    /// client actually supports any notion of compression.  Currently, we support
    /// the deflate (zlib) and gzip compression schemes.  I chose to not implement
    /// compress because it uses lzw which requires a license from
    /// Unisys.  For more information about the common compression types supported,
    /// see http:'www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.11 for details.</p>
    /// </remarks>
    /// <seealso cref="CompressingFilter"/>
    /// <seealso cref="Stream"/>
    public class CompressionModule : IHttpModule
    {
        private const string INSTALLED_KEY = "httpcompress.attemptedinstall";
        private static readonly object INSTALLED_TAG = new object();

        #region IHttpModule Members

        /// <summary>
        /// Init the handler and fulfill <see cref="IHttpModule"/>
        /// </summary>
        /// <remarks>
        /// This implementation hooks the ReleaseRequestState and PreSendRequestHeaders events to
        /// figure out as late as possible if we should install the filter.  Previous versions did
        /// not do this as well.
        /// </remarks>
        /// <param name="context">The <see cref="HttpApplication"/> this handler is working for.</param>
        public void Init(HttpApplication context)
        {
            context.ReleaseRequestState += CompressContent;
            context.PreSendRequestHeaders += CompressContent;
        }

        /// <summary>
        /// Implementation of <see cref="IHttpModule"/>
        /// </summary>
        /// <remarks>
        /// Currently empty.  Nothing to really do, as I have no member variables.
        /// </remarks>
        public void Dispose()
        {
        }

        #endregion

        /// <summary>
        /// EventHandler that gets ahold of the current request context and attempts to compress the output.
        /// </summary>
        /// <param name="sender">The <see cref="HttpApplication"/> that is firing this event.</param>
        /// <param name="e">Arguments to the event</param>
        private void CompressContent(object sender, EventArgs e)
        {
            bool isOutputCached = Null.NullBoolean;
            var app = (HttpApplication) sender;
            if ((app == null) || (app.Context == null) || (app.Context.Items == null))
            {
                return;
            }
            else
            {
                //Check for Output Caching - if output caching is used the cached content will already be compressed, 
                //but we still need to add the headers

                {
                    var isCached = app.Context.Items["DNNOutputCache"] as string;
                    if (!string.IsNullOrEmpty(isCached))
                    {
                        isOutputCached = bool.Parse(isCached);
                    }
                }
                if (!isOutputCached)
                {
                    var page = app.Context.Handler as CDefault;
                    if ((page == null))
                    {
                        return;
                    }
                }
            }
            if (app.Response == null || app.Response.ContentType == null || app.Response.ContentType.ToLower() != "text/html")
            {
                return;
            }
            if (!app.Context.Items.Contains(INSTALLED_KEY))
            {
                app.Context.Items.Add(INSTALLED_KEY, INSTALLED_TAG);
                string realPath = app.Request.Url.PathAndQuery;
                Settings _Settings = Settings.GetSettings();
                if (_Settings == null)
                {
                    return;
                }
                bool compress = true;
                if (_Settings.PreferredAlgorithm == Algorithms.None)
                {
                    compress = false;
                    if (!_Settings.Whitespace)
                    {
                        return;
                    }
                }
                string acceptedTypes = app.Request.Headers["Accept-Encoding"];
                if (_Settings.IsExcludedPath(realPath) || acceptedTypes == null)
                {
                    return;
                }
                app.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
                CompressingFilter filter = null;
                if (compress)
                {
                    string[] types = acceptedTypes.Split(',');
                    filter = GetFilterForScheme(types, app.Response.Filter, _Settings);
                    //Add the headers - we do this now - becuase if Output Caching is enabled we need to
                    //add the Headers regardless of whether compresion actually occurs in this request.
                    filter.WriteHeaders();
                }
                if (filter == null)
                {
                    if (_Settings.Whitespace)
                    {
                        app.Response.Filter = new WhitespaceFilter(app.Response.Filter, _Settings.Reg);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (_Settings.Whitespace)
                    {
                        app.Response.Filter = new WhitespaceFilter(filter, _Settings.Reg);
                    }
                    else
                    {
                        app.Response.Filter = filter;
                    }
                }
            }
        }

        /// <summary>
        /// Get ahold of a <see cref="CompressingFilter"/> for the given encoding scheme.
        /// If no encoding scheme can be found, it returns null.
        /// </summary>
        /// <remarks>
        /// See http:'www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.3 for details
        /// on how clients are supposed to construct the Accept-Encoding header.  This
        /// implementation follows those rules, though we allow the server to override
        /// the preference given to different supported algorithms.  I'm doing this as
        /// I would rather give the server control over the algorithm decision than
        /// the client.  If the clients send up * as an accepted encoding with highest
        /// quality, we use the preferred algorithm as specified in the config file.
        /// </remarks>
        public static CompressingFilter GetFilterForScheme(string[] schemes, Stream output, Settings prefs)
        {
            bool foundDeflate = false;
            bool foundGZip = false;
            bool foundStar = false;
            float deflateQuality = 0;
            float gZipQuality = 0;
            float starQuality = 0;
            bool isAcceptableDeflate;
            bool isAcceptableGZip;
            bool isAcceptableStar;
            for (int i = 0; i <= schemes.Length - 1; i++)
            {
                string acceptEncodingValue = schemes[i].Trim().ToLower();
                if (acceptEncodingValue.StartsWith("deflate"))
                {
                    foundDeflate = true;
                    float newDeflateQuality = GetQuality(acceptEncodingValue);
                    if (deflateQuality < newDeflateQuality)
                    {
                        deflateQuality = newDeflateQuality;
                    }
                }
                else if ((acceptEncodingValue.StartsWith("gzip") || acceptEncodingValue.StartsWith("x-gzip")))
                {
                    foundGZip = true;
                    float newGZipQuality = GetQuality(acceptEncodingValue);
                    if ((gZipQuality < newGZipQuality))
                    {
                        gZipQuality = newGZipQuality;
                    }
                }
                else if ((acceptEncodingValue.StartsWith("*")))
                {
                    foundStar = true;
                    float newStarQuality = GetQuality(acceptEncodingValue);
                    if ((starQuality < newStarQuality))
                    {
                        starQuality = newStarQuality;
                    }
                }
            }
            isAcceptableStar = foundStar && (starQuality > 0);
            isAcceptableDeflate = (foundDeflate && (deflateQuality > 0)) || (!foundDeflate && isAcceptableStar);
            isAcceptableGZip = (foundGZip && (gZipQuality > 0)) || (!foundGZip && isAcceptableStar);
            if (isAcceptableDeflate && !foundDeflate)
            {
                deflateQuality = starQuality;
            }
            if (isAcceptableGZip && !foundGZip)
            {
                gZipQuality = starQuality;
            }
            if ((!(isAcceptableDeflate || isAcceptableGZip || isAcceptableStar)))
            {
                return null;
            }
            if ((isAcceptableDeflate && (!isAcceptableGZip || (deflateQuality > gZipQuality))))
            {
                return new DeflateFilter(output);
            }
            if ((isAcceptableGZip && (!isAcceptableDeflate || (deflateQuality < gZipQuality))))
            {
                return new GZipFilter(output);
            }
            if ((isAcceptableDeflate && (prefs.PreferredAlgorithm == Algorithms.Deflate || prefs.PreferredAlgorithm == Algorithms.Default)))
            {
                return new DeflateFilter(output);
            }
            if ((isAcceptableGZip && prefs.PreferredAlgorithm == Algorithms.GZip))
            {
                return new GZipFilter(output);
            }
            if ((isAcceptableDeflate || isAcceptableStar))
            {
                return new DeflateFilter(output);
            }
            if ((isAcceptableGZip))
            {
                return new GZipFilter(output);
            }
            return null;
        }

        public static float GetQuality(string acceptEncodingValue)
        {
            int qParam = acceptEncodingValue.IndexOf("q=");
            if ((qParam >= 0))
            {
                float Val = 0;
                try
                {
                    Val = float.Parse(acceptEncodingValue.Substring(qParam + 2, acceptEncodingValue.Length - (qParam + 2)));
                }
                catch (FormatException exc)
                {
                    DnnLog.Debug(exc);

                }
                return Val;
            }
            else
            {
                return 1;
            }
        }
    }
}
