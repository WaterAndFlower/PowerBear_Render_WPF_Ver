using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using PowerBear_Render_WPF_Ver.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Materials {
    public class Lambertian : Material {
        /// <summary>
        /// 默认白色小球材质和纹理颜色
        /// </summary>
        public Lambertian() { }
        public Lambertian(double r, double g, double b) { this.albedo = new Vector3d(r, g, b); }
        public Lambertian(Vector3d albedo) { this.albedo = albedo; }
        public Lambertian(Texture texture) { this.mTexture = texture; }
        public Lambertian(Vector3d albedo, Texture texture) { this.albedo = albedo; this.mTexture = texture; }
        public override bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered) {
            var scatter_direction = rec.normal + Vector3d.Random_Unit_Vector().Normalized();
            scattered = new Ray(rec.p, scatter_direction);
            attenuation = albedo * mTexture.Value(rec.u, rec.v, rec.p);
            return true;
        }

        public Vector3d albedo { get; set; } = new Vector3d(x: 1, 1, 1);//光线衰减值（反射颜色值）
    }
}
