using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects
{
    public struct HitResult {
        public Vector3d p;
        public Vector3d normal;
        public double t; // 射线碰撞后at(t)参数
        public bool front_face; //T:射线从表面的外面射入，F:射线从表面内面摄入
        public Material mat;
        public void Set_Face_Normal(Ray r, Vector3d outward_normal) {
            front_face = Vector3d.Dot(r.direction, outward_normal) < 0;
            normal = front_face ? outward_normal : -1.0d * outward_normal;//取反，便于计算
        }
    };
    public abstract class HitTable {
        public abstract bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult);
    }
}
