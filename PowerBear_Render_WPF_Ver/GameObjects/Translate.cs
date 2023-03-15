using PowerBear_Render_WPF_Ver.PbMath;
using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    public class Translate : HitTable {
        public HitTable ptr { get; set; }
        public Vector3d offset { get; set; }
        public Translate() { }
        public Translate(HitTable ptr, Vector3d offset) {
            this.ptr = ptr;
            this.offset = offset;
        }

        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            Ray moved_r = new(ray.origin - offset, ray.direction);
            if (!ptr.Hit(moved_r, t_min, t_max, out hitResult))
                return false;
            hitResult.p += offset;
            hitResult.Set_Face_Normal(moved_r, hitResult.normal);
            return true;
        }

        public override bool Bounding_Box(out AABB? output_box) {
            if (!ptr.Bounding_Box(out output_box)) { return false; }
            output_box = new AABB(output_box.minimum + offset, output_box.maximum + offset);
            return true;
        }
    }
}
