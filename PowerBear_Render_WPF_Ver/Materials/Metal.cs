using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerBear_Render_WPF_Ver.Materials {
    [XmlInclude(typeof(Material))]
    public class Metal : Material {
        public Metal(Vector3d albeod, double fuzz) {
            this.albedo = albeod;
            this.fuzz = fuzz < 1 ? fuzz : 1;
        }
        public Metal() { }
        Vector3d Reflect(Vector3d v, Vector3d n) {
            return v - 2 * Vector3d.Dot(v, n) * n;
        }
        public override bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered) {
            Vector3d reflected = Reflect(Vector3d.Unit_Vector(r_in.direction), rec.normal);
            scattered = new Ray(rec.p, reflected + fuzz * Vector3d.Random_Unit_Vector().Normalized());
            attenuation = albedo * mTexture.Value(rec.u, rec.v, rec.p);
            return Vector3d.Dot(scattered.direction, rec.normal.Normalized()) > 0;
        }
        public Vector3d albedo;
        double fuzz = 0; // 光滑程度
    }
}
