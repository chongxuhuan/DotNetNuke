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

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Messaging.Data;

#endregion

namespace DotNetNuke.Services.Messaging
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The Controller class for Messaging
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public class MessagingController : IMessagingController
    {
        private static TabInfo _MessagingPage;

        #region "Constructors"

        public MessagingController() : this(GetDataService())
        {
        }

        public MessagingController(IMessagingDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Shared Methods"

        private static IMessagingDataService GetDataService()
        {
            var ds = ComponentFactory.GetComponent<IMessagingDataService>();

            if (ds == null)
            {
                ds = new MessagingDataService();
                ComponentFactory.RegisterComponentInstance<IMessagingDataService>(ds);
            }
            return ds;
        }

        #endregion

        #region "Public Shared Methods"

        public static string DefaultMessagingURL(string ModuleFriendlyName)
        {
            TabInfo page = MessagingPage(ModuleFriendlyName);
            if (page != null)
            {
                return MessagingPage(ModuleFriendlyName).FullUrl;
            }
            else
            {
                return null;
            }
        }

        public static TabInfo MessagingPage(string ModuleFriendlyName)
        {
            if (((_MessagingPage != null)))
            {
                return _MessagingPage;
            }

            var mc = new ModuleController();
            ModuleInfo md = mc.GetModuleByDefinition(PortalSettings.Current.PortalId, ModuleFriendlyName);
            if ((md != null))
            {
                ArrayList a = mc.GetModuleTabs(md.ModuleID);
                if ((a != null))
                {
                    var mi = a[0] as ModuleInfo;
                    if ((mi != null))
                    {
                        var tc = new TabController();
                        _MessagingPage = tc.GetTab(mi.TabID, PortalSettings.Current.PortalId, false);
                    }
                }
            }

            return _MessagingPage;
        }

        #endregion

        #region "Public Methods"

        public Message GetMessageByID(int PortalID, int UserID, int messageId)
        {
            return (Message) CBO.FillObject(_DataService.GetMessageByID(messageId), typeof (Message));
        }

        public List<Message> GetUserInbox(int PortalID, int UserID, int PageNumber, int PageSize)
        {
            return CBO.FillCollection<Message>(_DataService.GetUserInbox(PortalID, UserID, PageNumber, PageSize));
        }

        public int GetInboxCount(int PortalID, int UserID)
        {
            return _DataService.GetInboxCount(PortalID, UserID);
        }

        public int GetNewMessageCount(int PortalID, int UserID)
        {
            return _DataService.GetNewMessageCount(PortalID, UserID);
        }

        public Message GetNextMessageForDispatch(Guid SchedulerInstance)
        {
            return (Message) CBO.FillObject(_DataService.GetNextMessageForDispatch(SchedulerInstance), typeof (Message));
        }


        public long SaveMessage(Message message)
        {
            if ((PortalSettings.Current != null))
            {
                message.PortalID = PortalSettings.Current.PortalId;
            }

            if ((message.Conversation == null || message.Conversation == Guid.Empty))
            {
                message.Conversation = Guid.NewGuid();
            }

            return _DataService.SaveMessage(message);
        }

        public void UpdateMessage(Message message)
        {
            _DataService.UpdateMessage(message);
        }

        public void MarkMessageAsDispatched(int MessageID)
        {
            _DataService.MarkMessageAsDispatched(MessageID);
        }

        #endregion

        private readonly IMessagingDataService _DataService;
    }
}
