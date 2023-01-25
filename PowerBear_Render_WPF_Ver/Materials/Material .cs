using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;

namespace PowerBear_Render_WPF_Ver.Materials {
    public abstract class Material {
        /// <summary>
        /// 计算光线打在这个材质上，会发生什么反射
        /// </summary>
        public abstract bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered);
        public virtual Vector3d Emit(double u, double v, Vector3d p) {
            return Vector3d.Zero;
        }
    }
}
