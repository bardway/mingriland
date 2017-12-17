// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
#endregion

namespace TLAuto.BaseEx.Extensions
{
    public static class ImageExtensions
    {
        /// <summary>
        /// 缩小图片
        /// </summary>
        /// <param name="strOldPic">源图文件名(包括路径)</param>
        /// <param name="newPicSavePath">缩小后保存为文件名(包括路径)</param>
        /// <param name="intWidth">缩小至宽度</param>
        /// <param name="intHeight">缩小至高度</param>
        public static void SmallPic(string strOldPic, string newPicSavePath, int intWidth, int intHeight)
        {
            using (var oldImage = Image.FromFile(strOldPic))
            {
                using (var objNewPic = new Bitmap(oldImage, intWidth, intHeight))
                {
                    if (!File.Exists(newPicSavePath))
                    {
                        objNewPic.Save(newPicSavePath);
                    }
                }
            }
        }

        /// <summary>
        /// 按比例缩小图片，自动计算高度
        /// </summary>
        /// <param name="strOldPic">源图文件名(包括路径)</param>
        /// <param name="newPicSavePath">缩小后保存为文件名(包括路径)</param>
        /// <param name="intWidth">缩小至宽度</param>
        public static void SmallPic(string strOldPic, string newPicSavePath, int intWidth)
        {
            Bitmap objPic, objNewPic;
            try
            {
                objPic = new Bitmap(strOldPic);
                var intHeight = (intWidth / objPic.Width) * objPic.Height;
                objNewPic = new Bitmap(objPic, intWidth, intHeight);
                objNewPic.Save(newPicSavePath);
            }
            finally
            {
                objPic = null;
                objNewPic = null;
            }
        }

        /// <SUMMARY>
        /// 图片缩放(无损)
        /// </SUMMARY>
        /// <PARAM name="sourceFile">图片源路径</PARAM>
        /// <PARAM name="destFile">缩放后图片输出路径</PARAM>
        /// <PARAM name="destHeight">缩放后图片高度</PARAM>
        /// <PARAM name="destWidth">缩放后图片宽度</PARAM>
        /// <RETURNS></RETURNS>
        public static bool GetThumbnail(string sourceFile, string destFile, int destHeight, int destWidth)
        {
            var imgSource = Image.FromFile(sourceFile);
            var thisFormat = imgSource.RawFormat;
            int sW = 0, sH = 0;
            // 按比例缩放
            var sWidth = imgSource.Width;
            var sHeight = imgSource.Height;

            if ((sHeight > destHeight) || (sWidth > destWidth))
            {
                if (sWidth * destHeight > sHeight * destWidth)
                {
                    sW = destWidth;
                    sH = (destWidth * sHeight) / sWidth;
                }
                else
                {
                    sH = destHeight;
                    sW = (sWidth * destHeight) / sHeight;
                }
            }
            else
            {
                sW = sWidth;
                sH = sHeight;
            }

            var outBmp = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(outBmp);
            g.Clear(Color.WhiteSmoke);

            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgSource,
                        new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH),
                        0,
                        0,
                        imgSource.Width,
                        imgSource.Height,
                        GraphicsUnit.Pixel);
            g.Dispose();

            // 以下代码为保存图片时，设置压缩质量
            var encoderParams = new EncoderParameters();
            var quality = new long[1];
            quality[0] = 100;

            var encoderParam = new EncoderParameter(Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;

            try
            {
                //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。
                var arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICI = null;
                for (var x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICI = arrayICI[x]; //设置JPEG编码
                        break;
                    }
                }

                if (jpegICI != null)
                {
                    outBmp.Save(destFile, jpegICI, encoderParams);
                }
                else
                {
                    outBmp.Save(destFile, thisFormat);
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                imgSource.Dispose();
                outBmp.Dispose();
            }
        }

        /// <summary>
        /// 剪裁 -- 用GDI+
        /// </summary>
        /// <param name="b">原始Bitmap</param>
        /// <param name="StartX">开始坐标X</param>
        /// <param name="StartY">开始坐标Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <returns>剪裁后的Bitmap</returns>
        public static Bitmap KiCut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }

            var w = b.Width;
            var h = b.Height;

            if ((StartX >= w) || (StartY >= h))
            {
                return null;
            }

            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }

            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }

            try
            {
                var bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

                var g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();

                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取已缓存的图片
        /// </summary>
        /// <param name="imageSize">图片字节</param>
        /// <returns></returns>
        public static BitmapImage GetStreamBitmapImage(this byte[] imageSize)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(imageSize);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public static byte[] GetResizeImage(this byte[] imageSize)
        {
            Stream streamPhoto = new MemoryStream(imageSize);
            var bfPhoto = ReadBitmapFrame(streamPhoto);
            var nThumbnailSize = 200;
            int nWidth, nHeight;
            if (bfPhoto.Width > bfPhoto.Height)
            {
                nWidth = nThumbnailSize;
                nHeight = (int)((bfPhoto.Height * nThumbnailSize) / bfPhoto.Width);
            }
            else
            {
                nHeight = nThumbnailSize;
                nWidth = (int)((bfPhoto.Width * nThumbnailSize) / bfPhoto.Height);
            }
            var bfResize = FastResize(bfPhoto, nWidth, nHeight);
            var baResize = ToByteArray(bfResize);
            return baResize;
        }

        private static BitmapFrame FastResize(BitmapFrame bfPhoto, int nWidth, int nHeight)
        {
            var tbBitmap = new TransformedBitmap(bfPhoto, new ScaleTransform(nWidth / bfPhoto.Width, nHeight / bfPhoto.Height, 0, 0));
            return BitmapFrame.Create(tbBitmap);
        }

        private static byte[] ToByteArray(BitmapFrame bfResize)
        {
            var msStream = new MemoryStream();
            var pbdDecoder = new PngBitmapEncoder();
            pbdDecoder.Frames.Add(bfResize);
            pbdDecoder.Save(msStream);
            return msStream.ToArray();
        }

        private static BitmapFrame ReadBitmapFrame(Stream streamPhoto)
        {
            var bdDecoder = BitmapDecoder.Create(streamPhoto, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bdDecoder.Frames[0];
        }

        public static BitmapImage GetStreamBitmapImage(this byte[] imageSize, int decodePixelWidth)
        {
            var bitmap = new BitmapImage();
            bitmap.DecodePixelWidth = decodePixelWidth;
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(imageSize);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public static void SaveImage(this BitmapImage bitmapImage, string path)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            encoder.Save(fileStream);
            fileStream.Close();
        }

        public static void Resize(this BitmapImage bitmapImage, int width, int height, string savePath)
        {
            var newImage = new Bitmap(width, height);
            using (var gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(ToBitmap(bitmapImage), new Rectangle(0, 0, width, height));
            }

            newImage.Save(savePath);
        }

        public static void Resize(this BitmapImage bitmapImage, double width, string savePath)
        {
            var height = (width / bitmapImage.Width) * bitmapImage.Height;
            Resize(bitmapImage, (int)width, (int)height, savePath);
        }

        /// <summary>
        /// Creates a new ImageSource with the specified width/height
        /// </summary>
        /// <param name="source">Source image to resize</param>
        /// <param name="width">Width of resized image</param>
        /// <param name="height">Height of resized image</param>
        /// <returns>Resized image</returns>
        public static BitmapSource CreateResizedImage(ImageSource source, int width, int height)
        {
            // Target Rect for the resize operation
            var rect = new Rect(0, 0, width, height);

            // Create a DrawingVisual/Context to render with
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(source, rect);
            }

            // Use RenderTargetBitmap to resize the original image
            var resizedImage = new RenderTargetBitmap(
                                                      (int)rect.Width,
                                                      (int)rect.Height, // Resized dimensions
                                                      96,
                                                      96, // Default DPI values
                                                      PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            // Return the resized image
            return resizedImage;
        }

        public static Bitmap ToBitmap(this BitmapImage bitmapImage)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                var bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                var bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }
    }
}