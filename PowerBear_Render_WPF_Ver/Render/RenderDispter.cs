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
using PowerBear_Render_WPF_Ver.CameraObj;
using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.Textures;
using PowerBear_Render_WPF_Ver.Lights;

namespace PowerBear_Render_WPF_Ver.Render {
    /// <summary>
    /// 多线程类 规定（1，1）图像左上角（height，width）图像右下角
    /// </summary>
    public class RenderDispter {
        //====Public Var====
        public int width, height;
        public Byte[]? pixelColorBytes; //BGRA32
        public Action? OnFinish, FreshImage;
        public BackgroundWorker _BackWorker; //后台线程管理类，用于刷新
        public int sampleDepth = 1; //采样深度次数
        public Camera mCamera;
        public int cpus = 1; // 使用cpu核心数
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
        Vector3d Ray_Color(Ray ray, HitTable world, int depth) { //投射光线
            HitResult hitResult;
            if (depth <= 0) return new Vector3d(0, 0, 0);
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                //return new Vector3d(1, 0, 0);
                Ray scattered;
                Vector3d attenuation, emitColor = hitResult.mat.Emit(hitResult.u, hitResult.v, hitResult.p);
                if (hitResult.mat.Scatter(ray, hitResult, out attenuation, out scattered)) {
                    return emitColor + attenuation * Ray_Color(scattered, world, depth - 1);
                } else {
                    return emitColor; // 返回自发光的颜色
                }
            }
            // 返回背景颜色 TODO:使用一个背景小球，采样
            //return GobVar._BackColor;
            Vector3d directNormal = ray.direction.Normalized();
            var t = 0.7 * (directNormal.y() + 1.0d);
            var res = (1.0d - t) * Vector3d.Vector3DUse(1, 1, 1) + t * GobVar._BackColor;
            return res;
        }
        public void doRender() {
            var imgTexture = new ImageTexture(@"C:\Users\PowerBear\source\repos\PowerBear_Render_WPF_Ver\PowerBear_Render_WPF_Ver\Resources\Earth.jpg", 1d);

            try {
                Camera camera = mCamera;
                if (camera == null) throw new Exception("摄像机为空，啊哈！") { };
                // 材质属性
                var material_ground = new Lambertian(new Vector3d(0.2d, 0.3d, 0.0d));
                var texture_Perlin = new NoiseTexture(6);
                var material_center = new Lambertian(texture_Perlin);
                //var material_center = new Lambertian(new Vector3d(0.7d, 0.3d, 0.3d));
                var material_left = new Dielectric(1.5d);
                var material_right = new Metal(new Vector3d(0.8d, 0.6d, 0.2d), 0.3d);
                var material_glass = new Dielectric(index_of_refraction: 1.5d);
                var material_checker = new Lambertian(new CheckerTexture(new Vector3d(0.2, 0.3, 0.1), new Vector3d(0.9, 0.9, 0.9)));
                var material_ImageMat = new Lambertian(imgTexture);
                var material_Light = new DiffuseLightMat(new Solid_Color(10d, 10 * 0.45d, 0.40d), 1);
                // 处理世界场景数据
                Hittable_List world = new Hittable_List();

                var zz = -1d;
                Vector3d p0 = new(-1, 0, 0), p1 = new(1, 1, zz), p2 = new(1, -1, zz);


                //world.Add(new Triangle(p0, p1, p2, new Lambertian(0.2, 0.2, 0)));

                ObjModel md = new ObjModel(@"C:\Users\PowerBear\Desktop\out.obj");
                world.Add(md);

                world.Add(new Sphere(new Vector3d(0, -100, -1), 100, material_checker));
                //world.Add(new Sphere(new Vector3d(0, 0, -1), 1, material_ImageMat));
                //world.Add(new DielectricSphere(new Vector3d(-1, 0, -1), 0.5, material_glass));
                //world.Add(new Sphere(new Vector3d(1, 0, -1), 0.5, material_right));
                //world.Add(new Sphere(new Vector3d(0, 2, 0), 1, material_Light));

                // world.objects[2].needDebug = true;


                // ======BVH Build======
                Console.WriteLine("构建整个场景的BVH盒子");
                BVH_Tree worldBvh = new(world);
                // ---End---


                int sample_pixel_count = 1; // MSAA_LVEL=0
                if (GobVar.MSAA_Level == 1) { sample_pixel_count = 20; } else if (GobVar.MSAA_Level == 2) { sample_pixel_count = 50; } else if (GobVar.MSAA_Level == 3) { sample_pixel_count = 100; }
                //多线程设定
                var options = new ParallelOptions {
                    MaxDegreeOfParallelism = cpus
                };
                int pixelsCount = 0;

                //Ray ray = new Ray(new Vector3d(x: 0, y: 0, z: 1), new Vector3d(x: -0.5563959061483481, y: 0.2571223505685548, z: -1));
                //Ray_Color(ray, worldBvh, 50);


                //多线程运行
                Parallel.For(1, this.height + 1, options, i => {
                    Parallel.For(fromInclusive: 1, this.width + 1, options, j => {
                        Vector3d colorRes = new Vector3d(0, 0, 0); // 每个线程单独一个颜色值
                        colorRes.e[0] = colorRes.e[1] = colorRes.e[2] = 0d;
                        for (int k = 0; k < sample_pixel_count; k++) { //MSAA重要性采样
                            double uRandom = PbRandom.Random_Double(), vRandom = PbRandom.Random_Double();
                            if (GobVar.MSAA_Level == 0) { uRandom = vRandom = 0; }
                            var u = (1.0d * j + uRandom) / width;
                            var v = (1.0d * i + vRandom) / height;
                            Ray ray = camera.GetRay(u, v);
                            colorRes += Vector3d.Clamp(Ray_Color(ray, worldBvh, 50));
                        }
                        // DONE
                        var writeColor = colorRes / sample_pixel_count;//Gamma矫正
                        writeColor.e[0] = Math.Sqrt(writeColor.e[0]);
                        writeColor.e[1] = Math.Sqrt(writeColor.e[1]);
                        writeColor.e[2] = Math.Sqrt(writeColor.e[2]);
                        this.SetColorToBytes(i, j, new PbColorRGB(writeColor).ConvertToSysColor());
                        pixelsCount++;
                        //监听刷新
                        if (GobVar.NeedFlush1) {
                            _BackWorker.ReportProgress((int)(pixelsCount * 1.0d / (this.height * this.width) * 100), new Tuple<int, int, byte[]>(i, j, this.pixelColorBytes));
                            GobVar.NeedFlush1 = false;
                        }
                        //用户取消渲染
                        if (_BackWorker.CancellationPending) { throw new Exception("渲染取消了"); }
                    });
                });

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
