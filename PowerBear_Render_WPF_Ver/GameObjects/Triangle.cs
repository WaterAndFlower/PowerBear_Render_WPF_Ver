using PowerBear_Render_WPF_Ver.Materials;
using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    /// <summary>
    /// 三角形
    /// </summary>
    public class Triangle : HitTable {
        public Vector3d[] pointPos = new Vector3d[3]; // 三角形在三维空间中的点
        public int p0Index, p1Index, p2Index;//专门给OBJ模型用的顶点索引
        public Material mat = new Lambertian();
        public Triangle(Vector3d p0, Vector3d p1, Vector3d p2) { this.pointPos[0] = p0; this.pointPos[1] = p1; this.pointPos[2] = p2; }
        public Triangle(Vector3d p0, Vector3d p1, Vector3d p2, int index0, int index1, int index2) { this.pointPos[0] = p0; this.pointPos[1] = p1; this.pointPos[2] = p2; this.p0Index = index0; this.p1Index = index1; this.p2Index = index2; }
        public Triangle(Vector3d p0, Vector3d p1, Vector3d p2, Material mat) { this.pointPos[0] = p0; this.pointPos[1] = p1; this.pointPos[2] = p2; this.mat = mat; }
        /// <summary>
        /// u*P1+v*P2+(1-u-v)*P0
        /// </summary>
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            hitResult = new HitResult(); // 这部分看公式
            var e1 = pointPos[1] - pointPos[0];
            var e2 = pointPos[2] - pointPos[0];
            var q = ray.direction.Cross(e2);
            var a = e1.Dot(q);
            if (a > -1e-5 && a < 1e-5) return false; // 矩阵行列式为0
            var f = 1 / a;
            var s = ray.origin - pointPos[0];
            var u = f * s.Dot(q);
            if (u < 0.0d) return false;
            var r = s.Cross(e1);
            var v = f * ray.direction.Dot(r);
            if (v < 0.0d || u + v > 1.0d) return false;
            var t = f * e2.Dot(r);
            if (!(t >= t_min && t <= t_max)) return false;
            //判断为击中，开始设定HitRes
            hitResult.t = t;
            hitResult.u = u; hitResult.v = v;
            hitResult.Set_Face_Normal(ray, e1.CrossRight(e2));
            hitResult.p = ray.At(t);
            hitResult.mat = this.mat;
            hitResult.hitObj = this; // 需要在obj里面重新计算一次
            return true;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            double xmin = Math.Min(pointPos[0].x(), Math.Min(pointPos[1].x(), pointPos[2].x()));
            double ymin = Math.Min(pointPos[0].y(), Math.Min(pointPos[1].y(), pointPos[2].y()));
            double zmin = Math.Min(pointPos[0].z(), Math.Min(pointPos[1].z(), pointPos[2].z()));

            double xmax = Math.Max(pointPos[0].x(), Math.Max(pointPos[1].x(), pointPos[2].x()));
            double ymax = Math.Max(pointPos[0].y(), Math.Max(pointPos[1].y(), pointPos[2].y()));
            double zmax = Math.Max(pointPos[0].z(), Math.Max(pointPos[1].z(), pointPos[2].z()));

            output_box = new AABB(new Vector3d(xmin, ymin, zmin), new Vector3d(xmax, ymax, zmax));
            return true;
        }
    }
}
//https://zhuanlan.zhihu.com/p/405075535
//Moller-Trumbore算法:https://zhuanlan.zhihu.com/p/468132444
//https://blog.csdn.net/qq_22822335/article/details/50930364