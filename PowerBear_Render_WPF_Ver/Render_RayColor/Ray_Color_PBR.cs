using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render_RayColor {
    public class Ray_Color_PBR : IRayColor {
        public Vector3d Ray_Color(Ray ray, HitTable world, int depth) {
            HitResult hitResult;
            if (depth <= 0) return new Vector3d(x: 0, 0, 0);
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                //return new Vector3d(1, 0, 0);
                Ray scattered;
                Vector3d attenuation, emitColor = hitResult.mat.Emit(hitResult.u, hitResult.v, hitResult.p);
                if (hitResult.mat.Scatter(ray, hitResult, out attenuation, out scattered)) {
                    return emitColor + attenuation * Ray_Color(scattered, world, depth - 1);
                } else {
                    return emitColor; // 返回自发光的颜色
                }
            }
            // 返回背景颜色 TODO:使用一个背景小球，采样
            //return GobVar._BackColor;
            ray.origin = Vector3d.Zero;
            if (GobVar.skyObject.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                Ray scattered;
                Vector3d atten;
                if (hitResult.mat.Scatter(ray, hitResult, out atten, out scattered)) {
                    if (depth == GobVar.Render_Depth) {
                        return atten * 1d;
                    } else {
                        return atten * GobVar.MulSkyColor;
                    }
                }
            }
            return GobVar._BackColor;
        }
    }
}
