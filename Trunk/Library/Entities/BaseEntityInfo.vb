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

Imports System.Xml.Serialization
Imports System.Xml.Schema
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Tokens
Imports System
Imports System.Xml

Namespace DotNetNuke.Entities

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities
    ''' Class	 : BaseEntityInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' BaseEntityInfo provides auditing fields for Core tables.
    ''' </summary>
    ''' <history>
    ''' 	[jlucarino]	02/20/2009   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public MustInherit Class BaseEntityInfo

#Region "Private Members"

        Private _CreatedByUserID As Integer = Null.NullInteger
        Private _CreatedOnDate As DateTime
        Private _LastModifiedByUserID As Integer = Null.NullInteger
        Private _LastModifiedOnDate As DateTime

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the CreatedByUserID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[jlucarino]	02/20/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False), XmlIgnore()> _
        Public ReadOnly Property CreatedByUserID() As Integer
            Get
                Return _CreatedByUserID
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the CreatedOnDate
        ''' </summary>
        ''' <returns>A DateTime</returns>
        ''' <history>
        ''' 	[jlucarino]	02/20/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False), XmlIgnore()> _
        Public ReadOnly Property CreatedOnDate() As DateTime
            Get
                Return _CreatedOnDate
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the LastModifiedByUserID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[jlucarino]	02/20/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False), XmlIgnore()> _
        Public ReadOnly Property LastModifiedByUserID() As Integer
            Get
                Return _LastModifiedByUserID
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the LastModifiedOnDate
        ''' </summary>
        ''' <returns>A DateTime</returns>
        ''' <history>
        ''' 	[jlucarino]	02/20/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False), XmlIgnore()> _
        Public ReadOnly Property LastModifiedOnDate() As DateTime
            Get
                Return _LastModifiedOnDate
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the UserInfo object associated with this user
        ''' </summary>
        ''' <param name="PortalID">The PortalID associated with the desired user</param>
        ''' <returns>A UserInfo object</returns>
        ''' <history>
        ''' 	[jlucarino]	02/20/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False), XmlIgnore()> _
        Public ReadOnly Property CreatedByUser(ByVal PortalID As Integer) As UserInfo
            Get
                Dim _User As UserInfo = Nothing
                If _CreatedByUserID > Null.NullInteger Then
                    _User = UserController.GetUserById(PortalID, _CreatedByUserID)
                    Return _User
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the UserInfo object associated with this user
        ''' </summary>
        ''' <param name="PortalID">The PortalID associated with the desired user</param>
        ''' <returns>A UserInfo object</returns>
        ''' <history>
        ''' 	[jlucarino]	02/20/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False), XmlIgnore()> _
        Public ReadOnly Property LastModifiedByUser(ByVal PortalID As Integer) As UserInfo
            Get
                Dim _User As UserInfo = Nothing
                If _LastModifiedByUserID > Null.NullInteger Then
                    _User = UserController.GetUserById(PortalID, _LastModifiedByUserID)
                    Return _User
                Else
                    Return Nothing
                End If
            End Get
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a BaseEntityInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[jlucarino]	02/20/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub FillInternal(ByVal dr As System.Data.IDataReader)
            _CreatedByUserID = Null.SetNullInteger(dr("CreatedByUserID"))
            _CreatedOnDate = Null.SetNullDateTime(dr("CreatedOnDate"))
            _LastModifiedByUserID = Null.SetNullInteger(dr("LastModifiedByUserID"))
            _LastModifiedOnDate = Null.SetNullDateTime(dr("LastModifiedOnDate"))
        End Sub


        ''' <summary>
        ''' method used by cbo to fill readonly properties ignored by HydrateObject reflection
        ''' </summary>
        ''' <param name="dr">the data reader to use</param>
        ''' <remarks></remarks>
        Friend Sub FillBaseProperties(ByVal dr As System.Data.IDataReader)
            FillInternal(dr)
        End Sub

#End Region

    End Class
End Namespace

