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
    public class Ray : ICloneable {
        //Basis
        public Ray(Vector3d o, Vector3d d) { origin = o; direction = d; }
        public Vector3d At(double t) { return origin + t * direction; }

        public Object Clone() {
            Ray res = new Ray(origin, direction);
            res.t = this.t;
            return res;
        }

        //Class Struct
        public Vector3d origin;
        public Vector3d direction;
        public double t;
    }
}
