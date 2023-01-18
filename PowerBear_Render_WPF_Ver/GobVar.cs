using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PowerBear_Render_WPF_Ver {
    public static class GobVar {
        public static int renderWidth, renderHeight;
        public static bool NeedFlush1 { get; set; } = false;
        public static bool AllowPreview { get; set; } = true; //允许运行过程中预览结果
        public static WriteableBitmap? wBitmap1;
        public static int MSAA_Level = 0;//0: 关闭 1：4x倍
        //======Render Options======
        public static Vector3d _BackColor = new Vector3d();
        public static void BitmapWrPixels(ref WriteableBitmap bt, Byte[] pixelColorBytes) {
            bt.WritePixels(new Int32Rect(0, 0, (int)bt.Width, (int)bt.Height), pixelColorBytes, bt.BackBufferStride, 0);
        }
        //======Deault Objects======
        public static Lambertian DeaultMat = new Lambertian(new Vector3d(0.5, y: 0.5, 0.5));
        
        
        public static WriteableBitmap BitmapWrPixels(WriteableBitmap? bt, Byte[] pixelColorBytes) {
            if (bt == null) {
                bt = RenderDispter.CreateWriteableBitMap(GobVar.renderWidth, GobVar.renderHeight);
            }
            WriteableBitmap wb = bt;
            bt.Dispatcher.BeginInvoke(new Action(() => {
                bt.WritePixels(new Int32Rect(0, 0, (int)bt.Width, (int)bt.Height), pixelColorBytes, bt.BackBufferStride, 0);
                wb = bt;
            }));
            return wb;
        }
    }
}
