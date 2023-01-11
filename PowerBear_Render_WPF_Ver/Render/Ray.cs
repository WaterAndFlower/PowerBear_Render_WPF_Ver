using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render {
    /// <summary>
    /// 光线类，Origin + t * Direction 表示一条三维空间中的光线
    /// </summary>
    public class Ray {
        //Basis
        public Ray(Vector3d o, Vector3d d) { origin = o; direction = d; }
        Vector3d At(double t) { return origin + t * direction; }
        //Class Struct
        public Vector3d origin;
        public Vector3d direction;
        public double t;
    }
}
