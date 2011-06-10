using System;

using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Icons;

namespace DotNetNuke.Services.FileSystem
{
    internal class IconControllerWrapper : ComponentBase<IIconController, IconControllerWrapper>, IIconController
    {
        #region Implementation of IIconController

        public string IconURL(string key)
        {
            return IconController.IconURL(key);
        }

        #endregion
    }
}
