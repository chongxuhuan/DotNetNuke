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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;

#endregion

namespace DotNetNuke.Services.Search
{
    /// -----------------------------------------------------------------------------
    /// Namespace:  DotNetNuke.Services.Search
    /// Project:    DotNetNuke.Search.DataStore
    /// Class:      SearchDataStore
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The SearchDataStore is an implementation of the abstract SearchDataStoreProvider
    /// class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SearchDataStore : SearchDataStoreProvider
    {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddIndexWords adds the Index Words to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
		/// <param name="indexId">The Id of the SearchItem</param>
		/// <param name="searchItem">The SearchItem</param>
		/// <param name="language">The Language of the current Item</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        ///     [cnurse]    11/16/2004  replaced calls to separate content clean-up
        ///                             functions with new call to HtmlUtils.Clean().
        ///                             replaced logic to determine whether word should
        ///                             be indexed by call to CanIndexWord()
        ///     [vnguyen]   09/03/2010  added searchitem title to the content and
        ///                             also tab title, description, keywords where the
        ///                             content resides for indexed searching
        /// </history>
        /// -----------------------------------------------------------------------------
        private void AddIndexWords(int indexId, SearchItemInfo searchItem, string language)
        {
            var settings = new SearchConfig(SearchDataStoreController.GetSearchSettings(searchItem.ModuleId));
            var IndexWords = new Dictionary<string, int>();
            var IndexPositions = new Dictionary<string, List<int>>();
            string Content = GetSearchContent(searchItem);

            string title = HtmlUtils.StripPunctuation(searchItem.Title, true);

            // Tab and Module Metadata
            // Retreive module and page names
            ModuleInfo objModule = new ModuleController().GetModule(searchItem.ModuleId);
            TabInfo objTab = new TabController().GetTab(objModule.TabID, objModule.PortalID, false);
            string tabName = HtmlUtils.StripPunctuation(objTab.TabName, true);
            string tabTitle = HtmlUtils.StripPunctuation(objTab.Title, true);
            string tabDescription = HtmlUtils.StripPunctuation(objTab.Description, true);
            string tabKeywords = HtmlUtils.StripPunctuation(objTab.KeyWords, true);
            string tagfilter = PortalController.GetPortalSetting("SearchIncludedTagInfoFilter", objModule.PortalID, Host.SearchIncludedTagInfoFilter);

            // clean content
            Content = HtmlUtils.CleanWithTagInfo(Content, tagfilter, true);
            // append tab and module metadata
            Content = Content.ToLower() + title.ToLower() + " " + tabName.ToLower() + " " + tabTitle.ToLower() + " " + tabDescription.ToLower() + " " + tabKeywords.ToLower();

            string[] ContentWords = Content.Split(' ');
            int intWord = 0;
            foreach (string strWord in ContentWords)
            {
                if (CanIndexWord(strWord, language, settings))
                {
                    intWord = intWord + 1;
                    if (IndexWords.ContainsKey(strWord) == false)
                    {
                        IndexWords.Add(strWord, 0);
                        IndexPositions.Add(strWord, new List<int>());
                    }
                    IndexWords[strWord] = IndexWords[strWord] + 1;
                    IndexPositions[strWord].Add(intWord);
                }
            }
            Hashtable Words = GetSearchWords();
            int WordId;
            foreach (object objWord in IndexWords.Keys)
            {
                string strWord = Convert.ToString(objWord);
                if (Words.ContainsKey(strWord))
                {
                    WordId = Convert.ToInt32(Words[strWord]);
                }
                else
                {
                    WordId = DataProvider.Instance().AddSearchWord(strWord);
                    Words.Add(strWord, WordId);
                }
                int SearchItemWordID = DataProvider.Instance().AddSearchItemWord(indexId, WordId, IndexWords[strWord]);
                string strPositions = Null.NullString;
                foreach (int position in IndexPositions[strWord])
                {
                    strPositions += position + ",";
                }
                DataProvider.Instance().AddSearchItemWordPosition(SearchItemWordID, strPositions);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CanIndexWord determines whether the Word should be indexed
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="strWord">The Word to validate</param>
        /// <param name="Locale"></param>
        /// <param name="settings"></param>
        /// <returns>True if indexable, otherwise false</returns>
        /// <history>
        ///		[cnurse]	11/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private bool CanIndexWord(string strWord, string Locale, SearchConfig settings)
        {
            bool retValue = true;
            Hashtable CommonWords = GetCommonWords(Locale);
            if (Regex.IsMatch(strWord, "^\\d+$"))
            {
                if (!settings.SearchIncludeNumeric)
                {
                    retValue = false;
                }
            }
            else
            {
                if (strWord.Length < settings.SearchMinWordlLength || strWord.Length > settings.SearchMaxWordlLength)
                {
                    retValue = false;
                }
                else if (CommonWords.ContainsKey(strWord) && !settings.SearchIncludeCommon)
                {
                    retValue = false;
                }
            }
            return retValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetCommonWords gets a list of the Common Words for the locale
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="Locale">The locale string</param>
        /// <returns>A hashtable of common words</returns>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private Hashtable GetCommonWords(string Locale)
        {
            string strCacheKey = "CommonWords" + Locale;
            var objWords = (Hashtable) DataCache.GetCache(strCacheKey);
            if (objWords == null)
            {
                objWords = new Hashtable();
                IDataReader drWords = DataProvider.Instance().GetSearchCommonWordsByLocale(Locale);
                try
                {
                    while (drWords.Read())
                    {
                        objWords.Add(drWords["CommonWord"].ToString(), drWords["CommonWord"].ToString());
                    }
                }
                finally
                {
                    drWords.Close();
                    drWords.Dispose();
                }
                DataCache.SetCache(strCacheKey, objWords);
            }
            return objWords;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchWords gets a list of the current Words in the Database's Index
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A hashtable of words</returns>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private Hashtable GetSearchWords()
        {
            string strCacheKey = "SearchWords";
            var objWords = (Hashtable) DataCache.GetCache(strCacheKey);
            if (objWords == null)
            {
                objWords = new Hashtable();
                IDataReader drWords = DataProvider.Instance().GetSearchWords();
                try
                {
                    while (drWords.Read())
                    {
                        objWords.Add(drWords["Word"].ToString(), drWords["SearchWordsID"]);
                    }
                }
                finally
                {
                    drWords.Close();
                    drWords.Dispose();
                }
                DataCache.SetCache(strCacheKey, objWords, TimeSpan.FromMinutes(2));
            }
            return objWords;
        }

        protected virtual string GetSearchContent(SearchItemInfo SearchItem)
        {
            return SearchItem.Content;
        }

        public override SearchResultsInfoCollection GetSearchItems(int PortalID, int TabID, int ModuleID)
        {
            return SearchDataStoreController.GetSearchResults(PortalID, TabID, ModuleID);
        }

        public override SearchResultsInfoCollection GetSearchResults(int portalID, string criteria)
        {
            SearchResultsInfo searchResult;
            bool hasExcluded = Null.NullBoolean;
            bool hasMandatory = Null.NullBoolean;
            var objPortalController = new PortalController();
            PortalInfo objPortal = objPortalController.GetPortal(portalID);
            var _PortalSettings = new PortalSettings(objPortal);
            Hashtable commonWords = GetCommonWords(_PortalSettings.DefaultLanguage);
            criteria = criteria.ToLower();
            var searchWords = new SearchCriteriaCollection(criteria);
            var searchResults = new Dictionary<string, SearchResultsInfoCollection>();
            var dicResults = new Dictionary<int, Dictionary<int, SearchResultsInfo>>();
            foreach (SearchCriteria criterion in searchWords)
            {
                if (commonWords.ContainsKey(criterion.Criteria) == false || _PortalSettings.SearchIncludeCommon)
                {
                    if (!searchResults.ContainsKey(criterion.Criteria))
                    {
                        searchResults.Add(criterion.Criteria, SearchDataStoreController.GetSearchResults(portalID, criterion.Criteria));
                    }
                    if (searchResults.ContainsKey(criterion.Criteria))
                    {
                        foreach (SearchResultsInfo result in searchResults[criterion.Criteria])
                        {
                            if (!criterion.MustExclude)
                            {
                                if (dicResults.ContainsKey(result.SearchItemID))
                                {
                                    Dictionary<int, SearchResultsInfo> dic = dicResults[result.SearchItemID];
                                    if (dic.ContainsKey(result.TabId))
                                    {
                                        searchResult = dic[result.TabId];
                                        searchResult.Relevance += result.Relevance;
                                    }
                                    else
                                    {
                                        dic.Add(result.TabId, result);
                                    }
                                }
                                else
                                {
                                    var dic = new Dictionary<int, SearchResultsInfo>();
                                    dic.Add(result.TabId, result);
                                    dicResults.Add(result.SearchItemID, dic);
                                }
                            }
                        }
                    }
                }
            }
            foreach (SearchCriteria criterion in searchWords)
            {
                var mandatoryResults = new Dictionary<int, bool>();
                var excludedResults = new Dictionary<int, bool>();
                if (searchResults.ContainsKey(criterion.Criteria))
                {
                    foreach (SearchResultsInfo result in searchResults[criterion.Criteria])
                    {
                        if (criterion.MustInclude)
                        {
                            mandatoryResults[result.SearchItemID] = true;
                            hasMandatory = true;
                        }
                        else if (criterion.MustExclude)
                        {
                            excludedResults[result.SearchItemID] = true;
                            hasExcluded = true;
                        }
                    }
                }
                foreach (KeyValuePair<int, Dictionary<int, SearchResultsInfo>> kvpResults in dicResults)
                {
                    if (hasMandatory && (!mandatoryResults.ContainsKey(kvpResults.Key)))
                    {
                        foreach (SearchResultsInfo result in kvpResults.Value.Values)
                        {
                            result.Delete = true;
                        }
                    }
                    else if (hasExcluded && (excludedResults.ContainsKey(kvpResults.Key)))
                    {
                        foreach (SearchResultsInfo result in kvpResults.Value.Values)
                        {
                            result.Delete = true;
                        }
                    }
                }
            }
            var results = new SearchResultsInfoCollection();
            var objTabController = new TabController();
            var dicTabsAllowed = new Dictionary<int, Dictionary<int, bool>>();
            foreach (KeyValuePair<int, Dictionary<int, SearchResultsInfo>> kvpResults in dicResults)
            {
                foreach (SearchResultsInfo result in kvpResults.Value.Values)
                {
                    if (!result.Delete)
                    {
                        TabInfo objTab = objTabController.GetTab(result.TabId, portalID, false);
                        if (TabPermissionController.CanViewPage(objTab))
                        {
                            ModuleInfo objModule = new ModuleController().GetModule(result.ModuleId, result.TabId, false);
                            if (ModulePermissionController.CanViewModule(objModule))
                            {
                                results.Add(result);
                            }
                        }
                    }
                }
            }
            return results;
        }

        public override void StoreSearchItems(SearchItemInfoCollection SearchItems)
        {
            var Modules = new Dictionary<int, string>();
            foreach (SearchItemInfo item in SearchItems)
            {
                if (!Modules.ContainsKey(item.ModuleId))
                {
                    Modules.Add(item.ModuleId, "en-US");
                }
            }

            var objTabs = new TabController();
            var objModule = new ModuleInfo();
            var objTab = new TabInfo();

            SearchItemInfo searchItem;
            Dictionary<string, SearchItemInfo> indexedItems;
            SearchItemInfoCollection moduleItems;
            foreach (KeyValuePair<int, string> kvp in Modules)
            {
                indexedItems = SearchDataStoreController.GetSearchItems(kvp.Key);
                moduleItems = SearchItems.ModuleItems(kvp.Key);
                for (int iSearch = moduleItems.Count - 1; iSearch >= 0; iSearch += -1)
                {
                    searchItem = moduleItems[iSearch];
                    SearchItemInfo indexedItem = null;
                    if (indexedItems.TryGetValue(searchItem.SearchKey, out indexedItem))
                    {
                        //Get the tab where the search item resides -- used in date comparison
                        objModule = new ModuleController().GetModule(searchItem.ModuleId);
                        objTab = objTabs.GetTab(searchItem.TabId, objModule.PortalID, false);

                        if (indexedItem.PubDate < searchItem.PubDate || indexedItem.PubDate < objModule.LastModifiedOnDate || indexedItem.PubDate < objTab.LastModifiedOnDate)
                        {
                            try
                            {
                                if (searchItem.PubDate < objModule.LastModifiedOnDate)
                                {
                                    searchItem.PubDate = objModule.LastModifiedOnDate;
                                }
                                if (searchItem.PubDate < objTab.LastModifiedOnDate)
                                {
                                    searchItem.PubDate = objTab.LastModifiedOnDate;
                                }

                                searchItem.SearchItemId = indexedItem.SearchItemId;
                                SearchDataStoreController.UpdateSearchItem(searchItem);
                                SearchDataStoreController.DeleteSearchItemWords(searchItem.SearchItemId);
                                AddIndexWords(searchItem.SearchItemId, searchItem, kvp.Value);
                            }
                            catch (Exception ex)
                            {
                                Exceptions.Exceptions.LogException(ex);
                            }
                        }
                        indexedItems.Remove(searchItem.SearchKey);
                        SearchItems.Remove(searchItem);
                    }
                    else
                    {
                        try
                        {
                            int indexID = SearchDataStoreController.AddSearchItem(searchItem);
                            AddIndexWords(indexID, searchItem, kvp.Value);
                        }
                        catch (Exception ex)
                        {
                            Exceptions.Exceptions.LogSearchException(new SearchException(ex.Message, ex, searchItem));
                        }
                    }
                }
            }
        }
    }
}
