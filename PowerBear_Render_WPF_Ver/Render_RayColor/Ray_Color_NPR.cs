using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render_RayColor {
    public class Ray_Color_NPR : IRayColor {
        Vector3d LightPos = new(10, 10, 10); // 一个平行光源的位置
        Vector3d LightColor = new(1, 1, 1);
        Vector3d HighColor = new(1, 1, 1);

        Vector3d Reflect(Vector3d v, Vector3d n) {
            return v - 2 * Vector3d.Dot(v, n) * n;
        }


        public Vector3d Ray_Color(Ray ray, HitTable world, int depth) {
            HitResult hitResult;
            if (world.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                // 在这里实现算法部分
                Ray scattered;
                Vector3d attenuation, emitColor = hitResult.mat.Emit(hitResult.u, hitResult.v, hitResult.p);
                if (hitResult.mat.Scatter(ray, hitResult, out attenuation, out scattered)) {
                    Vector3d ambient = GobVar._BackColor * 0.5d;
                    Vector3d worldNormal = hitResult.normal.Normalized();
                    Vector3d worldLightDir = LightPos;
                    Vector3d diffuse = attenuation * LightColor * PbMath.PbMath.ClampRangeDouble(worldNormal.Dot(worldLightDir), 0, 1);
                    //开始计算高光
                    Vector3d reflectDir = Reflect(worldLightDir, worldNormal).Normalized();
                    //mCamera.origin - hitResult.p
                    Vector3d viewDir = (ray.direction).Normalized();
                    Vector3d specular = HighColor * LightColor * Math.Pow(PbMath.PbMath.ClampRangeDouble(reflectDir.Dot(viewDir), 0, 1d), 18);

                    return GobVar.MulSkyColor * ambient + diffuse + specular;
                    //return emitColor + attenuation * Ray_Color(scattered, world, depth - 1);
                } else {
                    return emitColor; // 返回自发光的颜色
                }
            }
            // 在这里实现光照盒反射部分
            ray.origin = Vector3d.Zero;
            if (GobVar.skyObject.Hit(ray, 0.0000001d, 0x3f3f3f3f, out hitResult)) {
                Ray scattered;
                Vector3d atten;
                if (hitResult.mat.Scatter(ray, hitResult, out atten, out scattered)) {
                    return atten * 1;
                }
            }
            return GobVar._BackColor;
        }
    }
}
