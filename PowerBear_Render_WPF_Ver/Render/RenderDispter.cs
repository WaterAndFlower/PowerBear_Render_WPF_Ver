﻿using PowerBear_Render_WPF_Ver.GameObjects;
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
using static System.Windows.Forms.Design.AxImporter;
using System.Reflection.Emit;
using PowerBear_Render_WPF_Ver.DAO;

namespace PowerBear_Render_WPF_Ver.Render {
    /// <summary>
    /// 多线程类 规定（1，1）图像左上角（height，width）图像右下角
    /// 【注】渲染管线的设置和调用在MainWindow.cs里，搜索RayColor即可
    /// Button_Click_StartRender方法里
    /// </summary>
    public class RenderDispter : BaseRenderDispter {
        //====Public Var====
        public int width, height;
        public BackgroundWorker _BackWorker; //后台线程管理类，用于刷新
        public int sampleDepth = 1; //采样深度次数
        public Camera mCamera;
        public int cpus = 1; // 使用cpu核心数
        public int startRow = 1, endRow = 500;

        BVH_Tree worldBvh;
        int sample_pixel_count = 1; // MSAA_LVEL=0
        public Byte[]? pixelColorBytes; //BGRA32 最后图片模样的数组
        public Byte[]? pixelRedPreview; //BGRA32 像素化着色器，红色背景图，预览用

        //方法组
        public RenderDispter(ToRenderDispterData parm) : base(parm) {
            _BackWorker = parm._BackWorker;
            width = parm.width;
            height = parm.height;
            sampleDepth = parm.sample_depth;
            mCamera = parm.mCamera;
            cpus = parm.cpus;
            startRow = parm.startRow; endRow = parm.endRow;

            switch (parm.sample_pixel_level) {
                case 0: {
                        sample_pixel_count = 1;
                        break;
                    }
                case 1: {
                        sample_pixel_count = 20;
                        break;
                    }
                case 2: {
                        sample_pixel_count = 50;
                        break;
                    }
                case 3: {
                        sample_pixel_count = 100;
                        break;
                    }
            }
            Init(width, height);
        }
        void Init(int width, int height) {
            if (this.pixelColorBytes == null) { this.pixelColorBytes = new Byte[width * height * 4]; }
            if (this.pixelRedPreview == null) { this.pixelRedPreview = new Byte[width * height * 4]; }
            this.width = width;
            this.height = height;
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
        Vector3d Ray_Color(Ray ray, HitAble world, int depth) { //投射光线
            HitResult hitResult;
            if (depth <= 0) return new Vector3d(x: 0, 0, 0);
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
            ray.origin = Vector3d.Zero;
            if (GobVar.skyObject.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                Ray scattered;
                Vector3d atten;
                if (hitResult.mat.Scatter(ray, hitResult, out atten, out scattered)) {
                    if (depth == GobVar.Render_Depth) {
                        return atten * 1d;
                    } else {
                        return atten * GobVar.MulSkyColor;
                    }
                }
            }
            return GobVar._BackColor;


            Vector3d directNormal = ray.direction.Normalized();
            var t = 0.7 * (directNormal.y() + 1.0d);
            var res = (1.0d - t) * Vector3d.Vector3DUse(1, 1, 1) + t * GobVar._BackColor;
            return res;
        }
        // ====== Phong 渲染部分 ======
        static Vector3d LightPos = new(10, 10, 10); // 一个平行光源的位置
        static Vector3d LightColor = new(1, 1, 1);
        static Vector3d HighColor = new(1, 1, 1);

        Vector3d Reflect(Vector3d v, Vector3d n) {
            return v - 2 * Vector3d.Dot(v, n) * n;
        }
        /// <summary>
        /// 采用裴详凤算法，一个Phong模型的光照计算
        /// </summary>
        Vector3d Ray_Color_Phong(Ray ray, HitAble world, int depth) {
            HitResult hitResult;
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                // 在这里实现算法部分
                Ray scattered;
                Vector3d attenuation, emitColor = hitResult.mat.Emit(hitResult.u, hitResult.v, hitResult.p);
                if (hitResult.mat.Scatter(ray, hitResult, out attenuation, out scattered)) {
                    //Console.WriteLine(hitResult.mat.GetType());


                    Vector3d ambient = GobVar._BackColor * 0.5d;
                    Vector3d worldNormal = hitResult.normal.Normalized();
                    Vector3d worldLightDir = LightPos;
                    Vector3d diffuse = attenuation * LightColor * PbMath.PbMath.ClampRangeDouble(worldNormal.Dot(worldLightDir), 0, 1);
                    //开始计算高光
                    Vector3d reflectDir = Reflect(worldLightDir, worldNormal).Normalized();
                    //mCamera.origin - hitResult.p
                    Vector3d viewDir = (ray.direction).Normalized();
                    Vector3d specular = HighColor * LightColor * Math.Pow(PbMath.PbMath.ClampRangeDouble(reflectDir.Dot(viewDir), 0, 1d), 18);

                    return GobVar.MulSkyColor * ambient + diffuse + specular;
                    //return emitColor + attenuation * Ray_Color(scattered, world, depth - 1);
                } else {
                    return emitColor; // 返回自发光的颜色
                }
            }
            // 在这里实现光照盒反射部分
            ray.origin = Vector3d.Zero;
            if (GobVar.skyObject.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                Ray scattered;
                Vector3d atten;
                if (hitResult.mat.Scatter(ray, hitResult, out atten, out scattered)) {
                    return atten * 1;
                }
            }
            return GobVar._BackColor;
        }

        // 像素化着色器，用于先看看图像的大体位置
        Vector3d Ray_Color_Preview(Ray ray, HitAble world) { //投射光线
            HitResult hitResult;
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                if (hitResult.hitObj != null) {
                    return new Vector3d(1, hitResult.hitObj._GUID * 0.4d, hitResult.hitObj._GUID * 0.8d); // RGB
                }
                return new Vector3d(1, 0, 0); // RGB
            } else {
                return new Vector3d(0, 0, 0);
            }
        }

        public void PreviewRender() {
            Camera camera = mCamera;
            Parallel.For(1, this.height + 1, i => {
                Parallel.For(1, this.width + 1, j => {
                    var u = (1.0d * j) / width;
                    var v = (1.0d * i) / height;
                    Ray ray = camera.GetRay(u, v);
                    var colorRes = Ray_Color_Preview(ray, worldBvh);
                    this.SetColorToBytes(i, j, new PbColorRGB(colorRes).ConvertToSysColor(), this.pixelRedPreview);
                    this.SetColorToBytes(i, j, new PbColorRGB(colorRes).ConvertToSysColor(), this.pixelColorBytes);
                });
            });
            if (GobVar.NeedFlush1) {
                _BackWorker.ReportProgress((int)(0 * 1.0d / (this.height * this.width) * 100), new Tuple<int, int, byte[]>(0, 0, this.pixelColorBytes));
                GobVar.NeedFlush1 = false;
            }
        }
        //CUDA：https://blog.csdn.net/theadore2017/article/details/110919384
        //利用GPU进行并行运算
        public override void DoRender(IRayColor RayColor) {

            worldBvh = new(GobVar.fnWorld);

            PreviewRender();

            if (GobVar.stopAtRenderColor == true) return; // 当只需要渲染像素时候，其他不渲染

            if (RayColor == null) return;
            // 做RayColor的前期处理
            RayColor.BeforeRender();

            try {
                Camera camera = mCamera;
                if (camera == null) throw new Exception("摄像机为空，啊哈！") { };

                // ======BVH Build======
                Console.WriteLine("构建整个场景的BVH盒子");
                // ---End---

                //多线程设定
                var options = new ParallelOptions {
                    MaxDegreeOfParallelism = cpus
                };
                int pixelsCount = 0;

                // =====Debug=====
                //Ray ray = new Ray(new Vector3d(x: 0, y: 1, z: 5), new Vector3d(x: 0d, y: 0, z: -1));
                //Ray_Color(ray, worldBvh, 50);

                var start = DateTime.Now;
                //多线程运行
                Parallel.For(startRow, endRow + 1, options, i => {
                    Parallel.For(1, this.width + 1, options, j => {
                        Vector3d colorRes = new(0, 0, 0); // 每个线程单独一个颜色值
                        colorRes.e[0] = colorRes.e[1] = colorRes.e[2] = 0d;

                        // 去判断，这个像素点，是不是红色的，需不需要进行MSAA
                        var actual_sample_pixel_count = Get_MSAA_Level(i, j);

                        for (int k = 0; k < actual_sample_pixel_count; k++) { //MSAA重要性采样
                            double uRandom = PbRandom.Random_Double(), vRandom = PbRandom.Random_Double();
                            if (actual_sample_pixel_count == 1) { uRandom = vRandom = 0; }
                            var u = (1.0d * j + uRandom) / width;
                            var v = (1.0d * i + vRandom) / height;
                            Ray ray = camera.GetRay(u, v);

                            colorRes += RayColor.Ray_Color(ray, worldBvh, GobVar.Render_Depth);
                        }

                        // DONE

                        var writeColor = colorRes / actual_sample_pixel_count;
                        writeColor = Vector3d.Clamp(writeColor);
                        //Gamma矫正
                        writeColor.e[0] = Math.Sqrt(writeColor.e[0]);
                        writeColor.e[1] = Math.Sqrt(writeColor.e[1]);
                        writeColor.e[2] = Math.Sqrt(writeColor.e[2]);

                        this.SetColorToBytes(i, j, new PbColorRGB(writeColor).ConvertToSysColor(), pixelColorBytes);
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
                var stop = DateTime.Now;
                _BackWorker.ReportProgress(100, (stop - start));
            }
            catch (Exception ex) {
                MessageBox.Show("渲染过程中出现错误\n" + ex.Message);
            }

            // 做Ray_Color的后期处理
            RayColor.AfterRender();
        }
        //---- End Of Render ----
        public static WriteableBitmap CreateWriteableBitMap(int width, int height) {
            return new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }



        /// <summary>
        /// 设置图像画面的颜色，操作PixelColorBytes数组，从1开始，到最终像素结束
        /// </summary>
        /// <param name="x">画面高度</param>
        /// <param name="y">画面宽度</param>
        /// <param name="color">系统库自带的颜色</param>
        void SetColorToBytes(int x, int y, System.Drawing.Color color, Byte[] array) {
            x--; y--;
            int point = x * width * 4 + y * 4;
            array[point] = color.B;
            array[point + 1] = color.G;
            array[point + 2] = color.R;
            array[point + 3] = 255;
        }
        /// <summary>
        /// 根据之前计算的红色信息点，这些点是重要的，来绘制图元
        /// </summary>
        int Get_MSAA_Level(int x, int y) {
            //return sample_pixel_count;
            bool needDo = false;
            for (int dx = -2; dx <= 2; dx++) {
                for (int dy = -2; dy <= 2; dy++) {
                    needDo |= Get_Red_Color(x + dx, y + dy);
                }
            }
            if (needDo) {
                return sample_pixel_count;
            } else {
                return 1;
            }
        }
        bool Get_Red_Color(int x, int y) {
            x--; y--;
            int point = x * width * 4 + y * 4;
            if (point >= 0 && point < this.pixelRedPreview.Length && this.pixelRedPreview[point + 2] == 255) {
                return true;
            } else {
                return false;
            }
        }
    }
}
