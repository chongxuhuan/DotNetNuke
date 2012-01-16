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
using System.Data;

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Data
{
    internal class DataService : ComponentBase<IDataService, DataService>, IDataService
        
    {
        private readonly DataProvider _provider = DataProvider.Instance();

        #region Messages CRUD

        public void SaveSocialMessage(Message message, int createUpdateUserID)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetSocialMessage()
        {
            throw new NotImplementedException();
        }

        public IDataReader GetSocialMessagesBySender()
        {
            throw new NotImplementedException();
        }

        public void DeleteSocialMessage(int messageID)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Message_Recipients CRUD

        public void SaveSocialMessageRecipient(MessageRecipient messageRecipient, int createUpdateUserID)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetSocialMessageRecipient()
        {
            throw new NotImplementedException();
        }

        public IDataReader GetSocialMessageRecipientsByUser()
        {
            throw new NotImplementedException();
        }

        public IDataReader GetSocialMessageRecipientsByMessage()
        {
            throw new NotImplementedException();
        }

        public void DeleteSocialMessageRecipient(int messageRecipientID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Message_Attachments CRUD

        public void SaveSocialMessageAttachment(MessageAttachment messageAttachment, int createUpdateUserID)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetSocialMessageAttachment()
        {
            throw new NotImplementedException();
        }

        public IDataReader GetSocialMessageAttachmentsByMessage()
        {
            throw new NotImplementedException();
        }

        public void DeleteSocialMessageAttachment(int messageAttachmentID)
        {
            throw new NotImplementedException();
        }

        #endregion
       
    }
}