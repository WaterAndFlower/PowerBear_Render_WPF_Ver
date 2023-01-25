using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Textures {
    /// <summary>
    /// 噪声纹理实现
    /// https://zhuanlan.zhihu.com/p/524628093
    /// </summary>
    public class NoiseTexture : Texture {
        public Perlin mPerlin = new Perlin();
        double scale = 1.0;
        public NoiseTexture(double scale = 1) { this.scale = scale; }
        public override Vector3d Value(double u, double v, Vector3d p) {
            return new Vector3d(1, 1, 1) * mPerlin.Turb(p * scale);
        }
    }
}
