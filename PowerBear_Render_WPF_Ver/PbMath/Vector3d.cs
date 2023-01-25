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
        public double LengthSquared() { return e[0] * e[0] + e[1] * e[1] + e[2] * e[2]; }
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
        // 定义索引器
        public double this[int i] { get { return e[i]; } }
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
        public static Vector3d Unit_Vector(Vector3d vec) {
            var res = new Vector3d() { e = vec.e };
            return res.Normalized();
        }
        public static Vector3d Random(double minx, double maxx) {
            return new Vector3d(PbRandom.Random_Double(minx, maxx), PbRandom.Random_Double(minx, maxx), PbRandom.Random_Double(minx, maxx));
        }
        public static Vector3d Clamp(Vector3d x) {
            return new Vector3d(PbMath.Clamp(x.x()), PbMath.Clamp(x.y()), PbMath.Clamp(x.z()));
        }
        public static Vector3d ClampRange(Vector3d x) {
            return new Vector3d(x.x() - (int)(x.x()), x.y() - (int)(x.y()), x.z() - (int)(x.z()));
        }
        /// <summary>
        /// 随机返回一个在单位球内的向量
        /// </summary>
        /// <returns>新向量</returns>
        public static Vector3d Random_In_Unit_Sphere() {
            while (true) {
                var p = Vector3d.Random(-1.0d, 1.0d);
                if (p.LengthSquared() > 1.0d) continue;
                return p;
            }
        }
        /// <summary>
        /// lambertian分布律算法，根据那个概率来返回最可能的Vector向量样子
        /// </summary>
        public static Vector3d Random_Unit_Vector() {
            var a = PbRandom.Random_Double(0.0d, 2.0d * Math.PI);
            var z = PbRandom.Random_Double(-1.0d, 1.0d);
            var r = Math.Sqrt(Math.Abs(1.0d - z * z));
            return new Vector3d(r * Math.Cos(a), r * Math.Sin(a), z);
        }

        public static Vector3d Zero { get; } = new Vector3d(0, 0, 0);

        // Structures Math
        public double[] e = new double[3];
    }
}
