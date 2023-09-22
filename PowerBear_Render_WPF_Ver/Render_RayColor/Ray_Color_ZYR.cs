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

namespace PowerBear_Render_WPF_Ver.Render_RayColor {
    class Ray_Color_ZYR : IRayColor {
        Lam_Normal_Map _mat = new(new ImageTexture("D:/Brick_Diffuse.JPG"), new ImageTexture("D:/Brick_Normal.JPG"));
        //Lam_Normal_Map _mat = new(new ImageTexture("D:/Brick_Diffuse.JPG"));
        public Vector3d Ray_Color(Ray ray, HitAble world, int depth) {
            HitResult hitResult;
            if (depth <= 0) return new Vector3d(x: 0, 0, 0);
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                //return new Vector3d(1, 0, 0);
                Ray scattered;
                Vector3d attenuation, emitColor = hitResult.mat.Emit(hitResult.u, hitResult.v, hitResult.p);
                if (_mat.Scatter(ray, hitResult, out attenuation, out scattered)) {
                    return emitColor + attenuation * Ray_Color(scattered, world, depth - 1);
                } else {
                    return emitColor; // 返回自发光的颜色
                }
            }

            // 返回背景颜色 TODO:使用一个背景小球，采样
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

        public Vector3d Ray_Color_BackUp(Ray ray, HitAble world, int depth) {
            HitResult hitResult;
            if (depth <= 0) return new Vector3d(x: 0, 0, 0);
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                var tangent = hitResult.tangent;
                var bit_tangent = Vector3d.Cross(tangent, hitResult.normal).Normalized();
                tangent = tangent / 2 + new Vector3d(0.5, 0.5, 0.5);
                bit_tangent = bit_tangent / 2 + new Vector3d(0.5, 0.5, 0.5);
                Vector3d out_color; Ray next_ray;
                _mat.Scatter(ray, hitResult, out out_color, out next_ray);
                return out_color;
            }
            // 返回背景颜色 TODO:使用一个背景小球，采样
            return new(0, 0, 0);
        }
    }
}
