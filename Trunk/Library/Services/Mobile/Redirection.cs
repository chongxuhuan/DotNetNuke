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
using System.Collections.Generic;

using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Mobile
{
	public class Redirection : IRedirection, IHydratable
	{
		private int _id = -1;
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public int PortalId
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int SourceTabId
		{
			get;
			set;
		}

		public RedirectionType Type
		{
			get;
			set;
		}

		private IList<IMatchRules> _matchRules;
		public IList<IMatchRules> MatchRules
		{
			get
			{
				return _matchRules;
			}
			set
			{
				_matchRules = value;
			}
		}

		public TargetType TargetType
		{
			get;
			set;
		}

		public object TargetValue
		{
			get;
			set;
		}

		public int SortOrder
		{
			get;
			set;
		}

		public int KeyID
		{
			get
			{
				return this.Id;
			}
			set
			{
				this.Id = value;
			}
		}

		public void Fill(System.Data.IDataReader dr)
		{

		}
	}
}
