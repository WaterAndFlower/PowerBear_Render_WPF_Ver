using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.PbMath {
    public class PbColorRGB : Vector3d {
        public PbColorRGB() { }
        public PbColorRGB(Vector3d v) { e[0] = v.e[0]; e[1] = v.e[1]; e[2] = v.e[2]; }
        /// <summary>
        /// 注意取值范围，为[0,1]
        /// </summary>
        /// <param name="r">红</param>
        /// <param name="g">绿</param>
        /// <param name="b">蓝</param>
        public PbColorRGB(double r, double g, double b) { e[0] = r; e[1] = g; e[2] = b; }
        public System.Drawing.Color ConvertToSysColor() {
            return System.Drawing.Color.FromArgb((byte)(e[0] * 255), (byte)(e[1] * 255), (byte)(e[2] * 255));
        }
    }
}
