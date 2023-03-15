using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerBear_Render_WPF_Ver.Textures {
    /// <summary>
    /// 材质纹理类，不允许直接实例化。令左下角是(0,0)，右上角是(1,1)。
    /// </summary>
    [XmlInclude(typeof(Solid_Color))]
    [XmlInclude(typeof(LinerColor))]
    public abstract class Texture {
        /// <summary>
        /// 返回纹理的颜色值。p是击中点在空间中的坐标。
        /// </summary>
        /// <param name="u">图像的u坐标</param>
        /// <param name="v">图像的v坐标</param>
        /// <param name="p">击中点的三维坐标</param>
        /// <returns></returns>
        public virtual Vector3d Value(double u, double v, Vector3d p) {
            return new Vector3d(128.0 / 255.0, 0, 128.0 / 255.0);//未能实现此方法，返回紫色
        }
    }
}
