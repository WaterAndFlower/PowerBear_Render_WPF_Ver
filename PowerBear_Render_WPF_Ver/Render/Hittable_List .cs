using PowerBear_Render_WPF_Ver.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render {
    public class Hittable_List : HitTable {
        public Hittable_List() { }
        public Hittable_List(HitTable obj) { objects.Add(obj); }
        public List<HitTable> objects = new List<HitTable>();
        public void Add(HitTable obj) { objects.Add(obj); }
        public void Clear() { objects.Clear(); }
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            HitResult temp_rec = new HitResult();
            hitResult = temp_rec;
            bool hit_any = false;
            var close_so_far = t_max;
            foreach (var item in objects) {
                if (item.Hit(ray, t_min, close_so_far, out temp_rec)) {
                    hit_any = true;
                    close_so_far = temp_rec.t;
                    hitResult = temp_rec;
                }
            }
            return hit_any;
        }
        public override bool Bounding_Box(out AABB output_box) {
            AABB ans_box = new AABB();
            output_box = new AABB();
            if (objects.Count == 0) { return false; }
            bool firstAABB = true;
            foreach (var obj in objects) {
                if (obj.Bounding_Box(out ans_box) == false) return false;
                output_box = firstAABB ? ans_box : AABB.Surrounding_Box(output_box, ans_box);
                firstAABB = false;
            }
            return true;
        }
        public override object Clone() {
            var t = new Hittable_List();
            foreach (var obj in objects) { t.Add(obj); }
            return t;
        }
    }
}
