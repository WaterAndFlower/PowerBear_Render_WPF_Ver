using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.PbMath {
    public class PbMath {
        public const double PI = Math.PI; // 3.141592657
        /// <summary>
        /// 返回[0,1]的double
        /// </summary>
        public static double Clamp(double x) {
            if (x >= 0 && x <= 1) return x;
            if (x > 1) return 1.0d;
            return 0.0d;
        }
    }
}
