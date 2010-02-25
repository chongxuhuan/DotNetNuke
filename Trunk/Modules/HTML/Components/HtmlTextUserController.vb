'
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
'
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.Html
    ''' Project:    DotNetNuke
    ''' Class:      HtmlTextUserController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The HtmlTextUserController is the Controller class for managing User information the HtmlText module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class HtmlTextUserController

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetHtmlTextUser retrieves a collection of HtmlTextUserInfo objects for an Item
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="UserID">The Id of the User</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetHtmlTextUser(ByVal UserID As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().GetHtmlTextUser(UserID), GetType(HtmlTextUserInfo))

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddHtmlTextUser creates a new HtmlTextUser for an Item
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="objHtmlTextUser">An HtmlTextUserInfo object</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddHtmlTextUser(ByVal objHtmlTextUser As HtmlTextUserInfo)

            DataProvider.Instance().AddHtmlTextUser(objHtmlTextUser.ItemID, objHtmlTextUser.StateID, objHtmlTextUser.ModuleID, objHtmlTextUser.TabID, objHtmlTextUser.UserID)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteHtmlTextUsers cleans up old HtmlTextUser records
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteHtmlTextUsers()

            DataProvider.Instance().DeleteHtmlTextUsers()

        End Sub
#End Region

    End Class

End Namespace

