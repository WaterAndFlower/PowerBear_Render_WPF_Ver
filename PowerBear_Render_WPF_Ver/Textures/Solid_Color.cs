using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Textures {
    public class Solid_Color : Texture {
        /// <summary>
        /// 初始值是White（1，1，1）
        /// </summary>
        public Solid_Color() { }
        public Solid_Color(Vector3d color) { this.color = color; }
        public Solid_Color(double r, double g, double b) { this.color = new Vector3d(r, g, b); }
        public override Vector3d Value(double u, double v, Vector3d p) {
            return color;
        }
        Vector3d color = new Vector3d(1, 1, 1);
    }
}
