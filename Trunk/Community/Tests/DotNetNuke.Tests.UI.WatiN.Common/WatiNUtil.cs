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
using System.Threading;

using WatiN.Core;
using WatiN.Core.Native.Windows;

namespace DotNetNuke.Tests.UI.WatiN.Common
{
    public class WatiNUtil
    {
        #region Miscellaneous Methods

        /// <summary>
        /// Closes the given IE instance.
        /// </summary>
        /// <param name="ieInstance">The IE instance to be closed.</param>
        public static void CloseIEInstance(IE ieInstance)
        {
            if (ieInstance != null)
            {
                ieInstance.ForceClose();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Opens a new IE instance with the given settings in front of other windows that are currently open.
        /// </summary>
        /// <param name="targetURL">The URL that the IE instance should browse to.</param>
        /// <param name="silentMode">If true the test will be run without displaying any IE instance or steps.</param>
        /// <param name="timeOut">The default time out to use when calling IE.AttachToIE(findby).</param>
        /// <param name="autoCloseIE">If true when a reference to the IE instance is destroyed the actual window will close.</param>
        /// <param name="movePointer">It true the mouse pointer will move to the top left corner of the screen when a new IE instance is created.</param>
        /// <returns>The new IE instance.</returns>
        public static IE OpenIEInstance(string targetURL, bool silentMode, int timeOut, bool autoCloseIE, bool movePointer)
        {
            Settings.MakeNewIeInstanceVisible = !silentMode;
            Settings.WaitForCompleteTimeOut = timeOut;
            Settings.WaitUntilExistsTimeOut = timeOut;
            Settings.AttachToBrowserTimeOut = timeOut;
            Settings.AutoMoveMousePointerToTopLeft = movePointer;

            IE ieInstance = new IE { AutoClose = autoCloseIE };
            if(!silentMode) ieInstance.ShowWindow(NativeMethods.WindowShowStyle.ShowMaximized);
            ieInstance.BringToFront();
            ieInstance.GoTo(targetURL);

            return ieInstance;
        }

        public static void SelectRadioButtonByName(IE ieInstance, string controlName, string selectedValue)
        {
            RadioButtonCollection rbCollection = ieInstance.RadioButtons.Filter(Find.ByName(s => s.Contains(controlName)));

            foreach (RadioButton rb in rbCollection)
            {
                if (selectedValue != rb.GetAttributeValue("value")) continue;
                rb.Checked = true;
                break;
            }
        }
        public static void SelectRadioButtonByName(Frame frame, string controlName, string selectedValue)
        {
            RadioButtonCollection rbCollection = frame.RadioButtons.Filter(Find.ByName(s => s.Contains(controlName)));

            foreach (RadioButton rb in rbCollection)
            {
                if (selectedValue != rb.GetAttributeValue("value")) continue;
                rb.Checked = true;
                break;
            }
        }

        public static void SelectCheckBoxByName(IE ieInstance, string selectedValue)
        {
            CheckBoxCollection cbCollection = ieInstance.CheckBoxes;
            foreach (CheckBox cb in cbCollection)
            {
                if (selectedValue != ieInstance.Label(Find.ByFor(cb.Id)).Text) continue;
                cb.Checked = true;
                break;
            }
        }
        public static void SelectAllCheckBoxes(IE ieInstance)
        {
            CheckBoxCollection cbCollection = ieInstance.CheckBoxes;
            foreach (CheckBox cb in cbCollection)
            {
                if (!cb.Enabled) continue;
                cb.Checked = true;
            }
        }
        #endregion    }
    }
}
