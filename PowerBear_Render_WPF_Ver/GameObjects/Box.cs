using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    public class Box : HitAble {
        public Vector3d pointMin, pointMax;
        public Hittable_List sides;
        private Material mat = new Lambertian();
        public Box() { pointMin = new(-0.5, -0.5, -0.5); pointMax = new(0.5, 0.5, 0.5); }
        public Box(Vector3d p0min, Vector3d p1max, Material mat) {
            this.pointMin = p0min;
            this.pointMax = p1max;
            this.mat = mat;
            sides = new Hittable_List();
            InitBox(p0min, p1max);
        }
        public Box(Vector3d p0min, Vector3d p1max) {
            this.pointMin = p0min;
            this.pointMax = p1max;
            sides = new Hittable_List();
            InitBox(p0min, p1max);
        }
        void InitBox(Vector3d p0, Vector3d p1) {
            sides.Add(new XY_Rect(p0.x(), p1.x(), p0.y(), p1.y(), p1.z(), mat));
            sides.Add(new XY_Rect(p0.x(), p1.x(), p0.y(), p1.y(), p0.z(), mat));
            sides.Add(new XZ_Rect(p0.x(), p1.x(), p0.z(), p1.z(), p1.y(), mat));
            sides.Add(new XZ_Rect(p0.x(), p1.x(), p0.z(), p1.z(), p0.y(), mat));
            sides.Add(new YZ_Rect(p0.y(), p1.y(), p0.z(), p1.z(), p1.x(), mat));
            sides.Add(new YZ_Rect(p0.y(), p1.y(), p0.z(), p1.z(), p0.x(), mat));
        }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            var res = sides.Hit(ray, t_min, t_max, out hitResult);
            hitResult.mat = this.mat;
            return res;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            output_box = new AABB(pointMin, pointMax);
            return true;
        }
        public override object Clone() {
            return new Box(this.pointMin, this.pointMax, this.mat);
        }
    }
}
