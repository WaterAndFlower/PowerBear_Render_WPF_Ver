using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PowerBear_Render_WPF_Ver {
    /// <summary>
    /// AddObject.xaml 的交互逻辑
    /// </summary>
    public partial class AddObjectWindow : Window {
        // ==== Materials ====
        public int MatPanelSelectedIndex { get; set; } = 0;
        public int TexPanelSelectedIndex { get; set; } = 0;
        public double MetalFuzz { get; set; } = 0.2;
        public double Index_Of_Refraction { get; set; } = 1.4d;
        // ==== TexTure ====
        public double Checker_Scale { get; set; } = 10d;
        public string ImgTexPath { get; set; } = "";
        public double NoiseScale { get; set; } = 1.0d;
        // ==== new Sphere ====
        public double SpRadius { get; set; } = 5;
        public double SpPosX { get; set; } = 0;
        public double SpPosY { get; set; } = 0;
        public double SpPosZ { get; set; } = 0;
        // ==== new BoX ====
        public Vector3d BoxPosMin { get; set; } = new(-1d, -1d, -1d);
        public Vector3d BoxPosMax { get; set; } = new(1d, 1d, 1d);
        // ==== new XZ panel ====
        public double PanelX { get; set; } = 10;
        public double PanelZ { get; set; } = 10;
        public double PanelY_Pos { get; set; } = 0;
        // ==== new ObjModel ====
        public string ObjPath { get; set; } = new("");
        public AddObjectWindow() {
            InitializeComponent();
            this.DataContext = this;
        }

        private Material GetUsrMat() { // 获得用户选择的Mat材质
            Material mat = new Lambertian(0, 255, 255);
            switch (MatPanelSelectedIndex) {
                case 0: //漫反射
                var uColor = uDiffuseColor.SelectedColor.Value;
                mat = new Lambertian(uColor.R * 1.0d / 255d, uColor.G * 1.0d / 255d, uColor.B * 1.0d / 255d);
                break;
                case 1: //金属
                uColor = uMetalColor.SelectedColor.Value;
                mat = new Metal(new(uColor.R / 255.0d, uColor.G / 255.0d, uColor.B / 255.0d), MetalFuzz);
                break;
                case 2: //玻璃
                uColor = uDielectricColor.SelectedColor.Value;
                mat = new Dielectric(Index_Of_Refraction, new(uColor.R / 255.0d, uColor.G / 255.0d, uColor.B / 255.0d));
                break;
            }
            Texture tex = new Solid_Color(0, 1, 1);
            switch (TexPanelSelectedIndex) {
                case 0: // Solid_Color
                var uColor = uSolidColor.SelectedColor.Value;
                tex = new Solid_Color(uColor.R / 255d, uColor.G / 255d, uColor.B / 255d);
                break;
                case 1: // 棋盘纹理
                var u1stColor = uCheckerFirstColor.SelectedColor.Value;
                var u2stColor = uCheckerSecColor.SelectedColor.Value;
                Vector3d t1 = new(ByteColorToDouble(u1stColor.R), ByteColorToDouble(u1stColor.G), ByteColorToDouble(u1stColor.B)), t2 = new(ByteColorToDouble(u2stColor.R), ByteColorToDouble(u2stColor.G), ByteColorToDouble(u2stColor.B));
                tex = new CheckerTexture(t1, t2, Checker_Scale);
                break;
                case 2: // 柏林噪声
                tex = new NoiseTexture(NoiseScale);
                break;
                case 3: // 图像纹理
                tex = new ImageTexture(ImgTexPath);
                break;
            }
            mat.mTexture = tex;
            return mat;
        }

        private void Button_Click_AddSphere(object sender, RoutedEventArgs e) {
            var mat = GetUsrMat();

            var newObj = new NormalObject(new Sphere(Vector3d.Zero, SpRadius, mat));
            newObj.offset = new(this.SpPosX, this.SpPosY, this.SpPosZ);
            newObj.objName = "未命名小球";

            GobVar.fnObjects.Add(newObj);
            GobVar.Render_Preview();
        }

        private void Button_Click_AddBox(object sender, RoutedEventArgs e) {
            var mat = GetUsrMat();

            var newObj = new NormalObject(new Box(BoxPosMin, BoxPosMax, mat));
            newObj.objName = "未命名盒体";

            GobVar.fnObjects.Add(newObj);
            GobVar.Render_Preview();
        }

        private void Button_Click_AddXZPanel(object sender, RoutedEventArgs e) {
            var mat = GetUsrMat();
            var newObj = new NormalObject(new XZ_Rect(-PanelX / 2.0d, PanelX / 2.0d, -PanelZ / 2.0d, PanelZ / 2.0d, PanelY_Pos, mat));
            newObj.objName = "未命名XZPanel";

            GobVar.fnObjects.Add(newObj);
            GobVar.Render_Preview();
        }

        private void Button_Click_ObjModel(object sender, RoutedEventArgs e) {
            var mat = GetUsrMat();
            var newObj = new NormalObject(new ObjModel(ObjPath, mat));
            newObj.objName = "未命名Obj物体";

            GobVar.fnObjects.Add(newObj);
            GobVar.Render_Preview();
        }

        double ByteColorToDouble(Byte col) {
            return col / 255d;
        }
    }
}
