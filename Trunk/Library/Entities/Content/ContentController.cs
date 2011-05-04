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

using System.Collections.Specialized;
using System.Data;
using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.Entities.Content
{
	/// <summary>
	/// ContentController provides the business layer of ContentItem.
	/// </summary>
	/// <remarks>
	/// It's better to use Util.GetContentController() instead of create a new instance directly.
	/// </remarks>
	/// <example>
	/// <code lang="C#">
	/// IContentController contentController = Util.GetContentController();
    /// desktopModule.Content = desktopModule.FriendlyName;
    /// desktopModule.Indexed = false;
    /// desktopModule.ContentTypeId = contentType.ContentTypeId;
	/// desktopModule.ContentItemId = contentController.AddContentItem(desktopModule);
	/// </code>
	/// </example>
    public class ContentController : IContentController
    {
        private readonly IDataService _dataService;

        #region Constructors

        public ContentController() : this(Util.GetDataService())
        {
        }

        public ContentController(IDataService dataService)
        {
            _dataService = dataService;
        }

        #endregion

        #region Public Methods

		/// <summary>
		/// Adds the content item.
		/// </summary>
		/// <param name="contentItem">The content item.</param>
		/// <returns>content item id.</returns>
		/// <exception cref="System.ArgumentNullException">content item is null.</exception>
        public int AddContentItem(ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);

            contentItem.ContentItemId = _dataService.AddContentItem(contentItem, UserController.GetCurrentUserInfo().UserID);

            return contentItem.ContentItemId;
        }

		/// <summary>
		/// Deletes the content item.
		/// </summary>
		/// <param name="contentItem">The content item.</param>
		/// <exception cref="System.ArgumentNullException">content item is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">content item's id less than 0.</exception>
        public void DeleteContentItem(ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);

            _dataService.DeleteContentItem(contentItem);
        }

		/// <summary>
		/// Gets the content item.
		/// </summary>
		/// <param name="contentItemId">The content item id.</param>
		/// <returns>content item.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Content item id is less than 0.</exception>
        public ContentItem GetContentItem(int contentItemId)
        {
            //Argument Contract
            Requires.NotNegative("contentItemId", contentItemId);

            return CBO.FillObject<ContentItem>(_dataService.GetContentItem(contentItemId));
        }

		/// <summary>
		/// Gets the content items by term name.
		/// </summary>
		/// <param name="term">The term name.</param>
		/// <returns>content item collection.</returns>
		/// <exception cref="System.ArgumentException">Term name is empty.</exception>
        public IQueryable<ContentItem> GetContentItemsByTerm(string term)
        {
            //Argument Contract
            Requires.NotNullOrEmpty("term", term);

            return CBO.FillQueryable<ContentItem>(_dataService.GetContentItemsByTerm(term));
        }

		/// <summary>
		/// Gets the un indexed content items.
		/// </summary>
		/// <returns>content item collection.</returns>
        public IQueryable<ContentItem> GetUnIndexedContentItems()
        {
            return CBO.FillQueryable<ContentItem>(_dataService.GetUnIndexedContentItems());
        }

		/// <summary>
		/// Updates the content item.
		/// </summary>
		/// <param name="contentItem">The content item.</param>
		/// <exception cref="System.ArgumentNullException">content item is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">content item's id less than 0.</exception>
        public void UpdateContentItem(ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);

            _dataService.UpdateContentItem(contentItem, UserController.GetCurrentUserInfo().UserID);
        }

		/// <summary>
		/// Adds the meta data.
		/// </summary>
		/// <param name="contentItem">The content item.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="System.ArgumentNullException">content item is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">content item's id less than 0.</exception>
		/// <exception cref="System.ArgumentException">Meta name is empty.</exception>
        public void AddMetaData(ContentItem contentItem, string name, string value)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);
            Requires.NotNullOrEmpty("name", name);

            _dataService.AddMetaData(contentItem, name, value);
        }

		/// <summary>
		/// Deletes the meta data.
		/// </summary>
		/// <param name="contentItem">The content item.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="System.ArgumentNullException">content item is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">content item's id less than 0.</exception>
		/// <exception cref="System.ArgumentException">Meta name is empty.</exception>
        public void DeleteMetaData(ContentItem contentItem, string name, string value)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);
            Requires.NotNullOrEmpty("name", name);

            _dataService.DeleteMetaData(contentItem, name, value);
        }

		/// <summary>
		/// Gets the meta data.
		/// </summary>
		/// <param name="contentItemId">The content item id.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentOutOfRangeException">content item's id less than 0.</exception>
        public NameValueCollection GetMetaData(int contentItemId)
        {
            //Argument Contract
            Requires.NotNegative("contentItemId", contentItemId);

            var metadata = new NameValueCollection();
            IDataReader dr = _dataService.GetMetaData(contentItemId);
            while (dr.Read())
            {
                metadata.Add(dr.GetString(0), dr.GetString(1));
            }

            return metadata;
        }

        #endregion
    }
}