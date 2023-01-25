﻿using PowerBear_Render_WPF_Ver.DAO;
using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PowerBear_Render_WPF_Ver.Textures {
    public class ImageTexture : Texture {
        public ImageTexture(string path, double scale = 1.0d) {
            try {
                var img = new BitmapImage(new Uri(path));
                width = img.PixelWidth; height = img.PixelHeight;
                this.colors = PbIO.BitmapToBytes(img);
                Console.WriteLine($"读取材质贴图：{path}");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace + "\n" + ex.Source);
                MessageBox.Show("创建ImageTexture出现错误：\n" + ex.Message);
                return;
            }
            this.scale = scale;
        }
        public override Vector3d Value(double u, double v, Vector3d p) {//左下角是（0，0）
            u = PbMath.PbMath.Clamp(u);
            v = PbMath.PbMath.Clamp(v);
            if (u >= 1) u = 0.999999999d;
            if (v >= 1) v = 0.999999999d;
            v = 1.0d - v;
            return scale * GetRGB((int)(height * v), (int)(width * u));
        }
        Vector3d GetRGB(int x, int y) { //左上角是（0，0）右下角是（height-1，width-1）
            int index = x * width * 4 + y * 4;
            if (index < 0) index = 0;
            if (index > colors.Length) index = colors.Length - 1;
            byte b = colors[index], g = colors[index + 1], r = colors[index + 2];
            var color_scale = 1.0 / 255.0;
            return new Vector3d(r * color_scale, g * color_scale, b * color_scale);
        }
        public Byte[] colors; // height*width*4
        int width, height; // BGRA
        public double scale = 1.0d;
    }
}