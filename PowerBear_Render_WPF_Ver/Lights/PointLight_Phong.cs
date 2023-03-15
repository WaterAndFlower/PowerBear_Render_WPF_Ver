using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Lights {
    public class PointLight_Phong {
        public Vector3d lightColor = new();
        public Vector3d pos = new();
        public double intensity = 1d;
    }
}
