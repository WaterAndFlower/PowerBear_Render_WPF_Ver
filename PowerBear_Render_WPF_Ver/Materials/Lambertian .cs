using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Materials {
    public class Lambertian : Material {
        public Lambertian(Vector3d albedo) { this.albedo = albedo; }
        public override bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered) {
            var scatter_direction = rec.normal + Vector3d.Random_Unit_Vector().Normalized();
            scattered = new Ray(rec.p, scatter_direction);
            attenuation = albedo;
            return true;
        }

        public Vector3d albedo;//光线衰减值（反射颜色值）
    }
}
