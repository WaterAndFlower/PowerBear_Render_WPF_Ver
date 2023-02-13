using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using PowerBear_Render_WPF_Ver.Textures;

namespace PowerBear_Render_WPF_Ver.Materials {
    public abstract class Material {
        /// <summary>
        /// 计算光线打在这个材质上，会发生什么反射。并且计算当前打入的颜色是什么值
        /// </summary>
        public abstract bool Scatter(Ray r_in, HitResult rec, out Vector3d attenuation, out Ray scattered);
        /// <summary>
        /// 自发光或者是，uv贴图的颜色
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual Vector3d Emit(double u, double v, Vector3d p) {
            return Vector3d.Zero;
        }
        public Texture mTexture { get; set; } = new Solid_Color(1, 1, 1);
    }
}
