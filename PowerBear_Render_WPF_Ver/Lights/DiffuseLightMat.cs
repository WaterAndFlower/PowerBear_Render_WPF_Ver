using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using PowerBear_Render_WPF_Ver.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Lights {
    public class DiffuseLightMat : Material {
        public Texture emit;
        public double scale = 10.0d;
        public DiffuseLightMat(Texture emitTex, double scale = 10.0d) { this.emit = emitTex; this.scale = scale; }
        public override bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered) {
            attenuation = Vector3d.Zero;
            scattered = new Ray(Vector3d.Zero, Vector3d.Zero);
            return false;
        }
        /// <summary>
        /// 获得自发光的颜色，需要自发光贴图的支持，传入Texture基类里面的获取uv的参数。
        /// *材质激发亮度要大于 1 ！*
        /// </summary>
        public override Vector3d Emit(double u, double v, Vector3d p) {
            return scale * emit.Value(u, v, p);
        }
    }
}
