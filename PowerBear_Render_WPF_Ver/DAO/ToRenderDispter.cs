using PowerBear_Render_WPF_Ver.CameraObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.DAO {
    /// <summary>
    /// 传递给多线程初始化的参数
    /// </summary>
    public class ToRenderDispter {
        public int width, height;
        public Camera mCamera;
        public int cpus; // 使用CPU核心数
    }
}
