using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using PowerBear_Render_WPF_Ver.Render;
using System.Windows;

namespace PowerBear_Render_WPF_Ver.DAO {
    public class PbIO {
        public static Byte[] BitmapToBytes(BitmapSource bmp) {
            Int32 PixelHeight = bmp.PixelHeight; // 图像高度
            Int32 PixelWidth = bmp.PixelWidth;   // 图像宽度
            Int32 Stride = PixelWidth << 2;         // 扫描行跨距
            Byte[] Pixels = new Byte[PixelHeight * Stride];
            if (bmp.Format == PixelFormats.Bgr32 || bmp.Format == PixelFormats.Bgra32) {   // 拷贝像素数据
                bmp.CopyPixels(Pixels, Stride, 0);
            } else {   // 先进行像素格式转换，再拷贝像素数据
                new FormatConvertedBitmap(bmp, PixelFormats.Bgr32, null, 0).CopyPixels(Pixels, Stride, 0);
            }
            return Pixels;
        }
        public static Byte[] Bitmap_TO_BGR(BitmapSource bmp) {
            Int32 mheight = bmp.PixelHeight;
            Int32 mwidth = bmp.PixelWidth;
            byte[] inpt = BitmapToBytes(bmp);
            byte[] outp = new byte[mwidth * mheight * 3];
            // B G R
            for (int i = 0, ii = 0; i < mwidth * mheight * 3; i += 3, ii += 4) { outp[i] = inpt[ii]; outp[i + 1] = inpt[ii + 1]; outp[i + 2] = inpt[ii + 2]; }
            return outp;
        }
        public static Byte[] BGRA_TO_BGR(byte[] inpt, int mheight, int mwidth) {
            byte[] outp = new byte[mwidth * mheight * 3];
            // B G R
            for (int i = 0, ii = 0; i < mwidth * mheight * 3; i += 3, ii += 4) { outp[i] = inpt[ii]; outp[i + 1] = inpt[ii + 1]; outp[i + 2] = inpt[ii + 2]; }
            return outp;
        }
        /// <summary>
        /// 必须是BGRA格式的数组
        /// </summary>
        public static WriteableBitmap CreateWriteableBitMap_BGRA(Byte[] pixelColorBytes, int width, int height) {
            var b = RenderDispter.CreateWriteableBitMap(width, height);
            b.WritePixels(new Int32Rect(0, 0, (int)b.Width, (int)b.Height), pixelColorBytes, b.BackBufferStride, 0);
            return b;
        }
        /// <summary>
        /// 传入RGB数组
        /// </summary>
        public static WriteableBitmap CreateWriteableBitMap_RGB(Byte[] colorRGB, int width, int height) {
            byte[] toBytesBGRA = new byte[width * height * 4];
            for (int i1 = 0, i2 = 0; i1 < toBytesBGRA.Length && i2 < colorRGB.Length; i1 += 4, i2 += 3) {
                toBytesBGRA[i1] = colorRGB[i2 + 2];
                toBytesBGRA[i1 + 1] = colorRGB[i2 + 1];
                toBytesBGRA[i1 + 2] = colorRGB[i2];
                toBytesBGRA[i1 + 3] = 255;
            }
            return CreateWriteableBitMap_BGRA(toBytesBGRA, width, height);
        }
    }
}
