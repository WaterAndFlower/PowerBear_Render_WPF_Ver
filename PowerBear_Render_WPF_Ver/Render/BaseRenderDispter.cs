using PowerBear_Render_WPF_Ver.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.Render {
    public struct OutRenderPixel {
        public int width, height;
        public List<Byte[]> res_list_pixels_BGRA;
    }
    /// <summary>
    /// 多线程渲染方式实现抽象类
    /// </summary>
    public abstract class BaseRenderDispter {
        public ToRenderDispterData _parm;
        public BaseRenderDispter(ToRenderDispterData parm) { this._parm = parm; }
        public abstract void DoRender(IRayColor RayColor); // 可替换的Ray_Color函数，切换渲染方式
        public virtual void AfterRender() { }
        public virtual RenderDispResult Render() {
            throw new NotImplementedException();
        }
        public virtual OutRenderPixel GetResult() {
            throw new NotImplementedException();
        }
    }
}
