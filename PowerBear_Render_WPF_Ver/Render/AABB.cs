using PowerBear_Render_WPF_Ver.PbMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render {
    public class AABB {
        public Vector3d minimum, maximum;
        public AABB() { minimum = new Vector3d(); maximum = new Vector3d(); }
        /// <summary>
        /// 构造一个AABB包围盒
        /// </summary>
        /// <param name="a">左下角，小</param>
        /// <param name="b">右上角，大</param>
        public AABB(Vector3d min, Vector3d max) { minimum = min; maximum = max; }
        /// <summary>
        /// 判断Ray是否击中AABB盒
        /// </summary>
        /// <param name="r">Ray光线</param>
        /// <param name="t_min">光线t下限</param>
        /// <param name="t_max">光线t上限</param>
        /// <returns></returns>
        public bool Hit(Ray r, double t_min, double t_max) {
            for (int a = 0; a < 3; a++) {
                var invD = 1.0d / r.direction[a];
                var t0 = (minimum[a] - r.origin[a]) * invD;
                var t1 = (maximum[a] - r.origin[a]) * invD;
                if (invD < 0) { var tt = t0; t0 = t1; t1 = tt; }
                t_min = t0 > t_min ? t0 : t_min;
                t_max = t1 < t_max ? t1 : t_max;
                if (t_min >= t_max) { return false; }
            }
            return true;
        }
        public static AABB Surrounding_Box(AABB box1, AABB box2) {
            Vector3d small = new(Math.Min(box1.minimum.x(), box2.minimum.x()),
                                 Math.Min(box1.minimum.y(), box2.minimum.y()),
                                 Math.Min(box1.minimum.z(), box2.minimum.z()));
            Vector3d big = new(Math.Max(box1.maximum.x(), box2.maximum.x()),
                               Math.Max(box1.maximum.y(), box2.maximum.y()),
                               Math.Max(box1.maximum.z(), box2.maximum.z()));
            return new AABB(small, big);
        }
    }
}
