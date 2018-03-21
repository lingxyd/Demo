using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperate
{
    class Program
    {
        static void Main(string[] args)
        {
            //string oriImg = @"D:\123.jpg";
            //string desImg = @"D:\123-006.jpg";
            string oriImg = @"D:\456.png";
            string desImg = @"D:\456-001.png";

            //var r = GetCompressImage(desImg, oriImg, 10);

            string msg = "";
            //var r = GetDigImage(desImg, oriImg, 2450, 350, 1000, 450, 95, out msg);
            var r = GetDigImage(desImg, oriImg, 490, 175, 125, 90, 95, out msg);

        }

        // 图片压缩
        public static bool GetCompressImage(string destPath, string srcPath, int quality)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(srcPath);
            ImageFormat tFormat = iSource.RawFormat;
            //以下代码为保存图片时，设置压缩质量 
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = quality;//设置压缩的比例1-100 
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                //ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                //ImageCodecInfo jpegICIinfo = null;
                //for (int x = 0; x < arrayICI.Length; x++)
                //{
                //    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                //    {
                //        jpegICIinfo = arrayICI[x];
                //        break;
                //    }
                //}

                ImageCodecInfo jpegICIinfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatDescription.Equals("JPEG"));
                //ImageCodecInfo jpegICIinfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == tFormat.Guid);

                if (jpegICIinfo != null)
                {
                    iSource.Save(destPath, jpegICIinfo, ep);
                }
                else
                {
                    iSource.Save(destPath, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                iSource.Dispose();
            }

        }

        // 图片裁剪
        public static bool GetDigImage(string destPath, string srcPath, int x, int y, int width, int height, int quality, out string error, string mimeType = "image/jpeg")
        {
            bool retVal = false;
            error = string.Empty;
            Image srcImage = null;
            Image destImage = null;
            Graphics graphics = null;
            try
            {
                //获取源图像
                srcImage = Image.FromFile(srcPath, false);
                //定义画布                                
                destImage = new Bitmap(width, height);
                //获取高清Graphics
                //graphics = GetGraphics(destImage);
                graphics = Graphics.FromImage(destImage);
                //设置质量
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                //InterpolationMode不能使用High或者HighQualityBicubic,如果是灰色或者部分浅色的图像是会在边缘处出一白色透明的线
                //用HighQualityBilinear却会使图片比其他两种模式模糊（需要肉眼仔细对比才可以看出）
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                //将源图像的某区域画到新画布上，注意最后一个参数GraphicsUnit.Pixel
                graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                //如果是覆盖则先释放源资源
                if (destPath == srcPath)
                {
                    srcImage.Dispose();
                }
                //保存到文件，同时进一步控制质量
                SaveImage2File(destPath, destImage, quality, mimeType);
                retVal = true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (srcImage != null)
                    srcImage.Dispose();
                if (destImage != null)
                    destImage.Dispose();
                if (graphics != null)
                    graphics.Dispose();
            }
            return retVal;
        }

        //// 文字水印
        //public static bool DrawWaterText(string destPath, string srcPath, string text, Font font, Brush brush, int pos, int padding, int quality, int opcity, out string error, string mimeType = "image/jpeg")
        //{
        //    bool retVal = false;
        //    error = string.Empty;
        //    Image srcImage = null;
        //    Image destImage = null;
        //    Graphics graphics = null;
        //    if (font == null)
        //    {
        //        font = new Font("微软雅黑", 20, FontStyle.Bold, GraphicsUnit.Pixel);//统一尺寸
        //    }
        //    if (brush == null)
        //    {
        //        brush = new SolidBrush(Color.White);
        //    }
        //    try
        //    {
        //        //获取源图像
        //        srcImage = Image.FromFile(srcPath, false);
        //        //定义画布，大小与源图像一样
        //        destImage = new Bitmap(srcImage);
        //        //获取高清Graphics
        //        graphics = GetGraphics(destImage);
        //        //将源图像画到画布上，注意最后一个参数GraphicsUnit.Pixel
        //        graphics.DrawImage(srcImage, new Rectangle(0, 0, destImage.Width, destImage.Height), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
        //        //如果水印字不为空，且不透明度大于0，则画水印
        //        if (!string.IsNullOrEmpty(text) && opcity > 0)
        //        {
        //            //获取可以用来绘制水印图片的有效区域
        //            Rectangle validRect = new Rectangle(padding, padding, srcImage.Width - padding * 2, srcImage.Height - padding * 2);
        //            //获取绘画水印文字的格式,即文字对齐方式
        //            StringFormat format = GetStringFormat(pos);
        //            //如果不透明度==100,那么直接将字画到当前画布上.
        //            if (opcity == 100)
        //            {
        //                graphics.DrawString(text, font, brush, validRect, format);
        //            }
        //            else
        //            {
        //                //如果不透明度在0到100之间,就要实现透明效果，文字没法透明，图片才能透明
        //                //则先将字画到一块临时画布，临时画布与destImage一样大，先将字写到这块画布上，再将临时画布画到主画布上，同时设置透明参数
        //                Bitmap transImg = null;
        //                Graphics gForTransImg = null;
        //                try
        //                {
        //                    //定义临时画布
        //                    transImg = new Bitmap(destImage);
        //                    //获取高清Graphics
        //                    gForTransImg = GetGraphics(transImg);
        //                    //绘制文字
        //                    gForTransImg.DrawString(text, font, brush, validRect, format);
        //                    //**获取带有透明度的ImageAttributes
        //                    ImageAttributes imageAtt = GetAlphaImgAttr(opcity);
        //                    //将这块临时画布画在主画布上
        //                    graphics.DrawImage(transImg, new Rectangle(0, 0, destImage.Width, destImage.Height), 0, 0, transImg.Width, transImg.Height, GraphicsUnit.Pixel, imageAtt);
        //                }
        //                catch (Exception ex)
        //                {
        //                    error = ex.Message;
        //                    return retVal;
        //                }
        //                finally
        //                {
        //                    if (transImg != null)
        //                        transImg.Dispose();
        //                    if (gForTransImg != null)
        //                        gForTransImg.Dispose();
        //                }
        //            }
        //        }
        //        //如果两个地址相同即覆盖，则提前Dispose源资源
        //        if (destPath.ToLower() == srcPath.ToLower())
        //        {
        //            srcImage.Dispose();
        //        }
        //        //保存到文件，同时进一步控制质量
        //        SaveImage2File(destPath, destImage, quality, mimeType);
        //        retVal = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex.Message;
        //    }
        //    finally
        //    {
        //        if (srcImage != null)
        //            srcImage.Dispose();
        //        if (destImage != null)
        //            destImage.Dispose();
        //        if (graphics != null)
        //            graphics.Dispose();
        //    }
        //    return retVal;
        //}


        private static void SaveImage2File(string path, Image destImage, int quality, string mimeType = "image/jpeg")
        {
            if (quality <= 0 || quality > 100) quality = 95;
            //创建保存的文件夹
            FileInfo fileInfo = new FileInfo(path);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
            //设置保存参数，保存参数里进一步控制质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qua = new long[] { quality };
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获取指定mimeType的mimeType的ImageCodecInfo
            var codecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(ici => ici.MimeType == mimeType);
            destImage.Save(path, codecInfo, encoderParams);
        }

    }
}
