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
    public class Lam_Normal_Map : Material {
        public override bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered) {
            attenuation = albedo * mTexture.Value(rec.u, rec.v, rec.p);

            var normal_world = new Vector3d();
            // Rechange Normal Direction
            var tangent = rec.tangent.Normalized();
            var bit_tangent = Vector3d.Cross(tangent, rec.normal).Normalized();

            if (mNormalMap == null) {
                normal_world = rec.normal;
            } else {
                var noraml_direct = mNormalMap.Value(rec.u, rec.v, rec.p);
                noraml_direct = noraml_direct * 2 - new Vector3d(1, 1, 1);
                normal_world = new Matrix3x3d(tangent, bit_tangent, rec.normal) * noraml_direct;
                normal_world = normal_world.Normalized();

                normal_world.e[0] = normal_world.x() * normal_scale;
                normal_world.e[1] = normal_world.y() * normal_scale;
                normal_world.e[2] = Math.Sqrt(1 - normal_world.y() * normal_world.y() - normal_world.x() * normal_world.x());
            }


            var scatter_direction = normal_world + Vector3d.Random_Unit_Vector().Normalized();
            scattered = new Ray(rec.p, scatter_direction);

            return true;
        }

        public Lam_Normal_Map(Texture texture, Texture? normal_map = null) {
            mNormalMap = normal_map;
            this.mTexture = texture;
        }

        public Texture? mNormalMap;

        public Vector3d albedo = new Vector3d(x: 1, 1, 1);//光线衰减值（反射颜色值）
        public double normal_scale = -1.12d;
    }
}
