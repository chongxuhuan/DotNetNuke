#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.XPath;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;
using System.Net;
using System.Xml;

#endregion

namespace DotNetNuke.Modules.Admin.Extensions
{
    public partial class PurchasedExtensions : ModuleUserControlBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Request["fileid"]!=null && Request["fileAction"]!=null)
            {
                GetFile(Request["fileAction"], Request["fileid"]);
            }
            fetchExtensions.Click += FetchExtensionsClick;

            setupCredentials.Visible = false;
            updateCredentials.Visible = false;
            fetchExtensions.Visible = false;

            setupCredentials.NavigateUrl = ModuleContext.EditUrl("Store");
            updateCredentials.NavigateUrl = ModuleContext.EditUrl("Store");
            Dictionary<string, string> settings = PortalController.GetPortalSettingsDictionary(ModuleContext.PortalId);
            if (settings.ContainsKey("Store_Username"))
            {
                fetchExtensions.Visible = true;
                updateCredentials.Visible = true;
                //GetExtensions2();
            }
            else
            {
                setupCredentials.Visible = true;
            }
        }

        private void GetFile(string fileAction, string fileId)
        {
            string fileCheck = Localization.GetString("StoreFile", LocalResourceFile);
            string postData = "";
            Stream oStream;
            Dictionary<string, string> settings = PortalController.GetPortalSettingsDictionary(ModuleContext.PortalId);
            PortalSecurity ps = new PortalSecurity();
            string username = ps.DecryptString(settings["Store_Username"], Config.GetDecryptionkey());
            string password = ps.DecryptString(settings["Store_Password"], Config.GetDecryptionkey());
            postData = postData + "username=" + username + "&password=" + password + "&fileid=" + fileId;

            WebRequest request = WebRequest.Create(fileCheck.ToString());

            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            // oStream = response.GetResponseStream();
            Stream remoteStream = null;
            Stream localStream = null;
            var installFolder = HttpContext.Current.Server.MapPath("~/Install/") + "module";
            string myfile = "test.zip";
            if (fileAction == "download")
            {
                var objResponse = HttpContext.Current.Response;
                var aByteArray = new byte[response.ContentLength];

                objResponse.ClearContent();
                objResponse.ClearHeaders();
                objResponse.BufferOutput = true;
                objResponse.AppendHeader("Content-Disposition", "attachment; filename=\"" + myfile + "\"");
                objResponse.AppendHeader("Content-Length", response.ContentLength.ToString());
                objResponse.ContentType = response.ContentType;
                objResponse.Flush();
                using (var aWriter = new BinaryWriter(Response.OutputStream))
                {
                    aWriter.Write(aByteArray, 0, aByteArray.Length);
                }
                objResponse.End();
            }
            else
            {
                try
                {
                    // Once the WebResponse object has been retrieved,
                    // get the stream object associated with the response's data
                    remoteStream = response.GetResponseStream();

                    // Create the local file
                    //localStream = File.Create(installFolder + "/" + myfile.ToLower().Replace(".zip", ".resources"));
                    localStream = File.Create(installFolder + "/" + "test.zip");

                    // Allocate a 1k buffer
                    var buffer = new byte[1024];
                    int bytesRead;

                    // Simple do/while loop to read from stream until
                    // no bytes are returned
                    do
                    {
                        // Read data (up to 1k) from the stream
                        bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                        // Write the data to the local file
                        localStream.Write(buffer, 0, bytesRead);

                        // Increment total bytes processed
                        //TODO fix this line bytesProcessed += bytesRead;
                    } while (bytesRead > 0);
                }
                finally
                {
                    // Close the response and streams objects here 
                    // to make sure they're closed even if an exception
                    // is thrown at some point
                    if (remoteStream != null)
                    {
                        remoteStream.Close();
                    }
                    if (localStream != null)
                    {
                        localStream.Close();
                    }
                }
            }
        }

        protected void FetchExtensionsClick(object sender, EventArgs e)
        {
            GetExtensions2();
        }

        protected void GetExtensions2()
        {
            string fileCheck = Localization.GetString("StoreFile", LocalResourceFile);
            string postData="";
            Stream oStream;
            Dictionary<string, string> settings = PortalController.GetPortalSettingsDictionary(ModuleContext.PortalId);
            PortalSecurity ps = new PortalSecurity();
            string username = ps.DecryptString(settings["Store_Username"], Config.GetDecryptionkey());
            string password = ps.DecryptString(settings["Store_Password"], Config.GetDecryptionkey());
            postData = postData + "username=" + username + "&password=" + password;
            
            WebRequest request = WebRequest.Create(fileCheck.ToString());
            
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
          //  string postData = "username=tanyariepe&password=primavera";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            oStream = response.GetResponseStream();
            XmlTextReader oReader;
            XPathDocument oXMLDocument;
            oReader = new XmlTextReader((oStream));
            DataTable dt = new DataTable();
            //instance of a datarow  
            DataRow drow;
            //creating two datacolums Column1 and Column2   
            DataColumn dcol1 = new DataColumn("Package", typeof(string));
            DataColumn dcol2 = new DataColumn("Filename", typeof(string));
            DataColumn dcol3 = new DataColumn("Download", typeof(string));

            DataColumn dcol4 = new DataColumn("Deploy", typeof(string));
            //adding datacolumn to datatable  
            dt.Columns.Add(dcol1);
            dt.Columns.Add(dcol2);
            dt.Columns.Add(dcol3);
            dt.Columns.Add(dcol4);
            oReader.XmlResolver = null;
            oXMLDocument = new XPathDocument(oReader);
            XPathNavigator nav = oXMLDocument.CreateNavigator();
            var iterator = nav.Select("orders/order/orderdetails/orderdetail");
            int i = 0;
            while (iterator.MoveNext())
            {
                //instance of a datarow  
                drow = dt.NewRow();
                //add rows to datatable  
                dt.Rows.Add(drow);
                var packageName = iterator.Current.GetAttribute("packagename", "").Replace("'", "''").Trim();
                var fileName = iterator.Current.SelectSingleNode("files/file").GetAttribute("filename", "");
                var fileId = iterator.Current.SelectSingleNode("files/file").GetAttribute("fileid", "");
                var deploy = iterator.Current.SelectSingleNode("files/file").GetAttribute("deploy", "");
                //add Column values  
                dt.Rows[i][dcol1] = packageName.ToString();
                dt.Rows[i][dcol2] = fileName.ToString();
                //dt.Rows[i][dcol3] = "<a class='dnnPrimaryAction' href='" + Localization.GetString("StoreFile", LocalResourceFile) + fileCheck +
                //                    "&fileid=" + fileId.ToString() + "'>" + LocalizeString("download") + "</a>";
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                dt.Rows[i][dcol3] = "<a class='dnnPrimaryAction' href='" + Globals.NavigateURL(_portalSettings.ActiveTab.TabID, Null.NullString, "fileAction","download","fileid", fileId.ToString()) + "'>" + LocalizeString("download") + "</a>";


                if (deploy == "false")
                {
                    //PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                    //dt.Rows[i][dcol4] = "<a class='dnnPrimaryAction' href='" + Globals.NavigateURL(_portalSettings.ActiveTab.TabID, Null.NullString, "fileAction", "deploy", "fileid", fileId.ToString()) + "'>" + LocalizeString("deploy") + "</a>";
                    dt.Rows[i][dcol4] = "<a class='dnnPrimaryAction' href=" + "\"" + ModuleContext.EditUrl("fileID", fileId.ToString(), "Download","package", packageName.ToString() ) + "\"" + ">" + LocalizeString("deploy") + "</a>";
                }
                else
                {
                    dt.Rows[i][dcol4] = "N/A";
                }
                i = i + 1;

            }


            grdSnow.DataSource = dt;
            grdSnow.DataBind();
        }

        protected void GetExtensions()
        {
            HttpWebRequest oRequest;
            WebResponse oResponse;
            Stream oStream;
            XmlTextReader oReader;
            XPathDocument oXMLDocument;


            string sRequest;

            string fileCheck = Localization.GetString("StoreFile", LocalResourceFile);
            Dictionary<string, string> settings = PortalController.GetPortalSettingsDictionary(ModuleContext.PortalId);
            PortalSecurity ps = new PortalSecurity();
            string username = ps.DecryptString(settings["Store_Username"], Config.GetDecryptionkey());
            string password = ps.DecryptString(settings["Store_Password"], Config.GetDecryptionkey());
            fileCheck = fileCheck + "&username=" + username + "&password=" + password;

            try
            {

                sRequest = fileCheck;
                try
                {
                    //make remote request
                    oRequest = (HttpWebRequest)WebRequest.Create(sRequest);
                    oRequest.Timeout = 10000; //10 seconds
                    oResponse = oRequest.GetResponse();
                    oStream = oResponse.GetResponseStream();
                }
                catch (Exception oExc)
                {
                    throw oExc;
                }

                //load XML document
                oReader = new XmlTextReader(oStream);


                DataTable dt = new DataTable();
                //instance of a datarow  
                DataRow drow;
                //creating two datacolums Column1 and Column2   
                DataColumn dcol1 = new DataColumn("Package", typeof(string));
                DataColumn dcol2 = new DataColumn("Filename", typeof(string));
                DataColumn dcol3 = new DataColumn("Download", typeof(string));

                DataColumn dcol4 = new DataColumn("Deploy", typeof(string));
                //adding datacolumn to datatable  
                dt.Columns.Add(dcol1);
                dt.Columns.Add(dcol2);
                dt.Columns.Add(dcol3);
                dt.Columns.Add(dcol4);
                oReader.XmlResolver = null;
                oXMLDocument = new XPathDocument(oReader);
                XPathNavigator nav = oXMLDocument.CreateNavigator();
                var iterator = nav.Select("orders/order/orderdetails/orderdetail");
                int i = 0;
                while (iterator.MoveNext())
                {
                    //instance of a datarow  
                    drow = dt.NewRow();
                    //add rows to datatable  
                    dt.Rows.Add(drow);
                    var packageName = iterator.Current.GetAttribute("packagename", "").Replace("'", "''").Trim();
                    var fileName = iterator.Current.SelectSingleNode("files/file").GetAttribute("filename", "");
                    var fileId = iterator.Current.SelectSingleNode("files/file").GetAttribute("fileid", "");
                    var deploy = iterator.Current.SelectSingleNode("files/file").GetAttribute("deploy", "");
                    //add Column values  
                    dt.Rows[i][dcol1] = packageName.ToString();
                    dt.Rows[i][dcol2] = fileName.ToString();
                    dt.Rows[i][dcol3] = "<a class='dnnPrimaryAction' href='" + Localization.GetString("StoreFile", LocalResourceFile) + fileCheck +
                                        "&fileid=" + fileId.ToString() + "'>"+ LocalizeString("download") + "</a>";
                    if (deploy == "true")
                    {
                        PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                        dt.Rows[i][dcol4] = Globals.NavigateURL(_portalSettings.ActiveTab.TabID, Null.NullString, "deployid", fileId.ToString());
                    }
                    else
                    {
                        dt.Rows[i][dcol4] = "N/A";
                    }
                    i = i + 1;

                }


                grdSnow.DataSource = dt;
                grdSnow.DataBind();

            }
            catch (Exception oExc)
            {
                throw oExc;
            }
        }

    }
}