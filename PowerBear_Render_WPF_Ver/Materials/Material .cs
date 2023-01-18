using PowerBear_Render_WPF_Ver.GameObjects;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;

namespace PowerBear_Render_WPF_Ver.Materials
{
    public abstract class Material
    {
        public abstract bool Scatter(Ray r_in, HitResult rec,out Vector3d attenuation,out Ray scattered);
    }
}
