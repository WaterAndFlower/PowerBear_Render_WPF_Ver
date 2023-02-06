using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.PbMath {
    /// <summary>
    /// 3x3 都double类型的矩阵，下标从0开始
    /// </summary>
    public class Matrix3x3d {
        public double[] e = new double[3 * 3];
        public Matrix3x3d(double[] e) { this.e = e; }
        public Matrix3x3d(double e0, double e1, double e2) { }
        public static Vector3d operator *(Matrix3x3d matx, Vector3d vector) {
            return new Vector3d((matx.e[0] * vector[0] + matx.e[1] * vector[1] + matx.e[2] * vector[2]),
                                (matx.e[3] * vector[0] + matx.e[4] * vector[1] + matx.e[5] * vector[2]),
                                (matx.e[6] * vector[0] + matx.e[7] * vector[1] + matx.e[8] * vector[2]));
        }
    }
}
