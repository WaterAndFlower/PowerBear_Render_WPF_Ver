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
    public class SkyMat : Material {
        public SkyMat(Texture tex) { _tex = tex; }

        public override bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered) {
            scattered = new Ray(Vector3d.Zero, Vector3d.Zero);
            attenuation = _tex.Value(rec.u, rec.v, r_in.direction);//这点最终于，要把光线方向传递进去。
            return true;
        }
        public Texture _tex;
    }
}
