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
using System.ComponentModel;
using PowerBear_Render_WPF_Ver.DAO;
using PowerBear_Render_WPF_Ver.CameraObj;

namespace PowerBear_Render_WPF_Ver {
    public static class GobVar {
        public static int renderWidth, renderHeight;
        public static bool NeedFlush1 { get; set; } = false;
        public static bool AllowPreview { get; set; } = true; //允许运行过程中预览结果
        public static WriteableBitmap? wBitmap1 { get; set; } //用于写入图像的Bitmap
        public static int Render_Depth { get; set; } = 50;
        public static string appStartupPath = System.IO.Directory.GetCurrentDirectory();
        //======Render Options======
        public static Vector3d _BackColor = new Vector3d();
        public static Hittable_List fnWorld = new Hittable_List(); // fnWorld = fnObjects + fnLights
        public static BindingList<PointLight_Phong> fnLights { get; set; } = new(); //存放的是Phong模型的光照
        public static bool stopAtRenderColor { get; set; } = false; //只进行像素着色器渲染，不渲染真正颜色
        //======Deault Objects======
        public static HitTable skyObject { get; set; } = new Sphere(new(0, 0, 0), 10000, new SkyMat(new Solid_Color(0.4d, 0.1d, 0.3d)));
        public static Lambertian DeaultMat = new Lambertian(new Vector3d(0.5, y: 0.5, 0.5));
        public static Hittable_List worldObjects = new(); // 世界场景数组，渲染器渲染这个
        public static BackgroundWorker backgroundWorker = new BackgroundWorker();// 切换背后的渲染管线
        public static double MulSkyColor = 1.3d;
        // ======场景物体=======
        public static Camera mCamera = new();
        static BindingList<NormalObject> _fnObjects = new();
        public static BindingList<NormalObject> fnObjects { get { return _fnObjects; } set { _fnObjects = value; } }
        public static ToRenderDispterData RenderDispData;

        /// <summary>
        /// 封装好的，Cornell_Box，是1984年创建的一个展示光照效果的演示程序
        /// 视角选择 位置：0 5 10 视觉方向：0 0 -1
        /// </summary>
        public static Hittable_List Cornell_Box() {
            Hittable_List objects = new();

            var red = new Lambertian(new Vector3d(.65, .05, .05));
            var white = new Lambertian(new Vector3d(.73, .73, .73));
            var green = new Lambertian(new Vector3d(.12, .45, .15));
            var light = new DiffuseLightMat(new Solid_Color(1, 1, 1), 20);

            objects.Add(new YZ_Rect(0, 10, -5, 5, 5, green));
            objects.Add(new YZ_Rect(0, 10, -5, 5, -5, red));


            objects.Add(new XZ_Rect(-5, 5, -5, 5, 0, white));
            objects.Add(new XZ_Rect(-5, 5, -5, 5, 10, white));

            objects.Add(new XY_Rect(-5, 5, 0, 10, -5, white));

            objects.Add(new XZ_Rect(-1.4, 1.4, -1.4, 1.4, 9.9999d, light));

            return objects;
        }
        //======Gobal Functions======
        public static void AppRunInit() { //在App.xaml.cs里面
            
            //byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8 };
            //GobVar.doDeNoise(data, 4, 0);
            try {
                var result = GobVar.TestString();
                Console.WriteLine(result);
                Console.WriteLine("和C++的DLL通讯成功");
            }
            catch (Exception e) {
                Console.WriteLine("和C++的DLL链接失败");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException?.Message);
            }


            // 初始的一些小场景 和 材质 之类的玩意
            /*
            var t = new Box(new(0, 0, 0), new(3, 3, 3));
            HitTable box1 = t;
            NormalObject box1Obj = new(box1);
            box1Obj.angleY = -40d;
            box1Obj.offset = new(-1.6, 0, -1.3);
            box1Obj.objName = "立方体";

            HitTable box2 = new Box(new(0, 0, 0), new(1.5, 1.5, 1.5));
            NormalObject box2Obj = new(box2);
            box2Obj.angleY = 40d;
            box2Obj.offset = new(1.8, 0, 0);
            box2Obj.objName = "小方体";

            HitTable cornell_Box = GobVar.Cornell_Box();
            NormalObject cornell_BoxObj = new(cornell_Box);
            cornell_BoxObj.objName = "光照盒子";


            HitTable xzPanel = new XZ_Rect(-3, 3, -3, 3, 5, new DiffuseLightMat(new Solid_Color(1, 1, 1), 30));
            NormalObject xzPanelObj = new(xzPanel);
            xzPanelObj.objName = "头顶光";

            HitTable xzPanel2 = new XZ_Rect(-100, 100, -100, 100, 0, new Lambertian());
            NormalObject xzPanel2Obj = new(xzPanel2);
            xzPanel2Obj.objName = "底部平面";

            fnObjects.Add(box1Obj);
            fnObjects.Add(box2Obj);
            fnObjects.Add(cornell_BoxObj);
            fnObjects.Add(xzPanelObj);
            fnObjects.Add(xzPanel2Obj);
            */
            //HitTable modelYiYi = new ObjModel("C:\\Users\\PowerBear\\Desktop\\Doc\\大创渲染器\\中间过程演示\\Model\\依依\\依依（1）.obj", new Lambertian(new ImageTexture("C:\\Users\\PowerBear\\Desktop\\Doc\\大创渲染器\\中间过程演示\\Model\\依依\\依依（1）.png")));

            //NormalObject modelYiyiObj = new(modelYiYi);
            //modelYiyiObj.objName = "依依";
            //fnObjects.Add(modelYiyiObj);

            HitTable sphere = new Sphere(Vector3d.Zero, 5, new Metal(new(0.5d, 0.5d, 0.5d), 0.1));
            sphere._GUID = 3;
            NormalObject sphereObj = new(sphere);
            sphereObj.offset = new(5.5, 0, 0);
            sphereObj.objName = "小球体_Metal";
            fnObjects.Add(sphereObj);

            HitTable sphere2 = new DielectricSphere(Vector3d.Zero, 5, new Dielectric(1.5d));
            sphere2._GUID = 1;
            NormalObject sphereObj2 = new(sphere2);
            sphereObj2.offset = new(-5.5, 0, 0);
            sphereObj2.objName = "小球体_Glass";
            fnObjects.Add(sphereObj2);

            //HitTable modelGirl = new ObjModel("C:\\Users\\PowerBear\\Desktop\\Doc\\大创渲染器\\BlenderProject\\大场景\\单独琵琶和女生.obj", new Lambertian(0.5d, 0.5d, 0.5d));

            //NormalObject modelGirlObj = new(modelGirl);
            //modelGirlObj.objName = "抱着琵琶女孩";
            //fnObjects.Add(modelGirlObj);

            // PbIO.JsonEncode();
            // 0 8 -15
            // C:\\Users\\PowerBear\\Desktop\\Doc\\大创渲染器\\中间过程演示\\Model\\FLY_MMD\\Fly_MIKU.obj
            //C:\\Users\\PowerBear\\Desktop\\Sample\\FLY_MMD\\Fly_MIKU.obj

            ObjModelMtl objModelMtl = new ObjModelMtl("D:\\MMD\\Models\\OBJ\\YYB式桜ミクv1.00\\OBJ\\Rabbit.obj");
            NormalObject objModelMtlObj = new(objModelMtl);
            objModelMtlObj.objName = "初音未来";
            fnObjects.Add(objModelMtlObj);
        }
        /// <summary>
        /// 出发了，像素级渲染，根据条件进行判断
        /// </summary>
        public static void Render_Preview() {
            if (MainWindow.Instance?.renderDetails.uAllowRenderPreview == false) return;
            GobVar.RenderDispData = new ToRenderDispterData() { width = GobVar.renderWidth, height = GobVar.renderHeight, mCamera = GobVar.mCamera, cpus = 8, sample_depth = 0, sample_pixel_level = 0, _BackWorker = GobVar.backgroundWorker, startRow = 1, endRow = GobVar.renderHeight };
            GobVar.stopAtRenderColor = true;
            MainWindow.Instance?.DoRender();
        }
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

        /// <summary>
        /// 测试C++部分的DLL文件是否链接成功了
        /// </summary>
        [DllImport("PowerBear_Render_CPP_DLL")]
        public extern static int TestDLL(int a, int b);

        [DllImport("PowerBear_Render_CPP_DLL", EntryPoint = "TestString")]
        public extern static char TestString();


        //调试cpp dll文件：https://blog.csdn.net/weixin_40314351/article/details/127652594

        /// <summary>
        /// 在渲染之前，根据设定的参数，设定渲染变量
        /// </summary>
        public static void InitRender() {
            // 设置Depth
            switch (MainWindow.Instance.Depth_Combox.SelectedIndex) {
                case 0:
                GobVar.Render_Depth = 10;
                break;
                case 1:
                GobVar.Render_Depth = 50;
                break;
                case 2:
                GobVar.Render_Depth = 100;
                break;
            }
            // 设置背景颜色方式
            fnWorld.Clear();
            foreach (var item in GobVar.fnObjects) if (item.needRender) fnWorld.Add(item);

            // Loop Before Rendering
            foreach (var item in GobVar.fnWorld.objects) {
                item.BeforeRendering();
            }

        }
    }
}
