using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    /// <summary>
    /// 一个XZ的平面
    /// </summary>
    public class XZ_Rect : HitAble {
        public Material mat;
        double x0, x1, z0, z1, k;//k是在y方向上的维度
        /// <summary>
        /// 一个xz平面，通过设定x轴上的界限和z轴上的界限，来划定区域，k来在y轴上移动
        /// </summary>
        /// <param name="_x0">x轴开始</param>
        /// <param name="_x1">x轴结束</param>
        /// <param name="_z0">z轴开始</param>
        /// <param name="_z1">z轴结束</param>
        /// <param name="_k">y轴上的位置</param>
        /// <param name="mat">材质</param>
        public XZ_Rect(double _x0, double _x1, double _z0, double _z1, double _k, Material mat) {
            this.x0 = _x0; this.x1 = _x1; this.z0 = _z0; this.z1 = _z1; this.k = _k; this.mat = mat;
        }
        public XZ_Rect() { }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            hitResult = new HitResult();
            var t = (k - ray.origin.y()) / ray.direction.y();
            if (t < t_min || t > t_max)
                return false;
            var x = ray.origin.x() + t * ray.direction.x();
            var z = ray.origin.z() + t * ray.direction.z();
            if (x < x0 || x > x1 || z < z0 || z > z1)
                return false;
            hitResult.u = (x - x0) / (x1 - x0);
            hitResult.v = (z - z0) / (z1 - z0);
            hitResult.t = t;
            Vector3d outward_normal = new(0, 1, 0);
            hitResult.Set_Face_Normal(ray, outward_normal);
            hitResult.mat = this.mat;
            hitResult.p = ray.At(t);
            return true;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            output_box = new(new Vector3d(x0, k - 0.00001d, z0), new Vector3d(x1, k + 0.00001d, z1));
            return true;
        }
        public override object Clone() {
            return new XZ_Rect(x0, x1, z0, z1, k, mat);
        }
    }

    /// <summary>
    /// 一个YZ平面
    /// </summary>
    public class YZ_Rect : HitAble {
        public Material mat;
        double y0, y1, z0, z1, k;//k是在y方向上的维度
        /// <summary>
        /// 一个yz平面，通过设定y轴上的界限和z轴上的界限，来划定区域，k来在x轴上移动
        /// </summary>
        /// <param name="_y0">y轴开始</param>
        /// <param name="_y1">y轴结束</param>
        /// <param name="_z0">z轴开始</param>
        /// <param name="_z1">z轴结束</param>
        /// <param name="_k">x轴上的位置</param>
        /// <param name="mat">材质</param>
        public YZ_Rect(double _y0, double _y1, double _z0, double _z1, double _k, Material mat) {
            this.y0 = _y0; this.y1 = _y1; this.z0 = _z0; this.z1 = _z1; this.k = _k; this.mat = mat;
        }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            hitResult = new HitResult();
            var t = (k - ray.origin.x()) / ray.direction.x();
            if (t < t_min || t > t_max)
                return false;
            var y = ray.origin.y() + t * ray.direction.y();
            var z = ray.origin.z() + t * ray.direction.z();
            if (y < y0 || y > y1 || z < z0 || z > z1)
                return false;
            hitResult.u = (y - y0) / (y1 - y0);
            hitResult.v = (z - z0) / (z1 - z0);
            hitResult.t = t;
            var outward_normal = new Vector3d(1, 0, 0);
            hitResult.Set_Face_Normal(ray, outward_normal);
            hitResult.mat = this.mat;
            hitResult.p = ray.At(t);
            return true;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            output_box = new(new Vector3d(k - 0.00001d, y0, z0), new Vector3d(k + 0.00001d, y1, z1));
            return true;
        }
        public override object Clone() {
            return new YZ_Rect(y0, y1, z0, z1, k, mat);
        }
    }
    /// <summary>
    /// XY平面
    /// </summary>
    public class XY_Rect : HitAble {
        public Material mat;
        double x0, x1, y0, y1, k;//k是在y方向上的维度
        /// <summary>
        /// 一个xz平面，通过设定x轴上的界限和z轴上的界限，来划定区域，k来在y轴上移动
        /// </summary>
        /// <param name="_x0">x轴开始</param>
        /// <param name="_x1">x轴结束</param>
        /// <param name="_y0">z轴开始</param>
        /// <param name="_y1">z轴结束</param>
        /// <param name="_k">y轴上的位置</param>
        /// <param name="mat">材质</param>
        public XY_Rect(double _x0, double _x1, double _y0, double _y1, double _k, Material mat) {
            this.x0 = _x0; this.x1 = _x1; this.y0 = _y0; this.y1 = _y1; this.k = _k; this.mat = mat;
        }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            hitResult = new HitResult();
            var t = (k - ray.origin.z()) / ray.direction.z();
            if (t < t_min || t > t_max)
                return false;
            var x = ray.origin.x() + t * ray.direction.x();
            var y = ray.origin.y() + t * ray.direction.y();
            if (x < x0 || x > x1 || y < y0 || y > y1)
                return false;
            hitResult.u = (x - x0) / (x1 - x0);
            hitResult.v = (y - y0) / (y1 - y0);
            hitResult.t = t;
            Vector3d outward_normal = new(0, 0, 1);
            hitResult.Set_Face_Normal(ray, outward_normal);
            hitResult.mat = this.mat;
            hitResult.p = ray.At(t);
            return true;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            output_box = new(new Vector3d(x0, y0, k - 0.00001d), new Vector3d(x1, y1, k + 0.00001d));
            return true;
        }
        public override object Clone() {
            return new XY_Rect(x0, x1, y0, y1, k, mat);
        }
    }
}