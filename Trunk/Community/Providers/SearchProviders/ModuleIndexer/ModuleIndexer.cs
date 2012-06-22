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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;

#endregion

namespace DotNetNuke.Services.Search
{
    /// -----------------------------------------------------------------------------
    /// Namespace:  DotNetNuke.Services.Search
    /// Project:    DotNetNuke.Search.Index
    /// Class:      ModuleIndexer
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ModuleIndexer is an implementation of the abstract IndexingProvider
    /// class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ModuleIndexer : IndexingProvider
    {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchIndexItems gets the SearchInfo Items for the Portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        ///     [vnguyen]   09/07/2010  Modified: Included logic to add TabId to searchItems
        /// </history>
        /// -----------------------------------------------------------------------------
        public override SearchItemInfoCollection GetSearchIndexItems(int PortalID)
        {
            var SearchItems = new SearchItemInfoCollection();
            SearchContentModuleInfoCollection SearchCollection = GetModuleList(PortalID);
            foreach (SearchContentModuleInfo ScModInfo in SearchCollection)
            {
                try
                {
                    SearchItemInfoCollection myCollection;
                    myCollection = ScModInfo.ModControllerType.GetSearchItems(ScModInfo.ModInfo);
                    if (myCollection != null)
                    {
                        foreach (SearchItemInfo searchItem in myCollection)
                        {
                            searchItem.TabId = ScModInfo.ModInfo.TabID;
                        }

                        SearchItems.AddRange(myCollection);
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                }
            }
            return SearchItems;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleList gets a collection of SearchContentModuleInfo Items for the Portal
        /// </summary>
        /// <remarks>
        /// Parses the Modules of the Portal, determining whetehr they are searchable.
        /// </remarks>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        protected SearchContentModuleInfoCollection GetModuleList(int PortalID)
        {
            var Results = new SearchContentModuleInfoCollection();
            var objModules = new ModuleController();
            ArrayList arrModules = objModules.GetSearchModules(PortalID);
            var businessControllers = new Hashtable();
            var htModules = new Hashtable();
            foreach (ModuleInfo objModule in arrModules)
            {
                if (!htModules.ContainsKey(objModule.ModuleID))
                {
                    try
                    {
                        //Check if the business controller is in the Hashtable
                        object objController = businessControllers[objModule.DesktopModule.BusinessControllerClass];
                        if (!String.IsNullOrEmpty(objModule.DesktopModule.BusinessControllerClass))
                        {
							//If nothing create a new instance
                            if (objController == null)
                            {
                                objController = Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass);
                                
								//Add to hashtable
								businessControllers.Add(objModule.DesktopModule.BusinessControllerClass, objController);
                            }
                            
							//Double-Check that module supports ISearchable
							if (objController is ISearchable)
                            {
                                var ContentInfo = new SearchContentModuleInfo();
                                ContentInfo.ModControllerType = (ISearchable) objController;
                                ContentInfo.ModInfo = objModule;
                                Results.Add(ContentInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Instrumentation.DnnLog.Error(ex);

                        try
                        {
                            string strMessage = string.Format("Error Creating BusinessControllerClass '{0}' of module({1}) id=({2}) in tab({3}) and portal({4}) ",
                                                              objModule.DesktopModule.BusinessControllerClass,
                                                              objModule.DesktopModule.ModuleName,
                                                              objModule.ModuleID,
                                                              objModule.TabID,
                                                              objModule.PortalID);
                            throw new Exception(strMessage, ex);
                        }
                        catch (Exception ex1)
                        {
                            Exceptions.Exceptions.LogException(ex1);
                        }
                    }
                    finally
                    {
                        htModules.Add(objModule.ModuleID, objModule.ModuleID);
                    }
                }
            }
            return Results;
        }
    }
}
