using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    public class Sphere : HitTable {
        public Sphere() { }
        public Sphere(Vector3d center, double radius) {
            this.center = center;
            this.radius = radius;
        }
        public Sphere(Vector3d center, double radius, Material mat) {
            this.center = center;
            this.radius = radius;
            this.mat = mat;
        }

        public Vector3d center = new Vector3d();
        public double radius;
        public Material mat = GobVar.DeaultMat;

        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            hitResult = new HitResult();
            Vector3d oc = ray.origin - center;
            var a = ray.direction.LengthSquared();
            var half_b = Vector3d.Dot(oc, ray.direction);
            var c = oc.LengthSquared() - radius * radius;
            var discriminant = half_b * half_b - a * c;
            if (discriminant < 0) return false;
            var sqrtd = Math.Sqrt(discriminant);
            // 查找在可接受的t范围内，最近的碰撞点
            var root = (-half_b - sqrtd) / a;
            if (root < t_min || t_max < root) {
                root = (-half_b + sqrtd) / a;
                if (root < t_min || t_max < root)
                    return false;
            }
            //保存结果
            hitResult.t = root;
            hitResult.p = ray.At(root);
            var out_normal = (hitResult.p - center) / radius;
            hitResult.Set_Face_Normal(ray, out_normal);
            hitResult.mat = this.mat;
            //保存UV 到 HitResult
            Sphere.Get_Sphere_UV(out_normal, out hitResult.u, out hitResult.v);
            return true;
        }

        public override bool Bounding_Box(out AABB output_box) {
            var t = new Vector3d(radius, radius, radius);
            output_box = new AABB(center - t, center + t);
            return true;
        }
        /// <summary>
        /// 返回在球上的u和v坐标，通过球的法线方向来判断
        /// </summary>
        /// <param name="p"></param>
        /// <param name="u">out u</param>
        /// <param name="v">out v</param>
        public static void Get_Sphere_UV(Vector3d p, out double u, out double v) {
            var theta = Math.Acos(-p.y());
            var phi = Math.Atan2(-p.z(), p.x()) + Math.PI;
            u = phi / (2 * Math.PI);
            v = theta / Math.PI;
        }
    }
}
