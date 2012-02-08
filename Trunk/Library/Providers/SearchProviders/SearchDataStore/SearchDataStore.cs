#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.Web;

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
		#region "Private Methods"

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
			//Get the Search Settings for this Portal
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
            
            // split content into words
            string[] ContentWords = Content.Split(' ');
            
            //process each word
            int intWord = 0;
            foreach (string strWord in ContentWords)
            {
                if (CanIndexWord(strWord, language, settings))
                {
                    var encodedWord = HttpUtility.HtmlEncode(strWord);
                    intWord = intWord + 1;
                    if (IndexWords.ContainsKey(encodedWord) == false)
                    {
                        IndexWords.Add(encodedWord, 0);
                        IndexPositions.Add(encodedWord, new List<int>());
                    }
                    //track number of occurrences of word in content
                    IndexWords[encodedWord] = IndexWords[encodedWord] + 1;
                    //track positions of word in content
                    IndexPositions[encodedWord].Add(intWord);
                }
            }
			
            //get list of words ( non-common )
            Hashtable Words = GetSearchWords(); //this could be cached
            int WordId;

            //iterate through each indexed word
            foreach (object objWord in IndexWords.Keys)
            {
                string strWord = Convert.ToString(objWord);
                if (Words.ContainsKey(strWord))
                {
					//word is in the DataStore
                    WordId = Convert.ToInt32(Words[strWord]);
                }
                else
                {
					//add the word to the DataStore
                    WordId = DataProvider.Instance().AddSearchWord(strWord);
                    Words.Add(strWord, WordId);
                }
                //add the indexword
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
			//Create Boolean to hold return value
            bool retValue = true;

            //get common words for exclusion
            Hashtable CommonWords = GetCommonWords(Locale);
            
			//Determine if Word is actually a number
			if (Regex.IsMatch(strWord, "^\\d+$"))
            {
                //Word is Numeric
                if (!settings.SearchIncludeNumeric)
                {
                    retValue = false;
                }
            }
            else
            {
				//Word is Non-Numeric
                //Determine if Word satisfies Minimum/Maximum length
                if (strWord.Length < settings.SearchMinWordlLength || strWord.Length > settings.SearchMaxWordlLength)
                {
                    retValue = false;
                }
                else if (CommonWords.ContainsKey(strWord) && !settings.SearchIncludeCommon)
                {
					//Determine if Word is a Common Word (and should be excluded)
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
		
		#endregion

		#region "Protected Methods"
		
        protected virtual string GetSearchContent(SearchItemInfo SearchItem)
        {
            return SearchItem.Content;
        }
		
		#endregion

		#region "Public Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchItems gets a collection of Search Items for a Module/Tab/Portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="PortalID">A Id of the Portal</param>
        /// <param name="TabID">A Id of the Tab</param>
        /// <param name="ModuleID">A Id of the Module</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public override SearchResultsInfoCollection GetSearchItems(int PortalID, int TabID, int ModuleID)
        {
            return SearchDataStoreController.GetSearchResults(PortalID, TabID, ModuleID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchResults gets the search results for a passed in criteria string
        /// </summary>
        /// <remarks>
        /// </remarks>
		/// <param name="portalID">A Id of the Portal</param>
		/// <param name="criteria">The criteria string</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public override SearchResultsInfoCollection GetSearchResults(int portalID, string criteria)
        {
            SearchResultsInfo searchResult;
            bool hasExcluded = Null.NullBoolean;
            bool hasMandatory = Null.NullBoolean;

            var objPortalController = new PortalController();
            PortalInfo objPortal = objPortalController.GetPortal(portalID);

            //Get the Settings for this Portal
            var _PortalSettings = new PortalSettings(objPortal);

            //We will assume that the content is in the locale of the Portal
            Hashtable commonWords = GetCommonWords(_PortalSettings.DefaultLanguage);

            //clean criteria
            criteria = criteria.ToLower();

            //split search criteria into words
            var searchWords = new SearchCriteriaCollection(criteria);

            var searchResults = new Dictionary<string, SearchResultsInfoCollection>();

            //dicResults is a Dictionary(Of SearchItemID, Dictionary(Of TabID, SearchResultsInfo)
            var dicResults = new Dictionary<int, Dictionary<int, SearchResultsInfo>>();

            //iterate through search criteria words
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
							//Add results to dicResults
                            if (!criterion.MustExclude)
                            {
                                if (dicResults.ContainsKey(result.SearchItemID))
                                {
                                    //The Dictionary exists for this SearchItemID already so look in the TabId keyed Sub-Dictionary
                                    Dictionary<int, SearchResultsInfo> dic = dicResults[result.SearchItemID];
                                    if (dic.ContainsKey(result.TabId))
                                    {
                                        //The sub-Dictionary contains the item already so update the relevance
                                        searchResult = dic[result.TabId];
                                        searchResult.Relevance += result.Relevance;
                                    }
                                    else
                                    {
										//Add Entry to Sub-Dictionary
                                        dic.Add(result.TabId, result);
                                    }
                                }
                                else
                                {
									//Create new TabId keyed Dictionary
                                    var dic = new Dictionary<int, SearchResultsInfo>();
                                    dic.Add(result.TabId, result);

                                    //Add new Dictionary to SearchResults
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
							//Add to mandatory results lookup
                            mandatoryResults[result.SearchItemID] = true;
                            hasMandatory = true;
                        }
                        else if (criterion.MustExclude)
                        {
							//Add to exclude results lookup
                            excludedResults[result.SearchItemID] = true;
                            hasExcluded = true;
                        }
                    }
                }
                foreach (KeyValuePair<int, Dictionary<int, SearchResultsInfo>> kvpResults in dicResults)
                {
                    //The key of this collection is the SearchItemID,  Check if the value of this collection should be processed
                    if (hasMandatory && (!mandatoryResults.ContainsKey(kvpResults.Key)))
                    {
                        //1. If mandatoryResults exist then only process if in mandatoryResults Collection
                        foreach (SearchResultsInfo result in kvpResults.Value.Values)
                        {
                            result.Delete = true;
                        }
                    }
                    else if (hasExcluded && (excludedResults.ContainsKey(kvpResults.Key)))
                    {
                        //2. Do not process results in the excludedResults Collection
                        foreach (SearchResultsInfo result in kvpResults.Value.Values)
                        {
                            result.Delete = true;
                        }
                    }
                }
            }
			
            //Process results against permissions and mandatory and excluded results
            var results = new SearchResultsInfoCollection();
            var objTabController = new TabController();
            var dicTabsAllowed = new Dictionary<int, Dictionary<int, bool>>();
            foreach (KeyValuePair<int, Dictionary<int, SearchResultsInfo>> kvpResults in dicResults)
            {
                foreach (SearchResultsInfo result in kvpResults.Value.Values)
                {
                    if (!result.Delete)
                    {
						//Check If authorised to View Tab
                        TabInfo objTab = objTabController.GetTab(result.TabId, portalID, false);
                        if (TabPermissionController.CanViewPage(objTab))
                        {
							//Check If authorised to View Module
                            ModuleInfo objModule = new ModuleController().GetModule(result.ModuleId, result.TabId, false);
                            if (ModulePermissionController.CanViewModule(objModule))
                            {
                                results.Add(result);
                            }
                        }
                    }
                }
            }
			
            //Return Search Results Collection
            return results;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// StoreSearchItems adds the Search Item to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="SearchItems">A Collection of SearchItems</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        ///     [vnguyen]   09/07/2010  Modified: Added a date comparison for LastModifiedDate on the Tab
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void StoreSearchItems(SearchItemInfoCollection SearchItems)
        {
            //For now as we don't support Localized content - set the locale to the default locale. This
            //is to avoid the error in GetDefaultLanguageByModule which artificially limits the number
            //of modules that can be indexed.  This will need to be addressed when we support localized content.
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

            //Process the SearchItems by Module to reduce Database hits
            foreach (KeyValuePair<int, string> kvp in Modules)
            {
                indexedItems = SearchDataStoreController.GetSearchItems(kvp.Key);

                //Get the Module's SearchItems to compare
                moduleItems = SearchItems.ModuleItems(kvp.Key);

                //As we will be potentially removing items from the collection iterate backwards
                for (int iSearch = moduleItems.Count - 1; iSearch >= 0; iSearch += -1)
                {
                    searchItem = moduleItems[iSearch];

                    //Get item from Indexed collection
                    SearchItemInfo indexedItem = null;
                    if (indexedItems.TryGetValue(searchItem.SearchKey, out indexedItem))
                    {
                        //Get the tab where the search item resides -- used in date comparison
                        objModule = new ModuleController().GetModule(searchItem.ModuleId);
                        objTab = objTabs.GetTab(searchItem.TabId, objModule.PortalID, false);

                        //Item exists so compare Dates to see if modified
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

                                
                                //Content modified so update SearchItem and delete item's Words Collection
                                searchItem.SearchItemId = indexedItem.SearchItemId;
                                SearchDataStoreController.UpdateSearchItem(searchItem);
                                SearchDataStoreController.DeleteSearchItemWords(searchItem.SearchItemId);

                                //re-index the content
                                AddIndexWords(searchItem.SearchItemId, searchItem, kvp.Value);
                            }
                            catch (Exception ex)
                            {
								//Log Exception
                                Exceptions.Exceptions.LogException(ex);
                            }
                        }
						
                        //Remove Items from both collections
                        indexedItems.Remove(searchItem.SearchKey);
                        SearchItems.Remove(searchItem);
                    }
                    else
                    {
                        try
                        {
							//Item doesn't exist so Add to Index
                            int indexID = SearchDataStoreController.AddSearchItem(searchItem);
							//index the content
                            AddIndexWords(indexID, searchItem, kvp.Value);
                        }
                        catch (Exception ex)
                        {
                            //Exception is probably a duplicate key error which is probably due to bad module data
                            Exceptions.Exceptions.LogSearchException(new SearchException(ex.Message, ex, searchItem));
                        }
                    }
                }
            }
        }
		
		#endregion
    }
}
