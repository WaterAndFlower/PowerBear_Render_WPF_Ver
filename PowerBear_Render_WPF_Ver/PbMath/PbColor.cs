using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.PbMath {
    public class PbColorRGB {
        public PbColorRGB() { }
        public PbColorRGB(Vector3d v) { vec = v; }
        /// <summary>
        /// 注意取值范围，为[0,1]
        /// </summary>
        /// <param name="r">红</param>
        /// <param name="g">绿</param>
        /// <param name="b">蓝</param>
        public PbColorRGB(double r, double g, double b) { vec.e[0] = r; vec.e[1] = g; vec.e[2] = b; }
        public System.Drawing.Color ConvertToSysColor() {
            return System.Drawing.Color.FromArgb((byte)(vec.e[0] * 255), (byte)(vec.e[1] * 255), (byte)(vec.e[2] * 255));
        }
        public Vector3d vec = new Vector3d();
    }
}
