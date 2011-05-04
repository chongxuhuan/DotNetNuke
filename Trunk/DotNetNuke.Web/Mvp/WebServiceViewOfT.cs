using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WebFormsMvp;

namespace DotNetNuke.Web.Mvp
{
    public abstract class WebServiceViewOfT<TModel> : ModuleViewBase, IView<TModel> where TModel : class, new()
    {
        private TModel _model;

        #region IView<TModel> Members

        public TModel Model
        {
            get
            {
                if ((_model == null))
                {
                    throw new InvalidOperationException(
                        "The Model property is currently null, however it should have been automatically initialized by the presenter. This most likely indicates that no presenter was bound to the control. Check your presenter bindings.");
                }
                return _model;
            }
            set { _model = value; }
        }

        #endregion
    }
}
