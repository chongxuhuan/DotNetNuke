/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Request
{
    internal class NormalizersFactory : IChainFactory<INormalizer<string>>
    {
        private static readonly ICollection<INormalizer<string>> _normalizers = new List<INormalizer<string>>();

        static NormalizersFactory()
        {
            _normalizers.Add(new VodafoneNormalizer());
            _normalizers.Add(new BlackBerryNormalizer());
            _normalizers.Add(new UPLinkNormalizer());
            _normalizers.Add(new YesWAPNormalizer());
            _normalizers.Add(new BabelFishNormalizer());
        }

        #region IChainFactory<INormalizer<string>> Members

        public ICollection<INormalizer<string>> Create()
        {
            return _normalizers;
        }

        #endregion
    }
}