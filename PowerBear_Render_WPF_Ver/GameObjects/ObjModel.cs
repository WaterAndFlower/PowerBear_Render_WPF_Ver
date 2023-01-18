using PowerBear_Render_WPF_Ver.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.GameObjects {
    /// <summary>
    /// Obj格式模型解析
    /// </summary>
    public class ObjModel : HitTable {
        public override bool Hit(Ray ray, double t_min, double t_max, out HitResult hitResult) {
            throw new NotImplementedException();
        }
    }
}
