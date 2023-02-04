using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.Lights;
using PowerBear_Render_WPF_Ver.Textures;

namespace PowerBear_Render_WPF_Ver {
    public static class GobVar {
        public static int renderWidth, renderHeight;
        public static bool NeedFlush1 { get; set; } = false;
        public static bool AllowPreview { get; set; } = true; //允许运行过程中预览结果
        public static WriteableBitmap? wBitmap1;
        public static int MSAA_Level = 0;//0: 关闭 1：4x倍
        //======Render Options======
        public static Vector3d _BackColor = new Vector3d();
        public static Hittable_List fnWorld = new Hittable_List();

        //======Deault Objects======
        public static Lambertian DeaultMat = new Lambertian(new Vector3d(0.5, y: 0.5, 0.5));
        public static Hittable_List worldObjects = new(); // 世界场景数组，渲染器渲染这个

        /// <summary>
        /// 封装好的，Cornell_Box，是1984年创建的一个展示光照效果的演示程序
        /// 视角选择 位置：277.5 277.5 -40 视觉方向：0 0 1
        /// </summary>
        public static Hittable_List Cornell_Box() {
            Hittable_List objects = new();

            var red = new Lambertian(new Vector3d(.65, .05, .05));
            var white = new Lambertian(new Vector3d(.73, .73, .73));
            var green = new Lambertian(new Vector3d(.12, .45, .15));
            var light = new DiffuseLightMat(new Solid_Color(1, 1, 1), 20);

            objects.Add(new YZ_Rect(0, 555, 0, 555, 555, green));
            objects.Add(new YZ_Rect(0, 555, 0, 555, 0, red));
            objects.Add(new XZ_Rect(113, 443, 127, 432, 554, light));
            objects.Add(new XZ_Rect(0, 555, 0, 555, 0, white));
            objects.Add(new XZ_Rect(0, 555, 0, 555, 555, white));
            objects.Add(new XY_Rect(0, 555, 0, 555, 555, white));

            return objects;
        }


        //======Gobal Functions======
        /// <summary>
        /// 将数组中的数据写入到Bitmap贴图里面
        /// </summary>
        public static WriteableBitmap BitmapWritePixels(WriteableBitmap? bt, Byte[] pixelColorBytes) {
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
        /// <summary>
        /// 将数组数据写入Bitmap贴图
        /// </summary>
        public static void BitmapWritePixels(ref WriteableBitmap bt, Byte[] pixelColorBytes) {
            bt.WritePixels(new Int32Rect(0, 0, (int)bt.Width, (int)bt.Height), pixelColorBytes, bt.BackBufferStride, 0);
        }
    }
}
