/*
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
*/

using System;
using Moq;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Web.Mvp.Framework;

namespace DotNetNuke.Tests.UI.Mvp
{
    public abstract class PresenterTestBase<TPresenter, TView, TModel>
        where TPresenter : Presenter<TView, TModel>
        where TView : class, IView
        where TModel : PresenterModel, new() {

        protected virtual TPresenter CreatePresenter() {
            return CreatePresenter(Constants.USER_ValidId);
        }

        protected virtual TPresenter CreatePresenter(bool isPostBack) {
            return CreatePresenter(Constants.USER_ValidId, isPostBack);
        }

        protected virtual TPresenter CreatePresenter(int userId) {
            return CreatePresenter(userId, false);
        }

        protected virtual TPresenter CreatePresenter(int userId, bool isPostBack) {
            return CreatePresenter(new TModel {
                UserId = userId,
                IsPostBack = isPostBack
            });
        }

        protected virtual TPresenter CreatePresenter(TModel model) {
            TPresenter presenter = ConstructPresenter();
            presenter.Initialize(new Mock<TView>().Object, model, new Mock<IPresenterEnvironment>().Object);
            ConfigureSettings();
            return presenter;
        }

        protected abstract TPresenter ConstructPresenter();

        protected virtual void ConfigureSettings() {
        }

        protected void RunAnonymousUserTest() {
            RunAnonymousUserTest(p => p.Load());
        }

        protected void RunAnonymousUserTest(Action<TPresenter> presenterCall) {
            // Arrange
            TPresenter presenter = CreatePresenter(Constants.USER_AnonymousUserId, false);

            // Act
            presenterCall(presenter);

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToLogin());
        }

        protected void RunUnauthorizedUserTest() {
            RunUnauthorizedUserTest(p => p.Load());
        }

        protected void RunUnauthorizedUserTest(Action<TPresenter> presenterCall) {
            // Arrange
            TPresenter presenter = CreatePresenter(Constants.USER_InValidId, false);
            SetupUnauthorizedUser(presenter);

            // Act
            presenterCall(presenter);

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToPresenter(presenter.Model.DefaultPresenter));
        }

        protected void RunInitializeWithNullViewTest() {
            TPresenter presenter = ConstructPresenter();
            AutoTester.ArgumentNull<TView>(m => presenter.Initialize(m, new TModel(), new Mock<IPresenterEnvironment>().Object));
        }

        protected void RunInitializeWithNullModelTest() {
            TPresenter presenter = ConstructPresenter();
            AutoTester.ArgumentNull<TModel>(m => presenter.Initialize(new Mock<TView>().Object, m, new Mock<IPresenterEnvironment>().Object));
        }

        protected void RunInitializeWithNullEnvironmentTest() {
            TPresenter presenter = ConstructPresenter();
            AutoTester.ArgumentNull<IPresenterEnvironment>(m => presenter.Initialize(new Mock<TView>().Object, new TModel(), m));
        }

        protected virtual void SetupUnauthorizedUser(TPresenter presenter) {

        }
    }
}
