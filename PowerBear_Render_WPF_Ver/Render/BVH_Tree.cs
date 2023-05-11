using PowerBear_Render_WPF_Ver.GameObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render {
    /// <summary>
    ///  对BVH的一个封装
    /// </summary>
    public class BVH_Tree : HitTable {
        BVH_Node? BVH_Node { get; set; }
        public BVH_Tree(Hittable_List hittable_List) {
            Stopwatch sw = new();
            sw.Start();
            BVH_Node = new BVH_Node(hittable_List);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine($"BVH构建完成了，总共：{hittable_List.objects.Count()}元素，耗时：{sw.Elapsed.TotalMilliseconds} ms");
            Console.BackgroundColor = ConsoleColor.Black;
            sw.Stop();
        }
        /// <summary>
        /// 区间[start,end)
        /// </summary>
        public BVH_Tree(IEnumerable<HitTable> src_objects, int start, int end) {
            Stopwatch sw = new();
            sw.Start();
            BVH_Node = new BVH_Node(src_objects, start, end);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine($"BVH构建完成了，总共：{src_objects.Count()}元素，耗时：{sw.Elapsed.TotalMilliseconds} ms");
            Console.BackgroundColor = ConsoleColor.Black;
            sw.Stop();
        }
        public BVH_Tree(IEnumerable<HitTable> src_objects) {
            Stopwatch sw = new();

            sw.Start();
            BVH_Node = new BVH_Node(src_objects, 0, src_objects.Count());
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine($"BVH构建完成了，总共：{src_objects.Count()}元素，耗时：{sw.Elapsed.TotalMilliseconds} ms");
            Console.BackgroundColor = ConsoleColor.Black;
            sw.Stop();
        }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            hitResult = new HitResult();

            if (BVH_Node != null) {
                var res = BVH_Node.Hit(ray, t_min, t_max, out hitResult);
                return res;
            } else { return false; }
        }
        public override bool Bounding_Box(out AABB? output_box) {
            if (BVH_Node != null) { return BVH_Node.Bounding_Box(out output_box); } else {
                return base.Bounding_Box(out output_box);
            }
        }
    }
}
