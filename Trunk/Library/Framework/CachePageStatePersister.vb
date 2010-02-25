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

Imports System.Text
Imports DotNetNuke.Services.Cache

Namespace DotNetNuke.Framework

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Framework
    ''' Project:    DotNetNuke
    ''' Class:      CachePageStatePersister
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' CachePageStatePersister provides a cache based page state peristence mechanism
    ''' </summary>
    ''' <history>
    '''		[cnurse]	11/30/2006	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CachePageStatePersister
        Inherits PageStatePersister

        Private Const VIEW_STATE_CACHEKEY As String = "__VIEWSTATE_CACHEKEY"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the CachePageStatePersister
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	    11/30/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal page As Page)
            MyBase.New(page)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads the Page State from the Cache
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	    11/30/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Load()

            ' Get the cache key from the web form data
            Dim key As String = TryCast(Page.Request.Params(VIEW_STATE_CACHEKEY), String)

            'Abort if cache key is not available or valid
            If String.IsNullOrEmpty(key) Or Not key.StartsWith("VS_") Then
                Throw New ApplicationException("Missing valid " + VIEW_STATE_CACHEKEY)
            End If

            Dim state As Pair = DataCache.GetCache(Of Pair)(key)

            If Not state Is Nothing Then
                'Set view state and control state
                ViewState = state.First
                ControlState = state.Second
            End If

            'Remove this ViewState from the cache as it has served its purpose
            DataCache.RemoveCache(key)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Saves the Page State to the Cache
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	    11/30/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Save()

            'No processing needed if no states available
            If ViewState Is Nothing And ControlState Is Nothing Then
                Exit Sub
            End If

            'Generate a unique cache key
            Dim key As New StringBuilder()
            With key
                .Append("VS_")
                .Append(IIf(Page.Session Is Nothing, Guid.NewGuid().ToString(), Page.Session.SessionID))
                .Append("_")
                .Append(DateTime.Now.Ticks.ToString())
            End With

            'Save view state and control state separately
            Dim state As New Pair(ViewState, ControlState)

            'Add view state and control state to cache
            Dim objDependency As DNNCacheDependency = Nothing
            DataCache.SetCache(key.ToString(), state, objDependency, DateTime.Now.AddMinutes(Page.Session.Timeout), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, Nothing)

            'Register hidden field to store cache key in
            Page.ClientScript.RegisterHiddenField(VIEW_STATE_CACHEKEY, key.ToString())

        End Sub

    End Class

End Namespace

