﻿using PowerBear_Render_WPF_Ver.DAO;
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

    public abstract class BaseRenderDispter {
        public ToRenderDispterData _parm;
        public BaseRenderDispter(ToRenderDispterData parm) { this._parm = parm; }
        public virtual void BeforeRender() { }
        public abstract void DoRender(); // 实现的时候，BeforeRender，AfterRender调用
        public virtual void AfterRender() { }
        public virtual RenderDispResult Render() {
            throw new NotImplementedException();
        }
        public virtual OutRenderPixel GetResult() {
            throw new NotImplementedException();
        }
    }
}