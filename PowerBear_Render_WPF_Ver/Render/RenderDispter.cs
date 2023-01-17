using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using syDrawing = System.Drawing;
using Point3d = PowerBear_Render_WPF_Ver.PbMath.Vector3d;
namespace PowerBear_Render_WPF_Ver.Render {
    /// <summary>
    /// 多线程类 规定（1，1）图像左上角（height，width）图像右下角
    /// </summary>
    public class RenderDispter {
        //Camera设定
        CameraObj cameraObj;
        //图像设定
        public int width, height;
        public Byte[]? pixelColorBytes; //BGRA32
        public Action? OnFinish, FreshImage;
        public BackgroundWorker _BackWorker; //后台线程管理类，用于刷新
        public int sampleDepth = 1; //采样深度次数

        //方法组
        public RenderDispter() { }
        public RenderDispter(int width, int height) {
            this.Init(width, height);
        }
        ~RenderDispter() {
            GobVar.wBitmap1 = null;
            pixelColorBytes = null;
        }
        //  ------渲染-----
        double Hit_Sphere(Point3d center, double radius, Ray r) {
            Vector3d oc = r.origin - center;
            var a = Vector3d.Dot(r.direction, r.direction);
            var b = 2.0 * Vector3d.Dot(oc, r.direction);
            var c = Vector3d.Dot(oc, oc) - radius * radius;
            var discriminant = b * b - 4 * a * c;
            if (discriminant < 0) { return -1.0d; } else {
                return (-b - Math.Sqrt(discriminant)) / (2.0 * a); //2a分之-b+-开跟 b平方 - 4ac
            }
        }
        Vector3d Ray_Color(Ray ray, HitTable world, int depth) {
            HitResult hitResult;
            if (depth <= 0) return new Vector3d(0, 0, 0);
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                Point3d colorCount = new();
                var nextVec = Vector3d.Random_Unit_Vector().Normalized();
                Point3d target = hitResult.p + hitResult.normal + nextVec;
                colorCount += 0.5d * Ray_Color(new Ray(hitResult.p, target - hitResult.p), world, depth - 1);
                return colorCount / 1;
            }
            Vector3d directNormal = ray.direction.Normalized();
            var t = 0.7 * (directNormal.y() + 1.0d);
            //Vector3d.Vector3DUse(0.5, 0.7, 1.0)
            var res = (1.0d - t) * Vector3d.Vector3DUse(1, 1, 1) + t * GobVar._BackColor;
            return res;
        }
        public void doRender() {
            try {
                var aspect_radio = 1.0 * width / height;
                var viewport_height = 2.0;
                var viewport_width = aspect_radio * viewport_height;
                //画幅设定项目
                Vector3d origin = new(0d, 0d, 0d);
                Vector3d upper_left_corner = new(-viewport_width, viewport_height, -1.0); //画面左上角
                Vector3d horizontal = new(2.0 * viewport_width, 0d, 0d);
                Vector3d vertical = new(0d, -2.0 * viewport_height, 0d);
                //World
                Hittable_List world = new Hittable_List();
                world.Add(new Sphere(new Vector3d(0, 0, -1), 0.5));
                world.Add(new Sphere(new Vector3d(0, -100.5, -1), 100));
                int sample_pixel_count = 1;
                if (GobVar.MSAA_Level == 1) { sample_pixel_count = 4; } else if (GobVar.MSAA_Level == 2) { sample_pixel_count = 100; }
                for (int i = 1; i <= this.height; i++) {
                    for (int j = 1; j <= this.width; j++) {
                        Vector3d colorRes = new Vector3d(0, 0, 0);
                        for (int k = 0; k < sample_pixel_count; k++) { //MSAA重要性采样
                            double uRandom = PbRandom.Random_Double(), vRandom = PbRandom.Random_Double();
                            if (GobVar.MSAA_Level == 0) { uRandom = vRandom = 0; }
                            var u = (1.0d * j + uRandom) / width;
                            var v = (1.0d * i + vRandom) / height;
                            Ray ray = new Ray(origin, upper_left_corner + u * horizontal + v * vertical);
                            colorRes += Vector3d.Clamp(Ray_Color(ray, world, 50));
                        }

                        var writeColor = colorRes / sample_pixel_count;//Gamma矫正
                        writeColor.e[0] = Math.Sqrt(writeColor.e[0]);
                        writeColor.e[1] = Math.Sqrt(writeColor.e[1]);
                        writeColor.e[2] = Math.Sqrt(writeColor.e[2]);
                        this.SetColorToBytes(i, j, new PbColorRGB(writeColor).ConvertToSysColor());
                        //监听刷新
                        if (GobVar.NeedFlush1) {
                            _BackWorker.ReportProgress((int)((1.0f * (i - 1) * (width) + j) / (this.height * this.width) * 100), new Tuple<int, int, byte[]>(i, j, this.pixelColorBytes));
                            GobVar.NeedFlush1 = false;
                        }

                    }
                }
                _BackWorker.ReportProgress(100);
            }
            catch (Exception ex) {
                MessageBox.Show("渲染过程中出现错误\n" + ex.Message);
            }
        }
        //---- End Of Render ----
        public static WriteableBitmap CreateWriteableBitMap(int width, int height) {
            return new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }
        /// <summary>
        /// 必须是BGRA格式的数组
        /// </summary>
        /// <param name="pixelColorBytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static WriteableBitmap CreateWriteableBitMap(Byte[] pixelColorBytes, int width, int height) {
            var b = RenderDispter.CreateWriteableBitMap(width, height);
            b.WritePixels(new Int32Rect(0, 0, (int)b.Width, (int)b.Height), pixelColorBytes, b.BackBufferStride, 0);
            return b;
        }

        void Init(int width, int height) {
            this.pixelColorBytes = new Byte[width * height * 4];
            this.width = width;
            this.height = height;
        }
        /// <summary>
        /// 设置图像画面的颜色，操作PixelColorBytes数组，从1开始，到最终像素结束
        /// </summary>
        /// <param name="x">画面高度</param>
        /// <param name="y">画面宽度</param>
        /// <param name="color">系统库自带的颜色</param>
        void SetColorToBytes(int x, int y, System.Drawing.Color color) {
            x--; y--;
            int point = x * width * 4 + y * 4;
            this.pixelColorBytes[point] = color.B;
            this.pixelColorBytes[point + 1] = color.G;
            this.pixelColorBytes[point + 2] = color.R;
            this.pixelColorBytes[point + 3] = 255;
        }
    }
}
