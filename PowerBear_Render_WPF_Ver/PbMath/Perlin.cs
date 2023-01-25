using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PowerBear_Render_WPF_Ver.PbMath {
    /// <summary>
    /// 柏林噪声算法 实现 小熊 2023年1月 C#版本
    /// https://blog.csdn.net/Liukairui/article/details/125496735
    /// </summary>
    public class Perlin {
        public static int point_count = 256;
        double[] ranFloat; // 梯度向量 float *（弃用）
        int[] perm_x, perm_y, perm_z;
        Vector3d[] ranvec; // 随机朝向向量 Vec3
        public Perlin() {
            ranvec = new Vector3d[point_count];
            for (int i = 0; i < point_count; ++i) ranvec[i] = Vector3d.Random_Unit_Vector().Normalized();
            perm_x = PerlinGeneratePerm();
            perm_y = PerlinGeneratePerm();
            perm_z = PerlinGeneratePerm();
        }
        public double Noise(Vector3d p) {
            var u = p.x() - Math.Floor(p.x());
            var v = p.y() - Math.Floor(p.y());
            var w = p.z() - Math.Floor(p.z());

            var i = (int)Math.Floor(p.x());
            var j = (int)Math.Floor(p.y());
            var k = (int)Math.Floor(p.z());
            var c = new Vector3d[2, 2, 2];

            for (int di = 0; di < 2; di++)
                for (int dj = 0; dj < 2; dj++)
                    for (int dk = 0; dk < 2; dk++) {
                        c[di, dj, dk] = ranvec[
                              perm_x[(i + di) & 255]
                            ^ perm_y[(j + dj) & 255]
                            ^ perm_z[(k + dk) & 255]];
                    }
            return PerlinInterp(c, u, v, w);
        }
        /// <summary>
        /// 多个相位相加，深度depth
        /// </summary>
        public double Turb(Vector3d p, int depth = 7) {
            var accum = 0.0;
            var temp_p = p;
            var weight = 1.0;

            for (int i = 0; i < depth; i++) {
                accum += weight * this.Noise(temp_p);
                weight *= 0.5;
                temp_p *= 2;
            }
            return Math.Abs(accum);
        }
        public static int[] PerlinGeneratePerm() {
            var p = new int[point_count];
            for (int i = 0; i < point_count; i++) p[i] = i;
            Permute(p, point_count);
            return p;
        }
        // 随机打乱之前的数组
        public static void Permute(int[] a, int n) {
            for (int i = n - 1; i > 0; i--) {
                int target = PbRandom.Random_int(i, 0);
                int tmp = a[i];
                a[i] = a[target];
                a[target] = tmp;
            }
        }
        /// <summary>
        /// 双线性插值平滑
        /// </summary>
        public static double TrilinerInterp(double[,,] c, double u, double v, double w) {
            var accum = 0.0d;
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++) {
                        accum += (i * u + (1 - i) * (1 - u)) *
                            (j * v + (1 - j) * (1 - v)) *
                            (k * w + (1 - k) * (1 - w)) * c[i, j, k];
                    }
            return accum;
        }
        public static double PerlinInterp(Vector3d[,,] c, double u, double v, double w) {
            var uu = u * u * (3.0 - 2 * u);
            var vv = v * v * (3.0 - 2 * v);
            var ww = w * w * (3.0 - 2 * w);
            var accum = 0.0d;
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++) {
                        Vector3d weight_v = new Vector3d(u - i, v - j, w - k);
                        accum += (i * uu + (1 - i) * (1 - uu)) *
                            (j * vv + (1 - j) * (1 - vv)) *
                            (k * ww + (1 - k) * (1 - ww)) * Vector3d.Dot(c[i, j, k], weight_v);
                    }
            return accum;
        }

        //还差一个相位，没有实现
        //https://zhuanlan.zhihu.com/p/524628093
    }
}
