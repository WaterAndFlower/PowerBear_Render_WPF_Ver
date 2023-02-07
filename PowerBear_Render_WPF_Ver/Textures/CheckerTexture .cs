using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Textures {
    public class CheckerTexture : Texture {
        public CheckerTexture(Texture odd, Texture even) { this.odd = odd; this.even = even; }
        public CheckerTexture(Vector3d oddColor, Vector3d evenColor) {
            odd = new Solid_Color(oddColor);
            even = new Solid_Color(evenColor);
        }
        public override Vector3d Value(double u, double v, Vector3d p) {
            var sines = Math.Sin(scale * p.x()) * Math.Sin(scale * p.y()) * Math.Sin(scale * p.z());
            if (sines < 0) {
                return odd.Value(u, v, p);
            } else {
                return even.Value(u, v, p);
            }
        }
        public Texture odd, even;
        public double scale { get; set; } = 10;
    }
}
