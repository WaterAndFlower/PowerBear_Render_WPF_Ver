﻿using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
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
    }
}
