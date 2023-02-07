using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    /// <summary>
    /// 一个通透的玻璃小球，封装了两个Sphere，一个半径为正数，一个半径为负数
    /// </summary>
    public class DielectricSphere : HitTable {
        public Vector3d center = new Vector3d();
        public double radius;
        public Dielectric mat = new Dielectric(1.5);

        Sphere m1, m2; // 大半径，小半径
        void SetSphere() {
            m1 = new Sphere(center, radius, mat);
            m2 = new Sphere(center, -0.9d * radius, mat);
        }

        public DielectricSphere() { }
        public DielectricSphere(Vector3d center, double radius) {
            this.center = center;
            this.radius = radius;
            SetSphere();
        }
        public DielectricSphere(Vector3d center, double radius, Dielectric mat) {
            this.center = center;
            this.radius = radius;
            this.mat = mat;
            SetSphere();
        }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            HitResult ans = new HitResult(), m1Res, m2Res;
            var m1Hit = m1.Hit(ray, t_min, t_max, out m1Res);
            if (m1Hit == true) ans = m1Res;
            var m2Hit = m2.Hit(ray, t_min, t_max, out m2Res);
            if (m1Hit == false || m1Hit == true && m2Hit == true && m2Res.t < ans.t) ans = m2Res;
            hitResult = ans;
            if (m2Hit | m1Hit == true) {
                return true;
            } else {
                return false;
            }
        }
        public override bool Bounding_Box(out AABB? output_box) {
            var t = new Vector3d(radius, radius, radius);
            output_box = new AABB(center - t, center + t);
            return true;
        }

        public override object Clone() {
            DielectricSphere m = new DielectricSphere(this.center, this.radius);
            m.mat = this.mat;
            return m;
        }
    }
}
