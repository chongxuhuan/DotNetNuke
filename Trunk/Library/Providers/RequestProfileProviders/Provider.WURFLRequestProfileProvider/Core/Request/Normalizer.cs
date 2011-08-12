/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Request
{
    internal class Normalizer : INormalizer<string>
    {
        private readonly ICollection<INormalizer<string>> _normalizers;


        /// <summary>
        /// Initializes a new instance of the <see cref="Normalizer"/> class.
        /// </summary>
        /// <param name="normalizers">The normalizers.</param>
        public Normalizer(ICollection<INormalizer<string>> normalizers)
        {
            if (normalizers == null)
            {
                _normalizers = new List<INormalizer<string>>();
            }
            else
            {
                _normalizers = normalizers;
            }
        }

        #region INormalizer<string> Members

        /// <summary>
        /// Normalizes the specified value by passing it through a chain of
        /// normalizers
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string Normalize(string value)
        {
            string normalizedValue = value;
            foreach (INormalizer<String> normalizer in _normalizers)
            {
                normalizedValue = normalizer.Normalize(normalizedValue);
            }

            return normalizedValue;
        }

        #endregion

        public void RegisterNormalizer(INormalizer<String> normalizer)
        {
            _normalizers.Add(normalizer);
        }
    }
}