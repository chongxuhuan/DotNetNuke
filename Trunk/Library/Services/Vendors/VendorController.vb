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

Imports System
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Services.Vendors

    Public Class VendorController

        Public Function AddVendor(ByVal objVendor As VendorInfo) As Integer
            Return DataProvider.Instance().AddVendor(objVendor.PortalId, objVendor.VendorName, objVendor.Unit, objVendor.Street, objVendor.City, objVendor.Region, objVendor.Country, objVendor.PostalCode, objVendor.Telephone, objVendor.Fax, objVendor.Cell, objVendor.Email, objVendor.Website, objVendor.FirstName, objVendor.LastName, objVendor.UserName, objVendor.LogoFile, objVendor.KeyWords, objVendor.Authorized.ToString)
        End Function

        Public Sub DeleteVendor(ByVal VendorID As Integer)
            DataProvider.Instance().DeleteVendor(VendorID)
            Dim objBanners As New BannerController
            objBanners.ClearBannerCache()
        End Sub

        Public Sub DeleteVendors()
            DeleteVendors(Null.NullInteger)
        End Sub

        Public Sub DeleteVendors(ByVal PortalID As Integer)
            Dim TotalRecords As Integer
            For Each vendor As VendorInfo In GetVendors(PortalID, True, Null.NullInteger, Null.NullInteger, TotalRecords)
                If vendor.Authorized = False Then
                    DeleteVendor(vendor.VendorId)
                End If
            Next
            Dim objBanners As New BannerController
            objBanners.ClearBannerCache()
        End Sub

        Public Sub UpdateVendor(ByVal objVendor As VendorInfo)
            DataProvider.Instance().UpdateVendor(objVendor.VendorId, objVendor.VendorName, objVendor.Unit, objVendor.Street, objVendor.City, objVendor.Region, objVendor.Country, objVendor.PostalCode, objVendor.Telephone, objVendor.Fax, objVendor.Cell, objVendor.Email, objVendor.Website, objVendor.FirstName, objVendor.LastName, objVendor.UserName, objVendor.LogoFile, objVendor.KeyWords, objVendor.Authorized.ToString)
        End Sub

        Public Function GetVendor(ByVal VendorID As Integer, ByVal PortalId As Integer) As VendorInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetVendor(VendorID, PortalId), GetType(VendorInfo)), VendorInfo)
        End Function

        Public Function GetVendors(Optional ByVal PortalId As Integer = -1, Optional ByVal Filter As String = "") As ArrayList
            Dim TotalRecords As Integer
            Return GetVendorsByName(Filter, PortalId, 0, 100000, TotalRecords)
        End Function

        Public Function GetVendors(ByVal PortalId As Integer, ByVal UnAuthorized As Boolean, ByVal PageIndex As Integer, ByVal PageSize As Integer, ByRef TotalRecords As Integer) As ArrayList
            Dim dr As IDataReader = DataProvider.Instance.GetVendors(PortalId, UnAuthorized, PageIndex, PageSize)
            Dim retValue As ArrayList = Nothing
            Try
                While dr.Read
                    TotalRecords = Convert.ToInt32(dr("TotalRecords"))
                End While
                dr.NextResult()
                retValue = CBO.FillCollection(dr, GetType(VendorInfo))
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
            Return retValue
        End Function

        Public Function GetVendorsByEmail(ByVal Filter As String, ByVal PortalId As Integer, ByVal Page As Integer, ByVal PageSize As Integer, ByRef TotalRecords As Integer) As ArrayList
            Dim dr As IDataReader = DataProvider.Instance.GetVendorsByEmail(Filter, PortalId, Page, PageSize)
            Try
                While dr.Read
                    TotalRecords = Convert.ToInt32(dr("TotalRecords"))
                End While
                dr.NextResult()
                Return CBO.FillCollection(dr, GetType(VendorInfo))
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
        End Function

        Public Function GetVendorsByName(ByVal Filter As String, ByVal PortalId As Integer, ByVal Page As Integer, ByVal PageSize As Integer, ByRef TotalRecords As Integer) As ArrayList
            Dim dr As IDataReader = DataProvider.Instance.GetVendorsByName(Filter, PortalId, Page, PageSize)
            Try
                While dr.Read
                    TotalRecords = Convert.ToInt32(dr("TotalRecords"))
                End While
                dr.NextResult()
                Return CBO.FillCollection(dr, GetType(VendorInfo))
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
        End Function

    End Class

End Namespace

