using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
            //string desImg = @"D:\123-06.jpg";
            string oriImg = @"D:\456.png";
            string desImg = @"D:\456-05.png";

            var r = GetCompressImage(desImg, oriImg, 10);

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


    }
}
