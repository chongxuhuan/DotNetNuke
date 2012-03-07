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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem.Internal;

namespace DotNetNuke.Common.Utilities
{
    class ImageUtils
    {
        static int imgHeight;
        static int imgWidth;
        public static Size GetSize(string sPath)
        {
            System.Drawing.Image g = System.Drawing.Image.FromFile(sPath);
            Size s = g.Size;
            g.Dispose();
            return s;
        }
        public static int GetHeight(string sPath)
        {
            System.Drawing.Image g = System.Drawing.Image.FromFile(sPath);
            int h = g.Height;
            g.Dispose();
            return h;
        }
        public static int GetWidth(string sPath)
        {
            System.Drawing.Image g = System.Drawing.Image.FromFile(sPath);
            int w = g.Width;
            g.Dispose();
            return w;
        }
        public static int GetHeightFromStream(Stream sFile)
        {
            System.Drawing.Image g = System.Drawing.Image.FromStream(sFile, true);
            return g.Height;
        }
        public static int GetWidthFromStream(Stream sFile)
        {
            System.Drawing.Image g = System.Drawing.Image.FromStream(sFile, true);
            int w = g.Width;
            g.Dispose();
            return w;
        }
        public static string CreateImage(string sFile)
        {
            System.Drawing.Image g = System.Drawing.Image.FromFile(sFile);
            int h = g.Height;
            int w = g.Width;
            g.Dispose();
            return CreateImage(sFile, h, w);
        }
        public static string CreateImage(string sFile, int intHeight, int intWidth)
        {
            string tmp = sFile;
            string orig = sFile;
            FileInfo fi = new FileInfo(sFile);
            tmp = fi.FullName.Replace(fi.Extension, "_TEMP" + fi.Extension);
            if (FileWrapper.Instance.Exists(tmp))
            {
                FileWrapper.Instance.SetAttributes(tmp, FileAttributes.Normal);
                FileWrapper.Instance.Delete(tmp);
            }

            File.Copy(sFile, tmp);
            System.Drawing.Bitmap original = new System.Drawing.Bitmap(tmp);

            System.Drawing.Imaging.PixelFormat format = original.PixelFormat;
            if (format.ToString().Contains("Indexed"))
            {
                format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            }

            int newHeight = 0;
            int newWidth = 0;
            newHeight = intHeight;
            newWidth = intWidth;
            Size imgSize = new Size();
            if (original.Width > newWidth || original.Height > newHeight)
            {
                imgSize = NewImageSize(original.Width, original.Height, newWidth, newHeight);
                imgHeight = imgSize.Height;
                imgWidth = imgSize.Width;
            }
            else
            {
                imgSize = new Size(original.Width, original.Height);
                imgHeight = original.Height;
                imgWidth = original.Width;
            }

            string sFileExt = fi.Extension;
            string sFileNoExtension = Path.GetFileNameWithoutExtension(sFile);
            string sPath = Path.GetDirectoryName(sFile);
            sPath = sPath.Replace("/", "\\");
            if (!sPath.EndsWith("\\"))
            {
                sPath += "\\";
            }
            Image img = Image.FromFile(tmp);
            Bitmap newImg = new Bitmap(imgWidth, imgHeight, format);
            newImg.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            Graphics canvas = Graphics.FromImage(newImg);
            canvas.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            canvas.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            if (sFileExt.ToLowerInvariant() != ".png")
            {
                canvas.Clear(Color.White);
                canvas.FillRectangle(Brushes.White, 0, 0, imgSize.Width, imgSize.Height);
            }
            canvas.DrawImage(img, 0, 0, imgSize.Width, imgSize.Height);
            img.Dispose();
            sFile = sPath;

            sFile += sFileNoExtension + sFileExt;
            if (FileWrapper.Instance.Exists(sFile))
            {
                FileWrapper.Instance.SetAttributes(sFile, FileAttributes.Normal);
                FileWrapper.Instance.Delete(sFile);
            }

            //newImg.Save
            var arrData = new byte[2048];
            Stream content = new MemoryStream();
            System.Drawing.Imaging.ImageFormat imgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
            if (sFileExt.ToLowerInvariant() == ".png")
            {
                imgFormat = System.Drawing.Imaging.ImageFormat.Png;
            }
            else if (sFileExt.ToLowerInvariant() == ".gif")
            {
                imgFormat = System.Drawing.Imaging.ImageFormat.Gif;
            }
            else if (sFileExt.ToLowerInvariant() == ".jpg")
            {
                imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            newImg.Save(content, imgFormat);
            using (var outStream = FileWrapper.Instance.Create(sFile))
            {
                var originalPosition = content.Position;
                content.Position = 0;

                try
                {
                    var intLength = content.Read(arrData, 0, arrData.Length);

                    while (intLength > 0)
                    {
                        outStream.Write(arrData, 0, intLength);
                        intLength = content.Read(arrData, 0, arrData.Length);
                    }
                }
                finally
                {
                    content.Position = originalPosition;
                }
            }

            newImg.Dispose();
            original.Dispose();

            canvas.Dispose();
            if (FileWrapper.Instance.Exists(tmp))
            {
                FileWrapper.Instance.SetAttributes(tmp, FileAttributes.Normal);
                FileWrapper.Instance.Delete(tmp);
            }



            return sFile;
        }
        public static string CreateJPG(string sFile, Bitmap img, int CompressionLevel)
        {
            Graphics bmpOutput = Graphics.FromImage(img);
            bmpOutput.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            bmpOutput.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Rectangle compressionRectange = new Rectangle(0, 0, imgWidth, imgHeight);
            bmpOutput.DrawImage(img, compressionRectange);
            System.Drawing.Imaging.ImageCodecInfo myImageCodecInfo = null;
            System.Drawing.Imaging.Encoder myEncoder = null;
            System.Drawing.Imaging.EncoderParameter myEncoderParameter = null;
            System.Drawing.Imaging.EncoderParameters myEncoderParameters = null;

            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
            myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, CompressionLevel);
            myEncoderParameters.Param[0] = myEncoderParameter;
            if (System.IO.File.Exists(sFile))
            {
                System.IO.File.Delete(sFile);
            }
            try
            {
                img.Save(sFile, myImageCodecInfo, myEncoderParameters);

            }
            catch
            {
            }


            img.Dispose();
            bmpOutput.Dispose();
            return sFile;
        }


        public static MemoryStream CreateImageForDB(Stream sFile, int intHeight, int intWidth)
        {
            MemoryStream newStream = new MemoryStream();
            System.Drawing.Image g = System.Drawing.Image.FromStream(sFile);
            //Dim thisFormat = g.RawFormat
            if (intHeight > 0 & intWidth > 0)
            {
                int newHeight = 0;
                int newWidth = 0;
                newHeight = intHeight;
                newWidth = intWidth;
                Size imgSize = new Size();
                if (g.Width > newWidth | g.Height > newHeight)
                {
                    imgSize = NewImageSize(g.Width, g.Height, newWidth, newHeight);
                    imgHeight = imgSize.Height;
                    imgWidth = imgSize.Width;
                }
                else
                {
                    imgHeight = g.Height;
                    imgWidth = g.Width;
                }
            }
            else
            {
                imgWidth = g.Width;
                imgHeight = g.Height;
            }

            Bitmap imgOutput1 = new Bitmap(g, imgWidth, imgHeight);
            Graphics bmpOutput = Graphics.FromImage(imgOutput1);
            bmpOutput.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            bmpOutput.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Rectangle compressionRectange = new Rectangle(0, 0, imgWidth, imgHeight);
            bmpOutput.DrawImage(g, compressionRectange);
            System.Drawing.Imaging.ImageCodecInfo myImageCodecInfo = null;
            System.Drawing.Imaging.Encoder myEncoder = null;
            System.Drawing.Imaging.EncoderParameter myEncoderParameter = null;
            System.Drawing.Imaging.EncoderParameters myEncoderParameters = null;
            myImageCodecInfo = (System.Drawing.Imaging.ImageCodecInfo)GetEncoderInfo("image/jpeg");
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
            myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, 90);
            myEncoderParameters.Param[0] = myEncoderParameter;
            imgOutput1.Save(newStream, myImageCodecInfo, myEncoderParameters);
            g.Dispose();
            imgOutput1.Dispose();
            bmpOutput.Dispose();
            return newStream;


        }
        public static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(string MYmimeType)
        {
            try
            {
                int i = 0;
                System.Drawing.Imaging.ImageCodecInfo[] encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                for (i = 0; i <= (encoders.Length - 1); i++)
                {
                    if ((encoders[i].MimeType == MYmimeType))
                    {
                        return encoders[i];
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static Size NewImageSize(int currentWidth, int currentHeight, int newWidth, int newHeight)
        {
            decimal decScale = default(decimal);
            if ((currentWidth / newWidth) > (currentHeight / newHeight))
            {
                decScale = Convert.ToDecimal(currentWidth / newWidth);
            }
            else
            {
                decScale = Convert.ToDecimal(currentHeight / newHeight);
            }
            newWidth = Convert.ToInt32(Math.Floor(currentWidth / decScale));
            newHeight = Convert.ToInt32(Math.Floor(currentHeight / decScale));

            Size NewSize = new Size(newWidth, newHeight);

            return NewSize;

        }
    }
}
