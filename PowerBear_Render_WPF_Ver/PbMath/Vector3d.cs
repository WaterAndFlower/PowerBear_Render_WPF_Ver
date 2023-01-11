using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PowerBear_Render_WPF_Ver.PbMath {
    /// <summary>
    /// Vector向量类，数学Helper，Double类型
    /// </summary>
    public class Vector3d {
        // Base Function
        public Vector3d() { }
        public Vector3d(double x, double y, double z) { e[0] = x; e[1] = y; e[2] = z; }
        public double x() { return e[0]; }
        public double y() { return e[1]; }
        public double z() { return e[2]; }
        public double Length() { return Math.Sqrt(e[0] * e[0] + e[1] * e[1] + e[2] * e[2]); }
        public double LengthSquare() { return e[0] * e[0] + e[1] * e[1] + e[2] * e[2]; }
        public override string ToString() {
            return $"Vector3 x:{this.x()} y:{this.y()} z:{this.z()}";
        }
        public double Dot(Vector3d b) { // 向量点积
            return e[0] * b.e[0] + e[1] * b.e[1] + e[2] * b.e[2];
        }
        public Vector3d Cross(Vector3d b) {
            return new Vector3d(this.e[1] * b.e[2] - this.e[2] * b.e[1],
                                this.e[2] * b.e[0] - this.e[0] * b.e[2],
                                this.e[0] * b.e[1] - this.e[1] * b.e[0]);
        }
        public Vector3d Normalized() { // 归一化向量
            return this / this.Length();
        }
        // Static Function
        public static Vector3d Vector3DUse(double x, double y, double z) { return new Vector3d(x, y, z); }
        public static Vector3d operator -(Vector3d a, Vector3d b) {
            Vector3d res = new Vector3d(a.x(), a.y(), a.z());
            res.e[0] -= b.e[0]; res.e[1] -= b.e[1]; res.e[2] -= b.e[2]; return res;
        }
        public static Vector3d operator +(Vector3d a, Vector3d b) {
            Vector3d res = new Vector3d(a.x(), a.y(), a.z());
            res.e[0] += b.e[0]; res.e[1] += b.e[1]; res.e[2] += b.e[2]; return res;
        }
        public static Vector3d operator *(Vector3d a, double value) {
            Vector3d res = new Vector3d(a.x(), a.y(), a.z());
            res.e[0] *= value; res.e[1] *= value; res.e[2] *= value; return res;
        }
        public static Vector3d operator *(double value, Vector3d a) {
            return a * value;
        }
        public static Vector3d operator *(Vector3d a, Vector3d b) {
            return new Vector3d(a.e[0] * b.e[0], a.e[1] * b.e[1], a.e[2] * b.e[2]);
        }
        public static Vector3d operator /(Vector3d a, double value) {
            Vector3d res = new Vector3d(a.x(), a.y(), a.z());
            res = res * (1.0d / value); return res;
        }
        public static double Dot(Vector3d a, Vector3d b) {
            return a.Dot(b);
        }
        public static Vector3d Cross(Vector3d a, Vector3d b) {
            return a.Cross(b);
        }
        // Structures Math
        public double[] e = new double[3];
    }
}
