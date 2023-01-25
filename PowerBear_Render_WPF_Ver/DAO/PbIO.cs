using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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
    }
}
