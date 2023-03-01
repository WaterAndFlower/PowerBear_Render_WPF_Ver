using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PowerBear_Render_WPF_Ver.Textures {
    public class LinerColor : Texture {
        public LinerColor() { }

        public LinerColor(Vector3d color1, Vector3d color2) {
            this.color1 = color1;
            this.color2 = color2;
        }

        public LinerColor(Color col1, Color col2) {
            color1 = new(col1.R / 255d, col1.G / 255d, col1.B / 255d);
            color2 = new(col2.R / 255d, col2.G / 255d, col2.B / 255d);
        }

        public override Vector3d Value(double u, double v, Vector3d p) {
            Vector3d directNormal = p.Normalized();
            var t = 0.5 * (directNormal.y() + 1.0d);
            var res = t * color1 + (1 - t) * color2;
            return res;
        }

        Vector3d color1 = new(0, 0, 0);
        Vector3d color2 = new(0, 0, 0);
    }
}
