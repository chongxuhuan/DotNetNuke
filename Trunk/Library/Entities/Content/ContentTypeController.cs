﻿#region Copyright

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

using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Data;

#endregion

namespace DotNetNuke.Entities.Content
{
	/// <summary>
	/// ContentTypeController provides the business layer of ContentType.
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <example>
	/// <code lang="C#">
	/// IContentTypeController typeController = new ContentTypeController();
    /// ContentType contentType = (from t in typeController.GetContentTypes()
    ///                            where t.ContentType == "DesktopModule"
    ///                            select t).SingleOrDefault();
    /// if(contentType == null)
    /// {
    ///     contentType = new ContentType {ContentType = "DesktopModule"};
    ///     contentType.ContentTypeId = typeController.AddContentType(contentType);
    /// }
	/// </code>
	/// </example>
    public class ContentTypeController : IContentTypeController
    {
        private readonly IDataService _DataService;
        private string _CacheKey = "ContentTypes";

        private int _CacheTimeOut = 20;

        #region "Constructors"

        public ContentTypeController() : this(Util.GetDataService())
        {
        }

        public ContentTypeController(IDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Methods"

        private object GetContentTypesCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillQueryable<ContentType>(_DataService.GetContentTypes());
        }

        #endregion

        #region "Public Methods"

		/// <summary>
		/// Adds the type of the content.
		/// </summary>
		/// <param name="contentType">Type of the content.</param>
		/// <returns>content type id.</returns>
		/// <exception cref="System.ArgumentNullException">content type is null.</exception>
		/// <exception cref="System.ArgumentException">contentType.ContentType is empty.</exception>
        public int AddContentType(ContentType contentType)
        {
            //Argument Contract
            Requires.NotNull("contentType", contentType);
            Requires.PropertyNotNullOrEmpty("contentType", "ContentType", contentType.ContentType);

            contentType.ContentTypeId = _DataService.AddContentType(contentType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);

            return contentType.ContentTypeId;
        }

		/// <summary>
		/// Clears the content type cache.
		/// </summary>
        public void ClearContentTypeCache()
        {
            DataCache.RemoveCache(_CacheKey);
        }

		/// <summary>
		/// Deletes the type of the content.
		/// </summary>
		/// <param name="contentType">Type of the content.</param>
		/// <exception cref="System.ArgumentNullException">content type is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">content type id is less than 0.</exception>
        public void DeleteContentType(ContentType contentType)
        {
            //Argument Contract
            Requires.NotNull("contentType", contentType);
            Requires.PropertyNotNegative("contentType", "ContentTypeId", contentType.ContentTypeId);

            _DataService.DeleteContentType(contentType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);
        }

		/// <summary>
		/// Gets the content types.
		/// </summary>
		/// <returns>content type collection.</returns>
        public IQueryable<ContentType> GetContentTypes()
        {
            return CBO.GetCachedObject<IQueryable<ContentType>>(new CacheItemArgs(_CacheKey, _CacheTimeOut), GetContentTypesCallBack);
        }

		/// <summary>
		/// Updates the type of the content.
		/// </summary>
		/// <param name="contentType">Type of the content.</param>
		/// <exception cref="System.ArgumentNullException">content type is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">content type id is less than 0.</exception>
		/// <exception cref="System.ArgumentException">contentType.ContentType is empty.</exception>
        public void UpdateContentType(ContentType contentType)
        {
            //Argument Contract
            Requires.NotNull("contentType", contentType);
            Requires.PropertyNotNegative("contentType", "ContentTypeId", contentType.ContentTypeId);
            Requires.PropertyNotNullOrEmpty("contentType", "ContentType", contentType.ContentType);

            _DataService.UpdateContentType(contentType);

            //Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey);
        }

        #endregion
    }
}