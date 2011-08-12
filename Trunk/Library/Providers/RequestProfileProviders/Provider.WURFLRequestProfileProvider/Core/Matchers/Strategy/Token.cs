/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Matchers.Strategy
{
    public class Token
    {
        private readonly string _content;
        private readonly int _weight;

        public Token(string content, int weight)
        {
            _content = content;
            _weight = weight;
        }

        public int Weight
        {
            get { return _weight; }
        }


        public string Content
        {
            get { return _content; }
        }
    }
}