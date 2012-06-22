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
using System;
using System.Collections.Generic;

namespace DotNetNuke.Services.Journal
{
    public interface IJournalController
    {
        JournalTypeInfo GetJournalType(string journalType);
        JournalTypeInfo GetJournalTypeById(int journalTypeId);
        JournalItem GetJournalItemByKey(int portalID, string objectKey);
        void DeleteJournalItemByKey(int portalID, string objectKey);
        void DeleteJournalItemByGroupId(int portalId, int groupId);
        void SaveJournalItem(JournalItem journalItem, int tabId);
        JournalItem GetJournalItem(int portalId, int userID, int journalId);
        void DeleteJournalItem(int portalId, int userID, int journalId);
        IEnumerable<JournalTypeInfo> GetJournalTypes(int portalId);
    }
}