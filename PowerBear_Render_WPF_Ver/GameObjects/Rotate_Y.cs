using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Windows.Controls;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    /// <summary>
    /// 绕着世界坐标轴的O点进行旋转，如果想要旋转正确，建议先在O点旋转完成，再去平移
    /// </summary>
    public class Rotate_Y : HitTable {
        public HitTable ptr { get; set; }
        public double angle { get; set; }
        public double radians { get; set; }
        public double sin_theta { get; set; }
        public double cos_theta { get; set; }

        public bool has_box;
        public AABB? bbox;
        Matrix3x3d Roate, RoateT;
        public Rotate_Y() { }
        public Rotate_Y(HitTable ptr, double angle) {
            this.ptr = ptr;
            this.angle = angle;
            this.radians = PbMath.PbMath.Degress_To_Radians(angle);
            this.sin_theta = Math.Sin(radians);
            this.cos_theta = Math.Cos(radians);

            Roate = new(new double[] { cos_theta,-sin_theta,0,
                                       0        ,         0,0,
                                       sin_theta,cos_theta, 1});
            RoateT = new(new double[] { cos_theta,sin_theta,0,
                                       0        ,         0,0,
                                       -sin_theta,cos_theta, 1});

            // 大部分GameObject拥有包围盒子，如果没有包围盒，那么光线求交将在BVH过程中始终不正常
            has_box = ptr.Bounding_Box(out bbox);

            if (bbox == null) return;

            // 新的外围盒的最小坐标值与最大坐标值，即左下角和右上角
            Vector3d minBoxPos = new(double.MaxValue, double.MaxValue, double.MaxValue);
            Vector3d maxBoxPos = new(double.MinValue, double.MinValue, double.MinValue);
            // 对原先拥有的包围盒进行变换
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 2; j++) {
                    for (int k = 0; k < 2; k++) {
                        var x = i * bbox.maximum.x() + (1 - i) * bbox.minimum.x();
                        var y = j * bbox.maximum.y() + (1 - j) * bbox.minimum.y();
                        var z = k * bbox.maximum.z() + (1 - k) * bbox.minimum.z();

                        // 算出当前顶点的新的x, y坐标值
                        var newx = cos_theta * x + sin_theta * z;
                        var newz = -sin_theta * x + cos_theta * z;

                        // 根据新的x,y坐标值来跟新min 和 max坐标值
                        Vector3d tester = new(newx, y, newz);

                        for (int c = 0; c < 3; c++) {
                            minBoxPos[c] = Math.Min(minBoxPos[c], tester[c]);
                            maxBoxPos[c] = Math.Max(maxBoxPos[c], tester[c]);
                        }
                    }
                }
            }
            bbox = new AABB(minBoxPos, maxBoxPos);
        }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            // 射线源点
            var origin = new Vector3d(ray.origin);
            // 射线方向
            var direction = new Vector3d(ray.direction);

            // 射线原点先经过 -θ的旋转
            origin[0] = cos_theta * ray.origin[0] - sin_theta * ray.origin[2];
            origin[2] = sin_theta * ray.origin[0] + cos_theta * ray.origin[2];

            // 射线方向也是先经过 -θ的旋转
            direction[0] = cos_theta * ray.direction[0] - sin_theta * ray.direction[2];
            direction[2] = sin_theta * ray.direction[0] + cos_theta * ray.direction[2];

            // 旋转变换过的射线rotated_r
            Ray rotated_r = new(origin, direction);

            // 用旋转变换过的射线与原始物体进行碰撞检测，如果没有碰撞则返回false
            if (!ptr.Hit(rotated_r, t_min, t_max, out hitResult))
                return false;

            // 算出的碰撞点p
            var p = new Vector3d(hitResult.p);
            // 算出的碰撞法线normal
            var normal = new Vector3d(hitResult.normal);
            // 碰撞点p需要正向旋转θ
            p[0] = cos_theta * hitResult.p[0] + sin_theta * hitResult.p[2];
            p[2] = -sin_theta * hitResult.p[0] + cos_theta * hitResult.p[2];

            // 碰撞法线normal也需要正向旋转θ
            normal[0] = cos_theta * hitResult.normal[0] + sin_theta * hitResult.normal[2];
            normal[2] = -sin_theta * hitResult.normal[0] + cos_theta * hitResult.normal[2];

            // 记录新的碰撞点和碰撞法线
            hitResult.p = p;
            //hitResult.normal = normal;
            hitResult.Set_Face_Normal(ray, normal); // 有Bug，已经修正

            return true;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            output_box = this.bbox;
            return this.bbox != null;
        }
    }
}
