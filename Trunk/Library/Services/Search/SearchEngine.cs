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

using System.Collections;

using DotNetNuke.Entities.Portals;

#endregion

namespace DotNetNuke.Services.Search
{
    /// -----------------------------------------------------------------------------
    /// Namespace:  DotNetNuke.Services.Search
    /// Project:    DotNetNuke
    /// Class:      SearchEngine
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The SearchEngine  manages the Indexing of the Portal content
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SearchEngine
    {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// IndexContent indexes all Portal content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public void IndexContent()
        {
            IndexingProvider Indexer = IndexingProvider.Instance();
            SearchDataStoreProvider.Instance().StoreSearchItems(GetContent(Indexer));
        }

        public void IndexContent(int PortalID)
        {
            IndexingProvider Indexer = IndexingProvider.Instance();
            SearchDataStoreProvider.Instance().StoreSearchItems(GetContent(PortalID, Indexer));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetContent gets all the content and passes it to the Indexer
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="Indexer">The Index Provider that will index the content of the portal</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        protected SearchItemInfoCollection GetContent(IndexingProvider Indexer)
        {
            var SearchItems = new SearchItemInfoCollection();
            var objPortals = new PortalController();
            PortalInfo objPortal;
            ArrayList arrPortals = objPortals.GetPortals();
            int intPortal;
            for (intPortal = 0; intPortal <= arrPortals.Count - 1; intPortal++)
            {
                objPortal = (PortalInfo) arrPortals[intPortal];
                SearchItems.AddRange(Indexer.GetSearchIndexItems(objPortal.PortalID));
            }
            return SearchItems;
        }

        protected SearchItemInfoCollection GetContent(int PortalID, IndexingProvider Indexer)
        {
            var SearchItems = new SearchItemInfoCollection();
            SearchItems.AddRange(Indexer.GetSearchIndexItems(PortalID));
            return SearchItems;
        }
    }
}
