using PowerBear_Render_WPF_Ver.GameObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render {
    //=======BVH包围盒子，划分空间方法=========
    class BVH_XYZ_Compare : IComparer<HitAble> {
        int axis;
        public BVH_XYZ_Compare(int axis) {
            this.axis = axis;
        }
        public int Compare(HitAble? x, HitAble? y) {
            return BVH_Node.Box_Compare(x, y, axis) == true ? 1 : 0;
        }
    }
    //=======BVH主要=======
    public class BVH_Node : HitAble {
        public HitAble? left;
        public HitAble? right;
        public AABB? box;
        public static bool Box_Compare(HitAble a, HitAble b, int axis) {
            AABB? box_a = new AABB(), box_b = new AABB();
            if (!a.Bounding_Box(out box_a) || !b.Bounding_Box(out box_b)) {
                Console.WriteLine("出错了！没有一个包围盒子，构建BVH失败。");
            }
            return box_a.minimum.e[axis] < box_b.minimum.e[axis];
        }
        public BVH_Node(Hittable_List hittable_List) {
            BuildTree(hittable_List.objects, 0, hittable_List.objects.Count);
        }
        /// <summary>
        /// 建造节点和建树，区间[start,end)，必须保证，所有物体都要有一个包围盒子。
        /// </summary>
        public BVH_Node(IEnumerable<HitAble> src_objects, int start, int end) {
            BuildTree(src_objects, start, end);
        }
        void BuildTree(IEnumerable<HitAble> src_objects, int start, int end) {
            var objs = src_objects as List<HitAble>;
            if (end - start <= 0 || objs == null) { box = null; Console.WriteLine($"BVH 无法建造，{start} -> {end}，范围不包含物体"); return; }

            int axis = PbMath.PbRandom.Random_int(0, 2); //0：在x轴上排序，1：在y轴上排序，2：在z轴上排序
            //Console.WriteLine(axis);
            int size = end - start;
            if (size == 1) {
                left = right = objs[start];
            } else {
                objs.Sort(start, size, new BVH_XYZ_Compare(axis));
                var mid = start + size / 2;
                left = new BVH_Node(objs, start, mid);
                right = new BVH_Node(objs, mid, end);
            }
            AABB? box_left = new AABB(), box_right = new AABB();
            if (!left.Bounding_Box(out box_left) || !right.Bounding_Box(out box_right)) {
                Console.WriteLine("BVH合并包围盒出错，因为子叶没有包围盒");
            } else {
                this.box = AABB.Surrounding_Box(box_left, box_right);
            }
        }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            hitResult = new HitResult();
            if (box == null || !box.Hit(ray, t_min, t_max)) {
                return false;
            }
            bool hitLeft = false, hitRight = false;
            HitResult temp;
            hitLeft = left.Hit(ray, t_min, t_max, out hitResult);
            hitRight = right.Hit(ray, t_min, hitLeft ? hitResult.t : t_max, out temp);
            if (hitRight == true) hitResult = temp;

            return hitLeft | hitRight;
        }
        public override bool Bounding_Box(out AABB? output_box) {
            if (this.box == null) { output_box = null; return false; }
            output_box = this.box;
            return true;
        }
    }
}
