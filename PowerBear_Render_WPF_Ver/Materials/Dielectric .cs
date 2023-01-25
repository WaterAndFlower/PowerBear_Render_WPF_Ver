using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Materials {
    /// <summary>
    /// 绝缘体材质，只发生折射。将球半径设置为负数，以此来获得一个上下颠倒的结果，虽然并不符合真实实际。
    /// </summary>
    public class Dielectric : Material {
        public double ir;
        public Vector3d attenuation = new Vector3d(1.0d, 1.0d, 1.0d);

        public Dielectric(double index_of_refraction, Vector3d attenuation) {
            ir = index_of_refraction;
            this.attenuation = attenuation;
        }
        public Dielectric(double index_of_refraction) { ir = index_of_refraction; }

        // 看Metal文件
        Vector3d Reflect(Vector3d v, Vector3d n) {
            return v - 2 * Vector3d.Dot(v, n) * n;
        }
        /// <summary>
        /// 计算光线折射方向
        /// </summary>
        /// <param name="uv">入射光方向，需要单位向量</param>
        /// <param name="n">表明法向量</param>
        /// <param name="etai_over_etat">介质的折射率，入射光角度比折射光角度 n/n`</param>
        /// <returns></returns>
        Vector3d Refract(Vector3d uv, Vector3d n, double etai_over_etat) {
            var cos_theta = Vector3d.Dot(-1.0d * uv, n);
            var r_out_parallel = etai_over_etat * (uv + cos_theta * n);
            var r_out_perp = -1.0d * Math.Sqrt(1.0d - r_out_parallel.LengthSquared()) * n;
            return r_out_perp + r_out_parallel;
        }
        /// <summary>
        /// 一个由Christophe Schlick提出的近似估算
        /// https://zhuanlan.zhihu.com/p/511181059
        /// </summary>
        static double Schlick(double cosine, double ref_idx) {
            var r0 = (1.0d - ref_idx) / (1 + ref_idx);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
        }
        public override bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered) {
            double etai_over_etat = rec.front_face ? (1.0d / ir) : ir;

            Vector3d unit_direction = Vector3d.Unit_Vector(r_in.direction);

            double cos_theta = Math.Min(Vector3d.Dot(-1.0d * unit_direction, rec.normal), 1.0d);
            double sin_theta = Math.Sqrt(1.0d - cos_theta * cos_theta);

            Vector3d resDir;
            if (etai_over_etat * sin_theta > 1.0d || Schlick(cos_theta, etai_over_etat) > PbRandom.Random_Double()) {
                attenuation = new Vector3d(1, 0, 0);
                resDir = Reflect(unit_direction, rec.normal); // 无实数解，发生反射
            } else {
                resDir = Refract(unit_direction, rec.normal, etai_over_etat); // 发生折射
            }

            attenuation = this.attenuation;
            scattered = new Ray(rec.p, resDir);
            return true;
        }
    }
}
